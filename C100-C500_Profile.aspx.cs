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
    protected void Page_Load(object sender, EventArgs e)
    {
        string username = Session["Username"] != null ? Session["Username"].ToString() : "";
        if (string.IsNullOrEmpty(username)) return;

        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        using (OleDbConnection con = new OleDbConnection(cs))
        {
            string query = "SELECT * FROM [Users] WHERE [username] = ?";
            OleDbCommand cmd = new OleDbCommand(query, con);
            cmd.Parameters.AddWithValue("?", username);

            con.Open();
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                txtUsername.Text = reader["username"].ToString();
                txtEmail.Text = reader["email"].ToString();
                txtPassword.Attributes["value"] = "********"; 
            }
            con.Close();
        }
    
    }

    protected void btnBackProfile_Click(object sender, EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }

    protected void txtUsername_TextChanged(object sender, EventArgs e)
    {

    }
    protected void btnDeleteProfile_Click(object sender, EventArgs e)
    {
        string username = Session["Username"] != null ? Session["Username"].ToString() : "";
        if (string.IsNullOrEmpty(username)) return;

        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        using (OleDbConnection con = new OleDbConnection(cs))
        {
            string query = "DELETE FROM [Users] WHERE [username] = ?";
            OleDbCommand cmd = new OleDbCommand(query, con);
            cmd.Parameters.AddWithValue("?", username);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            Session.Clear();
            Response.Redirect("Landing-page.aspx");
        }
    }

    protected void btnLogout_Click(object sender, EventArgs e)
    {
        Response.Redirect("Landing-page.aspx");
    }
}