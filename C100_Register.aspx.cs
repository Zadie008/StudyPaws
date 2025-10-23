using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

public partial class C100_Register : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void btnRegister_Click1(object sender, EventArgs e)
    {
        if (txtPassword.Text != txtConfirmPassword.Text)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowPasswordMismatch", "showPasswordMismatch();", true);
            return;
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "HidePasswordMismatch", "hidePasswordMismatch();", true);
        }

        if (!Page.IsValid)
        {
            return;
        }

        string username = txtUsername.Text.Trim();
        string hashedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(txtPassword.Text, "SHA1");
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        using (MySqlConnection con = new MySqlConnection(cs))
        {
            try
            {
                con.Open();
                string checkQuery = "SELECT COUNT(*) FROM Users WHERE username = @username";
                MySqlCommand checkCmd = new MySqlCommand(checkQuery, con);
                checkCmd.Parameters.AddWithValue("@username", username);
                int userCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (userCount > 0)
                {
                    // Username exists - show error panel using JavaScript
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowProfileExists",
                        "hideAllPanels(); showPanel('" + pnlProfileExists.ClientID + "');", true);
                    return;
                }
                string insertQuery = "INSERT INTO Users (username, password, iconNum, collectedPets) VALUES (@username, @password, @iconNum, @collectedPets)";
                MySqlCommand insertCmd = new MySqlCommand(insertQuery, con);
                insertCmd.Parameters.AddWithValue("@username", username);
                insertCmd.Parameters.AddWithValue("@password", hashedPassword);
                insertCmd.Parameters.AddWithValue("@iconNum", 1);
                insertCmd.Parameters.AddWithValue("@collectedPets", 1);

                int rowsAffected = insertCmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowConfirm",
                        "hideAllPanels(); showPanel('" + pnlConfirm.ClientID + "');", true);

                    string getUserIdQuery = "SELECT userID FROM Users WHERE username = @username";
                    MySqlCommand getUserIdCmd = new MySqlCommand(getUserIdQuery, con);
                    getUserIdCmd.Parameters.AddWithValue("@username", username);

                    object result = getUserIdCmd.ExecuteScalar();
                    if (result != null)
                    {
                        int userID = Convert.ToInt32(result);
                        string petQuery = "INSERT INTO UserPets (userID, petID, equippedStatus) VALUES (@userID, @petID, @equippedStatus)";
                        MySqlCommand insertCmd2 = new MySqlCommand(petQuery, con);
                        insertCmd2.Parameters.AddWithValue("@userID", userID);
                        insertCmd2.Parameters.AddWithValue("@petID", 1);
                        insertCmd2.Parameters.AddWithValue("@equippedStatus", true);
                        insertCmd2.ExecuteNonQuery();
                        string levelQuery = "INSERT INTO CurrentLevel (userID, levelID) VALUES (@userID, @levelID)";
                        MySqlCommand insertCmd3 = new MySqlCommand(levelQuery, con);
                        insertCmd3.Parameters.AddWithValue("@userID", userID);
                        insertCmd3.Parameters.AddWithValue("@levelID", 1);
                        insertCmd3.ExecuteNonQuery();
                        Session["UserID"] = userID;
                        Session["Username"] = username;
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "registerFail",
                        "alert('Something went wrong during registration. Please try again.');", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "dbError",
                    "alert('A database error occurred: " + ex.Message.Replace("'", "''") + "');", true);
            }
        }
    }
    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("Landing-page.aspx");
    }
    protected void btnWatchtut_Click(object sender, EventArgs e)
    {
        Response.Redirect("Tutorial.aspx");
    }
    protected void BtnNotut_Click(object sender, EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }
}