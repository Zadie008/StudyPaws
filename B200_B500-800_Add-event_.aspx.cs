using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Default2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            LoadTags();
        {
            if (Session["timerTitle"] != null)
            {
                txtEventTitle.Text = Session["timerTitle"].ToString();
            }

            if (Session["timerTag"] != null)
            {
                dropdownEventTag.SelectedValue = Session["timerTag"].ToString();
            }
        }
    }
    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("B1600_View-dashboard.aspx");
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        String desc = txtEventTitle.Text;
        int tag = int.Parse(dropdownEventTag.SelectedValue);
        int userID = Convert.ToInt32(Session["userID"]);

        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        using (OleDbConnection conn = new OleDbConnection(cs))
        {
            conn.Open();
            string sql = "INSERT into [CalendarEvent] ([eventDesc], [eventDate], [tagID], [userID]) VALUES (?, ?, ?, ?)";
            OleDbCommand cmd = new OleDbCommand(sql, conn);
            cmd.Parameters.AddWithValue("?", desc);
            cmd.Parameters.AddWithValue("?", DateTime.Today);
            cmd.Parameters.AddWithValue("?", tag);
            cmd.Parameters.AddWithValue("?", Session["userID"]);
            cmd.ExecuteNonQuery();
        }

        Response.Redirect("B1600_View-dashboard.aspx");
    }
    protected void btnNewTag_Click(object sender, EventArgs e)
    {
        txtTagTitle.Text = "";
        ScriptManager.RegisterStartupScript(this, GetType(), "showPopup", "showPopup();", true);
    }
    protected void btnAddTag_Click(Object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            String tagName = txtTagTitle.Text.Trim();
            int tagColourNum = int.Parse(hfTagColourNum.Value);
            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (OleDbConnection conn = new OleDbConnection(cs))
            {
                conn.Open();
                string query = "INSERT into [CalendarEventTag] ([tagName], [tagColourNum]) VALUES (?, ?)";
                OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("?", tagName);
                cmd.Parameters.AddWithValue("?", tagColourNum);
                cmd.ExecuteNonQuery();
            }
        }
    }
    protected void validatorTagColour_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = !string.IsNullOrEmpty(hfTagColourNum.Value);
    }
    protected void LoadTags()
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        using (OleDbConnection conn = new OleDbConnection(cs))
        {
            conn.Open();
            string query = "SELECT tagID, tagName FROM [CalendarEventTag]";
            OleDbCommand cmd = new OleDbCommand(query, conn);
            using (OleDbDataReader reader = cmd.ExecuteReader())
            {
                dropdownEventTag.Items.Clear();
                dropdownEventTag.Items.Add(new ListItem(""));

                while (reader.Read())
                {
                    string tagName = reader["tagName"].ToString();
                    int tagID = Convert.ToInt32(reader["tagID"]);

                    if (tagID != 1)
                    {
                        dropdownEventTag.Items.Add(new ListItem(tagName));
                    }
                }
            }
        }
    }   
}