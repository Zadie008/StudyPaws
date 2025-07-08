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
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["Username"] != null)
            {
                LoadUserData(Session["Username"].ToString());
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
}