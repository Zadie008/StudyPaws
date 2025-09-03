using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
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

            ddlFilter.Visible = IsToDoFilterVisible;
            
            //calendarDropDown.Visible = IsToDoFilterVisible;
            userIDHidden.Value = Convert.ToString(Session["userID"]);

            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string userID = GetUserID(username, cs);

            if (!IsPostBack)
            {
                LoadCalendarTags();
                ViewState["SelectedFilter"] = "All";
                ddlFilter.SelectedValue = "All";
                LoadTasks();

                LoadTagColours();
                
                if (calendarDropDown.Items.FindByValue("0") != null)
                {
                    calendarDropDown.SelectedValue = "0";
                }
                ViewState["SelectedCalendarTag"] = calendarDropDown.SelectedValue;

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
                //LoadPendingInvitesFromDB();
                //LoadUpcomingSessions();
            }
            else
            {
                LoadTasks();
                LoadTagColours(); // added to see if it fixes default black event dots

                if (ViewState["SelectedCalendarTag"] != null)
                {
                    string selectedValue = ViewState["SelectedCalendarTag"].ToString();
                    var item = calendarDropDown.Items.FindByValue(selectedValue);
                    if (item != null)
                    {
                        calendarDropDown.SelectedValue = selectedValue;
                    }
                }

                int year = int.Parse(hfYear.Value);
                int month = int.Parse(hfMonth.Value);
                LoadCalendar(year, month);
            }
            //LoadTasks();
        }
        else
        {
            Response.Redirect("Login.aspx");
        }

        //ShowNextInvite();
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

    
    private bool IsCalendarFilterVisible
    {
        get
        {
            return ViewState["CalendarFilterVisible"] != null && (bool)ViewState["CalendarFilterVisible"];
        }
        set
        {
            ViewState["CalendarFilterVisible"] = value;
        }
    }
    
    private void LoadCalendar(int year, int month)
    {
        if (ViewState["SelectedCalendarTag"] != null)
        {
            string selectedValue = ViewState["SelectedCalendarTag"].ToString();
            var item = calendarDropDown.Items.FindByValue(selectedValue);
            if (item != null)
            {
                calendarDropDown.SelectedValue = selectedValue;
            }
        }

        lblMonthYear.Text = new DateTime(year, month, 1).ToString("MMMM yyyy");
        literalCalendar.Text = GenerateCalendar(year, month);
        hfYear.Value = year.ToString();
        hfMonth.Value = month.ToString();
    }
    
    protected void calendarFilterBtn_Click(object sender, EventArgs e)
    {
        IsCalendarFilterVisible = !IsCalendarFilterVisible;
        calendarDropDown.Visible = IsCalendarFilterVisible;

        if (IsCalendarFilterVisible)
            LoadCalendarTags();
    }
    
    private void LoadCalendarTags()
    {
        calendarDropDown.Items.Clear();
        calendarDropDown.Items.Add(new ListItem("All Tags", "0"));

        using (MySqlConnection conn = new MySqlConnection(connString))
        {
            conn.Open();
            string sql = "SELECT tagID, tagName FROM CalendarEventTag";
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int tagID = Convert.ToInt32(reader["tagID"]);
                    string tagName = reader["tagName"].ToString();
                    calendarDropDown.Items.Add(new ListItem(tagName, tagID.ToString()));
                }
            }
        }
    }
    protected void calendarDropDown_SelectedIndexChanged(object sender, EventArgs e)
    {
        ViewState["SelectedCalendarTag"] = calendarDropDown.SelectedValue;
        int year = int.Parse(hfYear.Value);
        int month = int.Parse(hfMonth.Value);
        LoadCalendar(year, month);
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

        int selectedTagID = 0;
        if (ViewState["SelectedCalendarTag"] != null)
        {
            selectedTagID = Convert.ToInt32(ViewState["SelectedCalendarTag"]);
        }

        using (MySqlConnection conn = new MySqlConnection(connString))
        {
            conn.Open();
            string loadEvents = "SELECT eventID, eventDesc, tagID FROM CalendarEvent " +  "WHERE userID=@userID AND eventDate=@eventDate";
            if (selectedTagID > 0)
            {
                loadEvents += " AND tagID=@tagID";
            }
            MySqlCommand cmd = new MySqlCommand(loadEvents, conn);
            cmd.Parameters.AddWithValue("@userID", userID);
            cmd.Parameters.AddWithValue("@eventDate", day.Date);
            if (selectedTagID > 0)
            {
                cmd.Parameters.AddWithValue("@tagID", selectedTagID);
            }
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int eventID = Convert.ToInt32(reader["eventID"]);
                    String desc = reader["eventDesc"].ToString();
                    int tagID = reader["tagID"] != DBNull.Value ? Convert.ToInt32(reader["tagID"]) : 0;
                    //int tagID = Convert.ToInt32(reader["tagID"]);
                    
                    string tagColour = (tagColours!=null && tagColours.ContainsKey(tagID)) ? tagColours[tagID] : "#000000";

                    string source = "dashboard";
                    string targetPage = (tagID == 1) ? "DeleteStudySession.aspx" : "B300-B400_Edit_Delete_Event.aspx";

                    string eventHtml = "<div class='eventItem'>" +
                                       "<span class='eventDot' style='background-color:" + tagColour + ";'></span>" +
                                       "<a href='" + targetPage + "?eventID=" + eventID + "&from=" + source + "' style='color:inherit;text-decoration:none;'>" +
                                       HttpUtility.HtmlEncode(desc) +
                                       "</a></div>";
                    //string eventHtml = "<div class='eventItem'><span class='eventDot' style='background-color:" + tagColour + ";'></span>" + "<a href='B300-B400_Edit_Delete_Event.aspx?eventID=" + eventID + "' styler='color:inherit;text-decoration:none;'>" + HttpUtility.HtmlEncode(desc) + "</a></div>";
                    events.Add(eventHtml);
                }
            }
        }
        return events;
    }
    
    private void LoadTagColours()
    {
        tagColours = new Dictionary<int, string>();
        using (MySqlConnection conn = new MySqlConnection(connString))
        {
            conn.Open();
            String sql = "SELECT tagID, tagColourNum FROM CalendarEventTag";
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read()) 
                {
                    int tagID = Convert.ToInt32(reader["tagID"]);
                    string colourNum = reader["tagColourNum"].ToString();

                    string tagColour;
                    switch (colourNum)
                    {
                        case "1": tagColour =  "#F4CAE0";
                            break;
                        case "2": tagColour = "#BE95C4";
                            break;
                        case "3": tagColour = "#ADA7C9";
                            break;
                        case "4": tagColour = "#90A8C3";
                            break;
                        case "5": tagColour = "#64A6BD";
                            break;
                        case "6": tagColour = "#446791";
                            break;
                        default: tagColour = "#000000";
                            break;
                    }
                    tagColours[tagID] = tagColour;
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
            string sql = "SELECT * FROM ToDoListTask WHERE userID = @userID " + whereClause + " ORDER BY taskStatus DESC";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@userID", Session["userID"]);
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
        using (MySqlConnection conn = new MySqlConnection(connString))
        {
            conn.Open();
            string query = "UPDATE ToDoListTask SET taskStatus = TRUE WHERE taskID = @taskID AND taskStatus = FALSE";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@taskID", taskID);
            cmd.ExecuteNonQuery();
        }
        ViewState["PendingAction"] = null;
        ViewState["PendingTaskID"] = null;
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
                int newXP = currentXP + 10;

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

                    // Show Level Up popup
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
        ScriptManager.RegisterStartupScript(this, GetType(), "hideLevelUp", "hideLevelUp();", false);
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
    //private void LoadPendingInvitesFromDB()
    //{
    //    string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
    //    List<SessionInvite> pendingInvites = new List<SessionInvite>();
    //    string query = "SELECT StudySession.sessionID, StudySession.sessionTitle, StudySession.sessionTag, StudySession.sessionStart, StudySession.sessionEnd, Users.username FROM (StudySessionParticipants INNER JOIN StudySession ON StudySessionParticipants.sessionID = StudySession.sessionID) INNER JOIN Users ON StudySession.leaderID = Users.userID WHERE StudySessionParticipants.userID = @userID AND StudySessionParticipants.accepted = false ORDER BY StudySession.sessionID ASC";

    //    using (MySqlConnection conn = new MySqlConnection(cs))
    //    using (MySqlCommand cmd = new MySqlCommand(query, conn))
    //    {
    //        cmd.Parameters.AddWithValue("@userID", Session["userID"]);
    //        conn.Open();
    //        using (MySqlDataReader reader = cmd.ExecuteReader())
    //        {
    //            while (reader.Read())
    //            {
    //                pendingInvites.Add(new SessionInvite
    //                {
    //                    sessionID = Convert.ToInt32(reader["sessionID"]),
    //                    leaderUsername = reader["username"].ToString(),
    //                    title = reader["sessionTitle"].ToString(),
    //                    tag = reader["sessionTag"].ToString(),
    //                    startTime = Convert.ToDateTime(reader["sessionStart"]),
    //                    endTime = Convert.ToDateTime(reader["sessionEnd"])
    //                });
    //            }
    //        }
    //    }

    //    Session["PendingInvites"] = pendingInvites;
    //}

    //private void ShowNextInvite()
    //{
    //    List<SessionInvite> invites = Session["PendingInvites"] as List<SessionInvite>;
    //    if (invites != null && invites.Count > 0)
    //    {
    //        var invite = invites[0];

    //        litNotificationText.Text = "<p><span style='text-decoration:underline;'>Study session invitation</span></p><table><tr><td><p>From:</p></td><td><p><span style='font-weight:bold;'>" + invite.leaderUsername + "</span></p></td></tr>" + "<tr><td><p>Title:</p></td><td><p><span style='font-weight:bold;'>" + invite.title + "</span></p></td></tr>" + "<tr><td><p>Tag:</p></td><td><p><span style='font-weight:bold;'>" + invite.tag + "</span></p></td></tr>" + "<tr><td><p>Starts:</p></td><td><p><span style='font-weight:bold;'>" + invite.startTime.ToString("dddd, dd MMMM yyyy @ HH:mm") + "</span></p></td></tr><tr><td><p>Ends:</p></td><td><p><span style='font-weight:bold;'>" + invite.endTime.ToString("dddd, dd MMMM yyyy @ HH:mm") + "</span></p></td></tr></table>";

    //        hiddenSessionID.Value = invite.sessionID.ToString();

    //        imgNotificationRinging.Visible = true;
    //        imgNotificationNormal.Visible = false;
    //        notificationBadge.Visible = true;

    //        ScriptManager.RegisterStartupScript(this, this.GetType(), "showPopup", "showNotificationPopup();", true);
    //    }
    //    else
    //    {
    //        imgNotificationRinging.Visible = false;
    //        imgNotificationNormal.Visible = true;
    //        notificationBadge.Visible = false;

    //        ScriptManager.RegisterStartupScript(this, this.GetType(), "showPopupNone", "showNotificationPopup(false);", true);
    //    }
    //}

    //protected void btnYes_Click(object sender, EventArgs e)
    //{
    //    int sessionID = int.Parse(hiddenSessionID.Value);
    //    string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
    //    string updateQuery = "UPDATE StudySessionParticipants SET accepted = true WHERE sessionID = @sessionID AND userID = @userID";
    //    using (MySqlConnection conn = new MySqlConnection(cs))
    //    using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
    //    {
    //        cmd.Parameters.AddWithValue("@sessionID", sessionID);
    //        cmd.Parameters.AddWithValue("@userID", Session["userID"]);
    //        conn.Open();
    //        cmd.ExecuteNonQuery();
    //    }

    //    List<SessionInvite> invites = Session["PendingInvites"] as List<SessionInvite>;
    //    SessionInvite invite = null;

    //    if (invites != null)
    //    {
    //        foreach (SessionInvite i in invites)
    //        {
    //            if (i.sessionID == sessionID)
    //            {
    //                invite = i;
    //                break;
    //            }
    //        }
    //    }

    //    if (invite != null)
    //    {
    //        string insertQuery = "INSERT INTO CalendarEvent (eventDesc, eventDate, tagID, userID) VALUES (@eventDesc, @eventDate, @tagID, @userID)";
    //        using (MySqlConnection conn = new MySqlConnection(cs))
    //        using (MySqlCommand cmd2 = new MySqlCommand(insertQuery, conn))
    //        {
    //            string eventDesc = invite.title + " (From: " + invite.leaderUsername + ")";
    //            DateTime eventDate = invite.startTime;
    //            int tagID = 1;
    //            int userID = Convert.ToInt32(Session["userID"]);

    //            cmd2.Parameters.AddWithValue("@eventDesc", eventDesc);
    //            cmd2.Parameters.AddWithValue("@eventDate", eventDate);
    //            cmd2.Parameters.AddWithValue("@tagID", tagID);
    //            cmd2.Parameters.AddWithValue("@userID", userID);

    //            conn.Open();
    //            cmd2.ExecuteNonQuery();
    //        }
    //    }

    //    hiddenShowCalendar.Value = "true";

    //    RemoveInviteAndShowNext(sessionID);
    //}

    //protected void btnNo_Click(object sender, EventArgs e)
    //{
    //    hiddenShowConfirmation.Value = "true";
    //}

    //private void RemoveInviteAndShowNext(int sessionID)
    //{
    //    List<SessionInvite> invites = Session["PendingInvites"] as List<SessionInvite>;
    //    if (invites != null)
    //    {
    //        var currentInvite = invites.Find(i => i.sessionID == sessionID);
    //        if (currentInvite != null)
    //        {
    //            invites.Remove(currentInvite);
    //        }
    //        Session["PendingInvites"] = invites;
    //        ShowNextInvite();
    //    }
    //}

    //private void LoadUpcomingSessions()
    //{
    //    string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
    //    string query = "SELECT StudySession.sessionID, StudySession.sessionStart FROM StudySession INNER JOIN StudySessionParticipants ON StudySession.sessionID = StudySessionParticipants.sessionID WHERE StudySessionParticipants.userID = @userID AND StudySessionParticipants.accepted = true";

    //    List<string> jsSessionTimes = new List<string>();

    //    using (MySqlConnection conn = new MySqlConnection(cs))
    //    using (MySqlCommand cmd = new MySqlCommand(query, conn))
    //    {
    //        cmd.Parameters.AddWithValue("@userID", Session["userID"]);
    //        conn.Open();
    //        using (MySqlDataReader reader = cmd.ExecuteReader())
    //        {
    //            while (reader.Read())
    //            {
    //                int foundSessionID = Convert.ToInt32(reader["sessionID"]);
    //                DateTime sessionStart = Convert.ToDateTime(reader["sessionStart"]);

    //                string jsObject = "{ sessionID: " + foundSessionID + ", time: '" + sessionStart.ToString("yyyy-MM-ddTHH:mm:ss") + "' }";
    //                jsSessionTimes.Add(jsObject);

    //                TimeSpan timeUntilStart = sessionStart - DateTime.Now;
    //                if (timeUntilStart.TotalMinutes >= 0 && timeUntilStart.TotalMinutes <= 10)
    //                {
    //                    Session["sessionID"] = foundSessionID;
    //                }
    //            }
    //        }
    //    }

    //    if (jsSessionTimes.Count > 0)
    //    {
    //        string jsArray = "[" + string.Join(",", jsSessionTimes.ToArray()) + "]";
    //        ClientScript.RegisterStartupScript(this.GetType(), "registerSessions", "var upcomingSessions = " + jsArray + ";", true);
    //    }
    //}

    //protected void btnCalendar_Click(object sender, EventArgs e)
    //{
    //    Response.Redirect("B100_View-calendar.aspx");
    //}

    //protected void btnOk_Click(object sender, EventArgs e)
    //{
    //    Response.Redirect("B1600_View-dashboard.aspx");
    //}

    //protected void btnSure_Click(object sender, EventArgs e)
    //{
    //    int sessionID = int.Parse(hiddenSessionID.Value);
    //    string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
    //    string deleteQuery = "DELETE FROM StudySessionParticipants WHERE sessionID = @sessionID AND userID = @userID AND accepted = false";
    //    using (MySqlConnection conn = new MySqlConnection(cs))
    //    using (MySqlCommand cmd = new MySqlCommand(deleteQuery, conn))
    //    {
    //        cmd.Parameters.AddWithValue("@sessionID", sessionID);
    //        cmd.Parameters.AddWithValue("@userID", Session["userID"]);
    //        conn.Open();
    //        cmd.ExecuteNonQuery();
    //    }

    //    hiddenShowDeclineConfirmed.Value = "true";

    //    RemoveInviteAndShowNext(sessionID);
    //}

    //protected void btnNotSure_Click(object sender, EventArgs e)
    //{
    //    Response.Redirect("B1600_View-dashboard.aspx");
    //}

    //protected void btnOkayDeclined_Click(object sender, EventArgs e)
    //{
    //    Response.Redirect("B1600_View-dashboard.aspx");
    //}

    //protected void btnJoin_Click(object sender, EventArgs e)
    //{
    //    if (Session["sessionID"] != null && Session["userID"] != null)
    //    {
    //        int sessionID = Convert.ToInt32(Session["sessionID"]);
    //        int userID = Convert.ToInt32(Session["userID"]);

    //        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
    //        string updateQuery = "UPDATE StudySessionParticipants SET joined = true WHERE sessionID = @sessionID AND userID = @userID";

    //        using (MySqlConnection conn = new MySqlConnection(cs))
    //        using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
    //        {
    //            cmd.Parameters.AddWithValue("@sessionID", sessionID);
    //            cmd.Parameters.AddWithValue("@userID", userID);
    //            conn.Open();
    //            int rowsAffected = cmd.ExecuteNonQuery();

    //            if (rowsAffected > 0)
    //            {
    //                Response.Redirect("A1400_View-study-session.aspx");
    //            }
    //        }
    //    }
    //}
    // end: notification bell code
}