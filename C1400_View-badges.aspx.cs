using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

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
                    //need to copy all of this to every page. needs to call all these methods
                    int userXP = GetUserXP(cs, userID);
                    lblXPAmount.Text = userXP.ToString();
                    GetLevelInformation(cs, userID);
                    GetUserStats(cs, userID);
                    GetUserProfileIcon(cs, userID);
                    LoadPendingInvitesFromDB();
                    LoadUpcomingSessions();
                    LoadBadges();
                }
                else
                {
                    lblLevelNumber.Text = "N/A";
                    lblXPAmount.Text = "N/A";
                    lblPaws.Text = "N/A";
                }
            }
            else
            {
                Response.Redirect("Landing-page.aspx");
            }
            ShowNextInvite();
        }
    }

    public class BadgeIcon
    {
        public string badgeName { get; set; }
        public string badgeDescBronze { get; set; }
        public string badgeDescSilver { get; set; }
        public string badgeDescGold { get; set; }
        public string badgeIconNum { get; set; }
        public string badgeType { get; set; }
    }

    private void LoadBadges()
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        string query = @"
                        SELECT b.badgeName, b.badgeDescBronze, b.badgeDescSilver, b.badgeDescGold, 
                        b.badgeIconNum, ub.badgeType
                        FROM Badges b
                        LEFT JOIN UserBadge ub ON b.badgeID = ub.badgeID
                        WHERE ub.userID = ? OR ub.userID IS NULL";

        List<BadgeIcon> badgeList = new List<BadgeIcon>();

        using (OleDbConnection con = new OleDbConnection(cs))
        using (OleDbCommand cmd = new OleDbCommand(query, con))
        {
            cmd.Parameters.AddWithValue("?", Session["userID"]);

            con.Open();
            using (OleDbDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    BadgeIcon badge = new BadgeIcon
                    {
                        badgeName = reader["badgeName"].ToString(),
                        badgeDescBronze = reader["badgeDescBronze"].ToString(),
                        badgeDescSilver = reader["badgeDescSilver"].ToString(),
                        badgeDescGold = reader["badgeDescGold"].ToString(),
                        badgeIconNum = GetBadgeImagePath(Convert.ToInt32(reader["badgeIconNum"])),
                        badgeType = reader["badgeType"] == DBNull.Value ? "none" : reader["badgeType"].ToString().ToLower()
                    };
                    badgeList.Add(badge);
                }
            }
        }

        rpPets.DataSource = badgeList;
        rpPets.DataBind();
    }


    private string GetBadgeImagePath(int badgeImageID)
    {
        switch (badgeImageID)
        {
            case 1: return "~/Images/Badges/BusyBee.png";
            case 2: return "~/Images/Badges/LoneWoof.png";
            case 3: return "~/Images/Badges/EarlyBird.png";
            case 4: return "~/Images/Badges/NightOwl.png";
            case 5: return "~/Images/Badges/SlothingOnTheJob.png";
            case 6: return "~/Images/Badges/DreamTeam.png";
            case 7: return "~/Images/Badges/RignLeader.png";
            case 8: return "~/Images/Badges/Cancelotl.png";
            case 9: return "~/Images/Badges/NotMyBloblem.png";
            case 10: return "~/Images/Badges/AcademicWeapon.png";
            case 11: return "~/Images/Badges/11_Collector.png";
            case 12: return "~/Images/Badges/Meanie.png";
            case 13: return "~/Images/Badges/FriendsPurrever.png";
            case 14: return "~/Images/Badges/PeskyPelican.png";
            case 15: return "~/Images/Badges/PuffingPopular.png";
            case 16: return "~/Images/Badges/RisingStar.png";

            default: return "~/Images/Badges/PeskyPelican.png";
        }
    }
    protected string GetStarHtml(string badgeType)
    {
        int stars = 0;
        if (badgeType == "bronze") stars = 1;
        else if (badgeType == "silver") stars = 2;
        else if (badgeType == "gold") stars = 3;

        string filledStar = "<img src='Icons/icons8-star-filled-white-96.png' class='star-icon' />";
        string emptyStar = "<img src='Icons/icons8-star-white-96.png' class='star-icon' />";

        string html = "";
        for (int i = 0; i < 3; i++)
        {
            html += i < stars ? filledStar : emptyStar;
        }

        return html;
    }
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

    private void GetLevelInformation(string connectionString, string userID)
    {
        string query = "SELECT levelID FROM CurrentLevel WHERE userID = @userID";

        using (OleDbConnection con = new OleDbConnection(connectionString))
        using (OleDbCommand cmd = new OleDbCommand(query, con))
        {
            cmd.Parameters.AddWithValue("@userID", userID);
            try
            {
                con.Open();
                object result = cmd.ExecuteScalar();
                lblLevelNumber.Text = (result != null) ? result.ToString() : "N/A";
            }
            catch (Exception ex)
            {
                lblLevelNumber.Text = "ERR";
            }
        }
    }

    private void GetUserStats(string connectionString, string userID)
    {
        string query = "SELECT userCoinCount FROM Users WHERE userID = @userID";

        using (OleDbConnection con = new OleDbConnection(connectionString))
        using (OleDbCommand cmd = new OleDbCommand(query, con))
        {
            cmd.Parameters.AddWithValue("@userID", userID);
            try
            {
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
            catch (Exception ex)
            {
                lblPaws.Text = "ERR";
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
}