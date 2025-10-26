using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Web.Security;
using System.Web.UI;

public partial class _Default : System.Web.UI.Page
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

        if (Session["Username"] != null)
        {
            lblLoggedInUserName.Text = Session["Username"].ToString() + "!";

            string username = Session["Username"].ToString();
            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string userID = GetUserID(username, cs);

            if (!IsPostBack)
            {
                int userXP = GetUserXP(cs, userID);
                Tuple<int, int, int> levelInfo = GetLevelInformation(cs, userID);
                int currentLevel = levelInfo.Item1;
                int currentLevelXpAmount = levelInfo.Item2;
                int nextLevelXpAmount = levelInfo.Item3;

                lblLevelNumber.Text = currentLevel.ToString();

                CalculateXPProgressBar(userXP, currentLevelXpAmount, nextLevelXpAmount);
                GetUserStats(cs, userID);
                GetUserProfileIcon(cs, userID);
                LoadPendingInvitesFromDB();
                LoadUpcomingSessions();
                LoadEquippedPet(cs, userID);
            }

            ShowNextInvite(); // always show latest invite
        }
        else
        {
            Response.Redirect("Landing-page.aspx");
            lblLoggedInUserName.Text = "You are not logged in";
        }
        if (Request.Form["__EVENTTARGET"] == "checkNovember30Badge")
        {
            CheckAndAwardNovember30Badge();
        }
        else if (Request.Form["__EVENTTARGET"] == "checkChristmasBadge")
        {
            CheckAndAwardChristmasBadge();
        }
    }
    private void CheckAndAwardNovember30Badge()
    {
        DateTime today = DateTime.Now;
        if (today.Month == 11 && today.Day == 30) 
        {
            if (Session["UserID"] != null)
            {
                int userID = Convert.ToInt32(Session["UserID"]);
                AwardNovember30Badge(userID);
            }
            else if (Request.IsAuthenticated)
            {
                var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie != null)
                {
                    var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                    int userID = Convert.ToInt32(ticket.UserData);
                    AwardNovember30Badge(userID);
                }
            }
        }
    }
    private void CheckAndAwardChristmasBadge()
    {
        DateTime today = DateTime.Now;
        if (today.Month == 12 && today.Day == 25)
        {
            if (Session["UserID"] != null)
            {
                int userID = Convert.ToInt32(Session["UserID"]);
                AwardChristmasBadge(userID);
            }
            else if (Request.IsAuthenticated)
            {
                var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie != null)
                {
                    var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                    int userID = Convert.ToInt32(ticket.UserData);
                    AwardChristmasBadge(userID);
                }
            }
        }
    }
    private void AwardChristmasBadge(int userID)
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        using (MySqlConnection con = new MySqlConnection(cs))
        {
            con.Open();

            try
            {
                string checkBadgeQuery = "SELECT COUNT(*) FROM UserBadge WHERE userID = @userID AND badgeID = @badgeID";
                MySqlCommand checkBadgeCmd = new MySqlCommand(checkBadgeQuery, con);
                checkBadgeCmd.Parameters.AddWithValue("@userID", userID);
                checkBadgeCmd.Parameters.AddWithValue("@badgeID", 20);

                int existingBadgeCount = Convert.ToInt32(checkBadgeCmd.ExecuteScalar());

                if (existingBadgeCount == 0)
                {
                    string updateLoginQuery = "UPDATE Users SET logInChristmas = 1 WHERE userID = @userID";
                    MySqlCommand updateLoginCmd = new MySqlCommand(updateLoginQuery, con);
                    updateLoginCmd.Parameters.AddWithValue("@userID", userID);
                    updateLoginCmd.ExecuteNonQuery();
                    string insertBadgeQuery = "INSERT INTO UserBadge (userID, badgeID, badgeType) VALUES (@userID, @badgeID, @badgeType)";
                    MySqlCommand insertBadgeCmd = new MySqlCommand(insertBadgeQuery, con);
                    insertBadgeCmd.Parameters.AddWithValue("@userID", userID);
                    insertBadgeCmd.Parameters.AddWithValue("@badgeID", 20);
                    insertBadgeCmd.Parameters.AddWithValue("@badgeType", "Gold");

                    int rowsInserted = insertBadgeCmd.ExecuteNonQuery();

                    if (rowsInserted > 0)
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("Awarded Christmas gold badge to user {0}", userID));
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("User {0} already has Christmas badge", userID));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Error awarding Christmas badge: {0}", ex.Message));
            }
        }
    }
    private void AwardNovember30Badge(int userID)
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        using (MySqlConnection con = new MySqlConnection(cs))
        {
            con.Open();

            try
            {
                string checkBadgeQuery = "SELECT COUNT(*) FROM UserBadge WHERE userID = @userID AND badgeID = @badgeID";
                MySqlCommand checkBadgeCmd = new MySqlCommand(checkBadgeQuery, con);
                checkBadgeCmd.Parameters.AddWithValue("@userID", userID);
                checkBadgeCmd.Parameters.AddWithValue("@badgeID", 19);

                int existingBadgeCount = Convert.ToInt32(checkBadgeCmd.ExecuteScalar());

                if (existingBadgeCount == 0)
                {
                    string updateLoginQuery = "UPDATE Users SET logInHalloween = 1 WHERE userID = @userID";
                    MySqlCommand updateLoginCmd = new MySqlCommand(updateLoginQuery, con);
                    updateLoginCmd.Parameters.AddWithValue("@userID", userID);
                    updateLoginCmd.ExecuteNonQuery();
                    string insertBadgeQuery = "INSERT INTO UserBadge (userID, badgeID, badgeType) VALUES (@userID, @badgeID, @badgeType)";
                    MySqlCommand insertBadgeCmd = new MySqlCommand(insertBadgeQuery, con);
                    insertBadgeCmd.Parameters.AddWithValue("@userID", userID);
                    insertBadgeCmd.Parameters.AddWithValue("@badgeID", 19);
                    insertBadgeCmd.Parameters.AddWithValue("@badgeType", "Gold");

                    int rowsInserted = insertBadgeCmd.ExecuteNonQuery();

                    if (rowsInserted > 0)
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("Awarded November 30 gold badge to user {0}", userID));
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("User {0} already has November 30 badge", userID));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Error awarding November 30 badge: {0}", ex.Message));
            }
        }
    }
    // ---- COPY ----
    // ---- START ----

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

    // start: notification bell code
    private void LoadPendingInvitesFromDB()
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        List<SessionInvite> pendingInvites = new List<SessionInvite>();

        // nly select sessions that haven't started yet
        string query = @"SELECT StudySession.sessionID, StudySession.sessionTitle, StudySession.sessionTag, StudySession.sessionStart, StudySession.sessionEnd, Users.username FROM (StudySessionParticipants INNER JOIN StudySession ON StudySessionParticipants.sessionID = StudySession.sessionID) INNER JOIN Users ON StudySession.leaderID = Users.userID WHERE StudySessionParticipants.userID = @userID AND StudySessionParticipants.accepted = false AND StudySession.sessionStart > NOW() ORDER BY StudySession.sessionStart ASC";  // order by start time

        using (MySqlConnection conn = new MySqlConnection(cs))
        using (MySqlCommand cmd = new MySqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@userID", Session["userID"]);
            conn.Open();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    pendingInvites.Add(new SessionInvite
                    {
                        sessionID = Convert.ToInt32(reader["sessionID"]),
                        leaderUsername = reader["username"].ToString(),
                        title = reader["sessionTitle"].ToString(),
                        tag = reader["sessionTag"].ToString(),
                        startTime = Convert.ToDateTime(reader["sessionStart"]),
                        endTime = Convert.ToDateTime(reader["sessionEnd"])
                    });
                }
            }
        }

        Session["PendingInvites"] = pendingInvites;
    }

    private void ShowNextInvite()
    {
        List<SessionInvite> invites = Session["PendingInvites"] as List<SessionInvite>;
        if (invites != null && invites.Count > 0)
        {
            var invite = invites[0];

            litNotificationText.Text = "<p><span style='text-decoration:underline;'>Study session invitation</span></p><br /><table><tr><td><p>From:</p></td><td><p><span style='font-weight:bold;'>" + invite.leaderUsername + "</span></p></td></tr>" + "<tr><td><p>Title:</p></td><td><p><span style='font-weight:bold;'>" + invite.title + "</span></p></td></tr>" + "<tr><td><p>Tag:</p></td><td><p><span style='font-weight:bold;'>" + invite.tag + "</span></p></td></tr>" + "<tr><td><p>Starts:</p></td><td><p><span style='font-weight:bold;'>" + invite.startTime.ToString("dddd, dd MMMM yyyy @ HH:mm") + "</span></p></td></tr><tr><td><p>Ends:</p></td><td><p><span style='font-weight:bold;'>" + invite.endTime.ToString("dddd, dd MMMM yyyy @ HH:mm") + "</span></p></td></tr></table>";

            hiddenSessionID.Value = invite.sessionID.ToString();

            imgNotificationRinging.Visible = true;
            imgNotificationNormal.Visible = false;
            notificationBadge.Visible = true;

            ScriptManager.RegisterStartupScript(this, this.GetType(), "showPopup", "showNotificationPopup();", true);
        }
        else
        {
            imgNotificationRinging.Visible = false;
            imgNotificationNormal.Visible = true;
            notificationBadge.Visible = false;

            ScriptManager.RegisterStartupScript(this, this.GetType(), "showPopupNone", "showNotificationPopup(false);", true);
        }
    }

    protected void btnYes_Click(object sender, EventArgs e)
    {
        int sessionID = int.Parse(hiddenSessionID.Value);
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        List<SessionInvite> invites = Session["PendingInvites"] as List<SessionInvite>;
        SessionInvite invite = null;

        if (invites != null)
        {
            foreach (SessionInvite i in invites)
            {
                if (i.sessionID == sessionID)
                {
                    invite = i;
                    break;
                }
            }
        }

        int newEventID = 0;

        if (invite != null)
        {
            // insert into calendarevent table
            string insertQuery = "INSERT INTO CalendarEvent (eventDesc, eventDate, tagID, userID) VALUES (@eventDesc, @eventDate, @tagID, @userID); SELECT LAST_INSERT_ID();";
            using (MySqlConnection conn = new MySqlConnection(cs))
            using (MySqlCommand cmd2 = new MySqlCommand(insertQuery, conn))
            {
                string eventDesc = invite.title + " (From: " + invite.leaderUsername + ")";
                DateTime eventDate = invite.startTime;
                int tagID = 1;
                int userID = Convert.ToInt32(Session["userID"]);

                cmd2.Parameters.AddWithValue("@eventDesc", eventDesc);
                cmd2.Parameters.AddWithValue("@eventDate", eventDate);
                cmd2.Parameters.AddWithValue("@tagID", tagID);
                cmd2.Parameters.AddWithValue("@userID", userID);

                conn.Open();
                newEventID = Convert.ToInt32(cmd2.ExecuteScalar());
            }
        }

        // insert into studysessionparticipants table
        string updateQuery = "UPDATE StudySessionParticipants SET accepted = true, eventID = @eventID WHERE sessionID = @sessionID AND userID = @userID";
        using (MySqlConnection conn = new MySqlConnection(cs))
        using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
        {
            cmd.Parameters.AddWithValue("@eventID", newEventID);
            cmd.Parameters.AddWithValue("@sessionID", sessionID);
            cmd.Parameters.AddWithValue("@userID", Session["userID"]);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        hiddenShowCalendar.Value = "true";

        RemoveInviteAndShowNext(sessionID);
    }

    protected void btnNo_Click(object sender, EventArgs e)
    {
        hiddenShowConfirmation.Value = "true";
    }

    private void RemoveInviteAndShowNext(int sessionID)
    {
        List<SessionInvite> invites = Session["PendingInvites"] as List<SessionInvite>;
        if (invites != null)
        {
            var currentInvite = invites.Find(i => i.sessionID == sessionID);
            if (currentInvite != null)
            {
                invites.Remove(currentInvite);
            }
            Session["PendingInvites"] = invites;
            ShowNextInvite();
        }
    }

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

    protected void btnCalendar_Click(object sender, EventArgs e)
    {
        Response.Redirect("B100_View-calendar.aspx");
    }

    protected void btnOk_Click(object sender, EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }

    protected void btnSure_Click(object sender, EventArgs e)
    {
        int sessionID = int.Parse(hiddenSessionID.Value);
        int userID = Convert.ToInt32(Session["userID"]);
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        // Increment declinedStudySessionInvitations counter
        using (MySqlConnection con1 = new MySqlConnection(cs))
        {
            string updateCommand = "UPDATE Users SET declinedStudySessionInvitations = declinedStudySessionInvitations + 1 WHERE userID = @userID";
            using (MySqlCommand cmd = new MySqlCommand(updateCommand, con1))
            {
                cmd.Parameters.AddWithValue("@userID", userID);
                con1.Open();
                cmd.ExecuteNonQuery();
                con1.Close();
            }
        }

        // Check for declined invitation badges after incrementing
        CheckDeclinedStudySessionInvitationBadges(userID);

        // Delete study session participant
        string deleteQuery = "DELETE FROM StudySessionParticipants WHERE sessionID = @sessionID AND userID = @userID AND accepted = false";
        using (MySqlConnection conn = new MySqlConnection(cs))
        using (MySqlCommand cmd = new MySqlCommand(deleteQuery, conn))
        {
            cmd.Parameters.AddWithValue("@sessionID", sessionID);
            cmd.Parameters.AddWithValue("@userID", userID);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        hiddenShowDeclineConfirmed.Value = "true";

        RemoveInviteAndShowNext(sessionID);
    }

    // FOR GETTING DECLINED STUDY SESSION INVITATION BADGE
    public void CheckDeclinedStudySessionInvitationBadges(int userID)
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        using (MySqlConnection con = new MySqlConnection(cs))
        {
            con.Open();

            // Get current declined study session invitations count
            string getCountQuery = "SELECT declinedStudySessionInvitations FROM Users WHERE userID = @userID";
            int declinedCount = 0;
            using (MySqlCommand getCountCmd = new MySqlCommand(getCountQuery, con))
            {
                getCountCmd.Parameters.AddWithValue("@userID", userID);
                object result = getCountCmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    declinedCount = Convert.ToInt32(result);
                }
            }

            // Check and award badges based on declined study session invitations count
            if (declinedCount >= 15)
            {
                AwardBadge(con, userID, 9, "Gold");
            }
            else if (declinedCount >= 10)
            {
                AwardBadge(con, userID, 9, "Silver");
            }
            else if (declinedCount >= 5)
            {
                AwardBadge(con, userID, 9, "Bronze");
            }

            con.Close();
        }
    }

    private void AwardBadge(MySqlConnection con, int userID, int badgeID, string badgeType)
    {
        // Check if user already has any type of this badge
        string checkBadgeQuery = "SELECT COUNT(*) FROM UserBadge WHERE userID = @userID AND badgeID = @badgeID";
        int badgeCount = 0;
        using (MySqlCommand checkBadgeCmd = new MySqlCommand(checkBadgeQuery, con))
        {
            checkBadgeCmd.Parameters.AddWithValue("@userID", userID);
            checkBadgeCmd.Parameters.AddWithValue("@badgeID", badgeID);
            badgeCount = Convert.ToInt32(checkBadgeCmd.ExecuteScalar());
        }

        if (badgeCount == 0)
        {
            // No entry exists - INSERT new record
            string insertBadgeQuery = "INSERT INTO UserBadge (userID, badgeID, badgeType) VALUES (@userID, @badgeID, @badgeType)";
            using (MySqlCommand insertBadgeCmd = new MySqlCommand(insertBadgeQuery, con))
            {
                insertBadgeCmd.Parameters.AddWithValue("@userID", userID);
                insertBadgeCmd.Parameters.AddWithValue("@badgeID", badgeID);
                insertBadgeCmd.Parameters.AddWithValue("@badgeType", badgeType);
                insertBadgeCmd.ExecuteNonQuery();
            }
        }
        else
        {
            // Entry exists - UPDATE with new badgeType
            string updateBadgeQuery = "UPDATE UserBadge SET badgeType = @badgeType WHERE userID = @userID AND badgeID = @badgeID";
            using (MySqlCommand updateBadgeCmd = new MySqlCommand(updateBadgeQuery, con))
            {
                updateBadgeCmd.Parameters.AddWithValue("@userID", userID);
                updateBadgeCmd.Parameters.AddWithValue("@badgeID", badgeID);
                updateBadgeCmd.Parameters.AddWithValue("@badgeType", badgeType);
                updateBadgeCmd.ExecuteNonQuery();
            }
        }
    }

    protected void btnNotSure_Click(object sender, EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }

    protected void btnOkayDeclined_Click(object sender, EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }

    protected void btnJoin_Click(object sender, EventArgs e)
    {
        if (Session["sessionID"] != null && Session["userID"] != null)
        {
            int sessionID = Convert.ToInt32(Session["sessionID"]);
            int userID = Convert.ToInt32(Session["userID"]);

            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            string checkTimeQuery = @"SELECT sessionStart FROM StudySession WHERE sessionID = @sessionID AND DATE_ADD(sessionStart, INTERVAL 1 MINUTE) < NOW()";

            string updateQuery = "UPDATE StudySessionParticipants SET joined = true WHERE sessionID = @sessionID AND userID = @userID";

            using (MySqlConnection conn = new MySqlConnection(cs))
            {
                conn.Open();

                using (MySqlCommand checkCmd = new MySqlCommand(checkTimeQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@sessionID", sessionID);
                    var result = checkCmd.ExecuteScalar();

                    // session started more than 1 minute ago
                    if (result != null)
                    {
                        Response.Redirect("Default.aspx");
                        return;
                    }
                }

                // session started less than 1 minute ago, so can join study session
                using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn))
                {
                    updateCmd.Parameters.AddWithValue("@sessionID", sessionID);
                    updateCmd.Parameters.AddWithValue("@userID", userID);
                    int rowsAffected = updateCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Response.Redirect("A1400_View-study-session.aspx");
                    }
                }
            }
        }
    }
    // end: notification bell code

    // ---- COPY ----
    // ---- END ----

    // start: home page code
    private void LoadEquippedPet(string connectionString, string userID)
    {
        string command = "SELECT Pet.petType, Pet.colourNum FROM UserPets INNER JOIN Pet ON UserPets.petID = Pet.petID WHERE UserPets.userID = @userID AND UserPets.equippedStatus = True";

        using (MySqlConnection con = new MySqlConnection(connectionString))
        using (MySqlCommand cmd = new MySqlCommand(command, con))
        {
            cmd.Parameters.AddWithValue("@userID", userID);
            try
            {
                con.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string petType = reader["petType"].ToString();   // Cat/Dog/Fuzzy/Farm/Special
                        int colourNum = Convert.ToInt32(reader["colourNum"]); // 1/2/3/4/5

                        string imagePath = GetPetImagePath(petType, colourNum); // building file name of image
                        pet.ImageUrl = imagePath;

                        Session["EquippedPetImagePath"] = imagePath; // saved as session variable
                    }
                    else
                    {
                        pet.ImageUrl = "~/Images/Cat 1.png"; // default pet
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading equipped pet: " + ex.Message);
                pet.ImageUrl = "~/Images/Cat 1.png";
            }
        }
    }

    private string GetPetImagePath(string petType, int colourNum)
    {
        return string.Format("~/Images/{0} {1}.png", petType, colourNum);  // png or gif
    }
    // end: home page code

  
}