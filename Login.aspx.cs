using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

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
            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (MySqlConnection con = new MySqlConnection(cs))
            {
                string command = "SELECT * FROM Users WHERE username = ?username AND password = ?password";
                MySqlCommand cmd = new MySqlCommand(command, con);

                cmd.Parameters.AddWithValue("?username", txtUsername.Text);
                string hashedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(txtPassword.Text, "SHA1");
                cmd.Parameters.AddWithValue("?password", hashedPassword);

                con.Open();
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows && reader.Read())
                {
                    int userId = Convert.ToInt32(reader["UserID"]);
                    Session["UserID"] = userId;
                    Session["Username"] = txtUsername.Text;
                    FormsAuthentication.SetAuthCookie(txtUsername.Text, false);
                    Response.Redirect("Default.aspx");
                }
                else
                {
                    pnlLogin.Visible = true;
                    ScriptManager.RegisterStartupScript(this, GetType(), "popup", "showPopup();", true);
                }

                con.Close();
            }
        }
    }
}