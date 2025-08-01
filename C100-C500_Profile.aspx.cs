using System;
using System.Activities.Expressions;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.ServiceModel.Activities;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Default2 : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        string username = ""; 

        if (Session["Username"] != null)
        {
            username = Session["Username"].ToString();
        }

        if (!IsPostBack)
        {
            pnlDeleteProfile.Visible = false;
            pnlLogout.Visible = false;

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
            pnlLogout.Visible = false;
            pnlDeleteProfile.Visible = false;
        }

        LoadUserProfileIcon(username);
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
      pnlLogout.Visible = true;
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
        emailBadge();
    }
    public void emailBadge() //user earns email badge
    {
        string username = "";
        if (Session["Username"] != null)
        {
            username = Session["Username"].ToString();
        }
        if (string.IsNullOrEmpty(username)) return;
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        using (OleDbConnection con = new OleDbConnection(cs))
        {
            con.Open();
            string getUserQuery = "SELECT userID, email FROM [Users] WHERE [username] = ?";
            int userID = -1;
            string userEmail = null;
            using (OleDbCommand getUserCmd = new OleDbCommand(getUserQuery, con))
            {
                getUserCmd.Parameters.AddWithValue("?", username);
                OleDbDataReader reader = getUserCmd.ExecuteReader();
                if (reader.Read())
                {
                    userID = Convert.ToInt32(reader["userID"]);
                    userEmail = reader["email"].ToString();
                }
                reader.Close();
            }
            if (userID != -1 && !string.IsNullOrEmpty(userEmail))
            {
                string checkBadgeQuery = "SELECT COUNT(*) FROM [UserBadge] WHERE [userID] = ? AND [badgeID] = ?";
                int badgeCount = 0;
                using (OleDbCommand checkBadgeCmd = new OleDbCommand(checkBadgeQuery, con))
                {
                    checkBadgeCmd.Parameters.AddWithValue("?", userID);
                    checkBadgeCmd.Parameters.AddWithValue("?", 15);
                    badgeCount = (int)checkBadgeCmd.ExecuteScalar();
                }
                if (badgeCount == 0)
                {
                    string insertBadgeQuery = "INSERT INTO [UserBadge] ([userID], [badgeID], [badgeType]) VALUES (?, ?, ?)";
                    using (OleDbCommand insertBadgeCmd = new OleDbCommand(insertBadgeQuery, con))
                    {
                        insertBadgeCmd.Parameters.AddWithValue("?", userID);
                        insertBadgeCmd.Parameters.AddWithValue("?", 15);
                        insertBadgeCmd.Parameters.AddWithValue("?", "Gold");
                        insertBadgeCmd.ExecuteNonQuery();
                    }
                }
            }
            con.Close();
        }
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

        originalPass = newPassword; 
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
            Response.Write("<script>alert('Error: User session expired or not found.');</script>");
            return;
        }

        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        using (OleDbConnection con = new OleDbConnection(cs))
        {
            con.Open();
            OleDbTransaction transaction = null;

            try
            {
                transaction = con.BeginTransaction();
                Response.Write("Debug: Transaction started.<br/>");

                int userID = -1;
                string getUserIDQuery = "SELECT userID FROM [Users] WHERE [username] = ?";
                using (OleDbCommand cmdGetID = new OleDbCommand(getUserIDQuery, con, transaction))
                {
                    cmdGetID.Parameters.AddWithValue("?", username);

                    object result = cmdGetID.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        userID = Convert.ToInt32(result);
                        Response.Write("Debug: Retrieved UserID: " + userID.ToString() + "<br/>");
                    }
                    else
                    {
                        Response.Write("Debug: UserID not found for username: " + username + "<br/>");
                    }
                }

                if (userID == -1)
                {
                    Response.Write("<script>alert('Error: User not found in database for deletion. Nothing to delete.');</script>");
                    transaction.Rollback();
                    return;
                }

                // Delete FriendsList (as userIDfrom)
                string deleteFriendsListQuery1 = "DELETE FROM [FriendsList] WHERE [userIDfrom] = ?";
                using (OleDbCommand cmd = new OleDbCommand(deleteFriendsListQuery1, con, transaction))
                {
                    cmd.Parameters.AddWithValue("?", userID);
                    int rows = cmd.ExecuteNonQuery();
                    Response.Write("Debug: Deleted " + rows.ToString() + " rows from FriendsList (userIDfrom).<br/>");
                }

                // Delete FriendsList (as userIDto)
                string deleteFriendsListQuery2 = "DELETE FROM [FriendsList] WHERE [userIDto] = ?";
                using (OleDbCommand cmd = new OleDbCommand(deleteFriendsListQuery2, con, transaction))
                {
                    cmd.Parameters.AddWithValue("?", userID);
                    int rows = cmd.ExecuteNonQuery();
                    Response.Write("Debug: Deleted " + rows.ToString() + " rows from FriendsList (userIDto).<br/>");
                }

                // Delete FriendRequest (as userID)
                string deleteFriendRequestQuery1 = "DELETE FROM [FriendRequest] WHERE [userID] = ?";
                using (OleDbCommand cmd = new OleDbCommand(deleteFriendRequestQuery1, con, transaction))
                {
                    cmd.Parameters.AddWithValue("?", userID);
                    int rows = cmd.ExecuteNonQuery();
                    Response.Write("Debug: Deleted " + rows.ToString() + " rows from FriendRequest (userID).<br/>");
                }

                // Delete UserBadge
                string deleteUserBadgeQuery = "DELETE FROM [UserBadge] WHERE [userID] = ?";
                using (OleDbCommand cmd = new OleDbCommand(deleteUserBadgeQuery, con, transaction))
                {
                    cmd.Parameters.AddWithValue("?", userID);
                    int rows = cmd.ExecuteNonQuery();
                    Response.Write("Debug: Deleted " + rows.ToString() + " rows from UserBadge.<br/>");
                }

                // Delete UserPets
                string deleteUserPetsQuery = "DELETE FROM [UserPets] WHERE [userID] = ?";
                using (OleDbCommand cmd = new OleDbCommand(deleteUserPetsQuery, con, transaction))
                {
                    cmd.Parameters.AddWithValue("?", userID);
                    int rows = cmd.ExecuteNonQuery();
                    Response.Write("Debug: Deleted " + rows.ToString() + " rows from UserPets.<br/>");
                }

                // Delete CurrentLevel
                string deleteCurrentLevelQuery = "DELETE FROM [CurrentLevel] WHERE [userID] = ?";
                using (OleDbCommand cmd = new OleDbCommand(deleteCurrentLevelQuery, con, transaction))
                {
                    cmd.Parameters.AddWithValue("?", userID);
                    int rows = cmd.ExecuteNonQuery();
                    Response.Write("Debug: Deleted " + rows.ToString() + " rows from CurrentLevel.<br/>");
                }

                // Delete CalendarEvent
                string deleteCalendarEventQuery = "DELETE FROM [CalendarEvent] WHERE [userID] = ?";
                using (OleDbCommand cmd = new OleDbCommand(deleteCalendarEventQuery, con, transaction))
                {
                    cmd.Parameters.AddWithValue("?", userID);
                    int rows = cmd.ExecuteNonQuery();
                    Response.Write("Debug: Deleted " + rows.ToString() + " rows from CalendarEvent.<br/>");
                }

                // Delete  StudySessionParticipants
                string deleteStudySessionParticipationQuery = "DELETE FROM [StudySessionParticipants] WHERE [userID] = ?";
                using (OleDbCommand cmd = new OleDbCommand(deleteStudySessionParticipationQuery, con, transaction))
                {
                    cmd.Parameters.AddWithValue("?", userID);
                    int rows = cmd.ExecuteNonQuery();
                    Response.Write("Debug: Deleted " + rows.ToString() + " rows from StudySessionParticipa...<br/>");
                }

                // Delete Timer
                string deleteTimerQuery = "DELETE FROM [Timer] WHERE [userID] = ?";
                using (OleDbCommand cmd = new OleDbCommand(deleteTimerQuery, con, transaction))
                {
                    cmd.Parameters.AddWithValue("?", userID);
                    int rows = cmd.ExecuteNonQuery();
                    Response.Write("Debug: Deleted " + rows.ToString() + " rows from Timer.<br/>");
                }

                // Delete ToDoListTask
                string deleteToDoListTaskQuery = "DELETE FROM [ToDoListTask] WHERE [userID] = ?";
                using (OleDbCommand cmd = new OleDbCommand(deleteToDoListTaskQuery, con, transaction))
                {
                    cmd.Parameters.AddWithValue("?", userID);
                    int rows = cmd.ExecuteNonQuery();
                    Response.Write("Debug: Deleted " + rows.ToString() + " rows from ToDoListTask.<br/>");
                }

                //  delete  Users
                string deleteUserQuery = "DELETE FROM [Users] WHERE [userID] = ?";
                using (OleDbCommand cmdDeleteUser = new OleDbCommand(deleteUserQuery, con, transaction))
                {
                    cmdDeleteUser.Parameters.AddWithValue("?", userID);

                    int rowsAffected = cmdDeleteUser.ExecuteNonQuery();
                    Response.Write("Debug: Deleted " + rowsAffected.ToString() + " rows from Users table.<br/>");

                    if (rowsAffected > 0)
                    {
                        transaction.Commit();
                        Response.Write("Debug: Transaction committed successfully.<br/>");

                        Session.Clear();
                        Session.Abandon();
                        FormsAuthentication.SignOut();
                        Response.Redirect("Landing-page.aspx");
                    }
                    else
                    {
                        Response.Write("<script>alert('Error: User record not found in Users table for deletion. Transaction rolled back.');</script>");
                        transaction.Rollback();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('An error occurred during profile deletion: " + ex.Message.Replace("'", "\\'") + "');</script>");
                Response.Write("Debug: Full Error: " + ex.ToString() + "<br/>");

                if (transaction != null)
                {
                    transaction.Rollback();
                    Response.Write("Debug: Transaction rolled back due to error.<br/>");
                }
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                    Response.Write("Debug: Connection closed.<br/>");
                }
            }
        }
    }

    protected void btnCancelDelete_Click(object sender, EventArgs e)
    {
        pnlDeleteProfile.Visible = false;
    }

    protected void btnGoodbye_Click(object sender, EventArgs e)
    {
        Session.Clear();
        Session.Abandon();
        Response.Redirect("Landing-page.aspx");
    }
    private void LoadUserProfileIcon(string username)
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        using (OleDbConnection con = new OleDbConnection(cs))
        {
            string query = "SELECT iconNum FROM Users WHERE username = ?";
            OleDbCommand cmd = new OleDbCommand(query, con);
            cmd.Parameters.AddWithValue("?", username);

            try
            {
                con.Open();
                object result = cmd.ExecuteScalar();
                int iconNum;
                if (result != null && int.TryParse(result.ToString(), out iconNum))
                {
                    string imageUrl = GetProfileImagePath(iconNum);
                    string circleClass = GetCircleClass(iconNum);

                    profilePet.ImageUrl = imageUrl;
                    profileCircle.Attributes["class"] = "profileCircle " + circleClass;
                }
                else
                {
                    profilePet.ImageUrl = "~/Images/ProfilePictures/CatPfp.png";
                    profileCircle.Attributes["class"] = "profileCircle circle-cat";
                }
            }
            catch
            {
                profilePet.ImageUrl = "~/Images/ProfilePictures/CatPfp.png";
                profileCircle.Attributes["class"] = "profileCircle circle-cat";
            }
        }
    }
    private string GetProfileImagePath(int iconNum)
    {
        switch (iconNum)
        {
            case 1: return "~/Images/ProfilePictures/CatPfp.png";
            case 2: return "~/Images/ProfilePictures/DogPfp.png";
            case 3: return "~/Images/ProfilePictures/BunnyPfp.png";
            case 4: return "~/Images/ProfilePictures/CowPfp.png";
            case 5: return "~/Images/ProfilePictures/UnicornPfp.png";
            default: return "~/Images/ProfilePictures/CatPfp.png";
        }
    }
    private string GetCircleClass(int iconNum)
    {
        switch (iconNum)
        {
            case 1: return "circle-cat";
            case 2: return "circle-dog";
            case 3: return "circle-bunny";
            case 4: return "circle-cow";
            case 5: return "circle-unicorn";
            default: return "circle-cat";
        }
    }

   

    //protected void btnPress_Click(object sender, EventArgs e)
    //{
    //    string username = Session["Username"] != null ? Session["Username"].ToString() : "";

    //    if (string.IsNullOrEmpty(username))
    //    {
    //        Response.Write("<script>alert('Error: User session not found.');</script>");
    //        return;
    //    }

    //    string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
    //    using (OleDbConnection con = new OleDbConnection(cs))
    //    {
    //            con.Open();
    //            string selectQuery = "SELECT userXP FROM [Users] WHERE [username] = ?";
    //            OleDbCommand selectCmd = new OleDbCommand(selectQuery, con);
    //            selectCmd.Parameters.AddWithValue("?", username);

    //            object xpObj = selectCmd.ExecuteScalar();
    //            int currentXP = (xpObj != null && xpObj != DBNull.Value) ? Convert.ToInt32(xpObj) : 0;
    //            int newXP = currentXP + 10;
    //            // Update XP
    //            string updateQuery = "UPDATE [Users] SET userXP = ? WHERE username = ?";
    //            OleDbCommand updateCmd = new OleDbCommand(updateQuery, con);
    //            updateCmd.Parameters.AddWithValue("?", newXP);
    //            updateCmd.Parameters.AddWithValue("?", username);
    //            updateCmd.ExecuteNonQuery();
    //    }
    //}
}
