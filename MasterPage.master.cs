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
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        List<int> sessionIds = new List<int>();

        using (MySqlConnection conn = new MySqlConnection(cs))
        {
            conn.Open();
            string selectQuery = "SELECT sessionID FROM StudySession WHERE sessionStart BETWEEN ?startWindowBegin AND ?startWindowEnd";

            DateTime startWindowBegin = DateTime.Now.AddMinutes(0);
            DateTime startWindowEnd = DateTime.Now.AddMinutes(10);

            using (MySqlCommand cmd = new MySqlCommand(selectQuery, conn))
            {
                cmd.Parameters.AddWithValue("?startWindowBegin", startWindowBegin);
                cmd.Parameters.AddWithValue("?startWindowEnd", startWindowEnd);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sessionIds.Add(Convert.ToInt32(reader["sessionID"]));
                    }
                }
            }
        }

        foreach (int sessionID in sessionIds)
        {
            using (MySqlConnection conn = new MySqlConnection(cs))
            {
                conn.Open();
                HandleSessionParticipants(sessionID, conn);
            }
        }
    }

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
                    remaining = Convert.ToInt32(countCmd.ExecuteScalar());
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