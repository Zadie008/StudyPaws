using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Web;
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
        using (OleDbConnection conn = new OleDbConnection(cs))
        {
            conn.Open();
            // fetch all sessions starting in 10 minutes
            string selectQuery = "SELECT sessionID FROM StudySession WHERE sessionStart BETWEEN ? AND ? ";

            // checking for study session starting between 0-10 minutes
            DateTime startWindowBegin = DateTime.Now.AddMinutes(0);
            DateTime startWindowEnd = DateTime.Now.AddMinutes(10);

            using (OleDbCommand cmd = new OleDbCommand(selectQuery, conn))
            {
                cmd.Parameters.Add("?", OleDbType.Date).Value = startWindowBegin;
                cmd.Parameters.Add("?", OleDbType.Date).Value = startWindowEnd;

                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int sessionID = Convert.ToInt32(reader["sessionID"]);
                        HandleSessionParticipants(sessionID, conn);
                    }
                }
            }
        }
    }

    private void HandleSessionParticipants(int sessionID, OleDbConnection conn)
    {
        using (OleDbTransaction transaction = conn.BeginTransaction())
        {
            try
            {
                // 1. delete all participants who haven't accepted
                string deleteQuery = "DELETE FROM StudySessionParticipants WHERE sessionID = ? AND accepted = false";
                using (OleDbCommand deleteCmd = new OleDbCommand(deleteQuery, conn, transaction))
                {
                    deleteCmd.Parameters.AddWithValue("?", sessionID);
                    deleteCmd.ExecuteNonQuery();
                }

                // 2. count remaining participants
                string countQuery = "SELECT COUNT(*) FROM StudySessionParticipants WHERE sessionID = ?";
                int remaining;
                using (OleDbCommand countCmd = new OleDbCommand(countQuery, conn, transaction))
                {
                    countCmd.Parameters.AddWithValue("?", sessionID);
                    remaining = Convert.ToInt32(countCmd.ExecuteScalar());
                }

                if (remaining <= 1) // only leader or no one left
                {
                    // 3. first delete all remaining participants (child records)
                    string deleteAllParticipants = "DELETE FROM StudySessionParticipants WHERE sessionID = ?";
                    using (OleDbCommand delAllCmd = new OleDbCommand(deleteAllParticipants, conn, transaction))
                    {
                        delAllCmd.Parameters.AddWithValue("?", sessionID);
                        delAllCmd.ExecuteNonQuery();
                    }

                    // 4. then delete the session itself (parent record)
                    string deleteSessionQuery = "DELETE FROM StudySession WHERE sessionID = ?";
                    using (OleDbCommand deleteSessCmd = new OleDbCommand(deleteSessionQuery, conn, transaction))
                    {
                        deleteSessCmd.Parameters.AddWithValue("?", sessionID);
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