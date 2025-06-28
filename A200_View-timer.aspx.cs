using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Default2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int totalSeconds = Convert.ToInt32(Session["timerDuration"]);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        string formattedTime = minutes.ToString("D2") + ":" + seconds.ToString("D2");
        lblCountdown.Text = formattedTime;

        txtSessionTitle.Text = Session["timerTitle"].ToString();
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect("A100_Create-timer_3.aspx"); //testing
    }

    protected void btnStop_Click(object sender, EventArgs e)
    {

    }
}