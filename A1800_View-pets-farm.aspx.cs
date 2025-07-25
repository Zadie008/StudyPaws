using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class View_Pets_farm : System.Web.UI.Page
{
    private string userID;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["Username"] != null)
            {
                string username = Session["Username"].ToString();
                userID = LoadUserData(username);
                if (userID != null)
                {
                    LoadOwnedPets(userID);
                }
            }
        }
    }

    private void LoadOwnedPets(string userID)
    {
        ContentPlaceHolder content = (ContentPlaceHolder)Master.FindControl("mainContentPlaceHolder");

        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        string command = "SELECT Pet.colourNum, UserPets.equippedStatus FROM UserPets INNER JOIN Pet ON UserPets.petID = Pet.petID WHERE UserPets.userID = ? AND Pet.petType = ?";

        List<OwnedPet> ownedPets = new List<OwnedPet>();

        using (OleDbConnection con = new OleDbConnection(cs))
        using (OleDbCommand cmd = new OleDbCommand(command, con))
        {
            cmd.Parameters.AddWithValue("?", userID);
            cmd.Parameters.AddWithValue("?", "Farm"); // PET TYPE~~~~

            try
            {
                con.Open();
                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int colour = Convert.ToInt32(reader["colourNum"]);
                        bool equipped = (reader["equippedStatus"] != DBNull.Value) && Convert.ToBoolean(reader["equippedStatus"]);
                        ownedPets.Add(new OwnedPet(colour, equipped));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading owned pets: " + ex.Message);
            }
        }

        for (int i = 1; i <= 5; i++)
        {
            HtmlGenericControl circleDiv = (HtmlGenericControl)content.FindControl("circle" + i);
            if (circleDiv != null)
            {
                string classes = circleDiv.Attributes["class"];
                if (!string.IsNullOrEmpty(classes) && classes.Contains("equipped"))
                {
                    circleDiv.Attributes["class"] = classes.Replace("equipped", "").Trim();
                }
            }
        }

        int displayIndex = 1;

        foreach (OwnedPet pet in ownedPets)
        {
            Image petImg = (Image)content.FindControl("imgPet" + displayIndex);
            Button selectBtn = (Button)content.FindControl("btnSelect" + displayIndex);
            HtmlGenericControl circleDiv = (HtmlGenericControl)content.FindControl("circle" + displayIndex);

            if (petImg != null && selectBtn != null && circleDiv != null)
            {
                petImg.ImageUrl = string.Format("Images/Farm {0}.png", pet.ColourNum); // PET TYPE~~~~
                petImg.Visible = true;

                selectBtn.Visible = true;
                selectBtn.CommandArgument = pet.ColourNum.ToString();
                selectBtn.Attributes["data-colour"] = pet.ColourNum.ToString();

                if (pet.IsEquipped)
                {
                    string existingClass = circleDiv.Attributes["class"];
                    if (!existingClass.Contains("equipped"))
                    {
                        circleDiv.Attributes["class"] = existingClass + " equipped";
                    }
                }
            }

            displayIndex++;
        }

        // hide remaining pets
        for (int i = displayIndex; i <= 5; i++)
        {
            Image petImg = (Image)content.FindControl("imgPet" + i);
            Button selectBtn = (Button)content.FindControl("btnSelect" + i);
            HtmlGenericControl circleDiv = (HtmlGenericControl)content.FindControl("circle" + i);

            if (petImg != null) petImg.Visible = false;
            if (selectBtn != null) selectBtn.Visible = false;
            if (circleDiv != null) circleDiv.Visible = false;
        }
    }

    private Control FindControlRecursive(Control root, string id)
    {
        if (root.ID == id)
            return root;

        foreach (Control child in root.Controls)
        {
            Control found = FindControlRecursive(child, id);
            if (found != null)
                return found;
        }

        return null;
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        btnEquip.Click += new EventHandler(btnEquip_Click);
    }

    protected void btnEquip_Click(object sender, EventArgs e)
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        string selectedColourNum = hfSelectedColourNum.Value;
        string userID = Session["UserID"] as string;

        if (string.IsNullOrEmpty(selectedColourNum) || string.IsNullOrEmpty(userID))
        {
            return;
        }

        using (OleDbConnection con = new OleDbConnection(cs))
        {
            con.Open();

            // 1. uncheck all current equipped pets for this user
            string unequipQuery = "UPDATE UserPets SET equippedStatus = FALSE WHERE userID = ?";
            using (OleDbCommand cmdUnequip = new OleDbCommand(unequipQuery, con))
            {
                cmdUnequip.Parameters.AddWithValue("?", userID);
                cmdUnequip.ExecuteNonQuery();
            }

            // 2. get petID
            int petID = -1;
            string getPetIDQuery = "SELECT petID FROM Pet WHERE petType = 'Farm' AND colourNum = ?"; // PET TYPE~~~~

            using (OleDbCommand getPetIDCmd = new OleDbCommand(getPetIDQuery, con))
            {
                getPetIDCmd.Parameters.AddWithValue("?", selectedColourNum);
                object result = getPetIDCmd.ExecuteScalar();
                if (result != null)
                {
                    petID = Convert.ToInt32(result);
                }
                else
                {
                    return;
                }
            }

            // 3. equip the selected one (check the checkbox)
            string equipQuery = "UPDATE UserPets SET equippedStatus = TRUE WHERE userID = ? AND petID = ?";
            using (OleDbCommand equipCmd = new OleDbCommand(equipQuery, con))
            {
                equipCmd.Parameters.AddWithValue("?", userID);
                equipCmd.Parameters.AddWithValue("?", petID);
                equipCmd.ExecuteNonQuery();
            }
        }

        // update session variable
        Session["EquippedPetImagePath"] = string.Format("Images/Farm {0}.png", selectedColourNum); // PET TYPE~~~~

        // reload pets to reflect new equipped status
        LoadOwnedPets(userID);
    }

    // SELL PET (DELETING THE USERPETS ENTRY)
    protected void btnSell_Click(object sender, EventArgs e)
    {
        string selectedColourNum = hfSelectedColourNum.Value;
        userID = Session["UserID"] as string;

        if (string.IsNullOrEmpty(selectedColourNum) || string.IsNullOrEmpty(userID))
            return;

        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        int petID = -1;
        int sellPrice = 0;
        string petType = "";

        using (OleDbConnection con = new OleDbConnection(cs))
        {
            con.Open();

            string queryPet = "SELECT petID, sellPrice, petType FROM Pet WHERE petType = ? AND colourNum = ?";
            using (OleDbCommand cmd = new OleDbCommand(queryPet, con))
            {
                cmd.Parameters.AddWithValue("?", "Farm"); // PET TYPE~~~~
                cmd.Parameters.AddWithValue("?", selectedColourNum);
                using (OleDbDataReader reader = cmd.ExecuteReader())
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

        Session["petID"] = petID;
        Session["sellPrice"] = sellPrice;
        Session["colourNum"] = selectedColourNum;

        lblSellPrice.Text = sellPrice.ToString(); // UPDATE COIN LABEL
        ScriptManager.RegisterStartupScript(this, GetType(), "showPopup", "showPopup();", true);
    }
    protected void btnYes_Click(object sender, EventArgs e)
    {
        string userID = Session["UserID"] as string;
        if (Session["petID"] == null || Session["sellPrice"] == null || string.IsNullOrEmpty(userID))
            return;

        int petID = Convert.ToInt32(Session["petID"]);
        int sellPrice = Convert.ToInt32(Session["sellPrice"]);
        string colourNum = Session["colourNum"] as string;

        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        int currentCoins = 0;
        bool wasEquipped = false;

        using (OleDbConnection con = new OleDbConnection(cs))
        {
            con.Open();

            // CHECK IF SOLD PET WAS EQUIPPED
            string checkEquippedQuery = "SELECT equippedStatus FROM UserPets WHERE userID = ? AND petID = ?";
            using (OleDbCommand cmd = new OleDbCommand(checkEquippedQuery, con))
            {
                cmd.Parameters.AddWithValue("?", userID);
                cmd.Parameters.AddWithValue("?", petID);
                object result = cmd.ExecuteScalar();
                wasEquipped = result != null && Convert.ToBoolean(result);
            }

            // 1. DELETE PET FROM UserPets
            string deleteCommand = "DELETE FROM UserPets WHERE userID = ? AND petID = ?";
            using (OleDbCommand cmd = new OleDbCommand(deleteCommand, con))
            {
                cmd.Parameters.AddWithValue("?", userID);
                cmd.Parameters.AddWithValue("?", petID);
                cmd.ExecuteNonQuery();
            }

            // 2. GET CURRENT coin count
            string getCoinsQuery = "SELECT userCoinCount FROM Users WHERE userID = ?";
            using (OleDbCommand cmd = new OleDbCommand(getCoinsQuery, con))
            {
                cmd.Parameters.AddWithValue("?", userID);
                object result = cmd.ExecuteScalar();
                currentCoins = result != null ? Convert.ToInt32(result) : 0;
            }

            // 3. UPDATE coin count
            int updatedCoins = currentCoins + sellPrice;
            string updateCoins = "UPDATE Users SET userCoinCount = ? WHERE userID = ?";
            using (OleDbCommand cmd = new OleDbCommand(updateCoins, con))
            {
                cmd.Parameters.AddWithValue("?", updatedCoins);
                cmd.Parameters.AddWithValue("?", userID);
                cmd.ExecuteNonQuery();
            }

            if (wasEquipped)
            {
                // GET petID of Cat 1
                int cat1PetID = -1;
                string getCat1ID = "SELECT petID FROM Pet WHERE petType = 'Cat' AND colourNum = 1";
                using (OleDbCommand cmd = new OleDbCommand(getCat1ID, con))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                        cat1PetID = Convert.ToInt32(result);
                }

                // ENSURE CAT 1 IS OWNED BY USER
                bool ownsCat1 = false;
                string checkOwnership = "SELECT COUNT(*) FROM UserPets WHERE userID = ? AND petID = ?";
                using (OleDbCommand cmd = new OleDbCommand(checkOwnership, con))
                {
                    cmd.Parameters.AddWithValue("?", userID);
                    cmd.Parameters.AddWithValue("?", cat1PetID);
                    ownsCat1 = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }

                if (ownsCat1)
                {
                    string unequipAll = "UPDATE UserPets SET equippedStatus = FALSE WHERE userID = ?";
                    using (OleDbCommand cmd = new OleDbCommand(unequipAll, con))
                    {
                        cmd.Parameters.AddWithValue("?", userID);
                        cmd.ExecuteNonQuery();
                    }

                    string equipCat1 = "UPDATE UserPets SET equippedStatus = TRUE WHERE userID = ? AND petID = ?";
                    using (OleDbCommand cmd = new OleDbCommand(equipCat1, con))
                    {
                        cmd.Parameters.AddWithValue("?", userID);
                        cmd.Parameters.AddWithValue("?", cat1PetID);
                        cmd.ExecuteNonQuery();
                    }

                    // UPDATE SESSION FOR HOME PAGE PET PATH
                    Session["EquippedPetImagePath"] = "Images/Cat 1.png";
                }
            }
        }

        Session.Remove("petID");
        Session.Remove("sellPrice");
        Session.Remove("colourNum");

        lblPaws.Text = (currentCoins + sellPrice).ToString();
        LoadOwnedPets(userID);
    }

    protected void btnCats_Click(object sender, EventArgs e)
    {
        Response.Redirect("A1800_View-pets.aspx");
    }

    protected void btnDogs_Click(object sender, EventArgs e)
    {
        Response.Redirect("A1800_View-pets-dogs.aspx");
    }

    protected void btnFuzzy_Click(object sender, EventArgs e)
    {
        Response.Redirect("A1800_View-pets-fuzzy.aspx");
    }

    protected void btnFarm_Click(object sender, EventArgs e)
    {
        Response.Redirect("A1800_View-pets-farm.aspx");
    }

    protected void btnSpecial_Click(object sender, EventArgs e)
    {
        Response.Redirect("A1800_View-pets-special.aspx");
    }

    private string LoadUserData(string username)
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        string uid = GetUserID(username, cs);
        Session["UserID"] = uid;

        if (string.IsNullOrEmpty(uid))
        {
            lblPaws.Text = "N/A (User not found)";
            lblXPAmount.Text = "N/A";
            lblLevelNumber.Text = "N/A";
            return null;
        }

        GetLevelInformation(cs, uid);
        GetUserStats(cs, uid);
        return uid;
    }


    private string GetUserID(string username, string connectionString)
    {
        string query = "SELECT userID FROM Users WHERE username = @username";
        string userID = null;

        using (OleDbConnection con = new OleDbConnection(connectionString))
        {
            using (OleDbCommand cmd = new OleDbCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@username", username);

                try
                {
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        userID = result.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error getting userID: " + ex.Message);
                }
            }
        }
        return userID;
    }

    private void GetLevelInformation(string connectionString, string userID)
    {
        string query = "SELECT levelID FROM CurrentLevel WHERE userID = @userID";

        using (OleDbConnection con = new OleDbConnection(connectionString))
        {
            using (OleDbCommand cmd = new OleDbCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@userID", userID);

                try
                {
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    lblLevelNumber.Text = (result != null) ? result.ToString() : "N/A";
                }
                catch (Exception ex)
                {
                    lblLevelNumber.Text = "ERR";
                    Console.WriteLine("Error getting level: " + ex.Message);
                }
            }
        }
    }

    private void GetUserStats(string connectionString, string userID)
    {
        string query = "SELECT userXP, userCoinCount FROM Users WHERE userID = @userID";

        using (OleDbConnection con = new OleDbConnection(connectionString))
        {
            using (OleDbCommand cmd = new OleDbCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@userID", userID);

                try
                {
                    con.Open();
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblXPAmount.Text = reader["userXP"] != DBNull.Value ? reader["userXP"].ToString() : "0";
                            lblPaws.Text = reader["userCoinCount"] != DBNull.Value ? reader["userCoinCount"].ToString() : "0";
                        }
                        else
                        {
                            lblXPAmount.Text = "N/A";
                            lblPaws.Text = "N/A";
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblXPAmount.Text = "ERR";
                    lblPaws.Text = "ERR";
                    Console.WriteLine("Error getting user data: " + ex.Message);
                }
            }
        }
    }
}