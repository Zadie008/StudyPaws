using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class A200_View_timer : System.Web.UI.Page
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

        int totalSeconds = Convert.ToInt32(Session["timerDuration"]);
        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = totalSeconds % 60;

        string formattedTime = hours.ToString("D2") + ":" + minutes.ToString("D2") + ":" + seconds.ToString("D2");

        txtSessionTitle.Text = Session["timerTitle"].ToString();

        ClientScript.RegisterStartupScript(this.GetType(), "timerDurationScript", string.Format("var initialTime = {0};", totalSeconds), true);

        ClientScript.RegisterStartupScript(this.GetType(), "initialCountdownText", string.Format("document.addEventListener('DOMContentLoaded', function() {{ document.getElementById('mainContentPlaceHolder_lblCountdown').textContent = '{0}'; }});", formattedTime), true);

        if (Session["EquippedPetImagePath"] != null)
        {
            pet.ImageUrl = Session["EquippedPetImagePath"].ToString();
        }

        ddlFilter.Visible = IsToDoFilterVisible;
        userIDHidden.Value = Convert.ToString(Session["userID"]);

        ViewState["ToDoListVisible"] = true;
        toggleToDoList.ImageUrl = "~/Icons/icons8-double-right-white-96.png";
        ViewState["SelectedFilter"] = "All";
        ddlFilter.SelectedValue = "All";
        LoadTasks();
    }

    // EDIT TIMER (ADD MINUTES)
    [System.Web.Services.WebMethod]
    public static string UpdateTimerDuration(int addedSeconds)
    {
        try
        {
            int oldDuration = Convert.ToInt32(HttpContext.Current.Session["timerDuration"]);
            int newDuration = oldDuration + addedSeconds;

            HttpContext.Current.Session["timerDuration"] = newDuration;

            int timerID = Convert.ToInt32(HttpContext.Current.Session["timerID"]);

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("UPDATE Timer SET timerDuration = @duration WHERE timerID = @timerID", con);
                cmd.Parameters.AddWithValue("@duration", newDuration);
                cmd.Parameters.AddWithValue("@timerID", timerID);
                cmd.ExecuteNonQuery();
            }

            return "Success";
        }
        catch (Exception ex)
        {
            return "Error: " + ex.Message;
        }
    }

    // STOP TIMER (DELETING THE TIMER ENTRY)
    protected void btnYes_Click(object sender, EventArgs e)
    {
        if (Session["userID"] != null && Session["timerID"] != null)
        {
            int thisTimerID = Convert.ToInt32(Session["timerID"]);

            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (MySqlConnection con2 = new MySqlConnection(cs))
            {
                string deleteCommand = "DELETE FROM Timer WHERE timerID = @timerID";
                using (MySqlCommand cmd = new MySqlCommand(deleteCommand, con2))
                {
                    cmd.Parameters.AddWithValue("@timerID", thisTimerID);

                    con2.Open();
                    int code = cmd.ExecuteNonQuery();
                    con2.Close();

                    if (code == 1)
                    {
                        Response.Redirect("Default.aspx");
                    }
                }
            }
        }
    }

    //TO DO LIST METHODS
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

        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        using (MySqlConnection conn = new MySqlConnection(cs))
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

        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        using (MySqlConnection conn = new MySqlConnection(cs))
        {
            conn.Open();
            string sql = "INSERT INTO ToDoListTask (taskDesc, taskStatus, userID) VALUES (@desc, False, @userID)";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@desc", taskDesc);
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
            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (MySqlConnection conn = new MySqlConnection(cs))
            {
                conn.Open();
                string query = "UPDATE ToDoListTask SET taskStatus = NOT taskStatus WHERE taskID = @taskID";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@taskID", taskID);
                cmd.ExecuteNonQuery();
            }
            Response.Redirect(Request.RawUrl);
        }
        else if (e.CommandName == "Delete")
        {
            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (MySqlConnection conn = new MySqlConnection(cs))
            {
                conn.Open();
                string query = "DELETE FROM ToDoListTask WHERE taskID = @taskID";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@taskID", taskID);
                cmd.ExecuteNonQuery();
            }
            Response.Redirect(Request.RawUrl);
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

            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (MySqlConnection conn = new MySqlConnection(cs))
            {
                conn.Open();
                string query = "UPDATE ToDoListTask SET taskDesc = @desc WHERE taskID = @taskID";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@desc", newDesc);
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

    protected void toggleToDoList_Click(object sender, ImageClickEventArgs e)
    {
        if (toDoListPanel.Visible == true)
        {
            toDoListPanel.Visible = false;
            toggleToDoList.ImageUrl = "~/Icons/icons8-double-left-white-96.png";
        }
        else if (toDoListPanel.Visible == false)
        {
            toDoListPanel.Visible = true;
            toggleToDoList.ImageUrl = "~/Icons/icons8-double-right-white-96.png";
        }
    }
}