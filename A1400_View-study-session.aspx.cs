using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

public partial class View_study_session : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["sessionID"] == null)
        {
            Response.Redirect("Default.aspx");
            return;
        }

        int sessionID = (int)Session["sessionID"];

        if (!IsPostBack)
        {
            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(cs))
            {
                string query = "SELECT StudySession.sessionTitle, StudySession.sessionDuration FROM StudySession INNER JOIN StudySessionParticipants ON StudySession.sessionID = StudySessionParticipants.sessionID WHERE StudySession.sessionID = @sessionID";

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@sessionID", sessionID);
                    con.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string sessionTitle = reader["sessionTitle"].ToString();
                            int totalSeconds = Convert.ToInt32(reader["sessionDuration"]);

                            txtSessionTitle.Text = sessionTitle;

                            Session["sessionTitle"] = sessionTitle;
                            Session["sessionDuration"] = totalSeconds;

                            int hours = totalSeconds / 3600;
                            int minutes = (totalSeconds % 3600) / 60;
                            int seconds = totalSeconds % 60;

                            string formattedTime = hours.ToString("D2") + ":" + minutes.ToString("D2") + ":" + seconds.ToString("D2");

                            ClientScript.RegisterStartupScript(this.GetType(), "timerDurationScript", "var initialTime = " + totalSeconds + ";", true);

                            string countdownScript = "document.addEventListener('DOMContentLoaded', function() {" + " document.getElementById('mainContentPlaceHolder_lblCountdown').textContent = '" + formattedTime + "';" + " });";

                            ClientScript.RegisterStartupScript(this.GetType(), "initialCountdownText", countdownScript, true);
                        }
                    }
                }

                if (Session["EquippedPetImagePath"] != null)
                {
                    pet.ImageUrl = Session["EquippedPetImagePath"].ToString();
                }
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
            using (MySqlConnection con2 = new MySqlConnection(cs))
            {
                string deleteCommand = "DELETE FROM StudySessionParticipants WHERE sessionID = @sessionID AND userID = @userID";
                using (MySqlCommand cmd = new MySqlCommand(deleteCommand, con2))
                {
                    cmd.Parameters.AddWithValue("@sessionID", thisSessionID);
                    cmd.Parameters.AddWithValue("@userID", thisUserID);

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

    // COMPLETE STUDY SESSION (UPDATING COMPLETED ATTRIBUTE)
    [System.Web.Services.WebMethod]
    public static string MarkSessionAsCompleted()
    {
        try
        {
            if (HttpContext.Current.Session["sessionID"] != null && HttpContext.Current.Session["userID"] != null)
            {
                int sessionID = Convert.ToInt32(HttpContext.Current.Session["sessionID"]);
                int userID = Convert.ToInt32(HttpContext.Current.Session["userID"]);

                string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                string updateQuery = "UPDATE StudySessionParticipants SET completed = true WHERE sessionID = @sessionID AND userID = @userID";

                using (MySqlConnection conn = new MySqlConnection(cs))
                using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@sessionID", sessionID);
                    cmd.Parameters.AddWithValue("@userID", userID);
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return "success";
                    }
                    return "no_rows_updated";
                }
            }
            return "session_or_user_missing";
        }
        catch (Exception ex)
        {
            return "error: " + ex.Message;
        }
    }
}