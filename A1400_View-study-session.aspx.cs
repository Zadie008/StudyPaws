using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class View_study_session : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        using (OleDbConnection con = new OleDbConnection(cs))
        {
            string selectCommand = "SELECT sessionDuration FROM [StudySession] WHERE [userID] = ? AND [sessionID] = ? INNER JOIN ON [sessionID] FROM [StudySessionParticipants]";
            using (OleDbCommand cmd = new OleDbCommand(selectCommand, con))
            {
                //save sessionID in session var
                //save sessionTitle in session var
                //save sessionDuration in session var
            }

            int totalSeconds = Convert.ToInt32(Session["sessionDuration"]);
            int hours = totalSeconds / 3600;
            int minutes = (totalSeconds % 3600) / 60;
            int seconds = totalSeconds % 60;

            string formattedTime = hours.ToString("D2") + ":" + minutes.ToString("D2") + ":" + seconds.ToString("D2");

            txtSessionTitle.Text = Session["sessionTitle"].ToString();

            ClientScript.RegisterStartupScript(this.GetType(), "timerDurationScript", string.Format("var initialTime = {0};", totalSeconds), true); // should timerDurationScript be sessionDurationScript?

            ClientScript.RegisterStartupScript(this.GetType(), "initialCountdownText", string.Format("document.addEventListener('DOMContentLoaded', function() {{ document.getElementById('mainContentPlaceHolder_lblCountdown').textContent = '{0}'; }});", formattedTime), true);

            if (Session["EquippedPetImagePath"] != null)
            {
                pet.ImageUrl = Session["EquippedPetImagePath"].ToString();
            }
        }
    }

    // STOP STUDY SESSION (DELETING THE STUDY SESSION ENTRY FOR CURRENT USER)
    protected void btnYes_Click(object sender, EventArgs e)
    {
        if (Session["userID"] != null && Session["sessionID"] != null)
        {
            int thisSessionID = Convert.ToInt32(Session["sessionID"]);
            int thisUserID = Convert.ToInt32(Session["userID"]);

            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (OleDbConnection con2 = new OleDbConnection(cs))
            {
                string deleteCommand = "DELETE FROM [StudySessionParticipants] WHERE [sessionID] = ? AND [userID] = ?";
                using (OleDbCommand cmd = new OleDbCommand(deleteCommand, con2))
                {
                    cmd.Parameters.AddWithValue("?", thisSessionID);
                    cmd.Parameters.AddWithValue("?", thisUserID);

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
}