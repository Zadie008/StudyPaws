using System;
using System.Configuration;
using MySql.Data.MySqlClient;

public static class StudySessionHelper
{
    // updates the participant to "joined = true"
    public static bool JoinStudySession(int sessionID, int userID)
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        int rowsAffected = 0;

        using (MySqlConnection conn = new MySqlConnection(cs))
        {
            string updateQuery = "UPDATE StudySessionParticipants SET joined = TRUE WHERE sessionID = @sessionID AND userID = @userID";

            using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
            {
                cmd.Parameters.AddWithValue("@sessionID", sessionID);
                cmd.Parameters.AddWithValue("@userID", userID);
                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
        }

        return rowsAffected > 0;
    }

    // checks if a study session is starting soon (within 1 minute)
    public static bool ShouldPromptJoin(int sessionID)
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        using (MySqlConnection conn = new MySqlConnection(cs))
        {
            string query = "SELECT sessionStart FROM StudySession WHERE sessionID = @sessionID";
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@sessionID", sessionID);
                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    DateTime startTime;
                    bool parsed = DateTime.TryParse(result.ToString(), out startTime);
                    if (parsed)
                    {
                        DateTime now = DateTime.Now;
                        DateTime oneMinuteBefore = startTime.AddMinutes(-1);

                        if (now >= oneMinuteBefore && now < startTime)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
}
