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

            // checking for study session starting between 10-11 minutes (1 minute window)
            DateTime startWindowBegin = DateTime.Now.AddMinutes(10);
            DateTime startWindowEnd = DateTime.Now.AddMinutes(11);

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
        // 1. delete users who have not replied (replied = false)
        string deleteQuery = "DELETE FROM StudySessionParticipants WHERE sessionID = ? AND replied = false";
        using (OleDbCommand deleteCmd = new OleDbCommand(deleteQuery, conn))
        {
            deleteCmd.Parameters.AddWithValue("?", sessionID);
            deleteCmd.ExecuteNonQuery();
        }

        // 2. count how many participants remain for this session
        string countQuery = "SELECT COUNT(*) FROM StudySessionParticipants WHERE sessionID = ?";
        using (OleDbCommand countCmd = new OleDbCommand(countQuery, conn))
        {
            countCmd.Parameters.AddWithValue("?", sessionID);
            int remaining = Convert.ToInt32(countCmd.ExecuteScalar());

            if (remaining <= 1)
            {
                string deleteQuery2 = "DELETE FROM StudySession WHERE sessionID = ?";
                using (OleDbCommand deleteSessCmd = new OleDbCommand(deleteQuery2, conn))
                {
                    deleteSessCmd.Parameters.AddWithValue("?", sessionID);
                    deleteSessCmd.ExecuteNonQuery();
                }

                // delete all participants
                string deleteAllParticipants = "DELETE FROM StudySessionParticipants WHERE sessionID = ?";
                using (OleDbCommand delAllCmd = new OleDbCommand(deleteAllParticipants, conn))
                {
                    delAllCmd.Parameters.AddWithValue("?", sessionID);
                    delAllCmd.ExecuteNonQuery();
                }
            }
        }
    }
}