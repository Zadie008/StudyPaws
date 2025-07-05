using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Default2 : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            pnlDeleteProfile.Visible = false;

            string username = "";
            if (Session["Username"] != null)
            {
                username = Session["Username"].ToString();
            }
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

                    originalUsername = reader["username"].ToString();
                    originalPass = reader["password"].ToString();
                    originalEmail = reader["email"].ToString(); 
                }
                con.Close();
            }
        }
        else
        {
            pnlDeleteProfile.Visible = false;
        }
    }

    protected void btnBackProfile_Click(object sender, EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }

    protected void txtUsername_TextChanged(object sender, EventArgs e)
    {
        txtUsername.ReadOnly = false;
        btnEditUser.Visible = false;
        btnSaveUser.Visible = true;
        btnCancelUser.Visible = true;
    }
   

    protected void btnLogout_Click(object sender, EventArgs e)
    {
        Response.Redirect("Landing-page.aspx");
    }
    private string originalEmail
    {
        get { return ViewState["originalEmail"] as string; }
        set { ViewState["originalEmail"] = value; }
    }

    private string originalPass
    {
        get { return ViewState["originalPass"] as string; }
        set { ViewState["originalPass"] = value; }
    }

    private string originalUsername
    {
        get { return ViewState["originalUsername"] as string; }
        set { ViewState["originalUsername"] = value; }
    }

    protected void btnEditEmail_Click(object sender, EventArgs e)
    {
        txtEmail.ReadOnly = false;
        btnEditEmail.Visible = false;
        btnSaveEmail.Visible = true;
        btnCancelEmail.Visible = true;
    }

    protected void btnCancelEmail_Click(object sender, EventArgs e)
    {
        txtEmail.Text = originalEmail;
        txtEmail.ReadOnly = true;
        btnEditEmail.Visible = true;
        btnSaveEmail.Visible = false;
        btnCancelEmail.Visible = false;
    }

    protected void btnSaveEmail_Click(object sender, EventArgs e)
    {
        string newEmail = txtEmail.Text.Trim();
        string username = "";
        if (Session["Username"] != null)
        {
            username = Session["Username"].ToString();
        }
        if (string.IsNullOrEmpty(username)) return;

        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        using (OleDbConnection con = new OleDbConnection(cs))
        {
            string query = "UPDATE [Users] SET [email] = ? WHERE [username] = ?";
            OleDbCommand cmd = new OleDbCommand(query, con);
            cmd.Parameters.AddWithValue("?", newEmail);
            cmd.Parameters.AddWithValue("?", username);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        txtEmail.ReadOnly = true;
        btnEditEmail.Visible = true;
        btnSaveEmail.Visible = false;
        btnCancelEmail.Visible = false;

        originalEmail = newEmail; 
    }

    protected void btnCancelUser_Click(object sender, EventArgs e)
    {
        txtUsername.Text = originalUsername;
        txtUsername.ReadOnly = true;
        btnEditUser.Visible = true;
        btnSaveUser.Visible = false;
        btnCancelUser.Visible = false;
    }

    protected void btnCancelPass_Click(object sender, EventArgs e)
    {
        txtPassword.Text = originalPass;
        txtPassword.ReadOnly = true;
        btnEditPass.Visible = true;
        btnSavePass.Visible = false;
        btnCancelPass.Visible = false;
    }

    protected void btnSavePass_Click(object sender, EventArgs e)
    {
        string newPassword = txtPassword.Text.Trim();
        string username = "";
        if (Session["Username"] != null)
        {
            username = Session["Username"].ToString();
        }
        if (string.IsNullOrEmpty(username)) return;

        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        using (OleDbConnection con = new OleDbConnection(cs))
        {
            string query = "UPDATE [Users] SET [password] = ? WHERE [username] = ?";
            OleDbCommand cmd = new OleDbCommand(query, con);

            // Hash the password to match login format
            string hashedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(newPassword, "SHA1");

            cmd.Parameters.AddWithValue("?", hashedPassword);
            cmd.Parameters.AddWithValue("?", username);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        txtPassword.ReadOnly = true;
        btnEditPass.Visible = true;
        btnSavePass.Visible = false;
        btnCancelPass.Visible = false;

        originalPass = newPassword; // (Optional: you may want to hash this too if you compare it)
    }

    protected void btnSaveUser_Click(object sender, EventArgs e)
    {
        string newUsername = txtUsername.Text.Trim();
        string currentUsername = "";
        if (Session["Username"] != null)
        {
            currentUsername = Session["Username"].ToString();
        }
        if (string.IsNullOrEmpty(currentUsername)) return;

        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        using (OleDbConnection con = new OleDbConnection(cs))
        {
            string query = "UPDATE [Users] SET [username] = ? WHERE [username] = ?";
            OleDbCommand cmd = new OleDbCommand(query, con);
            cmd.Parameters.AddWithValue("?", newUsername);
            cmd.Parameters.AddWithValue("?", currentUsername);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        // Update the session with the new username
        Session["Username"] = newUsername;

        txtUsername.ReadOnly = true;
        btnEditUser.Visible = true;
        btnSaveUser.Visible = false;
        btnCancelUser.Visible = false;

        originalUsername = newUsername;
    }

    protected void btnEditPass_Click(object sender, EventArgs e)
    {
        txtPassword.ReadOnly = false;
        btnEditPass.Visible = false;
        btnSavePass.Visible = true;
        btnCancelPass.Visible = true;
    }
    protected void btnEditUser_Click(object sender, EventArgs e)
    {
        txtUsername.ReadOnly = false;
        btnEditUser.Visible = false;
        btnSaveUser.Visible = true;
        btnCancelUser.Visible = true;
    }
    protected void txtPassword_TextChanged(object sender, EventArgs e)
    {
        // You can leave this empty or remove the OnTextChanged from the ASPX if not needed
    }

    protected void btnChangeIcon_Click(object sender, EventArgs e)
    {
        Response.Redirect("ChangeProfilePhoto.aspx");
    }

    protected void btnLogout_Click1(object sender, EventArgs e)
    {
        Response.Redirect("Landing-page.aspx");
    }

    protected void deleteImageButton_Click(object sender, ImageClickEventArgs e)
    {
       pnlDeleteProfile.Visible = true;

       
    }

    protected void btnConfirmDeleteProfile_Click(object sender, EventArgs e)
    {
        string username = "";
        if (Session["Username"] != null)
        {
            username = Session["Username"].ToString();
        }

        if (string.IsNullOrEmpty(username))
        {
            return; 
        }

        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        using (OleDbConnection con = new OleDbConnection(cs))
        {
            string deleteQuery = "DELETE FROM [Users] WHERE [username] = ?";
            OleDbCommand cmd = new OleDbCommand(deleteQuery, con);
            cmd.Parameters.AddWithValue("?", username);

            con.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            con.Close();

            if (rowsAffected > 0)
            {
                Session.Clear();
                Session.Abandon();
                Response.Redirect("Landing-page.aspx");
            }
            else
            {
               
            }
        }
    }
    protected void btnCancelDelete_Click(object sender, EventArgs e)
    {
        pnlDeleteProfile.Visible = false;
    }

}
