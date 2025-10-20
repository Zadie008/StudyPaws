using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class View_study_session : System.Web.UI.Page
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

        if (Session["sessionID"] == null)
        {
            Response.Redirect("Login.aspx");
            return;
        }

        int sessionID = (int)Session["sessionID"];

        if (!IsPostBack)
        {
            // Mark user as joined when they access this page
            MarkUserAsJoined(sessionID);

            if (Session["userID"] != null)
            {
                LoadJoinedUsers();
            }

            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(cs))
            {
                string query = "SELECT StudySession.sessionTitle, StudySession.sessionDuration, StudySession.sessionStart FROM StudySession INNER JOIN StudySessionParticipants ON StudySession.sessionID = StudySessionParticipants.sessionID WHERE StudySession.sessionID = @sessionID";

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@sessionID", sessionID);
                    con.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string sessionTitle = reader["sessionTitle"].ToString();
                            int totalSeconds = Convert.ToInt32(reader["sessionDuration"]);
                            DateTime sessionDateTime = Convert.ToDateTime(reader["sessionStart"]);

                            txtSessionTitle.Text = sessionTitle;

                            Session["sessionTitle"] = sessionTitle;
                            Session["sessionDuration"] = totalSeconds;
                            Session["sessionDateTime"] = sessionDateTime;

                            // Initialize session with waiting room logic
                            InitializeSessionTimer(sessionDateTime, totalSeconds);
                        }
                    }
                }

                if (Session["EquippedPetImagePath"] != null)
                {
                    pet.ImageUrl = Session["EquippedPetImagePath"].ToString();
                }
            }

            if (Session["Username"] != null)
            {
                string username = Session["Username"].ToString();
                string userID = GetUserID(username, cs);

                int userXP = GetUserXP(cs, userID);
                Tuple<int, int, int> levelInfo = GetLevelInformation(cs, userID);
                int currentLevel = levelInfo.Item1;
                int currentLevelXpAmount = levelInfo.Item2;
                int nextLevelXpAmount = levelInfo.Item3;

                lblLevelNumber.Text = currentLevel.ToString();
                CalculateXPProgressBar(userXP, currentLevelXpAmount, nextLevelXpAmount);
                GetUserStats(cs, userID);
                GetUserProfileIcon(cs, userID);
            }
            else
            {
                Response.Redirect("Landing-page.aspx");
            }
        }
    }

    private void MarkUserAsJoined(int sessionID)
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        string updateQuery = "UPDATE StudySessionParticipants SET joined = true WHERE sessionID = @sessionID AND userID = @userID";

        using (MySqlConnection conn = new MySqlConnection(cs))
        using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
        {
            cmd.Parameters.AddWithValue("@sessionID", sessionID);
            cmd.Parameters.AddWithValue("@userID", Session["userID"]);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    private void InitializeSessionTimer(DateTime sessionStart, int totalSeconds)
    {
        DateTime now = DateTime.Now;
        TimeSpan timeUntilSession = sessionStart - now;

        if (timeUntilSession.TotalSeconds <= 0)
        {
            // Session should have already started - start timer immediately
            StartStudyTimer(totalSeconds);
        }
        else
        {
            // Show waiting room with countdown to session start
            ShowWaitingRoom(timeUntilSession.TotalSeconds, totalSeconds);
        }
    }

    private void ShowWaitingRoom(double secondsUntilStart, int sessionDuration)
    {
        string script = string.Format(@"
    document.addEventListener('DOMContentLoaded', function() {{
        // Show waiting room message
        var timerElement = document.getElementById('mainContentPlaceHolder_lblCountdown');
        if (timerElement) {{
            timerElement.innerHTML = 'Waiting for session to start...<br/><span style=""font-size: 0.7em;"">Starting in: <span id=""waitingCountdown"">{0}</span> seconds</span>';
        }}
        
        // Start waiting countdown
        var waitingSeconds = Math.round({0});
        var waitingInterval = setInterval(function() {{
            waitingSeconds--;
            var waitingElement = document.getElementById('waitingCountdown');
            if (waitingElement) {{
                waitingElement.textContent = waitingSeconds;
            }}
            
            if (waitingSeconds <= 0) {{
                clearInterval(waitingInterval);
                startTimer({1}); // Start the actual study timer
                if (timerElement) {{
                    timerElement.innerHTML = ''; // Clear waiting message
                }}
            }}
        }}, 1000);
    }});", secondsUntilStart, sessionDuration);

        ClientScript.RegisterStartupScript(this.GetType(), "WaitingRoomScript", script, true);
    }

    private void StartStudyTimer(int totalSeconds)
    {
        string script = string.Format(@"
    document.addEventListener('DOMContentLoaded', function() {{
        startTimer({0});
    }});", totalSeconds);
        ClientScript.RegisterStartupScript(this.GetType(), "StartTimerScript", script, true);
    }

    [System.Web.Services.WebMethod]
    [System.Web.Script.Services.ScriptMethod]
    public static string UpdateStudySessionRewards(int minutesStudied)
    {
        try
        {
            HttpContext context = HttpContext.Current;
            if (context.Session["UserID"] == null)
            {
                return "Error: User not authenticated";
            }

            string userID = context.Session["UserID"].ToString();
            int xpEarned = minutesStudied * 2; // 2 XP per minute
            int coinsEarned = minutesStudied * 2; // 2 coins per minute

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                // Update user XP
                string updateXPQuery = "UPDATE Users SET userXP = userXP + @xpEarned WHERE userID = @userID";
                MySqlCommand cmd = new MySqlCommand(updateXPQuery, con);
                cmd.Parameters.AddWithValue("@xpEarned", xpEarned);
                cmd.Parameters.AddWithValue("@userID", userID);
                cmd.ExecuteNonQuery();

                // Update user coins
                string updateCoinsQuery = "UPDATE Users SET userCoinCount = userCoinCount + @coinsEarned WHERE userID = @userID";
                MySqlCommand cmdCoins = new MySqlCommand(updateCoinsQuery, con);
                cmdCoins.Parameters.AddWithValue("@coinsEarned", coinsEarned);
                cmdCoins.Parameters.AddWithValue("@userID", userID);
                cmdCoins.ExecuteNonQuery();
            }

            return "Success: " + xpEarned + " XP and " + coinsEarned + " coins added";
        }
        catch (Exception ex)
        {
            return "Error: " + ex.Message;
        }
    }

    // STOP STUDY SESSION (DELETING THE STUDY SESSION ENTRY FOR CURRENT USER)
    protected void btnYes_Click(object sender, EventArgs e)
    {
        if (Session["userID"] != null && Session["sessionID"] != null)
        {
            int thisSessionID = Convert.ToInt32(Session["sessionID"]);
            int thisUserID = Convert.ToInt32(Session["userID"]);

            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (MySqlConnection con2 = new MySqlConnection(cs))
            {
                string deleteCommand = "DELETE FROM StudySessionParticipants WHERE sessionID = @sessionID AND userID = @userID";
                using (MySqlCommand cmd = new MySqlCommand(deleteCommand, con2))
                {
                    cmd.Parameters.AddWithValue("@sessionID", thisSessionID);
                    cmd.Parameters.AddWithValue("@userID", thisUserID);

                    con2.Open();
                    int code = cmd.ExecuteNonQuery();
                    con2.Close();

                    if (code == 1)
                    {
                        Response.Redirect("Default.aspx");
                    }
                }
            }
        }
    }

    // COMPLETE STUDY SESSION (UPDATING COMPLETED ATTRIBUTE)
    [System.Web.Services.WebMethod]
    public static string MarkSessionAsCompleted()
    {
        try
        {
            if (HttpContext.Current.Session["sessionID"] != null && HttpContext.Current.Session["userID"] != null)
            {
                int sessionID = Convert.ToInt32(HttpContext.Current.Session["sessionID"]);
                int userID = Convert.ToInt32(HttpContext.Current.Session["userID"]);

                string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                string updateQuery = "UPDATE StudySessionParticipants SET completed = true WHERE sessionID = @sessionID AND userID = @userID";

                using (MySqlConnection conn = new MySqlConnection(cs))
                using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@sessionID", sessionID);
                    cmd.Parameters.AddWithValue("@userID", userID);
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return "success";
                    }
                    return "no_rows_updated";
                }
            }
            return "session_or_user_missing";
        }
        catch (Exception ex)
        {
            return "error: " + ex.Message;
        }
    }

    // IN SESSION BLOCK - UPDATED TO SHOW ONLY JOINED USERS
    private void LoadJoinedUsers()
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        int sessionID = Convert.ToInt32(Session["sessionID"]);
        int currentUserID = Convert.ToInt32(Session["userID"]);

        using (MySqlConnection con = new MySqlConnection(cs))
        {
            string command = "SELECT u.userID, u.username, u.iconNum FROM StudySessionParticipants sp JOIN Users u ON sp.userID = u.userID WHERE sp.sessionID = @sessionID AND sp.joined = true";

            MySqlCommand cmd = new MySqlCommand(command, con);
            cmd.Parameters.AddWithValue("@sessionID", sessionID);

            con.Open();
            MySqlDataReader rdr = cmd.ExecuteReader();
            GridView1.DataSource = rdr;
            GridView1.DataBind();
        }
    }

    [System.Web.Services.WebMethod]
    public static List<object> GetJoinedUsers()
    {
        List<object> users = new List<object>();
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        if (HttpContext.Current.Session["sessionID"] == null)
            return users;

        int sessionID = Convert.ToInt32(HttpContext.Current.Session["sessionID"]);

        using (MySqlConnection con = new MySqlConnection(cs))
        {
            string query = "SELECT u.userID, u.username, u.iconNum FROM StudySessionParticipants sp JOIN Users u ON sp.userID = u.userID WHERE sp.sessionID = @sessionID AND sp.joined = true";

            using (MySqlCommand cmd = new MySqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@sessionID", sessionID);
                con.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new
                        {
                            userID = Convert.ToInt32(reader["userID"]),
                            username = reader["username"].ToString(),
                            iconNum = Convert.ToInt32(reader["iconNum"])
                        });
                    }
                }
            }
        }

        return users;
    }

    // Check if a user is already a friend
    public bool IsFriend(int targetUserID)
    {
        int currentUserID = Convert.ToInt32(Session["userID"]);
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        using (MySqlConnection con = new MySqlConnection(cs))
        {
            string query = @"SELECT COUNT(*) FROM FriendsList 
                       WHERE ((userIDfrom = @currentUserID AND userIDto = @targetUserID) 
                       OR (userIDfrom = @targetUserID AND userIDto = @currentUserID))
                       AND requestStatus = 'Accepted'";

            using (MySqlCommand cmd = new MySqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@currentUserID", currentUserID);
                cmd.Parameters.AddWithValue("@targetUserID", targetUserID);
                con.Open();
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }
    }

    private bool SendFriendRequest(int fromUserID, int toUserID)
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        using (MySqlConnection con = new MySqlConnection(cs))
        {
            // Check if friend request already exists
            string checkQuery = @"SELECT COUNT(*) FROM FriendsList 
                            WHERE (userIDfrom = @fromUserID AND userIDto = @toUserID) 
                            OR (userIDfrom = @toUserID AND userIDto = @fromUserID)";

            using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, con))
            {
                checkCmd.Parameters.AddWithValue("@fromUserID", fromUserID);
                checkCmd.Parameters.AddWithValue("@toUserID", toUserID);
                con.Open();
                int existingCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (existingCount > 0)
                {
                    return false; // Friend request or friendship already exists
                }
            }

            // Send friend request
            string insertQuery = "INSERT INTO FriendsList (userIDfrom, userIDto, requestStatus) VALUES (@fromUserID, @toUserID, 'Pending')";
            using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, con))
            {
                insertCmd.Parameters.AddWithValue("@fromUserID", fromUserID);
                insertCmd.Parameters.AddWithValue("@toUserID", toUserID);
                int rowsAffected = insertCmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
    }

    // Check if the user is the current user
    public bool IsCurrentUser(int userID)
    {
        return userID == Convert.ToInt32(Session["userID"]);
    }

    // Handle friend request
    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "SendFriendRequest")
        {
            string[] args = e.CommandArgument.ToString().Split('|');
            int targetUserID = Convert.ToInt32(args[0]);
            string targetUsername = args[1];
            int currentUserID = Convert.ToInt32(Session["userID"]);

            if (SendFriendRequest(currentUserID, targetUserID))
            {
                // Refresh the grid to update the button
                LoadJoinedUsers();

                // Show success message
                ScriptManager.RegisterStartupScript(this, this.GetType(), "friendRequestSent",
                    "alert('Friend request sent to " + targetUsername + "!');", true);
            }
        }
    }

    // start: header profile code
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
                Console.WriteLine("Error getting user ID: " + ex.Message);
                return null;
            }
        }
    }

    private int GetUserXP(string connectionString, string userID)
    {
        string query = "SELECT userXP FROM Users WHERE userID = @userID";
        int userXP = 0;

        using (MySqlConnection con = new MySqlConnection(connectionString))
        using (MySqlCommand cmd = new MySqlCommand(query, con))
        {
            cmd.Parameters.AddWithValue("@userID", userID);
            try
            {
                con.Open();
                object result = cmd.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out userXP))
                {
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting user XP: " + ex.Message);
            }
        }
        return userXP;
    }

    private Tuple<int, int, int> GetLevelInformation(string connectionString, string userID)
    {
        int currentLevel = 0;
        int currentLevelXpAmount = 0;
        int nextLevelXpAmount = 0;
        string currentLevelQuery = "SELECT levelID FROM CurrentLevel WHERE userID = @userID";
        using (MySqlConnection con = new MySqlConnection(connectionString))
        using (MySqlCommand cmdCurrentLevel = new MySqlCommand(currentLevelQuery, con))
        {
            cmdCurrentLevel.Parameters.AddWithValue("@userID", userID);
            con.Open();
            object result = cmdCurrentLevel.ExecuteScalar();
            if (result != null && int.TryParse(result.ToString(), out currentLevel))
            {
                lblLevelNumber.Text = currentLevel.ToString();
            }
            else
            {
                lblLevelNumber.Text = "N/A";
                return Tuple.Create(0, 0, 0);
            }
        }
        string currentLevelXPQuery = "SELECT xpAmount FROM Level WHERE levelNum = @currentLevel";
        using (MySqlConnection con = new MySqlConnection(connectionString))
        using (MySqlCommand cmdCurrentXP = new MySqlCommand(currentLevelXPQuery, con))
        {
            cmdCurrentXP.Parameters.AddWithValue("@currentLevel", currentLevel);
            con.Open();
            object result = cmdCurrentXP.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                currentLevelXpAmount = Convert.ToInt32(result);
            }
        }

        string nextLevelXPQuery = "SELECT xpAmount FROM Level WHERE levelNum = @nextLevel";
        using (MySqlConnection con = new MySqlConnection(connectionString))
        using (MySqlCommand cmdNextXP = new MySqlCommand(nextLevelXPQuery, con))
        {
            cmdNextXP.Parameters.AddWithValue("@nextLevel", currentLevel + 1);
            con.Open();
            object result = cmdNextXP.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                nextLevelXpAmount = Convert.ToInt32(result);
            }
            else
            {
                nextLevelXpAmount = currentLevelXpAmount;//when user reaches level 25
            }
        }

        return Tuple.Create(currentLevel, currentLevelXpAmount, nextLevelXpAmount);
    }

    private void CalculateXPProgressBar(int userXP, int currentLevelXpAmount, int nextLevelXpAmount)
    {
        if (nextLevelXpAmount <= currentLevelXpAmount)
        {
            xpProgressBar.Style["width"] = "100%";
            lblXPPercentage.Text = "100%";
            return;
        }
        int xpToNextLevel = nextLevelXpAmount - currentLevelXpAmount;
        int xpGainedInCurrentLevel = userXP - currentLevelXpAmount;

        if (xpToNextLevel > 0)
        {
            double progress = (double)xpGainedInCurrentLevel / xpToNextLevel * 100;
            if (progress < 0) progress = 0;
            if (progress > 100) progress = 100;

            xpProgressBar.Style["width"] = progress.ToString("F0") + "%";
            lblXPPercentage.Text = progress.ToString("F0") + "%";
        }
        else
        {
            xpProgressBar.Style["width"] = "100%";
            lblXPPercentage.Text = "100%";
        }
    }

    private void GetUserStats(string connectionString, string userID)
    {
        string query = "SELECT userCoinCount FROM Users WHERE userID = @userID";

        using (MySqlConnection con = new MySqlConnection(connectionString))
        using (MySqlCommand cmd = new MySqlCommand(query, con))
        {
            cmd.Parameters.AddWithValue("@userID", userID);

            con.Open();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    lblPaws.Text = reader["userCoinCount"] != DBNull.Value ? reader["userCoinCount"].ToString() : "0";
                }
                else
                {
                    lblPaws.Text = "N/A";
                }
            }
        }
    }

    private void GetUserProfileIcon(string connectionString, string userID)
    {
        string query = "SELECT iconNum FROM Users WHERE userID = @userID";

        using (MySqlConnection con = new MySqlConnection(connectionString))
        using (MySqlCommand cmd = new MySqlCommand(query, con))
        {
            cmd.Parameters.AddWithValue("@userID", userID);
            try
            {
                con.Open();
                object result = cmd.ExecuteScalar();
                int iconNum;
                if (result != null && int.TryParse(result.ToString(), out iconNum))
                {
                    string iconPath = GetProfileImagePath(iconNum);
                    string circleClass = GetCircleColorClass(iconNum);
                    profilePet.ImageUrl = iconPath;
                    profileCircle.Attributes["class"] = "profileCircle " + circleClass;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting profile icon: " + ex.Message);
            }
        }
    }

    public static string GetProfileImagePath(int iconNum)
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

    private string GetCircleColorClass(int iconNum)
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
    // end: header profile code
}