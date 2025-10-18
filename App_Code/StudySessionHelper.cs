using System;
using System.Configuration;
using MySql.Data.MySqlClient;

public static class StudySessionHelper
{
    public static bool JoinStudySession(int sessionID, int userID)
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        string updateQuery = "UPDATE StudySessionParticipants SET joined = true WHERE sessionID = @sessionID AND userID = @userID";

        using (MySqlConnection conn = new MySqlConnection(cs))
        using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
        {
            cmd.Parameters.AddWithValue("@sessionID", sessionID);
            cmd.Parameters.AddWithValue("@userID", userID);
            conn.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }
}
