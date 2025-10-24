using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
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
            LoadTags();

            if (Session["PendingEventTitle"] != null)
            {
                txtEventTitle.Text = Session["PendingEventTitle"].ToString();
                Session.Remove("PendingEventTitle");
            }
            if (Request.QueryString["newTag"] != null)
            {
                string newTagID = Request.QueryString["newTag"];
                if (dropdownEventTag.Items.FindByValue(newTagID) != null)
                {
                    dropdownEventTag.SelectedValue = newTagID;
                }
            }
            if (Request.QueryString["selectedTag"] != null)
            {
                string selectedTagID = Request.QueryString["selectedTag"];
                if (dropdownEventTag.Items.FindByValue(selectedTagID) != null)
                {
                    dropdownEventTag.SelectedValue = selectedTagID;
                }
            }
            if (Request.QueryString["date"] != null)
            {
                DateTime selectedDate;
                if (DateTime.TryParse(Request.QueryString["date"], out selectedDate))
                {
                    hiddenSelectedDate.Value = selectedDate.ToString("yyyy-MM-dd");
                }
            }
        }
            

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
                LoadUpcomingSessions();
            }
        }
        else
        {
            Response.Redirect("Landing-page.aspx");
        }
    }
    protected void btnBack_Click(object sender, EventArgs e)
    {
        string from = Request.QueryString["from"];
        if (from == "calendar")
        {
            Response.Redirect("B100_View-calendar.aspx");
        }
        else if (from == "dashboard")
        {
            Response.Redirect("B1600_View-dashboard.aspx");
        }
        else
        {
            Response.Redirect("Default.aspx");
        }
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Page.Validate("EventValidation");
        if (!Page.IsValid)
        {
            return;
        }
        if (string.IsNullOrWhiteSpace(txtEventTitle.Text)) // double check if not empty
        {
            return; 
        }
        String desc = txtEventTitle.Text;
        int userID = Convert.ToInt32(Session["userID"]);

        if (string.IsNullOrEmpty(dropdownEventTag.SelectedValue))
        {
            return;
        }
        int tag = int.Parse(dropdownEventTag.SelectedValue);

        DateTime eventDate = DateTime.Today;
        if (!string.IsNullOrEmpty(hiddenSelectedDate.Value))
        {
            DateTime.TryParse(hiddenSelectedDate.Value, out eventDate);
        }

        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        using (MySqlConnection conn = new MySqlConnection(cs))
        {
            conn.Open();
            string sql = "INSERT into CalendarEvent (eventDesc, eventDate, tagID, userID) VALUES (@desc, @eventDate, @tagID, @userID)";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@desc", desc);
            cmd.Parameters.AddWithValue("@eventDate", eventDate);
            cmd.Parameters.AddWithValue("@tagID", tag);
            cmd.Parameters.AddWithValue("@userID", userID);
            cmd.ExecuteNonQuery();
        }

        string from = Request.QueryString["from"];
        if (from == "calendar")
        {
            Response.Redirect("B100_View-calendar.aspx");
        }
        else if (from == "dashboard")
        {
            Response.Redirect("B1600_View-dashboard.aspx");
        }
        else
        {
            Response.Redirect("Default.aspx");
        }
    }
    protected void btnNewTag_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(txtEventTitle.Text))
        {
            Session["PendingEventTitle"] = txtEventTitle.Text;
        }
        Response.Redirect("B600_Add_Tags.aspx?from=addevent");
    }
    protected void LoadTags()
    {
        int userID = Convert.ToInt32(Session["userID"]);
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        using (MySqlConnection conn = new MySqlConnection(cs))
        {
            conn.Open();
            string query = "SELECT tagID, tagName FROM CalendarEventTag WHERE userIDLink = 0 OR userIDLink = @userID";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@userID", userID);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                dropdownEventTag.Items.Clear();
                dropdownEventTag.Items.Add(new ListItem(""));

                while (reader.Read())
                {
                    string tagName = reader["tagName"].ToString();
                    int tagID = Convert.ToInt32(reader["tagID"]);

                    if (tagID != 1)
                    {
                        dropdownEventTag.Items.Add(new ListItem(tagName, tagID.ToString()));
                    }
                }
            }
        }
    }
    protected void btnEditTag_Click(Object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(dropdownEventTag.SelectedValue))
        {
            int tagID = Convert.ToInt32(dropdownEventTag.SelectedValue);
            if (tagID == 1 || tagID == 2)
            {
                return;
            }
            if (!string.IsNullOrWhiteSpace(txtEventTitle.Text))
            {
                Session["PendingEventTitle"] = txtEventTitle.Text;
            }
            Session["EditTagID"] = tagID;
            Response.Redirect("B700-B800_Edit_Delete_Tags.aspx?from=addevent");
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
}