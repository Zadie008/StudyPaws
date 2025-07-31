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
        if (!IsPostBack)
        {
            if (Session["Username"] != null)
            {
                string username = Session["Username"].ToString(); //need this on every page
                string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                string userID = GetUserID(username, cs);

                if (!string.IsNullOrEmpty(userID))
                {
                    // Get user's current XP and level
                    int userXP = GetUserXP(cs, userID);
                    // REMOVED: lblXPAmount.Text = userXP.ToString(); // This line caused the error

                    // Get current level and the XP required for the next level
                    Tuple<int, int, int> levelInfo = GetLevelInformation(cs, userID);
                    int currentLevel = levelInfo.Item1;
                    int currentLevelXpAmount = levelInfo.Item2; // XP needed to reach current level
                    int nextLevelXpAmount = levelInfo.Item3; // XP needed to reach next level

                    lblLevelNumber.Text = currentLevel.ToString();

                    // Calculate progress for the progress bar
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

    //Please copy all of this code
    //everyline of code until you see double lines of comments
    private void LoadPendingInvitesFromDB()
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        List<SessionInvite> pendingInvites = new List<SessionInvite>();
        string query = "SELECT StudySession.sessionID, StudySession.sessionTitle, StudySession.sessionTag, StudySession.sessionStart, StudySession.sessionEnd, Users.username FROM (StudySessionParticipants INNER JOIN StudySession ON StudySessionParticipants.sessionID = StudySession.sessionID) INNER JOIN Users ON StudySession.leaderID = Users.userID WHERE StudySessionParticipants.userID = ? AND StudySessionParticipants.replied = false ORDER BY StudySession.sessionID ASC";

        using (OleDbConnection conn = new OleDbConnection(cs))
        using (OleDbCommand cmd = new OleDbCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("?", Session["userID"]);
            conn.Open();
            using (OleDbDataReader reader = cmd.ExecuteReader())
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
        string updateQuery = "UPDATE StudySessionParticipants SET replied = true, sessionStatus = 'Accepted' WHERE sessionID = ? AND userID = ?";
        using (OleDbConnection conn = new OleDbConnection(cs))
        using (OleDbCommand cmd = new OleDbCommand(updateQuery, conn))
        {
            cmd.Parameters.AddWithValue("?", sessionID);
            cmd.Parameters.AddWithValue("?", Session["userID"]);
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
            string insertQuery = "INSERT INTO CalendarEvent (eventDesc, eventDate, tagID, userID) VALUES (?, ?, ?, ?)";
            using (OleDbConnection conn = new OleDbConnection(cs))
            using (OleDbCommand cmd2 = new OleDbCommand(insertQuery, conn))
            {
                string eventDesc = invite.title + " (From: " + invite.leaderUsername + ")";
                DateTime eventDate = invite.startTime;
                int tagID = 1;
                int userID = Convert.ToInt32(Session["userID"]);

                cmd2.Parameters.AddWithValue("?", eventDesc);
                cmd2.Parameters.AddWithValue("?", eventDate);
                cmd2.Parameters.AddWithValue("?", tagID);
                cmd2.Parameters.AddWithValue("?", userID);

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
        string query = "SELECT StudySession.sessionID, StudySession.sessionStart FROM StudySession INNER JOIN StudySessionParticipants ON StudySession.sessionID = StudySessionParticipants.sessionID WHERE StudySessionParticipants.userID = ? AND StudySessionParticipants.replied = true AND StudySessionParticipants.sessionStatus = 'Accepted'";

        List<string> jsSessionTimes = new List<string>();

        using (OleDbConnection conn = new OleDbConnection(cs))
        using (OleDbCommand cmd = new OleDbCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("?", Session["userID"]);
            conn.Open();
            using (OleDbDataReader reader = cmd.ExecuteReader())
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
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        string deleteQuery = "DELETE FROM StudySessionParticipants WHERE sessionID = ? AND userID = ? AND replied = false";
        using (OleDbConnection conn = new OleDbConnection(cs))
        using (OleDbCommand cmd = new OleDbCommand(deleteQuery, conn))
        {
            cmd.Parameters.AddWithValue("?", sessionID);
            cmd.Parameters.AddWithValue("?", Session["userID"]);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        hiddenShowDeclineConfirmed.Value = "true";

        RemoveInviteAndShowNext(sessionID);
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
        if (Session["sessionID"] != null)
        {
            Response.Redirect("A1400_View-study-session.aspx");
        }
    }

    private string GetUserID(string username, string connectionString)
    {
        string query = "SELECT userID FROM Users WHERE username = @username";
        using (OleDbConnection con = new OleDbConnection(connectionString))
        using (OleDbCommand cmd = new OleDbCommand(query, con))
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

        using (OleDbConnection con = new OleDbConnection(connectionString))
        using (OleDbCommand cmd = new OleDbCommand(query, con))
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

    // CORRECTED: Enclosed 'Level' table name in square brackets for MS Access
    private Tuple<int, int, int> GetLevelInformation(string connectionString, string userID)
    {
        int currentLevel = 0;
        int currentLevelXpAmount = 0;
        int nextLevelXpAmount = 0;

        // Get the user's current level
        string currentLevelQuery = "SELECT levelID FROM CurrentLevel WHERE userID = @userID";
        using (OleDbConnection con = new OleDbConnection(connectionString))
        using (OleDbCommand cmdCurrentLevel = new OleDbCommand(currentLevelQuery, con))
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
                return Tuple.Create(0, 0, 0); // Return default values if current level not found
            }
        }

        // Query 1: Get XP amount for the current level
        // Enclose 'Level' in square brackets
        string currentLevelXPQuery = "SELECT xpAmount FROM [Level] WHERE levelNum = @currentLevel";
        using (OleDbConnection con = new OleDbConnection(connectionString))
        using (OleDbCommand cmdCurrentXP = new OleDbCommand(currentLevelXPQuery, con))
        {
            cmdCurrentXP.Parameters.AddWithValue("@currentLevel", currentLevel);
            con.Open();
            object result = cmdCurrentXP.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                currentLevelXpAmount = Convert.ToInt32(result);
            }
        }

        // Query 2: Get XP amount for the next level
        // Enclose 'Level' in square brackets
        string nextLevelXPQuery = "SELECT xpAmount FROM [Level] WHERE levelNum = @nextLevel";
        using (OleDbConnection con = new OleDbConnection(connectionString))
        using (OleDbCommand cmdNextXP = new OleDbCommand(nextLevelXPQuery, con))
        {
            cmdNextXP.Parameters.AddWithValue("@nextLevel", currentLevel + 1); // Get XP for the next level
            con.Open();
            object result = cmdNextXP.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                nextLevelXpAmount = Convert.ToInt32(result);
            }
            else
            {
                // If no next level found, it means the user is at the maximum level.
                // In this case, set nextLevelXpAmount to be the same as currentLevelXpAmount
                // so the progress bar shows 100% or adjust as per your logic for max level.
                nextLevelXpAmount = currentLevelXpAmount;
            }
        }

        return Tuple.Create(currentLevel, currentLevelXpAmount, nextLevelXpAmount);
    }

    // New method to calculate and set the progress bar width
    private void CalculateXPProgressBar(int userXP, int currentLevelXpAmount, int nextLevelXpAmount)
    {
        if (nextLevelXpAmount <= currentLevelXpAmount) // This means user is at max level or next level XP is not properly defined
        {
            xpProgressBar.Style["width"] = "100%";
            lblXPPercentage.Text = "100%";
            return;
        }

        // XP needed to progress from current level to next
        int xpToNextLevel = nextLevelXpAmount - currentLevelXpAmount;
        // XP gained within the current level
        int xpGainedInCurrentLevel = userXP - currentLevelXpAmount;

        if (xpToNextLevel > 0)
        {
            double progress = (double)xpGainedInCurrentLevel / xpToNextLevel * 100;
            if (progress < 0) progress = 0; // Ensure progress is not negative
            if (progress > 100) progress = 100; // Cap progress at 100%

            xpProgressBar.Style["width"] = progress.ToString("F0") + "%"; // Format to 0 decimal places
            lblXPPercentage.Text = progress.ToString("F0") + "%"; // Display percentage
        }
        else
        {
            // If xpToNextLevel is 0 or negative (shouldn't happen if levels are set correctly)
            // or if userXP is already at or above nextLevelXpAmount, consider it 100%
            xpProgressBar.Style["width"] = "100%";
            lblXPPercentage.Text = "100%";
        }
    }


    private void GetUserStats(string connectionString, string userID)
    {
        string query = "SELECT userCoinCount FROM Users WHERE userID = @userID";

        using (OleDbConnection con = new OleDbConnection(connectionString))
        using (OleDbCommand cmd = new OleDbCommand(query, con))
        {
            cmd.Parameters.AddWithValue("@userID", userID);

            con.Open();
            using (OleDbDataReader reader = cmd.ExecuteReader())
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

        using (OleDbConnection con = new OleDbConnection(connectionString))
        using (OleDbCommand cmd = new OleDbCommand(query, con))
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
    //please stop copying up until here
    //this seems to be the end
}

// SessionInvite class (if not already defined)
public class SessionInvite
{
    public int sessionID { get; set; }
    public string leaderUsername { get; set; }
    public string title { get; set; }
    public string tag { get; set; }
    public DateTime startTime { get; set; }
    public DateTime endTime { get; set; }
}