using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

public partial class C100_Register : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        
    }

    [WebMethod]
    public static bool CheckUsernameExists(string username)
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        using (OleDbConnection con = new OleDbConnection(cs))
        {
            string checkQuery = "SELECT COUNT(*) FROM [Users] WHERE [username] = ?";
            OleDbCommand checkCmd = new OleDbCommand(checkQuery, con);
            checkCmd.Parameters.AddWithValue("?", username);

            try
            {
                con.Open();
                int userCount = (int)checkCmd.ExecuteScalar();
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
        using (OleDbConnection con = new OleDbConnection(cs))
        {
            string checkQuery = "SELECT COUNT(*) FROM [Users] WHERE [username] = ?";
            OleDbCommand checkCmd = new OleDbCommand(checkQuery, con);
            checkCmd.Parameters.AddWithValue("?", username);

            try
            {
                con.Open();
                int userCount = (int)checkCmd.ExecuteScalar();
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

        using (OleDbConnection con = new OleDbConnection(cs))
        {
            try
            {
                con.Open();
                string checkQuery = "SELECT COUNT(*) FROM [Users] WHERE [username] = ?";
                OleDbCommand checkCmd = new OleDbCommand(checkQuery, con);
                checkCmd.Parameters.AddWithValue("?", username);
                int userCount = (int)checkCmd.ExecuteScalar();

                if (userCount > 0)
                {
                    pnlProfileExists.Visible = true;
                    pnlConfirm.Visible = false;
                    pnlTut.Visible = false;
                    return; 
                }

                string insertQuery = "INSERT INTO [Users] ([username], [password], [iconNum], [collectedPets]) VALUES (?, ?, ?, ?)";
                OleDbCommand insertCmd = new OleDbCommand(insertQuery, con);
                insertCmd.Parameters.AddWithValue("?", username);
                insertCmd.Parameters.AddWithValue("?", hashedPassword);
                insertCmd.Parameters.AddWithValue("?", 1); // iconNum
                insertCmd.Parameters.AddWithValue("?", 1); // user always starts with 1 collected pet

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
                string getUserIdQuery = "SELECT userID FROM [Users] WHERE [username] = ?";
                OleDbCommand getUserIdCmd = new OleDbCommand(getUserIdQuery, con);
                getUserIdCmd.Parameters.AddWithValue("?", username);

                object result = getUserIdCmd.ExecuteScalar();
                if (result != null)
                {
                    int userID = Convert.ToInt32(result);

                    string petQuery = "INSERT INTO [UserPets] ([userID], [petID], [equippedStatus]) VALUES (?, ?, ?)";
                    OleDbCommand insertCmd2 = new OleDbCommand(petQuery, con);
                    insertCmd2.Parameters.AddWithValue("?", userID);
                    insertCmd2.Parameters.AddWithValue("?", 1); // give them Cat 1
                    insertCmd2.Parameters.AddWithValue("?", true); // equip Cat 1

                    insertCmd2.ExecuteNonQuery();

                    string levelQuery = "INSERT INTO [CurrentLevel] ([userID], [levelID]) VALUES (?, ?)";
                    OleDbCommand insertCmd3 = new OleDbCommand(levelQuery, con);
                    insertCmd3.Parameters.AddWithValue("?", userID);
                    insertCmd3.Parameters.AddWithValue("?", 1); // level 1

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
