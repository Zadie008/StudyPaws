using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class View_Pets_fuzzy : System.Web.UI.Page
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
            cmd.Parameters.AddWithValue("?", "Fuzzy"); // PET TYPE~~~~

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
                petImg.ImageUrl = string.Format("Images/Fuzzy {0}.png", pet.ColourNum); // PET TYPE~~~~
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
            string getPetIDQuery = "SELECT petID FROM Pet WHERE petType = 'Fuzzy' AND colourNum = ?"; // PET TYPE~~~~

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
        Session["EquippedPetImagePath"] = string.Format("Images/Fuzzy {0}.png", selectedColourNum); // PET TYPE~~~~

        // reload pets to reflect new equipped status
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