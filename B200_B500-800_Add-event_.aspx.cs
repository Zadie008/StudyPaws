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
        {
            if (Session["timerTitle"] != null)
            {
                txtTitle.Text = Session["timerTitle"].ToString();
            }

            if (Session["timerTag"] != null)
            {
                dropdownTag.SelectedValue = Session["timerTag"].ToString();
            }
        }
    }
    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("B1600_View-dashboard.aspx");
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        String desc = txtTitle.Text;
        int tag = int.Parse(dropdownTag.SelectedValue);
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
}