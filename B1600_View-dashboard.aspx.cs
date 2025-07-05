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
        }
        DateTime currentDate = DateTime.Today;
        int year = currentDate.Year;
        int month = currentDate.Month;
        lblMonthYear.Text = currentDate.ToString("MMMM yyyy");
        literalCalendar.Text = GenerateCalendar(year, month);
    }
    private String GenerateCalendar(int year, int month)
    {
        StringBuilder sb = new StringBuilder();
        DateTime firstDayOfMonth = new DateTime(year, month, 1);
        int daysInMonth = DateTime.DaysInMonth(year, month);

        // Adjust so Monday = 0, Sunday = 6
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

        // Loop through rows (weeks)
        for (int week = 0; currentDay <= daysInMonth; week++)
        {
            sb.Append("<tr>");

            for (int dayOfWeek = 0; dayOfWeek < 7; dayOfWeek++)
            {
                // First row: add empty cells before the 1st day
                if (week == 0 && dayOfWeek < adjustedStartDay)
                {
                    sb.Append("<td></td>");
                }
                else if (currentDay <= daysInMonth)
                {
                    DateTime thisDay = new DateTime(year, month, currentDay);
                    string cssClass = thisDay.Date == DateTime.Today ? "today" : "";
                    sb.Append(string.Format("<td class='{0}'>{1}</td>", cssClass, currentDay));
                    currentDay++;
                }
                else
                {
                    sb.Append("<td></td>");
                }
            }

            sb.Append("</tr>");
        }

        sb.Append("</table>");
        return sb.ToString();
        /*StringBuilder sb = new StringBuilder();
        DateTime firstDayOfMonth = new DateTime(year, month, 1);
        int daysInMonth = DateTime.DaysInMonth(year, month);
        int startDayOfWeek = ((int)firstDayOfMonth.DayOfWeek+6)%7;

        sb.Append("<table class='calendarBox'>");
        sb.Append("<tr>");
        string[] dayNames = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };

        foreach (string dayName in dayNames)
        {
            sb.Append(string.Format("<th>{0}</th>", dayName)); 
        }
        sb.Append("</tr>");

        int currentDay = 1;

        for (int week = 0; currentDay <= daysInMonth; week++) 
            {
            if (week == 0 && DayOfWeek < 7; day)
            sb.Append("<td></td>");
            }

        for (int day = 1; day <= daysInMonth; day++)
        {
            DateTime thisDay = new DateTime(year, month, day);
            string cssClass = thisDay.Date == DateTime.Today ? "today" : "";
            int currentCol = startDayOfWeek;

            sb.Append(string.Format("<td class='{0}'>{1}</td>", cssClass, day)); 

            if (currentCol == 6)
            {
                sb.Append("</tr>");
                if (day != daysInMonth)
                {
                    sb.Append("<tr>");
                }
            }
        }
        int endDayOfWeek = ((int)new DateTime(year, month, daysInMonth).DayOfWeek+1);
        for (int i = endDayOfWeek + 1; i <= 6; i++)
        {
            sb.Append("<td></td>");
        }

        sb.Append("</tr></table>");
        return sb.ToString();
        */
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
}