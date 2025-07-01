using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Default2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["Username"] != null)
            {
                LoadUserData(Session["Username"].ToString());
                LoadFriendList(Session["Username"].ToString());
            }
        }
    }
    private void LoadUserData(string username)
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        string userID = GetUserID(username, cs);

        if (userID == null || userID == "")
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
                            lblXPAmount.Text = reader["userXP"] != DBNull.Value ?
                                reader["userXP"].ToString() : "0";
                            lblPaws.Text = reader["userCoinCount"] != DBNull.Value ?
                                reader["userCoinCount"].ToString() : "0";
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
    protected string GetProfilePictureUrl(string userID)
    {
        // Implement logic to get profile picture URL
        // This could come from a UserProfile table or similar
        return "Images/default-profile.png"; // Default image
    }

    private void LoadFriendList(string username)
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        string userID = GetUserID(username, cs);

        if (!string.IsNullOrEmpty(userID))
        {
            string query = @"
                SELECT 
                    f.friend3hipID, 
                    f.userID1, 
                    f.userID2, 
                    f.nickname,
                    CASE 
                        WHEN f.userID1 = @userID THEN u2.username
                        ELSE u1.username
                    END AS friendUsername
                FROM 
                    FriendList AS f
                INNER JOIN 
                    Users AS u1 ON f.userID1 = u1.userID
                INNER JOIN 
                    Users AS u2 ON f.userID2 = u2.userID
                WHERE 
                    f.userID1 = @userID OR f.userID2 = @userID";

            DataTable dtFriends = new DataTable();

            using (OleDbConnection con = new OleDbConnection(cs))
            {
                using (OleDbCommand cmd = new OleDbCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@userID", userID);

                    try
                    {
                        con.Open();
                        using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
                        {
                            da.Fill(dtFriends);
                        }

                    //    rptFriends.DataSource = dtFriends;
                    //    rptFriends.DataBind();
                    }
                    catch (Exception ex)
                    {
                        // Handle error
                        Console.WriteLine("Error loading friend list: " + ex.Message);
                    }
                }
            }
        }
    }

    protected void rptFriends_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Edit")
        {
            // Handle edit nickname
            string friendShipID = e.CommandArgument.ToString();
            // Implement edit logic
        }
        else if (e.CommandName == "Delete")
        {
            // Handle delete friend
            string friendShipID = e.CommandArgument.ToString();
            DeleteFriendShip(friendShipID);
            LoadFriendList(Session["Username"].ToString());
        }
    }

    private void DeleteFriendShip(string friendShipID)
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        string query = "DELETE FROM FriendList WHERE friend3hipID = @friendShipID";

        using (OleDbConnection con = new OleDbConnection(cs))
        {
            using (OleDbCommand cmd = new OleDbCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@friendShipID", friendShipID);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    // Handle error
                    Console.WriteLine("Error deleting friendship: " + ex.Message);
                }
            }
        }
    }

    protected void btnSearchFriends_Click(object sender, EventArgs e)
    {
        // Implement search functionality
        Response.Redirect("SearchFriends.aspx");
    }

    // Keep your existing methods (GetUserID, GetLevelInformation, GetUserStats) here
    // ...
}
