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
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Default2 : System.Web.UI.Page
{
    string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["userID"]!=null)
        {
            ddlFilter.Visible = IsToDoFilterVisible;
            userIDHidden.Value = Convert.ToString(Session["userID"]);

            //ddlFilter.CssClass = IsFilterVisible ? "toDoFilterDropDownList" : "toDoFilterDropDownList hidden";

            if (!IsPostBack)
            {
                ViewState["SelectedFilter"] = "All";
                ddlFilter.SelectedValue = "All";
                LoadTasks();

                DateTime currentDate = DateTime.Today;
                hfYear.Value = currentDate.Year.ToString();
                hfMonth.Value = DateTime.Today.Month.ToString();
                LoadCalendar(currentDate.Year, currentDate.Month);
            }
            else
            {
                LoadTasks();

                int year = int.Parse(hfYear.Value);
                int month = int.Parse(hfMonth.Value);
                LoadCalendar(year, month);
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
    protected bool showTaskControls
    {
        get
        {
            return ViewState["ShowTaskControls"] != null && (bool)ViewState["ShowTaskControls"];
        }
        set
        {
            ViewState["ShowTaskControls"]= value;  
        }
    }
    private void LoadCalendar(int year, int month)
    {
        lblMonthYear.Text = new DateTime(year, month, 1).ToString("MMMM yyyy");
        literalCalendar.Text = GenerateCalendar(year, month);
        hfYear.Value = year.ToString();
        hfMonth.Value = month.ToString();
    }
    protected void calendarFilterBtn_Click(object sender, EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("Button clicked");
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
                    int prevDay = daysInPrevMonth - (adjustedStartDay - dayOfWeek-1);
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
                        sb.Append("<div class='eventItem'><span class='eventDot'></span>");
                        sb.Append(HttpUtility.HtmlEncode(ev));
                        sb.Append("</div>");
                    }
                    sb.Append("</div>");

                    // '+' Button
                    sb.AppendFormat(
                        "<a class='addEventBtn' href='B200_B500-800_Add-event_.aspx?date={0}'>" + "<img src='Icons/icons8-add-new-white-96.png' class='addEventBtnImg' />" + "</a>",
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
        // Simulated data for now
        var sampleEvents = new Dictionary<string, List<string>>()
    {
        { "2025-07-01", new List<string> { "WRPV Assignment" } },
        { "2025-07-10", new List<string> { "WRAV Assignment" } },
        { "2025-07-18", new List<string> { "FS Document", "UI Designs" } }
    };

        string key = day.ToString("yyyy-MM-dd");
        return sampleEvents.ContainsKey(key) ? sampleEvents[key] : new List<string>();
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
        LoadTasks();
    }

    protected void rptTasks_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int taskID = Convert.ToInt32(e.CommandArgument);

        if (e.CommandName == "Toggle")
        {
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                conn.Open();
                string sql = "UPDATE ToDoListTask SET taskStatus = NOT taskStatus WHERE taskID = ?";
                OleDbCommand cmd = new OleDbCommand(sql, conn);
                cmd.Parameters.AddWithValue("?", taskID);
                cmd.ExecuteNonQuery();
            }
            Response.Redirect(Request.RawUrl);
        }
        else if (e.CommandName == "Delete")
        {
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                conn.Open();
                string sql = "DELETE FROM ToDoListTask WHERE taskID = ?";
                OleDbCommand cmd = new OleDbCommand(sql, conn);
                cmd.Parameters.AddWithValue("?", taskID);
                cmd.ExecuteNonQuery();
            }
            Response.Redirect(Request.RawUrl);
        }
        //else if (e.CommandName == "Edit")
        //{
        //    TextBox txt = (TextBox)e.Item.FindControl("txtEditDesc");
        //    string newDesc = txt.Text.Trim();

        //    using (OleDbConnection conn = new OleDbConnection(connString))
        //    {
        //        conn.Open();
        //        string sql = "UPDATE ToDoListTask SET taskDesc = ? WHERE taskID = ?";
        //        OleDbCommand cmd = new OleDbCommand(sql, conn);
        //        cmd.Parameters.AddWithValue("?", newDesc);
        //        cmd.Parameters.AddWithValue("?", taskID);
        //        cmd.ExecuteNonQuery();
        //    }
        //}
        else if(e.CommandName == "ShowControls")
        {
            showTaskControls = !showTaskControls;
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
        ddlFilter.Visible= IsToDoFilterVisible;
    }
    protected void rptTasks_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            TextBox txt = (TextBox)e.Item.FindControl("txtEditDesc");
            ImageButton editBtn = (ImageButton)e.Item.FindControl("editBtn");

            if (Request.Form[editBtn.UniqueID] != null)
            {
                txt.ReadOnly = false;
                txt.Focus();
            }
        }
    }
    protected void txtNewTask_TextChanged(object sender, EventArgs e)
    {
        btnAdd_Click(sender, e);
    }
}
