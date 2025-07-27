using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class A200_View_timer : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
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

        //if (!IsPostBack)
        //{
        //    ViewState["ToDoListVisible"] = true;
        //    toggleToDoList.ImageUrl = "~/Icons/icons8-double-right-white-96.png";
        //    ViewState["SelectedFilter"] = "All";
        //    ddlFilter.SelectedValue = "All";
        //    LoadTasks();
        //}
        //else
        //{
        //    bool isToDoVisible = ViewState["ToDoListVisible"] != null && (bool)ViewState["ToDoListVisible"];
        //    toggleToDoList.ImageUrl = isToDoVisible ? "~/Icons/icons8-double-right-white-96.png" : "~/Icons/icons8-double-left-white-96.png";
        //    LoadTasks();
        //}
    }

    // COMPLETE TIMER (COUNTDOWN ENDS) - for Zadie~~~~~~~~~~~~~~~~~~~~


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
            using (OleDbConnection con = new OleDbConnection(connectionString))
            {
                con.Open();
                OleDbCommand cmd = new OleDbCommand("UPDATE Timer SET timerDuration = ? WHERE timerID = ?", con);
                cmd.Parameters.AddWithValue("?", newDuration);
                cmd.Parameters.AddWithValue("?", timerID);
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
            using (OleDbConnection con2 = new OleDbConnection(cs))
            {
                string deleteCommand = "DELETE FROM [Timer] WHERE [timerID] = ?";
                using (OleDbCommand cmd = new OleDbCommand(deleteCommand, con2))
                {
                    cmd.Parameters.AddWithValue("?", thisTimerID);

                    con2.Open();
                    int code = cmd.ExecuteNonQuery();
                    con2.Close();

                    if (code == 1)
                    {
                        //Session["timerID"] = null;
                        //Session["timerTitle"] = null;
                        //Session["timerTag"] = null;
                        //Session["timerDuration"] = null;

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
        using (OleDbConnection conn = new OleDbConnection(cs))
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

        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        using (OleDbConnection conn = new OleDbConnection(cs))
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
            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (OleDbConnection conn = new OleDbConnection(cs))
            {
                conn.Open();
                string query = "UPDATE ToDoListTask SET taskStatus = NOT taskStatus WHERE taskID = ?";
                OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("?", taskID);
                cmd.ExecuteNonQuery();
            }
            Response.Redirect(Request.RawUrl);
        }
        else if (e.CommandName == "Delete")
        {
            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (OleDbConnection conn = new OleDbConnection(cs))
            {
                conn.Open();
                string query = "DELETE FROM ToDoListTask WHERE taskID = ?";
                OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("?", taskID);
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
            using (OleDbConnection conn = new OleDbConnection(cs))
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
    protected void toggleToDoList_Click(object sender, ImageClickEventArgs e) 
    {
        if (toDoListPanel.Visible == true)
        {
            toDoListPanel.Visible = false;
            toggleToDoList.ImageUrl = "~/Icons/icons8-double-left-white-96.png";
        }
        else if(toDoListPanel.Visible == false)
        {
            toDoListPanel.Visible= true;
            toggleToDoList.ImageUrl = "~/Icons/icons8-double-right-white-96.png";
        }
        //bool isToDoVisible = ViewState["ToDoListVisible"] != null && (bool)ViewState["ToDoListVisible"];

        //toDoListPanel.Visible = !isToDoVisible;
        //ViewState["ToDoListVisible"] = !isToDoVisible;

        //toggleToDoList.ImageUrl = !isToDoVisible ? "~/Icons/icons8-double-right-white-96.png" : "~/Icons/icons8-double-left-white-96.png";
    }

}