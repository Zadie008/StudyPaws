using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

public partial class Default2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.IsAuthenticated)
        {
            var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                Session["UserID"] = ticket.UserData;
            }
        }

        string username = "";

        if (Session["Username"] != null)
        {
            username = Session["Username"].ToString();
        }

        if (!IsPostBack)
        {
            if (string.IsNullOrEmpty(username)) return;

            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(cs))
            {
                string query = "SELECT * FROM Users WHERE username = @username";
                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.Parameters.AddWithValue("@username", username);

                con.Open();
                MySqlDataReader reader = cmd.ExecuteReader();
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
            LoadUpcomingSessions();
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
        using (MySqlConnection con = new MySqlConnection(cs))
        {
            string query = "UPDATE Users SET email = @email WHERE username = @username";
            MySqlCommand cmd = new MySqlCommand(query, con);
            cmd.Parameters.AddWithValue("@email", newEmail);
            cmd.Parameters.AddWithValue("@username", username);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        using (MySqlConnection con = new MySqlConnection(cs))
        {
            string query = "UPDATE Users SET addedEmailAddress = 1 WHERE username = @username";
            MySqlCommand cmd = new MySqlCommand(query, con);
            cmd.Parameters.AddWithValue("@username", username);

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
        using (MySqlConnection con = new MySqlConnection(cs))
        {
            con.Open();
            string getUserQuery = "SELECT userID, email FROM Users WHERE username = @username";
            int userID = -1;
            string userEmail = null;
            using (MySqlCommand getUserCmd = new MySqlCommand(getUserQuery, con))
            {
                getUserCmd.Parameters.AddWithValue("@username", username);
                MySqlDataReader reader = getUserCmd.ExecuteReader();
                if (reader.Read())
                {
                    userID = Convert.ToInt32(reader["userID"]);
                    userEmail = reader["email"].ToString();
                }
                reader.Close();
            }

           
            if (userID != -1 && !string.IsNullOrEmpty(userEmail))
            {
                string checkBadgeQuery = "SELECT COUNT(*) FROM UserBadge WHERE userID = @userID AND badgeID = @badgeID";
                int badgeCount = 0;
                using (MySqlCommand checkBadgeCmd = new MySqlCommand(checkBadgeQuery, con))
                {
                    checkBadgeCmd.Parameters.AddWithValue("@userID", userID);
                    checkBadgeCmd.Parameters.AddWithValue("@badgeID", 17); 
                    badgeCount = Convert.ToInt32(checkBadgeCmd.ExecuteScalar());
                }

                if (badgeCount == 0)
                {
                    string insertBadgeQuery = "INSERT INTO UserBadge (userID, badgeID, badgeType) VALUES (@userID, @badgeID, @badgeType)";
                    using (MySqlCommand insertBadgeCmd = new MySqlCommand(insertBadgeQuery, con))
                    {
                        insertBadgeCmd.Parameters.AddWithValue("@userID", userID);
                        insertBadgeCmd.Parameters.AddWithValue("@badgeID", 17); 
                        insertBadgeCmd.Parameters.AddWithValue("@badgeType", "Gold");
                        insertBadgeCmd.ExecuteNonQuery();

                        System.Diagnostics.Debug.WriteLine(string.Format("Awarded gold email badge to user {0}", userID));
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
        using (MySqlConnection con = new MySqlConnection(cs))
        {
            string query = "UPDATE Users SET password = @password WHERE username = @username";
            MySqlCommand cmd = new MySqlCommand(query, con);

            string hashedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(newPassword, "SHA1");

            cmd.Parameters.AddWithValue("@password", hashedPassword);
            cmd.Parameters.AddWithValue("@username", username);

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
        using (MySqlConnection con = new MySqlConnection(cs))
        {
            string query = "UPDATE Users SET username = @newUsername WHERE username = @currentUsername";
            MySqlCommand cmd = new MySqlCommand(query, con);
            cmd.Parameters.AddWithValue("@newUsername", newUsername);
            cmd.Parameters.AddWithValue("@currentUsername", currentUsername);

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
        // Empty event handler
    }

    protected void btnChangeIcon_Click(object sender, EventArgs e)
    {
        Response.Redirect("ChangeProfilePhoto.aspx");
    }

    protected void btnLogout_Click1(object sender, EventArgs e)
    {
        Response.Redirect("Landing-page.aspx");
    }

    /*protected void deleteImageButton_Click(object sender, ImageClickEventArgs e)
    {
        pnlDeleteProfile.Visible = true;
    }*/

    protected void btnConfirmDeleteProfile_Click(object sender, EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("DELETE PROFILE BUTTON CLICKED - START");

        string username = "";
        if (Session["Username"] != null)
        {
            username = Session["Username"].ToString();
            System.Diagnostics.Debug.WriteLine("Username from session: " + username);
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("NO USERNAME IN SESSION");
            Response.Write("<script>alert('Error: User session expired or not found.');</script>");
            return;
        }

        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        using (MySqlConnection con = new MySqlConnection(cs))
        {
            con.Open();
            MySqlTransaction transaction = null;

            try
            {
                transaction = con.BeginTransaction();
                System.Diagnostics.Debug.WriteLine("Transaction started");

                int userID = -1;
                string getUserIDQuery = "SELECT userID FROM Users WHERE username = @username";
                using (MySqlCommand cmdGetID = new MySqlCommand(getUserIDQuery, con, transaction))
                {
                    cmdGetID.Parameters.AddWithValue("@username", username);

                    object result = cmdGetID.ExecuteScalar();
                    System.Diagnostics.Debug.WriteLine("User ID query executed");

                    if (result != null && result != DBNull.Value)
                    {
                        userID = Convert.ToInt32(result);
                        System.Diagnostics.Debug.WriteLine("User ID found: " + userID);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("NO USER ID FOUND FOR USERNAME: " + username);
                        Response.Write("<script>alert('Error: User not found in database.');</script>");
                        transaction.Rollback();
                        return;
                    }
                }

                System.Diagnostics.Debug.WriteLine("Deleting related records for user ID: " + userID);

                // Delete related records in all tables (REMOVED FriendRequest since table doesn't exist)
                string[] deleteQueries = new string[]
                {
                "DELETE FROM FriendsList WHERE userIDfrom = @userID",
                "DELETE FROM FriendsList WHERE userIDto = @userID",
                // "DELETE FROM FriendRequest WHERE userID = @userID", // REMOVE THIS LINE
                "DELETE FROM UserBadge WHERE userID = @userID",
                "DELETE FROM UserPets WHERE userID = @userID",
                "DELETE FROM CurrentLevel WHERE userID = @userID",
                "DELETE FROM CalendarEvent WHERE userID = @userID",
                "DELETE FROM StudySessionParticipants WHERE userID = @userID",
                "DELETE FROM Timer WHERE userID = @userID",
                "DELETE FROM ToDoListTask WHERE userID = @userID"
                };

                foreach (string query in deleteQueries)
                {
                    try
                    {
                        using (MySqlCommand cmd = new MySqlCommand(query, con, transaction))
                        {
                            cmd.Parameters.AddWithValue("@userID", userID);
                            int rows = cmd.ExecuteNonQuery();
                            System.Diagnostics.Debug.WriteLine("Executed: " + query + " - Rows affected: " + rows);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("ERROR executing query: " + query + " - " + ex.Message);
                        // Continue with other queries even if one fails
                    }
                }

                // Finally delete the user
                string deleteUserQuery = "DELETE FROM Users WHERE userID = @userID";
                using (MySqlCommand cmdDeleteUser = new MySqlCommand(deleteUserQuery, con, transaction))
                {
                    cmdDeleteUser.Parameters.AddWithValue("@userID", userID);

                    int rowsAffected = cmdDeleteUser.ExecuteNonQuery();
                    System.Diagnostics.Debug.WriteLine("User deletion query executed - Rows affected: " + rowsAffected);

                    if (rowsAffected > 0)
                    {
                        System.Diagnostics.Debug.WriteLine("USER DELETED SUCCESSFULLY - COMMITTING TRANSACTION");
                        transaction.Commit();
                        Session.Clear();
                        Session.Abandon();
                        FormsAuthentication.SignOut();
                        System.Diagnostics.Debug.WriteLine("Redirecting to landing page");
                        Response.Redirect("Landing-page.aspx");
                        return;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("NO ROWS AFFECTED IN USER DELETION - ROLLING BACK");
                        Response.Write("<script>alert('Error: User record not found in Users table for deletion.');</script>");
                        transaction.Rollback();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is System.Threading.ThreadAbortException)
                {
                    System.Diagnostics.Debug.WriteLine("ThreadAbortException - Redirect in progress");
                    return;
                }
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

    /*protected void btnCancelDelete_Click(object sender, EventArgs e)
    {
        pnlDeleteProfile.Visible = false;
    }*/

    protected void btnGoodbye_Click(object sender, EventArgs e)
    {
        Session.Clear();
        Session.Abandon();
        Response.Redirect("Landing-page.aspx");
    }

    private void LoadUserProfileIcon(string username)
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        using (MySqlConnection con = new MySqlConnection(cs))
        {
            string query = "SELECT iconNum FROM Users WHERE username = @username";
            MySqlCommand cmd = new MySqlCommand(query, con);
            cmd.Parameters.AddWithValue("@username", username);

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

    // start: join study session code
    private void LoadUpcomingSessions()
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        string query = "SELECT StudySession.sessionID, StudySession.sessionStart FROM StudySession INNER JOIN StudySessionParticipants ON StudySession.sessionID = StudySessionParticipants.sessionID WHERE StudySessionParticipants.userID = @userID AND StudySessionParticipants.accepted = true";

        List<string> jsSessionTimes = new List<string>();

        using (MySqlConnection conn = new MySqlConnection(cs))
        using (MySqlCommand cmd = new MySqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@userID", Session["userID"]);
            conn.Open();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int foundSessionID = Convert.ToInt32(reader["sessionID"]);
                    DateTime sessionStart = Convert.ToDateTime(reader["sessionStart"]);

                    string jsObject = "{ sessionID: " + foundSessionID + ", time: '" + sessionStart.ToString("yyyy-MM-ddTHH:mm:ss") + "' }";
                    jsSessionTimes.Add(jsObject);

                    TimeSpan timeUntilStart = sessionStart - DateTime.Now;
                    if (timeUntilStart.TotalMinutes >= 0 && timeUntilStart.TotalMinutes <= 10)
                    {
                        Session["sessionID"] = foundSessionID;
                    }
                }
            }
        }

        if (jsSessionTimes.Count > 0)
        {
            string jsArray = "[" + string.Join(",", jsSessionTimes.ToArray()) + "]";
            ClientScript.RegisterStartupScript(this.GetType(), "registerSessions", "var upcomingSessions = " + jsArray + ";", true);
        }
    }

    protected void btnJoin_Click(object sender, EventArgs e)
    {
        if (Session["sessionID"] != null && Session["userID"] != null)
        {
            int sessionID = Convert.ToInt32(Session["sessionID"]);
            int userID = Convert.ToInt32(Session["userID"]);

            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string updateQuery = "UPDATE StudySessionParticipants SET joined = true WHERE sessionID = @sessionID AND userID = @userID";

            using (MySqlConnection conn = new MySqlConnection(cs))
            using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
            {
                cmd.Parameters.AddWithValue("@sessionID", sessionID);
                cmd.Parameters.AddWithValue("@userID", userID);
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Response.Redirect("A1400_View-study-session.aspx");
                }
            }
        }
    }
    // end: join study session code
    // Handle delete label hidden button click
    protected void btnDeleteLabelHiddenTrigger_Click(object sender, EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("=== btnDeleteLabelHiddenTrigger_Click STARTED ===");

        if (hfDeleteLabelClicked.Value == "true")
        {
            System.Diagnostics.Debug.WriteLine("Delete label hidden trigger clicked - awarding pet");
            hfDeleteLabelClicked.Value = "false"; // Reset

            string userID = Session["UserID"] as string;

            if (string.IsNullOrEmpty(userID) && Session["Username"] != null)
            {
                string username = Session["Username"].ToString();
                string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                userID = GetUserID(username, cs);
            }

            if (!string.IsNullOrEmpty(userID))
            {
                bool petAdded = InsertDeleteLabelSecretPet(Convert.ToInt32(userID));

                if (petAdded)
                {
                    System.Diagnostics.Debug.WriteLine("Delete label pet added successfully, showing popup");

                    string script = @"
                    console.log('Delete label secret script executed');
                    if (typeof showDeleteSecretPopup === 'function') {
                        setTimeout(function() {
                            showDeleteSecretPopup();
                        }, 500);
                    } else {
                        console.error('showDeleteSecretPopup function not found');
                    }";

                    ScriptManager.RegisterStartupScript(this, GetType(), "showDeleteLabelPopup", script, true);
                    System.Diagnostics.Debug.WriteLine("Delete label popup script registered");
                }
            }
        }

        System.Diagnostics.Debug.WriteLine("=== btnDeleteLabelHiddenTrigger_Click COMPLETED ===");
    }

    // Method to insert the delete label secret pet into userPets table
    private bool InsertDeleteLabelSecretPet(int userID)
    {
        System.Diagnostics.Debug.WriteLine("InsertDeleteLabelSecretPet called for user " + userID);

        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        using (MySqlConnection con = new MySqlConnection(cs))
        {
            con.Open();

            // First check if the user already has this pet to avoid duplicates
            // Using petID 29 for the fourth secret pet
            string checkQuery = "SELECT COUNT(*) FROM UserPets WHERE userID = @userID AND petID = 29";
            using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, con))
            {
                checkCmd.Parameters.AddWithValue("@userID", userID);
                int existingCount = Convert.ToInt32(checkCmd.ExecuteScalar());
                System.Diagnostics.Debug.WriteLine("Existing delete label pets count: " + existingCount);

                if (existingCount > 0)
                {
                    System.Diagnostics.Debug.WriteLine("User already has delete label secret pet");
                    return false;
                }
            }

            // Insert new pet (userPetsID will auto-increment, equippedStatus = 0)
            string insertQuery = "INSERT INTO UserPets (userID, petID, equippedStatus) VALUES (@userID, 29, 0)";
            using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, con))
            {
                insertCmd.Parameters.AddWithValue("@userID", userID);
                int rowsAffected = insertCmd.ExecuteNonQuery();
                System.Diagnostics.Debug.WriteLine("Rows affected by delete label insert: " + rowsAffected);

                if (rowsAffected > 0)
                {
                    System.Diagnostics.Debug.WriteLine("Successfully added pet 29 for user " + userID);
                    return true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No rows affected by delete label insert");
                    return false;
                }
            }
        }
    }

    // Redirect to SecretPets page
    protected void btnViewSecretPet4_Click(object sender, EventArgs e)
    {
        // Use JavaScript redirect to avoid ThreadAbortException
        string script = "window.location.href = 'SecretPets.aspx';";
        ScriptManager.RegisterStartupScript(this, GetType(), "redirectToSecretPets4", script, true);
    }
    private string GetUserID(string username, string connectionString)
    {
        string query = "SELECT userID FROM Users WHERE username = @username";
        using (MySqlConnection con = new MySqlConnection(connectionString))
        using (MySqlCommand cmd = new MySqlCommand(query, con))
        {
            cmd.Parameters.AddWithValue("@username", username);
            try
            {
                con.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? result.ToString() : null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error getting user ID: " + ex.Message);
                return null;
            }
        }
    }
}