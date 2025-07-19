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
            if (Session["userID"] != null)
            {
                string user = Session["userID"].ToString();
                if (user != null)
                {
                    LoadTasks(user);
                }

                DateTime currentDate = DateTime.Today;
                hfYear.Value = currentDate.Year.ToString();
                hfMonth.Value = DateTime.Today.Month.ToString();
                LoadCalendar(currentDate.Year, currentDate.Month);
            }
            else
            {
                Response.Redirect("Login.aspx");
            }

        }
        else
        {
            if (Session["userID"] != null)
            {
                string user = Session["userID"].ToString();
                if (user != null)
                {
                    LoadTasks(user);
                }

                int year = int.Parse(hfYear.Value);
                int month = int.Parse(hfMonth.Value);
                LoadCalendar(year, month);
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
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

    private void LoadTasks(string userId)
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        using (OleDbConnection con = new OleDbConnection(cs))
        {
            string command = "SELECT [taskID], [taskDesc], [taskStatus] FROM [ToDoListTask] WHERE userID = @id ORDER BY [taskStatus] ASC";

            OleDbCommand cmd = new OleDbCommand(command, con);
            cmd.Parameters.AddWithValue("@id", Session["userID"]);

            con.Open();
            OleDbDataReader collection = cmd.ExecuteReader();
            gridViewTaskList.DataSource = collection;
            gridViewTaskList.DataBind();
        }
    }

    protected void addTaskBtn_Click(object sender, ImageClickEventArgs e)
    {
        string taskDesc = newTaskText.Text.Trim();
        string user = Session["userID"].ToString();

        if (!string.IsNullOrEmpty(taskDesc))
        {
            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (OleDbConnection con = new OleDbConnection(cs))
            {
                string query = "INSERT INTO ToDoListTask (taskDesc, taskStatus, userID) VALUES (?, ?, ?)";
                using (OleDbCommand command = new OleDbCommand(query, con))
                {
                    command.Parameters.AddWithValue("?", taskDesc);
                    command.Parameters.AddWithValue("?", false);
                    command.Parameters.AddWithValue("?", user);

                    con.Open();
                    command.ExecuteNonQuery();
                }
            }
            newTaskText.Text = "";
            LoadTasks(user);
        }
        
    }
    protected void gridViewTaskList_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        string user = Session["userID"].ToString();

        int taskId;
        if (int.TryParse(e.CommandArgument.ToString(), out taskId))
        {
            if (e.CommandName == "ToggleStatus")
            {
                ToggleTaskStatus(taskId);
                LoadTasks(user);
            }
            else if (e.CommandName == "DeleteTask")
            {
                DeleteTask(taskId);
                LoadTasks(user);
            }
            // Optional: Edit handling here if needed
        }
    }

    //private void EditTask()
    //{
    //    int taskId = Convert.ToInt32(hdnTaskId.Value);
    //    string taskDesc = hdnTaskText.Value;

    //    string connectionString = GetConnectionString();
    //    using (OleDbConnection connection = new OleDbConnection(connectionString))
    //    {
    //        string query = "UPDATE ToDoListTask SET taskDesc = ? WHERE taskID = ?";
    //        using (OleDbCommand command = new OleDbCommand(query, connection))
    //        {
    //            command.Parameters.AddWithValue("?", taskDesc);
    //            command.Parameters.AddWithValue("?", taskId);

    //            connection.Open();
    //            command.ExecuteNonQuery();
    //        }
    //    }
    //}

    private void ToggleTaskStatus( int taskId)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString; 
        using (OleDbConnection connection = new OleDbConnection(connectionString))
        {
            string selectQuery = "SELECT taskStatus FROM ToDoListTask WHERE taskID = ?";
            OleDbCommand selectCmd = new OleDbCommand(selectQuery, connection);
            selectCmd.Parameters.AddWithValue("?", taskId);

            connection.Open();
            object result = selectCmd.ExecuteScalar();
            bool currentStatus = Convert.ToBoolean(result);

            string updateQuery = "UPDATE ToDoListTask SET taskStatus = ? WHERE taskID = ?";
            OleDbCommand updateCmd = new OleDbCommand(updateQuery, connection);
            updateCmd.Parameters.AddWithValue("?", !currentStatus);
            updateCmd.Parameters.AddWithValue("?", taskId);

            updateCmd.ExecuteNonQuery();
        }
    }

    private void DeleteTask(int taskId)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        using (OleDbConnection connection = new OleDbConnection(connectionString))
        {
            string query = "DELETE FROM ToDoListTask WHERE taskID = ?";
            using (OleDbCommand command = new OleDbCommand(query, connection))
            {
                command.Parameters.AddWithValue("?", taskId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
    protected void toDoFilterBtn_Click(object sender, ImageClickEventArgs e)
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        string userID = Session["userID"].ToString();
        string statusFilter = toDoFilterDropDown.SelectedValue;

        List<string> conditions = new List<string>();
        List<OleDbParameter> parameters = new List<OleDbParameter>();

        // Always filter by userID
        conditions.Add("userID = ?");
        parameters.Add(new OleDbParameter("userID", userID));

        bool filter = true;
        if (statusFilter == "Completed")
            filter = true;
        if (statusFilter == "In Progress")
            filter = false;

        if (statusFilter == "All")
        {
            using (OleDbConnection con = new OleDbConnection(cs))
            {
                string command = "SELECT taskID, taskStatus, taskDesc FROM ToDoListTask WHERE userID = ? ORDER BY taskStatus ASC";

                OleDbCommand cmd = new OleDbCommand(command, con);

                foreach (var param in parameters)
                {
                    cmd.Parameters.Add(param);
                }

                con.Open();
                OleDbDataReader reader = cmd.ExecuteReader();
                gridViewTaskList.DataSource = reader;
                gridViewTaskList.DataBind();
            }
        }
        // Optional: Tag filter
        else if (!string.IsNullOrEmpty(statusFilter))
        {
            conditions.Add("taskStatus = ?");
            parameters.Add(new OleDbParameter("taskStatus", filter));

            string whereClause = string.Join(" AND ", conditions);

            using (OleDbConnection con = new OleDbConnection(cs))
            {
                string command = "SELECT taskStatus, taskDesc FROM ToDoListTask WHERE " + whereClause;

                OleDbCommand cmd = new OleDbCommand(command, con);

                foreach (var param in parameters)
                {
                    cmd.Parameters.Add(param);
                }

                con.Open();
                OleDbDataReader reader = cmd.ExecuteReader();
                gridViewTaskList.DataSource = reader;
                gridViewTaskList.DataBind();
            }
        }
    }
}