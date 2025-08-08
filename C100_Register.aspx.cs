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

    [WebMethod]
    public static bool CheckUsernameExists(string username)
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        using (MySqlConnection con = new MySqlConnection(cs))
        {
            string checkQuery = "SELECT COUNT(*) FROM Users WHERE username = @username";
            MySqlCommand checkCmd = new MySqlCommand(checkQuery, con);
            checkCmd.Parameters.AddWithValue("@username", username);

            try
            {
                con.Open();
                int userCount = Convert.ToInt32(checkCmd.ExecuteScalar());
                return userCount > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database error checking username: " + ex.Message);
                return false;
            }
        }
    }

    protected void cvUsername_ServerValidate(object source, ServerValidateEventArgs args)
    {
        string username = txtUsername.Text.Trim();
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        using (MySqlConnection con = new MySqlConnection(cs))
        {
            string checkQuery = "SELECT COUNT(*) FROM Users WHERE username = @username";
            MySqlCommand checkCmd = new MySqlCommand(checkQuery, con);
            checkCmd.Parameters.AddWithValue("@username", username);

            try
            {
                con.Open();
                int userCount = Convert.ToInt32(checkCmd.ExecuteScalar());
                args.IsValid = (userCount == 0);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database error during CustomValidator: " + ex.Message);
                args.IsValid = false;
            }
        }
    }

    protected void btnRegister_Click1(object sender, EventArgs e)
    {
        if (txtPassword.Text != txtConfirmPassword.Text)
        {
            lblPasswordMismatch.Visible = true;
            return;
        }
        else
        {
            lblPasswordMismatch.Visible = false;
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
                    pnlProfileExists.Visible = true;
                    pnlConfirm.Visible = false;
                    pnlTut.Visible = false;
                    return;
                }

                string insertQuery = "INSERT INTO Users (username, password, iconNum, collectedPets) VALUES (@username, @password, @iconNum, @collectedPets)";
                MySqlCommand insertCmd = new MySqlCommand(insertQuery, con);
                insertCmd.Parameters.AddWithValue("@username", username);
                insertCmd.Parameters.AddWithValue("@password", hashedPassword);
                insertCmd.Parameters.AddWithValue("@iconNum", 1); // iconNum
                insertCmd.Parameters.AddWithValue("@collectedPets", 1); // user always starts with 1 collected pet

                int rowsAffected = insertCmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    pnlConfirm.Visible = true;
                    pnlTut.Visible = false;
                    pnlProfileExists.Visible = false;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "registerFail", "alert('Something went wrong during registration. Please try again.');", true);
                }

                // fetch userID
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
                    insertCmd2.Parameters.AddWithValue("@petID", 1); // give them Cat 1
                    insertCmd2.Parameters.AddWithValue("@equippedStatus", true); // equip Cat 1

                    insertCmd2.ExecuteNonQuery();

                    string levelQuery = "INSERT INTO CurrentLevel (userID, levelID) VALUES (@userID, @levelID)";
                    MySqlCommand insertCmd3 = new MySqlCommand(levelQuery, con);
                    insertCmd3.Parameters.AddWithValue("@userID", userID);
                    insertCmd3.Parameters.AddWithValue("@levelID", 1); // level 1

                    insertCmd3.ExecuteNonQuery();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "userIdError", "alert('Could not retrieve userID for new user.');", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "dbError", "alert('A database error occurred: " + ex.Message + "');", true);
                System.Diagnostics.Debug.WriteLine("Registration error: " + ex.Message);
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("Landing-page.aspx");
    }

    protected void btnOkay_Click(object sender, EventArgs e)
    {
        pnlConfirm.Visible = false;
        pnlTut.Visible = true;
    }

    protected void btnWatchtut_Click(object sender, EventArgs e) //Watching the tutorial need to still do
    {
        pnlTut.Visible = false;
        Response.Redirect("Default.aspx");
    }

    protected void BtnNotut_Click(object sender, EventArgs e) //Not watching the tutorial
    {
        pnlTut.Visible = false;
        Response.Redirect("Default.aspx");
    }

    protected void btnUnderstandExists_Click(object sender, EventArgs e) //must direct them back to the page
    {
        pnlProfileExists.Visible = false;
    }
}