using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class View_study_session : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.IsAuthenticated)
        {
            var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                Session["UserID"] = ticket.UserData;
            }
        }

        if (Session["sessionID"] == null)
        {
            Response.Redirect("Login.aspx");
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

            if (Session["Username"] != null)
            {
                string username = Session["Username"].ToString();

                string userID = GetUserID(username, cs);

                if (!IsPostBack)
                {
                    int userXP = GetUserXP(cs, userID);
                    Tuple<int, int, int> levelInfo = GetLevelInformation(cs, userID);
                    int currentLevel = levelInfo.Item1;
                    int currentLevelXpAmount = levelInfo.Item2;
                    int nextLevelXpAmount = levelInfo.Item3;

                    lblLevelNumber.Text = currentLevel.ToString();

                    CalculateXPProgressBar(userXP, currentLevelXpAmount, nextLevelXpAmount);
                    GetUserStats(cs, userID);
                    GetUserProfileIcon(cs, userID);
                }
            }
            else
            {
                Response.Redirect("Landing-page.aspx");
            }
        }
    }
    [System.Web.Services.WebMethod]
    [System.Web.Script.Services.ScriptMethod]
    public static string UpdateStudySessionRewards(int minutesStudied)
    {
        try
        {
            HttpContext context = HttpContext.Current;
            if (context.Session["UserID"] == null)
            {
                return "Error: User not authenticated";
            }

            string userID = context.Session["UserID"].ToString();
            int xpEarned = minutesStudied * 5;
            int coinsEarned = minutesStudied * 10;

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                // Update user XP
                string updateXPQuery = "UPDATE Users SET userXP = userXP + @xpEarned WHERE userID = @userID";
                MySqlCommand cmd = new MySqlCommand(updateXPQuery, con);
                cmd.Parameters.AddWithValue("@xpEarned", xpEarned);
                cmd.Parameters.AddWithValue("@userID", userID);
                cmd.ExecuteNonQuery();

                // Update user coins
                string updateCoinsQuery = "UPDATE Users SET userCoinCount = userCoinCount + @coinsEarned WHERE userID = @userID";
                MySqlCommand cmdCoins = new MySqlCommand(updateCoinsQuery, con);
                cmdCoins.Parameters.AddWithValue("@coinsEarned", coinsEarned);
                cmdCoins.Parameters.AddWithValue("@userID", userID);
                cmdCoins.ExecuteNonQuery();
            }

            return "Success: " + xpEarned + " XP and " + coinsEarned + " coins added";
        }
        catch (Exception ex)
        {
            return "Error: " + ex.Message;
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

    // start: header profile code
    private string GetUserID(string username, string connectionString)
    {
        string query = "SELECT userID FROM Users WHERE username = @username";
        using (MySqlConnection con = new MySqlConnection(connectionString))
        using (MySqlCommand cmd = new MySqlCommand(query, con))
        {
            cmd.Parameters.AddWithValue("@username", username);
            try
            {
                con.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? result.ToString() : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting user ID: " + ex.Message);
                return null;
            }
        }
    }

    private int GetUserXP(string connectionString, string userID)
    {
        string query = "SELECT userXP FROM Users WHERE userID = @userID";
        int userXP = 0;

        using (MySqlConnection con = new MySqlConnection(connectionString))
        using (MySqlCommand cmd = new MySqlCommand(query, con))
        {
            cmd.Parameters.AddWithValue("@userID", userID);
            try
            {
                con.Open();
                object result = cmd.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out userXP))
                {
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting user XP: " + ex.Message);
            }
        }
        return userXP;
    }

    private Tuple<int, int, int> GetLevelInformation(string connectionString, string userID)
    {
        int currentLevel = 0;
        int currentLevelXpAmount = 0;
        int nextLevelXpAmount = 0;
        string currentLevelQuery = "SELECT levelID FROM CurrentLevel WHERE userID = @userID";
        using (MySqlConnection con = new MySqlConnection(connectionString))
        using (MySqlCommand cmdCurrentLevel = new MySqlCommand(currentLevelQuery, con))
        {
            cmdCurrentLevel.Parameters.AddWithValue("@userID", userID);
            con.Open();
            object result = cmdCurrentLevel.ExecuteScalar();
            if (result != null && int.TryParse(result.ToString(), out currentLevel))
            {
                lblLevelNumber.Text = currentLevel.ToString();
            }
            else
            {
                lblLevelNumber.Text = "N/A";
                return Tuple.Create(0, 0, 0);
            }
        }
        string currentLevelXPQuery = "SELECT xpAmount FROM Level WHERE levelNum = @currentLevel";
        using (MySqlConnection con = new MySqlConnection(connectionString))
        using (MySqlCommand cmdCurrentXP = new MySqlCommand(currentLevelXPQuery, con))
        {
            cmdCurrentXP.Parameters.AddWithValue("@currentLevel", currentLevel);
            con.Open();
            object result = cmdCurrentXP.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                currentLevelXpAmount = Convert.ToInt32(result);
            }
        }

        string nextLevelXPQuery = "SELECT xpAmount FROM Level WHERE levelNum = @nextLevel";
        using (MySqlConnection con = new MySqlConnection(connectionString))
        using (MySqlCommand cmdNextXP = new MySqlCommand(nextLevelXPQuery, con))
        {
            cmdNextXP.Parameters.AddWithValue("@nextLevel", currentLevel + 1);
            con.Open();
            object result = cmdNextXP.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                nextLevelXpAmount = Convert.ToInt32(result);
            }
            else
            {
                nextLevelXpAmount = currentLevelXpAmount;//when user reaches level 25
            }
        }

        return Tuple.Create(currentLevel, currentLevelXpAmount, nextLevelXpAmount);
    }

    private void CalculateXPProgressBar(int userXP, int currentLevelXpAmount, int nextLevelXpAmount)
    {
        if (nextLevelXpAmount <= currentLevelXpAmount)
        {
            xpProgressBar.Style["width"] = "100%";
            lblXPPercentage.Text = "100%";
            return;
        }
        int xpToNextLevel = nextLevelXpAmount - currentLevelXpAmount;
        int xpGainedInCurrentLevel = userXP - currentLevelXpAmount;

        if (xpToNextLevel > 0)
        {
            double progress = (double)xpGainedInCurrentLevel / xpToNextLevel * 100;
            if (progress < 0) progress = 0;
            if (progress > 100) progress = 100;

            xpProgressBar.Style["width"] = progress.ToString("F0") + "%";
            lblXPPercentage.Text = progress.ToString("F0") + "%";
        }
        else
        {
            xpProgressBar.Style["width"] = "100%";
            lblXPPercentage.Text = "100%";
        }
    }

    private void GetUserStats(string connectionString, string userID)
    {
        string query = "SELECT userCoinCount FROM Users WHERE userID = @userID";

        using (MySqlConnection con = new MySqlConnection(connectionString))
        using (MySqlCommand cmd = new MySqlCommand(query, con))
        {
            cmd.Parameters.AddWithValue("@userID", userID);

            con.Open();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    lblPaws.Text = reader["userCoinCount"] != DBNull.Value ? reader["userCoinCount"].ToString() : "0";
                }
                else
                {
                    lblPaws.Text = "N/A";
                }
            }
        }
    }

    private void GetUserProfileIcon(string connectionString, string userID)
    {
        string query = "SELECT iconNum FROM Users WHERE userID = @userID";

        using (MySqlConnection con = new MySqlConnection(connectionString))
        using (MySqlCommand cmd = new MySqlCommand(query, con))
        {
            cmd.Parameters.AddWithValue("@userID", userID);
            try
            {
                con.Open();
                object result = cmd.ExecuteScalar();
                int iconNum;
                if (result != null && int.TryParse(result.ToString(), out iconNum))
                {
                    string iconPath = GetProfileImagePath(iconNum);
                    string circleClass = GetCircleColorClass(iconNum);
                    profilePet.ImageUrl = iconPath;
                    profileCircle.Attributes["class"] = "profileCircle " + circleClass;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting profile icon: " + ex.Message);
            }
        }
    }

    private string GetProfileImagePath(int iconNum)
    {
        switch (iconNum)
        {
            case 1: return "~/Images/ProfilePictures/CatPfp.png";
            case 2: return "~/Images/ProfilePictures/DogPfp.png";
            case 3: return "~/Images/ProfilePictures/BunnyPfp.png";
            case 4: return "~/Images/ProfilePictures/CowPfp.png";
            case 5: return "~/Images/ProfilePictures/UnicornPfp.png";
            default: return "~/Images/ProfilePictures/CatPfp.png";
        }
    }

    private string GetCircleColorClass(int iconNum)
    {
        switch (iconNum)
        {
            case 1: return "circle-cat";
            case 2: return "circle-dog";
            case 3: return "circle-bunny";
            case 4: return "circle-cow";
            case 5: return "circle-unicorn";
            default: return "circle-cat";
        }
    }
    // end: header profile code
}