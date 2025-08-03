using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;

public partial class Default2 : System.Web.UI.Page
{
    string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["userID"] != null)
        {
            ddlFilter.Visible = IsToDoFilterVisible;
            userIDHidden.Value = Convert.ToString(Session["userID"]);
            string username = Session["Username"].ToString();
            string userID = GetUserID(username, connString);
            if (!IsPostBack)
            {
                ViewState["SelectedFilter"] = "All";
                ddlFilter.SelectedValue = "All";
                LoadTasks();
                int userXP = GetUserXP(connString, userID);
                Tuple<int, int, int> levelInfo = GetLevelInformation(connString, userID);
                int currentLevel = levelInfo.Item1;
                int currentLevelXpAmount = levelInfo.Item2;
                int nextLevelXpAmount = levelInfo.Item3;
                lblLevelNumber.Text = currentLevel.ToString();
                CalculateXPProgressBar(userXP, currentLevelXpAmount, nextLevelXpAmount);

                GetUserStats(connString, userID);
                GetUserProfileIcon(connString, userID);
                LoadPendingInvitesFromDB();
                LoadUpcomingSessions();
               
            }
            else
            {
                LoadTasks();
            }
        }
        else
        {
            Response.Redirect("Login.aspx");
        }

        ShowNextInvite();
    }
    private bool IsToDoFilterVisible
    {
        get
        {
            return ViewState["FilterVisible"] != null && (bool)ViewState["FilterVisible"];
        }
        set
        {
            ViewState["FilterVisible"] = value;
        }
    }
    private void LoadTasks()
    {
        string filter = ddlFilter.SelectedValue ?? "All";
        ViewState["SelectedFilter"] = filter;

        string whereClause = "";

        if (filter == "Completed")
            whereClause = "AND taskStatus = True";
        else if (filter == "InProgress")
            whereClause = "AND taskStatus = False";

        DataTable dt = new DataTable();

        using (OleDbConnection conn = new OleDbConnection(connString))
        {
            conn.Open();
            string sql = "SELECT * FROM ToDoListTask WHERE userID = ? " + whereClause + " ORDER BY taskStatus DESC";
            OleDbCommand cmd = new OleDbCommand(sql, conn);
            cmd.Parameters.AddWithValue("?", Session["userID"]);
            dt.Load(cmd.ExecuteReader());
        }

        rptTasks.DataSource = dt;
        rptTasks.DataBind();
    }
    protected void txtNewTask_TextChanged(object sender, EventArgs e)
    {
        btnAdd_Click(sender, e);
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        string taskDesc = txtNewTask.Text.Trim();
        if (taskDesc == "")
            return;

        using (OleDbConnection conn = new OleDbConnection(connString))
        {
            conn.Open();
            string sql = "INSERT into [ToDoListTask] ([taskDesc], [taskStatus], [userID]) VALUES (?, False, ?)";
            OleDbCommand cmd = new OleDbCommand(sql, conn);
            cmd.Parameters.AddWithValue("?", taskDesc);
            cmd.Parameters.AddWithValue("?", Session["userID"]);
            cmd.ExecuteNonQuery();
        }

        txtNewTask.Text = "";
        Response.Redirect(Request.RawUrl);
    }

    protected void rptTasks_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int taskID = Convert.ToInt32(e.CommandArgument);

        if (e.CommandName == "Toggle")
        {
            HiddenField taskIDHidden = (HiddenField)e.Item.FindControl("taskIDHidden");
            bool currentStatus = GetTaskStatus(taskID);

            if (!currentStatus)
            {
                ViewState["PendingAction"] = "Toggle";
                ViewState["PendingTaskID"] = taskID;
                ScriptManager.RegisterStartupScript(this, GetType(), "showTogglePopup", "showPopup();", true);
            }
            else
            {
                ToggleTaskStatus(taskID);
                //LoadTasks();
            }


        }
        else if (e.CommandName == "Delete")
        {
            ViewState["PendingAction"] = "Delete";
            ViewState["PendingTaskID"] = taskID;

            ScriptManager.RegisterStartupScript(this, GetType(), "showDeletePopup", "showPopupDelete();", true);

        }
        else if (e.CommandName == "Edit")
        {
            ViewState["EditingTaskID"] = e.CommandArgument.ToString();
            LoadTasks();
        }
        else if (e.CommandName == "Save")
        {
            TextBox txtEditDesc = (TextBox)e.Item.FindControl("txtEditDesc");
            string newDesc = txtEditDesc.Text.Trim();
            if (string.IsNullOrWhiteSpace(newDesc))
                return;

            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                conn.Open();
                string query = "UPDATE ToDoListTask SET taskDesc = ? WHERE taskID = ?";
                OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("?", newDesc);
                cmd.Parameters.AddWithValue("?", taskID);
                cmd.ExecuteNonQuery();
            }
            ViewState["EditingTaskID"] = null;
            LoadTasks();
        }
    }

    protected void ddlFilter_SelectedIndexChanged(object sender, EventArgs e)
    {
        string selectedFilter = ddlFilter.SelectedValue;
        ViewState["SelectedFilter"] = selectedFilter;
        LoadTasks();
    }
    protected void toDoFilterBtn_Click(object sender, EventArgs e)
    {
        IsToDoFilterVisible = !IsToDoFilterVisible;
        ddlFilter.Visible = IsToDoFilterVisible;
    }
    protected void rptTasks_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            TextBox txtDesc = (TextBox)e.Item.FindControl("txtEditDesc");
            ImageButton editBtn = (ImageButton)e.Item.FindControl("editBtn");
            ImageButton saveBtn = (ImageButton)e.Item.FindControl("saveEditBtn");
            HiddenField taskIDHidden = (HiddenField)e.Item.FindControl("taskIDHidden");

            if (txtDesc != null && editBtn != null && saveBtn != null && taskIDHidden != null)
            {
                string editingTaskID = Convert.ToString(ViewState["EditingTaskID"]);


                if (editingTaskID == taskIDHidden.Value)
                {
                    txtDesc.ReadOnly = false;
                    editBtn.Visible = false;
                    saveBtn.Visible = true;
                    txtDesc.Focus();
                }
                else
                {
                    txtDesc.ReadOnly = true;
                    editBtn.Visible = true;
                    saveBtn.Visible = false;
                }
            }
        }
    }
    protected void btnYesDelete_Click(object sender, EventArgs e)
    {
        string action = ViewState["PendingAction"] as string;
        int taskID = Convert.ToInt32(ViewState["PendingTaskID"]);

        if (action == "Delete")
        {
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                conn.Open();
                string query = "DELETE FROM ToDoListTask WHERE taskID = ?";
                OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("?", taskID);
                cmd.ExecuteNonQuery();
            }
            Response.Redirect(Request.RawUrl);
            ViewState["PendingAction"] = null;
            ViewState["PendingTaskID"] = null;
        }
    }
    protected void btnNoDelete_Click(Object sender, EventArgs e)
    {
        ViewState["PendingAction"] = null;
        ViewState["PendingTaskID"] = null;
        LoadTasks();
    }
    private bool GetTaskStatus(int taskID)
    {
        using (OleDbConnection conn = new OleDbConnection(connString))
        {
            conn.Open();
            string query = "SELECT taskStatus FROM ToDoListTask WHERE taskID = ?";
            OleDbCommand cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue("?", taskID);
            var result = cmd.ExecuteScalar();
            return result != DBNull.Value && Convert.ToBoolean(result);
        }
    }
    private void ToggleTaskStatus(int taskID)
    {

        using (OleDbConnection conn = new OleDbConnection(connString))
        {
            conn.Open();
            string query = "UPDATE ToDoListTask SET taskStatus = NOT taskStatus WHERE taskID = ?";
            OleDbCommand cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue("?", taskID);
            cmd.ExecuteNonQuery();
        }
        //Response.Redirect(Request.RawUrl);
        ViewState["PendingAction"] = null;
        ViewState["PendingTaskID"] = null;
    }

    protected void btnYayLevelUp_Click(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(this, GetType(), "hideLevelUp", "hideLevelUp();", false);
    }

    protected void btnThankYou_Click(object sender, EventArgs e)
    {
        //if (ViewState["PendingAction"] != null && ViewState["PendingAction"].ToString() == "Toggle" && ViewState["PendingTaskID"] != null)
        //{
        //    int taskID = Convert.ToInt32(ViewState["PendingTaskID"]);
        //    ToggleTaskStatus(taskID);
        //}
        string userID = userIDHidden.Value;
         
        if (string.IsNullOrEmpty(userID))
        {
            Response.Write("<script>alert('Error: User not found.');</script>");
            return;
        }

        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        using (OleDbConnection con = new OleDbConnection(cs))
        {
            try
            {
                con.Open();

                // Get current XP
                string selectXPQuery = "SELECT userXP FROM [Users] WHERE userID = ?";
                OleDbCommand selectXPCmd = new OleDbCommand(selectXPQuery, con);
                selectXPCmd.Parameters.AddWithValue("?", userID);

                object xpObj = selectXPCmd.ExecuteScalar();
                int currentXP = (xpObj != null && xpObj != DBNull.Value) ? Convert.ToInt32(xpObj) : 0;
                int newXP = currentXP + 10;

                // Update XP
                string updateXPQuery = "UPDATE [Users] SET userXP = ? WHERE userID = ?";
                OleDbCommand updateXPCmd = new OleDbCommand(updateXPQuery, con);
                updateXPCmd.Parameters.AddWithValue("?", newXP);
                updateXPCmd.Parameters.AddWithValue("?", userID);
                updateXPCmd.ExecuteNonQuery();

                // Check new level
                string getNewLevelQuery = "SELECT MAX(levelNum) FROM [Level] WHERE xpAmount <= ?";
                OleDbCommand getNewLevelCmd = new OleDbCommand(getNewLevelQuery, con);
                getNewLevelCmd.Parameters.AddWithValue("?", newXP);

                object newLevelObj = getNewLevelCmd.ExecuteScalar();
                int newLevelNum = (newLevelObj != null && newLevelObj != DBNull.Value) ? Convert.ToInt32(newLevelObj) : 1;

                // Get current level
                string getCurrentLevelQuery = "SELECT levelID FROM CurrentLevel WHERE userID = ?";
                OleDbCommand getCurrentLevelCmd = new OleDbCommand(getCurrentLevelQuery, con);
                getCurrentLevelCmd.Parameters.AddWithValue("?", userID);

                object currentLevelObj = getCurrentLevelCmd.ExecuteScalar();
                int currentLevel = (currentLevelObj != null && currentLevelObj != DBNull.Value) ? Convert.ToInt32(currentLevelObj) : 1;

                if (newLevelNum > currentLevel)
                {
                    
                    // Update level
                    string updateLevelQuery = "UPDATE CurrentLevel SET levelID = ? WHERE userID = ?";
                    OleDbCommand updateLevelCmd = new OleDbCommand(updateLevelQuery, con);
                    updateLevelCmd.Parameters.AddWithValue("?", newLevelNum);
                    updateLevelCmd.Parameters.AddWithValue("?", userID);
                    updateLevelCmd.ExecuteNonQuery();

                    // Show Level Up popup
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowLevelUp", "showLevelUp();", true);


                }
                else
                {
                    
                   // ScriptManager.RegisterStartupScript(this, GetType(), "xpAddedAlert", "alert('XP added!');", true);
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('An error occurred: " + ex.Message + "');</script>");
            }
        }

        
        UpdateXPDisplay(userID);
        LoadTasks();
    }

    private void UpdateXPDisplay(string userID)
    {
        int userXP = GetUserXP(connString, userID);
        var levelInfo = GetLevelInformation(connString, userID);
        int currentLevel = levelInfo.Item1;
        int currentLevelXpAmount = levelInfo.Item2;
        int nextLevelXpAmount = levelInfo.Item3;
        lblLevelNumber.Text = currentLevel.ToString();
        CalculateXPProgressBar(userXP, currentLevelXpAmount, nextLevelXpAmount);
    }

    // start: header profile code
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

    private Tuple<int, int, int> GetLevelInformation(string connectionString, string userID)
    {
        int currentLevel = 0;
        int currentLevelXpAmount = 0;
        int nextLevelXpAmount = 0;
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
                return Tuple.Create(0, 0, 0);
            }
        }
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

        string nextLevelXPQuery = "SELECT xpAmount FROM [Level] WHERE levelNum = @nextLevel";
        using (OleDbConnection con = new OleDbConnection(connectionString))
        using (OleDbCommand cmdNextXP = new OleDbCommand(nextLevelXPQuery, con))
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
    // end: header profile code

    // start: notification bell code
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
    // end: notification bell code
}