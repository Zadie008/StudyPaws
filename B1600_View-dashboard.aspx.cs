using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Default2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadTasks("All");
            //Session["Tasks"] = new List<TaskItem>();
            //Calendar.SelectedDate = DateTime.Today;
            //UpdateEventList();
            DateTime currentDate = DateTime.Today;
            hfYear.Value = currentDate.Year.ToString();
            hfMonth.Value = DateTime.Today.Month.ToString();
            LoadCalendar(currentDate.Year, currentDate.Month);
        }
        else
        {
            int year = int.Parse(hfYear.Value);
            int month = int.Parse(hfMonth.Value);
            LoadCalendar(year, month);
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
                    int prevDay = daysInPrevMonth - (adjustedStartDay - dayOfWeek-1);
                    sb.Append(string.Format("<td class='otherMonth'>{0}</td>", prevDay));
                }
                else if (currentDay <= daysInMonth)
                {
                    DateTime thisDay = new DateTime(year, month, currentDay);
                    string cssClass = thisDay.Date == DateTime.Today ? "today" : "";
                    if (cssClass == "today")
                    {
                        sb.Append(string.Format("<td><span class='today'>{0}</span></td>", currentDay));
                    }
                    else
                    {
                        sb.Append(string.Format("<td>{0}</td>", currentDay));
                    }

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
    private void UpdateEventList()
    {
        /*DateTime selectedDate = calendar.SelectedDate;
        lblSelectedDate.Text = $"Events for {selectedDate.ToLongDateString()}";
        listEevents.Items.Clear();

        if (EventStore.ContainsKey(selectedDate))
        {
            foreach (String ev in EventDtore[selectedDate])
            {
                listEvents.Items.Add(new ListItem(ev));
            }
        }*/
    }

    protected void btnAddTask_Click(object sender, EventArgs e)
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        using (OleDbConnection con = new OleDbConnection(cs))
        {
            String query = "INSERT INTO ToDoListTask (taskDesc, taskStatus, userID) VALUES (@desc, 0, @userID)";
            OleDbCommand cmd = new OleDbCommand(query, con);
            cmd.Parameters.AddWithValue("@desc", txtNewTask.Text.Trim());
            cmd.Parameters.AddWithValue("@userID", Session["userID"]);
            con.Open();
            cmd.ExecuteNonQuery();
        }
        txtNewTask.Text = "";
        LoadTasks("All");
    }
    protected void chkComplete_CheckedChange(object sender, EventArgs e)
    {
        CheckBox chk = (CheckBox)sender;
        RepeaterItem item = (RepeaterItem)chk.NamingContainer;
        string taskID = chk.ToolTip;

        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        using (OleDbConnection con = new OleDbConnection(cs))
        {
            bool isChecked = chk.Checked;
            String query = "UPDATE [ToDoListTask] SET taskStatus = @status WHERE taskID = @id";
            OleDbCommand cmd = new OleDbCommand(query, con);
            cmd.Parameters.AddWithValue("@status", isChecked);
            cmd.Parameters.AddWithValue("@id", taskID);
            con.Open();
            cmd.ExecuteNonQuery();
        }
        LoadTasks("All");
    }
    private void LoadTasks(String filter)
    {
        List<TaskItem> tasks = new List<TaskItem>();

        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        using (OleDbConnection con = new OleDbConnection(cs))
        {
            /*String query = "SELECT * FROM [ToDoListTask] WHERE userID = @userID";

            if (filter == "All")
                query += " AND taskStatus = 1";
            else if (filter == "In Porgress")
                query += " AND taskStatus =0";

            OleDbCommand cmd = new OleDbCommand(query, con);
            cmd.Parameters.AddWithValue("@userID", Session["userID"]);
            if (filter != "All")
                cmd.Parameters.AddWithValue("@filter", filter);

            con.Open();
            OleDbDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                tasks.Add(new TaskItem
                {
                    taskID = Convert.ToInt32(reader["taskID"]),
                    taskDesc = reader["taskDesc"].ToString(),
                    taskStatus = Convert.ToBoolean(reader["taskStatus"])
                });
            }*/
        }
        var sorted = tasks.OrderBy(t => t.taskStatus == false).ToList();
        rptTasks.DataSource = sorted;
        rptTasks.DataBind();
    }
    public class TaskItem
    {
        public int taskID { get; set; }
        public string taskDesc { get; set; }
        public bool taskStatus { get; set; }
    }

    protected void filter_Click(object sender, EventArgs e)
    {
        String filter = ((LinkButton)sender).Text;
        LoadTasks(filter);
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
}