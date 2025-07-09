using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Default2 : System.Web.UI.Page
{
    // update the session var for equipped pet image path once the user equips a new one!!!!!!!!!!!!

    private string userID;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["Username"] != null)
            {
                string username = Session["Username"].ToString();
                LoadUserData(username);
                LoadOwnedPets(userID);
            }
        }
    }

    private void LoadOwnedPets(string userID)
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        string command = "SELECT UserPets.petID AND Pet.colourNum FROM UserPets INNER JOIN Pet ON UserPets.petID = Pet.petID WHERE UserPets.userID = @userID AND Pet.petType = @petType";

        List<int> ownedPetColours = new List<int>();

        using (OleDbConnection con = new OleDbConnection(cs))
        using (OleDbCommand cmd = new OleDbCommand(command, con))
        {
            cmd.Parameters.AddWithValue("@userID", userID);
            cmd.Parameters.AddWithValue("@petType", "Cat");

            try
            {
                con.Open();
                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["colourNum"] != DBNull.Value)
                        {
                            ownedPetColours.Add(Convert.ToInt32(reader["colourNum"]));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading owned pets: " + ex.Message);
            }
        }

        // loop through the colourNum values 1-5
        for (int i = 1; i <= 5; i++)
        {
            Image petImg = (Image)FindControlRecursive(this, "mainContentPlaceHolder_imgPet" + i);
            Button selectBtn = (Button)FindControlRecursive(this, "mainContentPlaceHolder_btnSelect" + i);

            if (petImg != null && selectBtn != null)
            {
                bool owned = ownedPetColours.Contains(i);
                petImg.Visible = owned;
                selectBtn.Visible = owned;
            }
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

    private void LoadUserData(string username)
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        userID = GetUserID(username, cs);

        if (string.IsNullOrEmpty(userID))
        {
            lblPaws.Text = "N/A (User not found)";
            lblXPAmount.Text = "N/A";
            lblLevelNumber.Text = "N/A";
            return;
        }

        GetLevelInformation(cs, userID);
        GetUserStats(cs, userID);
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