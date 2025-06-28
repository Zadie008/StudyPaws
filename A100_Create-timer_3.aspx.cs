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
        if (Session["userID"] == null)
        {
            Session["userID"] = 1; // TESTING ONLY!!!!!!
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("A100_Create-timer_2.aspx");
    }

    protected void btnStart_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            /*if (Session["userID"] != null)
            {*/
                string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

                using (OleDbConnection con = new OleDbConnection(cs))
                {
                    string command = "INSERT INTO [Timer] ([timerTitle], [timerTag], [timerDuration], [userID]) VALUES (@title, @tag, @duration, @id)";
                    OleDbCommand cmd = new OleDbCommand(command, con);

                    int minutes = int.Parse(txtTimeMinutes.Text);
                    int seconds = int.Parse(txtTimeSeconds.Text);
                    int totalSeconds = (minutes * 60) + seconds;

                    cmd.Parameters.AddWithValue("@title", Session["timerTitle"]);
                    cmd.Parameters.AddWithValue("@tag", Session["timerTag"]);
                    cmd.Parameters.AddWithValue("@duration", totalSeconds);
                    cmd.Parameters.AddWithValue("@id", Session["userID"]);

                    con.Open();
                    int code = cmd.ExecuteNonQuery();
                    con.Close();

                    /*if (code > 0)
                    {*/
                        Session["timerDuration"] = totalSeconds;
                        Response.Redirect("A200_View-timer.aspx");
                    /*}*/
                }
            /*}
            else
            {
                Response.Redirect("Login.aspx");
            }*/
        }
    }
}