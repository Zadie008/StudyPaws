using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class MasterPage : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CheckUpcomingStudySessions();
    }

    private void CheckUpcomingStudySessions()
    {
        if (Session["UserID"] == null) return;

        int userID = Convert.ToInt32(Session["UserID"]);
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        using (MySqlConnection conn = new MySqlConnection(cs))
        {
            conn.Open();

            string selectQuery = "SELECT ss.sessionID, ss.sessionTitle, ss.sessionStart FROM StudySession ss INNER JOIN StudySessionParticipants ssp ON ss.sessionID = ssp.sessionID WHERE ssp.userID = ?userID AND ssp.accepted = 'yes' AND ssp.joined = 'no' AND ss.sessionStart BETWEEN ?joinWindowStart AND ?joinWindowEnd LIMIT 1";

            DateTime joinWindowStart = DateTime.Now.AddMinutes(-1); // 1 minute ago
            DateTime joinWindowEnd = DateTime.Now.AddMinutes(1);    // 1 minute from now

            using (MySqlCommand cmd = new MySqlCommand(selectQuery, conn))
            {
                cmd.Parameters.AddWithValue("?userID", userID);
                cmd.Parameters.AddWithValue("?joinWindowStart", joinWindowStart);
                cmd.Parameters.AddWithValue("?joinWindowEnd", joinWindowEnd);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int sessionID = Convert.ToInt32(reader["sessionID"]);
                        string sessionTitle = reader["sessionTitle"].ToString();

                        Session["sessionID"] = sessionID;

                        string script = string.Format(@"
                        setTimeout(function() {{
                            document.getElementById('mainContentPlaceHolder_hiddenJoinSessionID').value = '{0}';
                            document.getElementById('popup').style.display = 'flex';
                        }}, 1000);", sessionID);

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowJoinPopup", script, true);
                    }
                }
            }
        }
    }

    // OLD CODE:
    private void HandleSessionParticipants(int sessionID, MySqlConnection conn)
    {
        using (MySqlTransaction transaction = conn.BeginTransaction())
        {
            try
            {
                // 1. delete all participants who haven't accepted
                string deleteQuery = "DELETE FROM StudySessionParticipants WHERE sessionID = ?sessionID AND accepted = false";
                using (MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, conn, transaction))
                {
                    deleteCmd.Parameters.AddWithValue("?sessionID", sessionID);
                    deleteCmd.ExecuteNonQuery();
                }

                // 2. count remaining participants
                string countQuery = "SELECT COUNT(*) FROM StudySessionParticipants WHERE sessionID = ?sessionID";
                int remaining;
                using (MySqlCommand countCmd = new MySqlCommand(countQuery, conn, transaction))
                {
                    countCmd.Parameters.AddWithValue("?sessionID", sessionID);
                    remaining = Convert.ToInt32(countCmd.ExecuteNonQuery());
                }

                if (remaining <= 1) // only leader or no one left
                {
                    // 3. first delete all remaining participants (child records)
                    string deleteAllParticipants = "DELETE FROM StudySessionParticipants WHERE sessionID = ?sessionID";
                    using (MySqlCommand delAllCmd = new MySqlCommand(deleteAllParticipants, conn, transaction))
                    {
                        delAllCmd.Parameters.AddWithValue("?sessionID", sessionID);
                        delAllCmd.ExecuteNonQuery();
                    }

                    // 4. then delete the session itself (parent record)
                    string deleteSessionQuery = "DELETE FROM StudySession WHERE sessionID = ?sessionID";
                    using (MySqlCommand deleteSessCmd = new MySqlCommand(deleteSessionQuery, conn, transaction))
                    {
                        deleteSessCmd.Parameters.AddWithValue("?sessionID", sessionID);
                        deleteSessCmd.ExecuteNonQuery();
                    }
                }

                // commit if everything succeeded
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                System.Diagnostics.Debug.WriteLine("Error handling session participants: " + ex.Message);
            }
        }
    }
}