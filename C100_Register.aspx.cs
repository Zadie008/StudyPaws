using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

public partial class C100_Register : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        

    }


    protected void btnRegister_Click1(object sender, EventArgs e)
    {
        if (txtPassword.Text != txtConfirmPassword.Text)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "passwordMismatch", "alert('Passwords do not match.');", true);
            return;
        }

        string username = txtUsername.Text.Trim();
        string hashedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(txtPassword.Text, "SHA1");
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        using (OleDbConnection con = new OleDbConnection(cs))
        {
            string checkQuery = "SELECT COUNT(*) FROM [Users] WHERE [username] = ?";
            OleDbCommand checkCmd = new OleDbCommand(checkQuery, con);
            checkCmd.Parameters.AddWithValue("?", username);

            con.Open();
            int userCount = (int)checkCmd.ExecuteScalar();

            if (userCount > 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "userExists", "alert('Username already exists.');", true);
                con.Close();
                return;
            }

            string insertQuery = "INSERT INTO [Users] ([username], [password]) VALUES (?, ?)";
            OleDbCommand insertCmd = new OleDbCommand(insertQuery, con);
            insertCmd.Parameters.AddWithValue("?", username);
            insertCmd.Parameters.AddWithValue("?", hashedPassword);

            int rowsAffected = insertCmd.ExecuteNonQuery();
            con.Close();

            if (rowsAffected > 0)
            {
                pnlConfirm.Visible = true;
                ScriptManager.RegisterStartupScript(this, GetType(), "popup", "showPopup();", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "registerFail", "alert('Something went wrong. Please try again.');", true);
            }

          
        }
        pnlTut.Visible = true;
    }
    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("Landing-page.aspx");
    }
}