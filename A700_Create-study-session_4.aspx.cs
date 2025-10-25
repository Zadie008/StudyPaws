using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

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

        if (!IsPostBack)
        {
            if (Session["Username"] != null)
            {
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
                }
            }
            else
            {
                Response.Redirect("Landing-page.aspx");
            }

            ShowNextInvite();
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("A700_Create-study-session_3.aspx");
    }

    protected void btnSchedule_Click(object sender, EventArgs e)
    {
        if (Session["Username"] != null)
        {
            Page.Validate("timerValidation");

            if (!Page.IsValid)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showValidation",
                    "validateAllFields();", true);
                return;
            }

            // Client-side date validation will prevent the postback if date is invalid
            DateTime sessionDate;
            if (!DateTime.TryParse(txtFilterDate.Text, out sessionDate))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showDateError",
                    "document.getElementById('dateValidationError').textContent = 'Please enter a valid date'; document.getElementById('dateValidationError').style.display = 'inline';", true);
                return;
            }

            if (sessionDate.Date < DateTime.Today)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showDateError",
                    "document.getElementById('dateValidationError').textContent = 'Cannot schedule study session for a past date'; document.getElementById('dateValidationError').style.display = 'inline';", true);
                return;
            }

            if (Session["userID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            // get invited friends from session
            List<string> invitedFriends = Session["selectedFriends"] as List<string>;
            if (invitedFriends == null)
            {
                invitedFriends = new List<string>();
            }

            try
            {
                // parse date and time values
                int startHours = int.Parse(txtStartTimeHours.Text);
                int startMinutes = int.Parse(txtStartTimeMinutes.Text);
                int endHours = int.Parse(txtEndTimeHours.Text);
                int endMinutes = int.Parse(txtEndTimeMinutes.Text);

                DateTime sessionStart = sessionDate.AddHours(startHours).AddMinutes(startMinutes);
                DateTime sessionEnd;

                if (endHours < startHours || (endHours == startHours && endMinutes < startMinutes))
                {
                    // Session spans across midnight - end time is next day
                    sessionEnd = sessionDate.AddDays(1).AddHours(endHours).AddMinutes(endMinutes);
                }
                else
                {
                    // Normal session within the same day
                    sessionEnd = sessionDate.AddHours(endHours).AddMinutes(endMinutes);
                }

                TimeSpan duration = sessionEnd - sessionStart;
                int totalSeconds = (int)duration.TotalSeconds;

                if (totalSeconds < 60)
                {
                    minTotalTimeValidator.ErrorMessage = "Study Session must be at least 1 minute";
                    minTotalTimeValidator.IsValid = false;
                    ScriptManager.RegisterStartupScript(this, GetType(), "showValidation",
                        "validateAllFields();", true);
                    return;
                }

                string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                using (MySqlConnection con = new MySqlConnection(cs))
                {
                    con.Open();

                    using (MySqlTransaction transaction = con.BeginTransaction())
                    {
                        try
                        {
                            // 1. insert the study session
                            string sessionCommand = "INSERT INTO StudySession (sessionTitle, sessionTag, sessionStart, sessionEnd, sessionDuration, leaderID) VALUES (@title, @tag, @start, @end, @duration, @leaderId)";

                            using (MySqlCommand cmd = new MySqlCommand(sessionCommand, con, transaction))
                            {
                                cmd.Parameters.AddWithValue("@title", Session["sessionTitle"]);
                                cmd.Parameters.AddWithValue("@tag", Session["sessionTag"]);
                                cmd.Parameters.AddWithValue("@start", sessionStart);
                                cmd.Parameters.AddWithValue("@end", sessionEnd);
                                cmd.Parameters.AddWithValue("@duration", totalSeconds);
                                cmd.Parameters.AddWithValue("@leaderId", Convert.ToInt32(Session["userID"]));

                                cmd.ExecuteNonQuery();
                            }

                            // 2. get the new session ID
                            int newSessionID;
                            using (MySqlCommand cmdID = new MySqlCommand("SELECT LAST_INSERT_ID()", con, transaction))
                            {
                                newSessionID = Convert.ToInt32(cmdID.ExecuteScalar());
                                Session["sessionID"] = newSessionID;
                                Session["sessionDuration"] = totalSeconds;
                            }

                            // 3. insert the session creator as participant
                            string creatorCommand = "INSERT INTO StudySessionParticipants (sessionID, userID, accepted) VALUES (@sessionId, @userId, @accepted)";

                            using (MySqlCommand cmdCreator = new MySqlCommand(creatorCommand, con, transaction))
                            {
                                cmdCreator.Parameters.AddWithValue("@sessionId", newSessionID);
                                cmdCreator.Parameters.AddWithValue("@userId", Convert.ToInt32(Session["userID"]));
                                cmdCreator.Parameters.AddWithValue("@accepted", true); // accepted by default
                                cmdCreator.ExecuteNonQuery();
                            }

                            // 4. insert all invited friends as participants
                            if (invitedFriends.Count > 0)
                            {
                                // get userIDs for all invited usernames
                                Dictionary<string, int> usernameToIdMap = new Dictionary<string, int>();
                                string getUserIdsCommand = "SELECT userID, username FROM Users WHERE username IN (" + string.Join(",", invitedFriends.Select(f => "@username" + invitedFriends.IndexOf(f))) + ")";

                                using (MySqlCommand cmdGetIds = new MySqlCommand(getUserIdsCommand, con, transaction))
                                {
                                    for (int i = 0; i < invitedFriends.Count; i++)
                                    {
                                        cmdGetIds.Parameters.AddWithValue("@username" + i, invitedFriends[i]);
                                    }

                                    using (MySqlDataReader reader = cmdGetIds.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            usernameToIdMap.Add(reader["username"].ToString(), Convert.ToInt32(reader["userID"]));
                                        }
                                    }
                                }

                                // insert all participants
                                string friendCommand = "INSERT INTO StudySessionParticipants (sessionID, userID, accepted) VALUES (@sessionId, @userId, @accepted)";

                                foreach (string friendUsername in invitedFriends)
                                {
                                    int friendId;
                                    if (usernameToIdMap.TryGetValue(friendUsername, out friendId))
                                    {
                                        using (MySqlCommand cmdFriend = new MySqlCommand(friendCommand, con, transaction))
                                        {
                                            cmdFriend.Parameters.AddWithValue("@sessionId", newSessionID);
                                            cmdFriend.Parameters.AddWithValue("@userId", friendId);
                                            cmdFriend.Parameters.AddWithValue("@accepted", false); // not replied yet
                                            cmdFriend.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }

                            //5. insert into calendar event table
                            string eventCommand = "INSERT into CalendarEvent (eventDesc, eventDate, tagID, userID) VALUES (@desc, @eventDate, @tagID, @userID)";
                            using (MySqlCommand cmdEvent = new MySqlCommand(eventCommand, con, transaction))
                            {
                                cmdEvent.Parameters.AddWithValue("@desc", Session["sessionTitle"]);
                                cmdEvent.Parameters.AddWithValue("@eventDate", sessionDate);
                                cmdEvent.Parameters.AddWithValue("@tagID", 1);
                                cmdEvent.Parameters.AddWithValue("@userID", Convert.ToInt32(Session["userID"]));

                                cmdEvent.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            // Increment scheduledStudySessions counter and check for badges
                            int thisUserID = Convert.ToInt32(Session["userID"]);
                            using (MySqlConnection con2 = new MySqlConnection(cs))
                            {
                                string updateCommand = "UPDATE Users SET scheduledStudySessions = scheduledStudySessions + 1 WHERE userID = @userID";
                                using (MySqlCommand cmd = new MySqlCommand(updateCommand, con2))
                                {
                                    cmd.Parameters.AddWithValue("@userID", thisUserID);
                                    con2.Open();
                                    cmd.ExecuteNonQuery();
                                    con2.Close();
                                }
                            }

                            // Check for scheduled study session badges
                            CheckScheduledStudySessionBadges(thisUserID);

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "showPopup", "showSuccessPopup();", true);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            minTotalTimeValidator.ErrorMessage = "Error creating study session: " + ex.Message;
                            minTotalTimeValidator.IsValid = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                minTotalTimeValidator.ErrorMessage = "An error occurred: " + ex.Message;
                minTotalTimeValidator.IsValid = false;
                ScriptManager.RegisterStartupScript(this, GetType(), "showValidation",
                    "validateAllFields();", true);
            }
        }
    }

    // FOR GETTING SCHEDULED STUDY SESSIONS BADGE
    public void CheckScheduledStudySessionBadges(int userID)
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        using (MySqlConnection con = new MySqlConnection(cs))
        {
            con.Open();

            string getCountQuery = "SELECT scheduledStudySessions FROM Users WHERE userID = @userID";
            int scheduledCount = 0;
            using (MySqlCommand getCountCmd = new MySqlCommand(getCountQuery, con))
            {
                getCountCmd.Parameters.AddWithValue("@userID", userID);
                object result = getCountCmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    scheduledCount = Convert.ToInt32(result);
                }
            }

            if (scheduledCount >= 15)
            {
                AwardBadge(con, userID, 7, "Gold");
            }
            else if (scheduledCount >= 10)
            {
                AwardBadge(con, userID, 7, "Silver");
            }
            else if (scheduledCount >= 5)
            {
                AwardBadge(con, userID, 7, "Bronze");
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

    protected void minTotalTimeValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            int startHours = int.Parse(txtStartTimeHours.Text);
            int startMinutes = int.Parse(txtStartTimeMinutes.Text);
            int endHours = int.Parse(txtEndTimeHours.Text);
            int endMinutes = int.Parse(txtEndTimeMinutes.Text);

            int startTotal = (startHours * 3600) + (startMinutes * 60);
            int endTotal = (endHours * 3600) + (endMinutes * 60);

            // Handle sessions that span across midnight (same fix as client-side)
            int duration;
            if (endTotal < startTotal)
            {
                // Session spans across midnight - add 24 hours to end time
                duration = (endTotal + (24 * 3600)) - startTotal;
            }
            else
            {
                // Normal session within the same day
                duration = endTotal - startTotal;
            }

            args.IsValid = duration >= 60;
            minTotalTimeValidator.ErrorMessage = args.IsValid ? "" : "Study Session must be at least 1 minute";
        }
        catch
        {
            args.IsValid = false;
            minTotalTimeValidator.ErrorMessage = "Invalid time values";
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
        string query = "SELECT StudySession.sessionID, StudySession.sessionTitle, StudySession.sessionTag, StudySession.sessionStart, StudySession.sessionEnd, Users.username FROM (StudySessionParticipants INNER JOIN StudySession ON StudySessionParticipants.sessionID = StudySession.sessionID) INNER JOIN Users ON StudySession.leaderID = Users.userID WHERE StudySessionParticipants.userID = @userID AND StudySessionParticipants.accepted = false ORDER BY StudySession.sessionID ASC";

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

            litNotificationText.Text = "<p><span style='text-decoration:underline;'>Study session invitation</span></p><table><tr><td><p>From:</p></td><td><p><span style='font-weight:bold;'>" + invite.leaderUsername + "</span></p></td></tr>" + "<tr><td><p>Title:</p></td><td><p><span style='font-weight:bold;'>" + invite.title + "</span></p></td></tr>" + "<tr><td><p>Tag:</p></td><td><p><span style='font-weight:bold;'>" + invite.tag + "</span></p></td></tr>" + "<tr><td><p>Starts:</p></td><td><p><span style='font-weight:bold;'>" + invite.startTime.ToString("dddd, dd MMMM yyyy @ HH:mm") + "</span></p></td></tr><tr><td><p>Ends:</p></td><td><p><span style='font-weight:bold;'>" + invite.endTime.ToString("dddd, dd MMMM yyyy @ HH:mm") + "</span></p></td></tr></table>";

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
        string updateQuery = "UPDATE StudySessionParticipants SET accepted = true WHERE sessionID = @sessionID AND userID = @userID";
        using (MySqlConnection conn = new MySqlConnection(cs))
        using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
        {
            cmd.Parameters.AddWithValue("@sessionID", sessionID);
            cmd.Parameters.AddWithValue("@userID", Session["userID"]);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

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

        if (invite != null)
        {
            string insertQuery = "INSERT INTO CalendarEvent (eventDesc, eventDate, tagID, userID) VALUES (@eventDesc, @eventDate, @tagID, @userID)";
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
                cmd2.ExecuteNonQuery();
            }
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
        Response.Redirect("A700_Create-study-session_4.aspx");
    }

    protected void btnSure_Click(object sender, EventArgs e)
    {
        int sessionID = int.Parse(hiddenSessionID.Value);
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        string deleteQuery = "DELETE FROM StudySessionParticipants WHERE sessionID = @sessionID AND userID = @userID AND accepted = false";
        using (MySqlConnection conn = new MySqlConnection(cs))
        using (MySqlCommand cmd = new MySqlCommand(deleteQuery, conn))
        {
            cmd.Parameters.AddWithValue("@sessionID", sessionID);
            cmd.Parameters.AddWithValue("@userID", Session["userID"]);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        hiddenShowDeclineConfirmed.Value = "true";

        RemoveInviteAndShowNext(sessionID);
    }

    protected void btnNotSure_Click(object sender, EventArgs e)
    {
        Response.Redirect("A700_Create-study-session_4.aspx");
    }

    protected void btnOkayDeclined_Click(object sender, EventArgs e)
    {
        Response.Redirect("A700_Create-study-session_4.aspx");
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
}