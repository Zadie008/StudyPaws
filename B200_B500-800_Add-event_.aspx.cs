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
        LoadTags();
        if (!IsPostBack)
            LoadTags();
        //{
        //    if (Session["timerTitle"] != null)
        //    {
        //        txtEventTitle.Text = Session["timerTitle"].ToString();
        //    }

        //    if (Session["timerTag"] != null)
        //    {
        //        dropdownEventTag.SelectedValue = Session["timerTag"].ToString();
        //    }
        //}
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
        ScriptManager.RegisterStartupScript(this, GetType(), "showPopup", "showPopup1();", true);
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
                if (!string.IsNullOrEmpty(hiddenSelectedTagID.Value))
                {
                    int tagID = int.Parse(hiddenSelectedTagID.Value);
                    string update = "UPDATE CalendarEventTag SET tagName = ?, tagColourNum = ? WHERE tagID = ?";
                    OleDbCommand cmdUpdate = new OleDbCommand(update, conn);
                    cmdUpdate.Parameters.AddWithValue("?", tagName);
                    cmdUpdate.Parameters.AddWithValue("?", tagColourNum);
                    cmdUpdate.Parameters.AddWithValue("?", tagID);
                    cmdUpdate.ExecuteNonQuery();
                }
                else
                {
                    string query = "INSERT into [CalendarEventTag] ([tagName], [tagColourNum]) VALUES (?, ?)";
                    OleDbCommand cmd = new OleDbCommand(query, conn);
                    cmd.Parameters.AddWithValue("?", tagName);
                    cmd.Parameters.AddWithValue("?", tagColourNum);
                    cmd.ExecuteNonQuery();
                }
            }
            Response.Redirect(Request.RawUrl);
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
                        dropdownEventTag.Items.Add(new ListItem(tagName, tagID.ToString()));
                    }
                }
            }
        }
    }
    protected void btnDeleteTag_Click(object sender, EventArgs e)
    {
        //ViewState["PendingAction"] = "Delete";
        //ViewState["PendingTagID"] = Convert.ToInt32(hiddenSelectedTagID.Value);
        ScriptManager.RegisterStartupScript(this, GetType(), "showDeletePopup", "showDeletePopup();", true);
    }
    protected void btnEditTag_Click(Object sender, EventArgs e)
    {
        if (dropdownEventTag.SelectedIndex > 0 && dropdownEventTag.SelectedValue!= "")
        {
            int tagID = Convert.ToInt32(dropdownEventTag.SelectedValue);
            hiddenSelectedTagID.Value = tagID.ToString();

            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (OleDbConnection conn = new OleDbConnection(cs))
            {
                conn.Open();
                string query = "SELECT tagName, tagColourNum FROM [CalendarEventTag] WHERE tagID = ?";
                OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("?", tagID);
                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        txtTagTitleEdit.Text = reader["tagName"].ToString();
                        hfEditTagColourNum.Value = reader["tagColourNum"].ToString();

                        ScriptManager.RegisterStartupScript(this, GetType(), "setColourEdit", "selectTagColour(" + reader["tagColourNum"].ToString() + ");", true);
                    }
                }
            }
            ScriptManager.RegisterStartupScript(this, GetType(), "showPopup2", "showPopup2();", true);
        }
    }
    protected void btnYesDelete_Click(object sender, EventArgs e)
    {
        if (dropdownEventTag.SelectedIndex > 0 && dropdownEventTag.SelectedValue != "")
        {
            int tagID = Convert.ToInt32(dropdownEventTag.SelectedValue);
            hiddenSelectedTagID.Value = tagID.ToString();

            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (OleDbConnection conn = new OleDbConnection(cs))
            {
                conn.Open();
                string query = "DELETE FROM CalendarEventTag WHERE tagID = ?";
                OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("?", tagID);
                cmd.ExecuteNonQuery();
                ScriptManager.RegisterStartupScript(this, GetType(), "hideDeletePopup", "hideDeletePopup();", true);
            }
        }
    }
    //protected void btnNoDelete_Click(Object sender, EventArgs e)
    //{
    //    ViewState["PendingAction"] = null;
    //    ViewState["PendingTagID"] = null;
    //    ScriptManager.RegisterStartupScript();
    //}
}