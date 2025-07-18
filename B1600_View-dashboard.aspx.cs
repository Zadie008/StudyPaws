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
            LoadTasks();
            DateTime currentDate = DateTime.Today;
            hfYear.Value = currentDate.Year.ToString();
            hfMonth.Value = DateTime.Today.Month.ToString();
            LoadCalendar(currentDate.Year, currentDate.Month);
        }
        if (Request.QueryString["toggle"] != null)
        {
            int taskId = int.Parse(Request.QueryString["toggle"]);
            ToggleTaskStatus(taskId);
            Response.Redirect("B1600_View-dashboard.aspx");
            return;
        }
        else
        {
            int year = int.Parse(hfYear.Value);
            int month = int.Parse(hfMonth.Value);
            LoadCalendar(year, month);

            //string eventArgument = Request["__EVENTARGUMENT"];
            string action = hdnTaskAction.Value;

            if (!string.IsNullOrEmpty(action))
            {
                switch (action)
                {
                    case "load":
                        LoadTasks();
                        break;
                    case "add":
                        AddTask();
                        break;
                    case "edit":
                        EditTask();
                        break;
                    case "delete":
                        DeleteTask();
                        break;
                }
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

    private void LoadTasks()
    {
        // Clear existing tasks
        gridViewTaskList.Controls.Clear();
        int userId = GetCurrentUserId();

        string connectionString = GetConnectionString();
        using (OleDbConnection connection = new OleDbConnection(connectionString))
        {
            string query = "SELECT taskID, taskDesc, taskStatus FROM ToDoListTask WHERE userID = ? ORDER BY taskStatus ASC, taskID DESC";
            using (OleDbCommand command = new OleDbCommand(query, connection))
            {
                command.Parameters.AddWithValue("?", userId);
                connection.Open();
                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string taskId = reader["taskID"].ToString();
                        string taskDesc = reader["taskDesc"].ToString();
                        bool isCompleted = Convert.ToBoolean(reader["taskStatus"]);

                        AddTaskToUI(taskId, taskDesc, isCompleted);
                    }
                }
            }
        }
    }

    private void AddTask()
    {
        string taskDesc = hdnTaskText.Value;
        int userId = GetCurrentUserId();

        string connectionString = GetConnectionString();
        using (OleDbConnection connection = new OleDbConnection(connectionString))
        {
            string query = "INSERT INTO ToDoListTask (taskDesc, taskStatus, userID) VALUES (?, ?, ?)";
            using (OleDbCommand command = new OleDbCommand(query, connection))
            {
                command.Parameters.AddWithValue("?", taskDesc);
                command.Parameters.AddWithValue("?", false);
                command.Parameters.AddWithValue("?", userId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Reload tasks to get the new task with proper ID
        LoadTasks();
    }

    private void EditTask()
    {
        int taskId = Convert.ToInt32(hdnTaskId.Value);
        string taskDesc = hdnTaskText.Value;

        string connectionString = GetConnectionString();
        using (OleDbConnection connection = new OleDbConnection(connectionString))
        {
            string query = "UPDATE ToDoListTask SET taskDesc = ? WHERE taskID = ?";
            using (OleDbCommand command = new OleDbCommand(query, connection))
            {
                command.Parameters.AddWithValue("?", taskDesc);
                command.Parameters.AddWithValue("?", taskId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }

    private void ToggleTaskStatus( int taskId)
    {
        string connectionString = GetConnectionString();
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
            
            updateCmd.ExecuteNonQuery ();
        }
    }

    private void DeleteTask()
    {
        int taskId = Convert.ToInt32(hdnTaskId.Value);

        string connectionString = GetConnectionString();
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

    private void AddTaskToUI(string taskId, string taskDesc, bool isCompleted)
    {
        Panel taskItem = new Panel();
        taskItem.CssClass = "task-item";
        taskItem.Attributes["data-task-id"] = taskId;

        // Checkbox button
        Button checkbox = new Button();
        checkbox.CssClass = "task-checkbox" + (isCompleted ? " checked" : "");
        checkbox.Attributes["onclick"] = "toggleCheckBox(" + taskId + "); return false;";
        taskItem.Controls.Add(checkbox);

        // Task text
        Label taskText = new Label();
        taskText.CssClass = "task-text" + (isCompleted ? " completed" : "");
        taskText.Text = taskDesc;
        taskItem.Controls.Add(taskText);

        // Task actions (edit/delete)
        Panel taskActions = new Panel();
        taskActions.CssClass = "task-actions";

        Button editButton = new Button();
        editButton.CssClass = "edit-button";
        editButton.Text = "✎";
        editButton.Attributes["onclick"] = "editTask(this); return false;";
        taskActions.Controls.Add(editButton);

        Button deleteButton = new Button();
        deleteButton.CssClass = "delete-button";
        deleteButton.Text = "✕";
        deleteButton.Attributes["onclick"] = "deleteTask('" + taskId + "'); return false;";
        taskActions.Controls.Add(deleteButton);

        taskItem.Controls.Add(taskActions);

        gridViewTaskList.Controls.Add(taskItem);
    }

    private int GetCurrentUserId()
    {
        // Implement your own logic to get the current user's ID
        // This is just a placeholder
        if (Session["userID"] != null)
        {
            return Convert.ToInt32(Session["userID"]);
        }
        return 1; // Default user ID if not logged in
    }

    private string GetConnectionString()
    {
        // Return your Access database connection string
        return ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString; ;
    }

    protected void toDoFilterBtn_Click(object sender, EventArgs e)
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        string userID = Session["userID"].ToString();
        string statusFilter = toDoFilterDropDown.SelectedValue;

        List<string> conditions = new List<string>();
        List<OleDbParameter> parameters = new List<OleDbParameter>();

        // Always filter by userID
        conditions.Add("userID = ?");
        parameters.Add(new OleDbParameter("userID", userID));

        // Optional: Tag filter
        if (!string.IsNullOrEmpty(statusFilter))
        {
            conditions.Add("status = ?");
            parameters.Add(new OleDbParameter("status", statusFilter));
        }

        string whereClause = string.Join(" AND ", conditions);

        using (OleDbConnection con = new OleDbConnection(cs))
        {
            string command = "SELECT timerDateCreated AS [Date Created], timerTitle AS Title, timerTag AS Tag, timerDuration AS Duration FROM Timer WHERE " + whereClause + " ORDER BY timerDateCreated DESC";

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