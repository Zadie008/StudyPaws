using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("Landing-page.aspx");
    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            Session["Username"] = txtUsername.Text;

            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (OleDbConnection con = new OleDbConnection(cs))
            {
                string command = "SELECT * FROM [Users] WHERE [username] = ? AND [password] = ?";
                OleDbCommand cmd = new OleDbCommand(command, con);

                cmd.Parameters.AddWithValue("?", txtUsername.Text);

                string hashedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(txtPassword.Text, "SHA1");
                cmd.Parameters.AddWithValue("?", hashedPassword);

                con.Open();
                OleDbDataReader collection = cmd.ExecuteReader();

                if (collection.HasRows && collection.Read())
                {
                    FormsAuthentication.SetAuthCookie(txtUsername.Text, false);
                    Response.Redirect("Landing-page.aspx");
                }
                else
                {
                    
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid login. Please try again.');", true);
                }

                con.Close();
            }
        }
    }
}