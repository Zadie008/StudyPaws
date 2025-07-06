using System;
using System.Configuration;
using System.Data.OleDb;
using System.Web.UI;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Username"] != null)
        {
            lblLoggedInUserName.Text = Session["Username"].ToString() + "!";
            LoadUserData(Session["Username"].ToString());
        }
        else
        {
            Response.Redirect("Landing-page.aspx");
            lblLoggedInUserName.Text = "You are not logged in";
            lblPaws.Text = "N/A";
            lblXPAmount.Text = "N/A";
            lblLevelNumber.Text = "N/A";
        }
    }

    private void LoadUserData(string username)
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        string userID = GetUserID(username, cs);

        if (string.IsNullOrEmpty(userID))
        {
            lblPaws.Text = "N/A (User not found)";
            lblXPAmount.Text = "N/A";
            lblLevelNumber.Text = "N/A";
            return;
        }

        GetLevelInformation(cs, userID);
        GetUserStats(cs, userID);
        GetUserProfileIcon(cs, userID);
    }

    private string GetUserID(string username, string connectionString)
    {
        string query = "SELECT userID FROM Users WHERE username = @username";
        string userID = null;

        using (OleDbConnection con = new OleDbConnection(connectionString))
        using (OleDbCommand cmd = new OleDbCommand(query, con))
        {
            cmd.Parameters.AddWithValue("@username", username);
            try
            {
                con.Open();
                object result = cmd.ExecuteScalar();
                if (result != null)
                    userID = result.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting userID: " + ex.Message);
            }
        }
        return userID;
    }

    private void GetLevelInformation(string connectionString, string userID)
    {
        string query = "SELECT levelID FROM CurrentLevel WHERE userID = @userID";

        using (OleDbConnection con = new OleDbConnection(connectionString))
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

    private void GetUserStats(string connectionString, string userID)
    {
        string query = "SELECT userXP, userCoinCount FROM Users WHERE userID = @userID";

        using (OleDbConnection con = new OleDbConnection(connectionString))
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

    private void GetUserProfileIcon(string connectionString, string userID)
    {
        string query = "SELECT iconNum FROM Users WHERE userID = @userID";

        using (OleDbConnection con = new OleDbConnection(connectionString))
        using (OleDbCommand cmd = new OleDbCommand(query, con))
        {
            cmd.Parameters.AddWithValue("@userID", userID);
            try
            {
                con.Open();
                object result = cmd.ExecuteScalar();
                int iconNum;
                if (result != null && int.TryParse(result.ToString(), out iconNum))
                {
                    string iconPath = GetProfileImagePath(iconNum);
                    profilePet.ImageUrl = iconPath;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting profile icon: " + ex.Message);
            }
        }
    }

    private string GetProfileImagePath(int iconNum)
    {
        switch (iconNum)
        {
            case 1: return "~/ProfilePictures/CatPfp.png";
            case 2: return "~/ProfilePictures/DogPfp.png";
            case 3: return "~/ProfilePictures/BunnyPfp.png";
            case 4: return "~/ProfilePictures/CowPfp.png";
            case 5: return "~/ProfilePictures/UnicornPfp.png";
            default: return "~/ProfilePictures/CatPfp.png";
        }
    }
}