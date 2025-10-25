using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Default2 : System.Web.UI.Page
{
    string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
    private Dictionary<int, string> tagColours;
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

        if (Session["userID"] != null)
        {
            string username = Session["Username"].ToString();
            userIDHidden.Value = Convert.ToString(Session["userID"]);

            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string userID = GetUserID(username, cs);

            LoadTagColours();
            if (!IsPostBack)
            {
                ViewState["SelectedFilter"] = "All";
                ddlFilter.SelectedValue = "All";
                LoadTasks();

                DateTime currentDate = DateTime.Today;
                hfYear.Value = currentDate.Year.ToString();
                hfMonth.Value = DateTime.Today.Month.ToString();
                LoadCalendar(currentDate.Year, currentDate.Month);

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

                // Add level up check
                CheckForLevelUp(userID);
            }
            else
            {
                LoadTasks();
                LoadTagColours();
            }
        }
        else
        {
            Response.Redirect("Login.aspx");
        }

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
    
    private void LoadCalendar(int year, int month)
    {
        lblMonthYear.Text = new DateTime(year, month, 1).ToString("MMMM yyyy");
        literalCalendar.Text = GenerateCalendar(year, month);
        hfYear.Value = year.ToString();
        hfMonth.Value = month.ToString();
    }
    private String GenerateCalendar(int year, int month)
    {
        StringBuilder sb = new StringBuilder();
        DateTime firstDayOfMonth = new DateTime(year, month, 1);
        int daysInMonth = DateTime.DaysInMonth(year, month);

        int adjustedStartDay = ((int)firstDayOfMonth.DayOfWeek + 6) % 7;

        sb.Append("<table class='calendarBox'>");
        sb.Append("<tr>");
        string[] dayNames = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
        foreach (string dayName in dayNames)
        {
            sb.Append(string.Format("<th class='weeks'>{0}</th>", dayName));
        }
        sb.Append("</tr>");

        int currentDay = 1;

        DateTime prevMonth = firstDayOfMonth.AddMonths(-1);
        int daysInPrevMonth = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);

        DateTime nextMonth = firstDayOfMonth.AddMonths(1);
        int week = 0;

        while (currentDay <= daysInMonth)
        {
            sb.Append("<tr>");

            for (int dayOfWeek = 0; dayOfWeek < 7; dayOfWeek++)
            {

                if (week == 0 && dayOfWeek < adjustedStartDay)
                {
                    int prevDay = daysInPrevMonth - (adjustedStartDay - dayOfWeek - 1);
                    sb.Append(string.Format("<td class='otherMonth'>{0}</td>", prevDay));
                }
                else if (currentDay <= daysInMonth)
                {
                    DateTime thisDay = new DateTime(year, month, currentDay);
                    bool isToday = thisDay.Date == DateTime.Today;

                    List<string> events = GetEventsForDay(thisDay);

                    sb.Append("<td class='calendarCell'>");

                    // Day number
                    sb.Append("<div class='dayNumber'>");
                    if (isToday)
                    {
                        sb.AppendFormat("<span class='today'>{0}</span>", currentDay);
                    }
                    else
                    {
                        sb.AppendFormat("{0}", currentDay);
                    }
                    sb.Append("</div>");

                    // Events
                    sb.Append("<div class='events scrollableEvents'>");
                    foreach (string ev in events)
                    {
                        sb.Append(ev);
                    }
                    sb.Append("</div>");

                    // '+' Button
                    sb.AppendFormat(
                        "<a class='addEventBtn' href='B200_B500_Add-event_Select_Tag.aspx?date={0}&from=dashboard'>" + "<img src='Icons/icons8-add-new-white-96.png' class='addEventBtnImg' />" + "</a>",
                        thisDay.ToString("yyyy-MM-dd")
                    );

                    sb.Append("</td>");
                    currentDay++;

                }
                else
                {
                    int nextDay = (currentDay - daysInMonth);
                    sb.Append(string.Format("<td class='otherMonth'>{0}</td>", nextDay));
                    currentDay++;
                }
            }

            sb.Append("</tr>");
            week++;
        }

        sb.Append("</table>");
        return sb.ToString();

    }
    private List<string> GetEventsForDay(DateTime day)
    {
        List<string> events = new List<string>();
        int userID = Convert.ToInt32(Session["userID"]);

        using (MySqlConnection conn = new MySqlConnection(connString))
        {
            conn.Open();
            string loadEvents = "SELECT eventID, eventDesc, tagID FROM CalendarEvent " + "WHERE userID=@userID AND DATE(eventDate)=@eventDate";
            
            MySqlCommand cmd = new MySqlCommand(loadEvents, conn);
            cmd.Parameters.AddWithValue("@userID", userID);
            cmd.Parameters.AddWithValue("@eventDate", day.Date);
            
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int eventID = Convert.ToInt32(reader["eventID"]);
                    String desc = reader["eventDesc"].ToString();
                    int tagID = reader["tagID"] != DBNull.Value ? Convert.ToInt32(reader["tagID"]) : 0;
                   
                    
                    string tagColour = (tagColours!=null && tagColours.ContainsKey(tagID)) ? tagColours[tagID] : "#000000";

                    string source = "dashboard";
                    string targetPage = (tagID == 1) ? "DeleteStudySession.aspx" : "B300-B400_Edit_Delete_Event.aspx";

                    string eventHtml = "<div class='eventItem'>" +
                                       "<span class='eventDot' style='background-color:" + tagColour + ";'></span>" +
                                       "<a href='" + targetPage + "?eventID=" + eventID + "&from=" + source + "' style='color:inherit;text-decoration:none;'>" +
                                       HttpUtility.HtmlEncode(desc) +
                                       "</a></div>";
                  
                    events.Add(eventHtml);
                }
            }
        }
        return events;
    }
    
    private void LoadTagColours()
    {
        tagColours = new Dictionary<int, string>();
        int userID = Convert.ToInt32(Session["userID"]);
        using (MySqlConnection conn = new MySqlConnection(connString))
        {
            conn.Open();
            String sql = "SELECT tagID, tagColourNum FROM CalendarEventTag WHERE userIDLink = 0 OR userIDLink = @userID";
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@userID", userID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int tagID = Convert.ToInt32(reader["tagID"]);
                        string colourNum = reader["tagColourNum"].ToString();

                        string tagColour;
                        switch (colourNum)
                        {
                            case "1":
                                tagColour = "#F4CAE0";
                                break;
                            case "2":
                                tagColour = "#BE95C4";
                                break;
                            case "3":
                                tagColour = "#ADA7C9";
                                break;
                            case "4":
                                tagColour = "#90A8C3";
                                break;
                            case "5":
                                tagColour = "#64A6BD";
                                break;
                            case "6":
                                tagColour = "#446791";
                                break;
                            default:
                                tagColour = "#000000";
                                break;
                        }
                        tagColours[tagID] = tagColour;
                    }
                }
            }
            
        }
    }
    protected void btnPrevMonth_Click(Object sender, EventArgs e)
    {
        int year = int.Parse(hfYear.Value);
        int month = int.Parse(hfMonth.Value);

        DateTime prevMonth = new DateTime(year, month, 1).AddMonths(-1);
        LoadCalendar(prevMonth.Year, prevMonth.Month);
    }
    protected void btnNextMonth_Click(Object sender, EventArgs e)
    {
        int year = int.Parse(hfYear.Value);
        int month = int.Parse(hfMonth.Value);

        DateTime nextMonth = new DateTime(year, month, 1).AddMonths(1);
        LoadCalendar(nextMonth.Year, nextMonth.Month);
    }
    protected void btnToday_Click(Object sender, EventArgs e)
    {
        DateTime today = DateTime.Today;
        hfYear.Value = today.Year.ToString();
        hfMonth.Value = today.Month.ToString();
        LoadCalendar(today.Year, today.Month);
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

        using (MySqlConnection conn = new MySqlConnection(connString))
        {
            conn.Open();
            string sql = "SELECT * FROM ToDoListTask WHERE userID = @userID " + whereClause + " ORDER BY taskStatus ASC, taskID DESC";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@userID", Session["userID"]);
            dt.Load(cmd.ExecuteReader());
        }

        rptTasks.DataSource = dt;
        rptTasks.DataBind();
    }
    protected void txtNewTask_TextChanged(object sender, EventArgs e)
    {
        if (ViewState["TaskJustAdded"] == null || !(bool)ViewState["TaskJustAdded"])
        {
            btnAdd_Click(sender, e);
            ViewState["TaskJustAdded"] = true;
        }
        //btnAdd_Click(sender, e);
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        if (ViewState["TaskJustAdded"] != null && (bool)ViewState["TaskJustAdded"])
        {
            ViewState["TaskJustAdded"] = null;
            return;
        }
        string taskDesc = txtNewTask.Text.Trim();
        if (taskDesc == "")
            return;

        using (MySqlConnection conn = new MySqlConnection(connString))
        {
            conn.Open();
            string sql = "INSERT into ToDoListTask (taskDesc, taskStatus, userID) VALUES (@taskDesc, False, @userID)";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@taskDesc", taskDesc);
            cmd.Parameters.AddWithValue("@userID", Session["userID"]);
            cmd.ExecuteNonQuery();
        }

        txtNewTask.Text = "";
        ViewState["TaskJustAdded"] = true;
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

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = "UPDATE ToDoListTask SET taskDesc = @newDesc WHERE taskID = @taskID";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@newDesc", newDesc);
                cmd.Parameters.AddWithValue("@taskID", taskID);
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
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                string query = "DELETE FROM ToDoListTask WHERE taskID = @taskID";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@taskID", taskID);
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
        using (MySqlConnection conn = new MySqlConnection(connString))
        {
            conn.Open();
            string query = "SELECT taskStatus FROM ToDoListTask WHERE taskID = @taskID";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@taskID", taskID);
            var result = cmd.ExecuteScalar();
            return result != DBNull.Value && Convert.ToBoolean(result);
        }
    }
    private void ToggleTaskStatus(int taskID)
    {
        string userID = Session["userID"].ToString();
        if (string.IsNullOrEmpty(userID))
        {
            return;
        }
        using (MySqlConnection conn = new MySqlConnection(connString))
        {
            conn.Open();
            using (MySqlTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    string query = "UPDATE ToDoListTask SET taskStatus = TRUE WHERE taskID = @taskID AND taskStatus = FALSE";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@taskID", taskID);
                    cmd.ExecuteNonQuery();

                    //updated collected pets badge
                    string completedTasksQuery = "UPDATE Users SET completedTasks = completedTasks + 1 WHERE userID = @userID";
                    MySqlCommand updateCompletedTasksCmd = new MySqlCommand(completedTasksQuery, conn, transaction);
                    updateCompletedTasksCmd.Parameters.AddWithValue("@userID", userID);
                    updateCompletedTasksCmd.ExecuteNonQuery();

                    transaction.Commit();

                    CheckCompletedTasksBadge(conn, Convert.ToInt32(userID));
                    CheckCombinedCompletionBadges(conn, Convert.ToInt32(userID));
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Transaction error: " + ex.Message);
                }
            }

        }
        ViewState["PendingAction"] = null;
        ViewState["PendingTaskID"] = null;
    }
    // CHECKING COMPLETED TASKS BADGE
    private static void CheckCompletedTasksBadge(MySqlConnection con, int userID)
    {
        string getCountQuery = "SELECT completedTasks FROM Users WHERE userID = @userID";
        int completedCount = 0;
        using (MySqlCommand getCountCmd = new MySqlCommand(getCountQuery, con))
        {
            getCountCmd.Parameters.AddWithValue("@userID", userID);
            object result = getCountCmd.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                completedCount = Convert.ToInt32(result);
            }
        }

        if (completedCount >= 150)
        {
            AwardBadgeStatic(con, userID, 1, "Gold");
        }
        else if (completedCount >= 100)
        {
            AwardBadgeStatic(con, userID, 1, "Silver");
        }
        else if (completedCount >= 50)
        {
            AwardBadgeStatic(con, userID, 1, "Bronze");
        }
    }
    // FOR GETTING COMBINED COMPLETION BADGE (completedTasks, completedTimers, completedStudySessions)
    private static void CheckCombinedCompletionBadges(MySqlConnection con, int userID)
    {
        string getCountsQuery = "SELECT completedTasks, completedTimers, completedStudySessions FROM Users WHERE userID = @userID";
        int completedTasks = 0;
        int completedTimers = 0;
        int completedStudySessions = 0;

        using (MySqlCommand getCountsCmd = new MySqlCommand(getCountsQuery, con))
        {
            getCountsCmd.Parameters.AddWithValue("@userID", userID);
            using (MySqlDataReader reader = getCountsCmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    completedTasks = reader["completedTasks"] != DBNull.Value ? Convert.ToInt32(reader["completedTasks"]) : 0;
                    completedTimers = reader["completedTimers"] != DBNull.Value ? Convert.ToInt32(reader["completedTimers"]) : 0;
                    completedStudySessions = reader["completedStudySessions"] != DBNull.Value ? Convert.ToInt32(reader["completedStudySessions"]) : 0;
                }
            }
        }

        if (completedTasks >= 50 && completedTimers >= 50 && completedStudySessions >= 50)
        {
            AwardBadgeStatic(con, userID, 10, "Gold");
        }
        else if (completedTasks >= 25 && completedTimers >= 25 && completedStudySessions >= 25)
        {
            AwardBadgeStatic(con, userID, 10, "Silver");
        }
        else if (completedTasks >= 10 && completedTimers >= 10 && completedStudySessions >= 10)
        {
            AwardBadgeStatic(con, userID, 10, "Bronze");
        }
    }
    private static void AwardBadgeStatic(MySqlConnection con, int userID, int badgeID, string badgeType)
    {
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
    protected void btnThankYou_Click(object sender, EventArgs e)
    {
        if (ViewState["PendingAction"] != null && ViewState["PendingAction"].ToString() == "Toggle" && ViewState["PendingTaskID"] != null)
        {
            int taskID = Convert.ToInt32(ViewState["PendingTaskID"]);
            ToggleTaskStatus(taskID);
        }
        string userID = userIDHidden.Value;

        if (string.IsNullOrEmpty(userID))
        {
            Response.Write("<script>alert('Error: User not found.');</script>");
            return;
        }

        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        using (MySqlConnection con = new MySqlConnection(cs))
        {
            try
            {
                con.Open();

                // Get current XP
                string selectXPQuery = "SELECT userXP FROM Users WHERE userID = @userID";
                MySqlCommand selectXPCmd = new MySqlCommand(selectXPQuery, con);
                selectXPCmd.Parameters.AddWithValue("@userID", userID);

                object xpObj = selectXPCmd.ExecuteScalar();
                int currentXP = (xpObj != null && xpObj != DBNull.Value) ? Convert.ToInt32(xpObj) : 0;
                int newXP = currentXP + 1;

                // Update XP
                string updateXPQuery = "UPDATE Users SET userXP = @newXP WHERE userID = @userID";
                MySqlCommand updateXPCmd = new MySqlCommand(updateXPQuery, con);
                updateXPCmd.Parameters.AddWithValue("@newXP", newXP);
                updateXPCmd.Parameters.AddWithValue("@userID", userID);
                updateXPCmd.ExecuteNonQuery();

                // Check new level
                string getNewLevelQuery = "SELECT MAX(levelNum) FROM Level WHERE xpAmount <= @newXP";
                MySqlCommand getNewLevelCmd = new MySqlCommand(getNewLevelQuery, con);
                getNewLevelCmd.Parameters.AddWithValue("@newXP", newXP);

                object newLevelObj = getNewLevelCmd.ExecuteScalar();
                int newLevelNum = (newLevelObj != null && newLevelObj != DBNull.Value) ? Convert.ToInt32(newLevelObj) : 1;

                // Get current level
                string getCurrentLevelQuery = "SELECT levelID FROM CurrentLevel WHERE userID = @userID";
                MySqlCommand getCurrentLevelCmd = new MySqlCommand(getCurrentLevelQuery, con);
                getCurrentLevelCmd.Parameters.AddWithValue("@userID", userID);

                object currentLevelObj = getCurrentLevelCmd.ExecuteScalar();
                int currentLevel = (currentLevelObj != null && currentLevelObj != DBNull.Value) ? Convert.ToInt32(currentLevelObj) : 1;

                if (newLevelNum > currentLevel)
                {
                    // Update level
                    string updateLevelQuery = "UPDATE CurrentLevel SET levelID = @newLevelNum WHERE userID = @userID";
                    MySqlCommand updateLevelCmd = new MySqlCommand(updateLevelQuery, con);
                    updateLevelCmd.Parameters.AddWithValue("@newLevelNum", newLevelNum);
                    updateLevelCmd.Parameters.AddWithValue("@userID", userID);
                    updateLevelCmd.ExecuteNonQuery();

                    if (newLevelNum >= 25)
                    {
                        AwardBadgeStatic(con, Convert.ToInt32(userID), 16, "Gold");
                    }
                    else if (newLevelNum >= 15)
                    {
                        AwardBadgeStatic(con, Convert.ToInt32(userID), 16, "Silver");
                    }
                    else if (newLevelNum >= 5)
                    {
                        AwardBadgeStatic(con, Convert.ToInt32(userID), 16, "Bronze");
                    }

                    // Show Level Up popup
                    Session["ShowLevelUpPopup"] = true;
                    Session["CurrentLevel"] = newLevelNum;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowLevelUp", "showLevelUp();", true);
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
    protected void btnYayLevelUp_Click(object sender, EventArgs e)
    {
        // This will be handled by the OnClientClick now
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
    
    private void CheckForLevelUp(string userID)
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        int userXP = GetUserXP(cs, userID);
        Tuple<int, int, int> levelInfo = GetLevelInformation(cs, userID);
        int currentLevel = levelInfo.Item1;

        // Check if user leveled up since last visit
        if (Session["LastKnownLevel"] != null)
        {
            int lastLevel = Convert.ToInt32(Session["LastKnownLevel"]);
            if (currentLevel > lastLevel)
            {
                // Level up detected!
                Session["ShowLevelUpPopup"] = true;
                Session["CurrentLevel"] = currentLevel;
            }
        }

        // Update last known level
        Session["LastKnownLevel"] = currentLevel;
    }
    // Handle paw hidden button click
    protected void btnPawHiddenTrigger_Click(object sender, EventArgs e)
    {
        if (hfPawClicked.Value == "true")
        {
            System.Diagnostics.Debug.WriteLine("Paw hidden trigger clicked - awarding pet");
            hfPawClicked.Value = "false"; // Reset
            AwardPawSecretPet();
        }
    }

    private void AwardPawSecretPet()
    {
        System.Diagnostics.Debug.WriteLine("AwardPawSecretPet called");

        string userID = Session["UserID"] as string;

        if (string.IsNullOrEmpty(userID) && Session["Username"] != null)
        {
            string username = Session["Username"].ToString();
            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            userID = GetUserID(username, cs);
        }

        if (!string.IsNullOrEmpty(userID))
        {
            bool petAdded = InsertPawSecretPet(Convert.ToInt32(userID));

            if (petAdded)
            {
                System.Diagnostics.Debug.WriteLine("Paw pet added successfully, showing popup");

                // Use a different approach - register the script and don't do anything else
                string script = @"
                console.log('Script executed from C# - showing paw popup');
                setTimeout(function() {
                    showPawSecretPopup();
                }, 100);";

                ScriptManager.RegisterStartupScript(this, GetType(), "showPawPopup", script, true);
                System.Diagnostics.Debug.WriteLine("Popup script registered");
            }
        }
    }

    // Method to insert the paw secret pet into userPets table
    private bool InsertPawSecretPet(int userID)
    {
        System.Diagnostics.Debug.WriteLine("InsertPawSecretPet called for user " + userID);

        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        using (MySqlConnection con = new MySqlConnection(cs))
        {
            con.Open();

            // First check if the user already has this pet to avoid duplicates
            // Using petID 28 for the third secret pet (assuming 26=coffee, 27=study spirit)
            string checkQuery = "SELECT COUNT(*) FROM UserPets WHERE userID = @userID AND petID = 28";
            using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, con))
            {
                checkCmd.Parameters.AddWithValue("@userID", userID);
                int existingCount = Convert.ToInt32(checkCmd.ExecuteScalar());
                System.Diagnostics.Debug.WriteLine("Existing paw pets count: " + existingCount);

                if (existingCount > 0)
                {
                    System.Diagnostics.Debug.WriteLine("User already has paw secret pet");
                    return false;
                }
            }

            // Insert new pet (userPetsID will auto-increment, equippedStatus = 0)
            string insertQuery = "INSERT INTO UserPets (userID, petID, equippedStatus) VALUES (@userID, 28, 0)";
            using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, con))
            {
                insertCmd.Parameters.AddWithValue("@userID", userID);
                int rowsAffected = insertCmd.ExecuteNonQuery();
                System.Diagnostics.Debug.WriteLine("Rows affected by paw insert: " + rowsAffected);

                if (rowsAffected > 0)
                {
                    System.Diagnostics.Debug.WriteLine("Successfully added pet 28 for user " + userID);
                    return true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No rows affected by paw insert");
                    return false;
                }
            }
        }
    }

    // Redirect to SecretPets page
    protected void btnViewSecretPet3_Click(object sender, EventArgs e)
    {
        // Use this instead of Response.Redirect to avoid ThreadAbortException
        string script = "window.location.href = 'SecretPets.aspx';";
        ScriptManager.RegisterStartupScript(this, GetType(), "redirectToSecretPets", script, true);
    }
}