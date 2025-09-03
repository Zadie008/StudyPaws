<%@ WebHandler Language="C#" Class="SellHandler" %>

using System;
using System.Web;
using System.Web.SessionState;
using MySql.Data.MySqlClient;
using System.Configuration;

public class SellHandler : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        
        string action = context.Request["action"];
        string userID = context.Session["UserID"] as string;
        
        if (string.IsNullOrEmpty(userID))
        {
            context.Response.Write("ERROR: User not logged in");
            return;
        }

        try
        {
            switch (action)
            {
                case "getSellPrice":
                    string colourNum = context.Request["colourNum"];
                    GetSellPrice(context, userID, colourNum);
                    break;
                case "confirmSell":
                    ConfirmSell(context, userID);
                    break;
                default:
                    context.Response.Write("ERROR: Invalid action");
                    break;
            }
        }
        catch (Exception ex)
        {
            context.Response.Write("ERROR: " + ex.Message);
        }
    }

    private void GetSellPrice(HttpContext context, string userID, string colourNum)
    {
        if (string.IsNullOrEmpty(colourNum))
        {
            context.Response.Write("ERROR: No colour selected");
            return;
        }

        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        int petID = -1;
        int sellPrice = 0;
        string petType = "";

        using (MySqlConnection con = new MySqlConnection(cs))
        {
            con.Open();

            petType = context.Request["petType"] ?? "Cat";

            using (MySqlCommand cmd = new MySqlCommand("SELECT Pet.petID, Pet.sellPrice, Pet.petType FROM Pet INNER JOIN UserPets ON Pet.petID = UserPets.petID WHERE UserPets.userID = @userID AND Pet.petType = @petType AND Pet.colourNum = @colourNum", con))
            {
                cmd.Parameters.AddWithValue("@userID", userID);
                cmd.Parameters.AddWithValue("@petType", petType);
                cmd.Parameters.AddWithValue("@colourNum", colourNum);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        petID = Convert.ToInt32(reader["petID"]);
                        sellPrice = Convert.ToInt32(reader["sellPrice"]);
                        petType = reader["petType"].ToString();
                    }
                }
            }
        }

        if (colourNum == "1" && petType == "Cat")
        {
            context.Response.Write("CANNOT_SELL");
            return;
        }

        context.Session["petID"] = petID;
        context.Session["sellPrice"] = sellPrice;
        context.Session["colourNum"] = colourNum;

        context.Response.Write(sellPrice.ToString());
    }

    private void ConfirmSell(HttpContext context, string userID)
    {
        if (context.Session["petID"] == null || context.Session["sellPrice"] == null)
        {
            context.Response.Write("ERROR: Session data missing");
            return;
        }

        int petID = Convert.ToInt32(context.Session["petID"]);
        int sellPrice = Convert.ToInt32(context.Session["sellPrice"]);
        string colourNum = context.Session["colourNum"] as string;

        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        int currentCoins = 0;
        bool wasEquipped = false;

        using (MySqlConnection con = new MySqlConnection(cs))
        {
            con.Open();

            // CHECK IF SOLD PET WAS EQUIPPED
            string checkEquippedQuery = "SELECT equippedStatus FROM UserPets WHERE userID = @userID AND petID = @petID";
            using (MySqlCommand cmd = new MySqlCommand(checkEquippedQuery, con))
            {
                cmd.Parameters.AddWithValue("@userID", userID);
                cmd.Parameters.AddWithValue("@petID", petID);
                object result = cmd.ExecuteScalar();
                wasEquipped = result != null && Convert.ToBoolean(result);
            }

            // 1. DELETE PET FROM UserPets
            string deleteCommand = "DELETE FROM UserPets WHERE userID = @userID AND petID = @petID";
            using (MySqlCommand cmd = new MySqlCommand(deleteCommand, con))
            {
                cmd.Parameters.AddWithValue("@userID", userID);
                cmd.Parameters.AddWithValue("@petID", petID);
                int rows = cmd.ExecuteNonQuery();
            }

            // 2. GET CURRENT coin count
            string getCoinsQuery = "SELECT userCoinCount FROM Users WHERE userID = @userID";
            using (MySqlCommand cmd = new MySqlCommand(getCoinsQuery, con))
            {
                cmd.Parameters.AddWithValue("@userID", userID);
                object result = cmd.ExecuteScalar();
                currentCoins = result != null ? Convert.ToInt32(result) : 0;
            }

            // 3. UPDATE coin count
            int updatedCoins = currentCoins + sellPrice;
            string updateCoins = "UPDATE Users SET userCoinCount = @coins WHERE userID = @userID";
            using (MySqlCommand cmd = new MySqlCommand(updateCoins, con))
            {
                cmd.Parameters.AddWithValue("@coins", updatedCoins);
                cmd.Parameters.AddWithValue("@userID", userID);
                cmd.ExecuteNonQuery();
            }

            if (wasEquipped)
            {
                // GET petID of Cat 1
                int cat1PetID = -1;
                string getCat1ID = "SELECT petID FROM Pet WHERE petType = 'Cat' AND colourNum = 1";
                using (MySqlCommand cmd = new MySqlCommand(getCat1ID, con))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                        cat1PetID = Convert.ToInt32(result);
                }

                // ENSURE CAT 1 IS OWNED BY USER
                bool ownsCat1 = false;
                string checkOwnership = "SELECT COUNT(*) FROM UserPets WHERE userID = @userID AND petID = @petID";
                using (MySqlCommand cmd = new MySqlCommand(checkOwnership, con))
                {
                    cmd.Parameters.AddWithValue("@userID", userID);
                    cmd.Parameters.AddWithValue("@petID", cat1PetID);
                    ownsCat1 = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }

                if (ownsCat1)
                {
                    string unequipAll = "UPDATE UserPets SET equippedStatus = FALSE WHERE userID = @userID";
                    using (MySqlCommand cmd = new MySqlCommand(unequipAll, con))
                    {
                        cmd.Parameters.AddWithValue("@userID", userID);
                        cmd.ExecuteNonQuery();
                    }

                    string equipCat1 = "UPDATE UserPets SET equippedStatus = TRUE WHERE userID = @userID AND petID = @petID";
                    using (MySqlCommand cmd = new MySqlCommand(equipCat1, con))
                    {
                        cmd.Parameters.AddWithValue("@userID", userID);
                        cmd.Parameters.AddWithValue("@petID", cat1PetID);
                        cmd.ExecuteNonQuery();
                    }

                    // UPDATE SESSION FOR HOME PAGE PET PATH
                    context.Session["EquippedPetImagePath"] = "Images/Cat 1.png";
                }
            }
        }

        context.Session.Remove("petID");
        context.Session.Remove("sellPrice");
        context.Session.Remove("colourNum");

        context.Response.Write((currentCoins + sellPrice).ToString());
    }

    public bool IsReusable
    {
        get { return false; }
    }
}