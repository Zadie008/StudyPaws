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
        int totalSeconds = Convert.ToInt32(Session["timerDuration"]);
        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = totalSeconds % 60;

        string formattedTime = hours.ToString("D2") + ":" + minutes.ToString("D2") + ":" + seconds.ToString("D2");

        txtSessionTitle.Text = Session["timerTitle"].ToString();

        ClientScript.RegisterStartupScript(this.GetType(), "timerDurationScript",
            string.Format("var initialTime = {0};", totalSeconds), true);

        ClientScript.RegisterStartupScript(this.GetType(), "initialCountdownText",
            string.Format("document.addEventListener('DOMContentLoaded', function() {{ document.getElementById('mainContentPlaceHolder_lblCountdown').textContent = '{0}'; }});", formattedTime), true);
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        //Response.Redirect("A100_Create-timer_3.aspx"); //testing
    }

    [System.Web.Services.WebMethod]
    public static void UpdateTimerDuration(int addedSeconds)
    {
        int oldDuration = Convert.ToInt32(HttpContext.Current.Session["timerDuration"]);
        int newDuration = oldDuration + addedSeconds;

        HttpContext.Current.Session["timerDuration"] = newDuration;

        string connectionString = ConfigurationManager.ConnectionStrings["YourConn"].ConnectionString;
        using (OleDbConnection conn = new OleDbConnection(connectionString))
        {
            conn.Open();
            OleDbCommand cmd = new OleDbCommand("UPDATE Timers SET timerDuration = ? WHERE timerID = ?", conn);
            cmd.Parameters.AddWithValue("?", newDuration);
            cmd.Parameters.AddWithValue("?", HttpContext.Current.Session["timerID"]);
            cmd.ExecuteNonQuery();
        }
    }
}