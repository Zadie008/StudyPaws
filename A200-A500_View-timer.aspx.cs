using System;
using System.Collections.Generic;
using System.Configuration;
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
    
}