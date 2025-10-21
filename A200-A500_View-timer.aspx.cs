using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class A200_View_timer : System.Web.UI.Page
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

        if (!IsPostBack)
        {
            if (Session["Username"] != null)
            {
                string username = Session["Username"].ToString();

                string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
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

    //COMPLETED TIMER
    [System.Web.Services.WebMethod]
    [System.Web.Script.Services.ScriptMethod]
    public static string UpdateUserXP(int minutesStudied)
    {
        try
        {
            HttpContext context = HttpContext.Current;
            if (context.Session["UserID"] == null)
            {
                return "Error: User not authenticated";
            }

            string userID = context.Session["UserID"].ToString();
            int xpEarned = minutesStudied * 1; // 1 XP per minute
            int coinsEarned = minutesStudied * 1; // 1 coin per minute

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

                // Increment completedTimers counter
                string updateTimersQuery = "UPDATE Users SET completedTimers = completedTimers + 1 WHERE userID = @userID";
                MySqlCommand cmdTimers = new MySqlCommand(updateTimersQuery, con);
                cmdTimers.Parameters.AddWithValue("@userID", userID);
                cmdTimers.ExecuteNonQuery();

                // Get current time to check for time based badges
                DateTime currentTime = DateTime.Now;
                int currentHour = currentTime.Hour;

                CheckTimerCompletionBadges(con, Convert.ToInt32(userID));

                // 04:00-08:00
                if (currentHour >= 4 && currentHour < 8)
                {
                    CheckEarlyMorningTimerBadges(con, Convert.ToInt32(userID));
                }

                // 22:00-02:00
                if (currentHour >= 22 || currentHour < 2)
                {
                    CheckLateNightTimerBadges(con, Convert.ToInt32(userID));
                }
            }

            return "Success: " + xpEarned + " XP and " + coinsEarned + " coins added";
        }
        catch (Exception ex)
        {
            return "Error: " + ex.Message;
        }
    }

    // FOR GETTING TIMER COMPLETION BADGE
    private static void CheckTimerCompletionBadges(MySqlConnection con, int userID)
    {
        string getCountQuery = "SELECT completedTimers FROM Users WHERE userID = @userID";
        int completedCount = 0;
        using (MySqlCommand getCountCmd = new MySqlCommand(getCountQuery, con))
        {
            getCountCmd.Parameters.AddWithValue("@userID", userID);
            object result = getCountCmd.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                completedCount = Convert.ToInt32(result);
            }
        }

        if (completedCount >= 150)
        {
            AwardBadgeStatic(con, userID, 2, "Gold");
        }
        else if (completedCount >= 50)
        {
            AwardBadgeStatic(con, userID, 2, "Silver");
        }
        else if (completedCount >= 10)
        {
            AwardBadgeStatic(con, userID, 2, "Bronze");
        }
    }

    // FOR GETTING EARLY MORNING TIMER BADGE
    private static void CheckEarlyMorningTimerBadges(MySqlConnection con, int userID)
    {
        string getCountQuery = "SELECT completedTimersEarly FROM Users WHERE userID = @userID";
        int earlyMorningCount = 0;
        using (MySqlCommand getCountCmd = new MySqlCommand(getCountQuery, con))
        {
            getCountCmd.Parameters.AddWithValue("@userID", userID);
            object result = getCountCmd.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                earlyMorningCount = Convert.ToInt32(result);
            }
        }

        string updateQuery = "UPDATE Users SET completedTimersEarly = completedTimersEarly + 1 WHERE userID = @userID";
        using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, con))
        {
            updateCmd.Parameters.AddWithValue("@userID", userID);
            updateCmd.ExecuteNonQuery();
        }

        earlyMorningCount++;

        if (earlyMorningCount >= 15)
        {
            AwardBadgeStatic(con, userID, 3, "Gold");
        }
        else if (earlyMorningCount >= 10)
        {
            AwardBadgeStatic(con, userID, 3, "Silver");
        }
        else if (earlyMorningCount >= 5)
        {
            AwardBadgeStatic(con, userID, 3, "Bronze");
        }
    }

    // FOR GETTING LATE NIGHT TIMER BADGE
    private static void CheckLateNightTimerBadges(MySqlConnection con, int userID)
    {
        string getCountQuery = "SELECT completedTimersLate FROM Users WHERE userID = @userID";
        int lateNightCount = 0;
        using (MySqlCommand getCountCmd = new MySqlCommand(getCountQuery, con))
        {
            getCountCmd.Parameters.AddWithValue("@userID", userID);
            object result = getCountCmd.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                lateNightCount = Convert.ToInt32(result);
            }
        }

        string updateQuery = "UPDATE Users SET completedTimersLate = completedTimersLate + 1 WHERE userID = @userID";
        using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, con))
        {
            updateCmd.Parameters.AddWithValue("@userID", userID);
            updateCmd.ExecuteNonQuery();
        }

        lateNightCount++;

        if (lateNightCount >= 15)
        {
            AwardBadgeStatic(con, userID, 4, "Gold");
        }
        else if (lateNightCount >= 10)
        {
            AwardBadgeStatic(con, userID, 4, "Silver");
        }
        else if (lateNightCount >= 5)
        {
            AwardBadgeStatic(con, userID, 4, "Bronze");
        }
    }

    private static void AwardBadgeStatic(MySqlConnection con, int userID, int badgeID, string badgeType)
    {
        string checkBadgeQuery = "SELECT COUNT(*) FROM UserBadge WHERE userID = @userID AND badgeID = @badgeID";
        int badgeCount = 0;
        using (MySqlCommand checkBadgeCmd = new MySqlCommand(checkBadgeQuery, con))
        {
            checkBadgeCmd.Parameters.AddWithValue("@userID", userID);
            checkBadgeCmd.Parameters.AddWithValue("@badgeID", badgeID);
            badgeCount = Convert.ToInt32(checkBadgeCmd.ExecuteScalar());
        }

        if (badgeCount == 0)
        {
            // No entry exists - INSERT new record
            string insertBadgeQuery = "INSERT INTO UserBadge (userID, badgeID, badgeType) VALUES (@userID, @badgeID, @badgeType)";
            using (MySqlCommand insertBadgeCmd = new MySqlCommand(insertBadgeQuery, con))
            {
                insertBadgeCmd.Parameters.AddWithValue("@userID", userID);
                insertBadgeCmd.Parameters.AddWithValue("@badgeID", badgeID);
                insertBadgeCmd.Parameters.AddWithValue("@badgeType", badgeType);
                insertBadgeCmd.ExecuteNonQuery();
            }
        }
        else
        {
            // Entry exists - UPDATE with new badgeType
            string updateBadgeQuery = "UPDATE UserBadge SET badgeType = @badgeType WHERE userID = @userID AND badgeID = @badgeID";
            using (MySqlCommand updateBadgeCmd = new MySqlCommand(updateBadgeQuery, con))
            {
                updateBadgeCmd.Parameters.AddWithValue("@userID", userID);
                updateBadgeCmd.Parameters.AddWithValue("@badgeID", badgeID);
                updateBadgeCmd.Parameters.AddWithValue("@badgeType", badgeType);
                updateBadgeCmd.ExecuteNonQuery();
            }
        }
    }

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
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("UPDATE Timer SET timerDuration = @duration WHERE timerID = @timerID", con);
                cmd.Parameters.AddWithValue("@duration", newDuration);
                cmd.Parameters.AddWithValue("@timerID", timerID);
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
            int userID = Convert.ToInt32(Session["userID"]);

            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (MySqlConnection con1 = new MySqlConnection(cs))
            {
                string updateCommand = "UPDATE Users SET stoppedTimers = stoppedTimers + 1 WHERE userID = @userID";
                using (MySqlCommand cmd = new MySqlCommand(updateCommand, con1))
                {
                    cmd.Parameters.AddWithValue("@userID", userID);
                    con1.Open();
                    cmd.ExecuteNonQuery();
                    con1.Close();
                }
            }

            CheckStoppedTimersBadges(userID);

            using (MySqlConnection con2 = new MySqlConnection(cs))
            {
                string deleteCommand = "DELETE FROM Timer WHERE timerID = @timerID";
                using (MySqlCommand cmd = new MySqlCommand(deleteCommand, con2))
                {
                    cmd.Parameters.AddWithValue("@timerID", thisTimerID);

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

    // FOR GETTING STOPPED TIMERS BADGE
    private void CheckStoppedTimersBadges(int userID)
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        using (MySqlConnection con = new MySqlConnection(cs))
        {
            con.Open();

            string getCountQuery = "SELECT stoppedTimers FROM Users WHERE userID = @userID";
            int stoppedCount = 0;
            using (MySqlCommand getCountCmd = new MySqlCommand(getCountQuery, con))
            {
                getCountCmd.Parameters.AddWithValue("@userID", userID);
                object result = getCountCmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    stoppedCount = Convert.ToInt32(result);
                }
            }

            if (stoppedCount >= 15)
            {
                AwardBadge(con, userID, 5, "Gold");
            }
            else if (stoppedCount >= 10)
            {
                AwardBadge(con, userID, 5, "Silver");
            }
            else if (stoppedCount >= 5)
            {
                AwardBadge(con, userID, 5, "Bronze");
            }

            con.Close();
        }
    }

    private void AwardBadge(MySqlConnection con, int userID, int badgeID, string badgeType)
    {
        string checkBadgeQuery = "SELECT COUNT(*) FROM UserBadge WHERE userID = @userID AND badgeID = @badgeID";
        int badgeCount = 0;
        using (MySqlCommand checkBadgeCmd = new MySqlCommand(checkBadgeQuery, con))
        {
            checkBadgeCmd.Parameters.AddWithValue("@userID", userID);
            checkBadgeCmd.Parameters.AddWithValue("@badgeID", badgeID);
            badgeCount = Convert.ToInt32(checkBadgeCmd.ExecuteScalar());
        }

        if (badgeCount == 0)
        {
            // No entry exists - INSERT new record
            string insertBadgeQuery = "INSERT INTO UserBadge (userID, badgeID, badgeType) VALUES (@userID, @badgeID, @badgeType)";
            using (MySqlCommand insertBadgeCmd = new MySqlCommand(insertBadgeQuery, con))
            {
                insertBadgeCmd.Parameters.AddWithValue("@userID", userID);
                insertBadgeCmd.Parameters.AddWithValue("@badgeID", badgeID);
                insertBadgeCmd.Parameters.AddWithValue("@badgeType", badgeType);
                insertBadgeCmd.ExecuteNonQuery();
            }
        }
        else
        {
            // Entry exists - UPDATE with new badgeType
            string updateBadgeQuery = "UPDATE UserBadge SET badgeType = @badgeType WHERE userID = @userID AND badgeID = @badgeID";
            using (MySqlCommand updateBadgeCmd = new MySqlCommand(updateBadgeQuery, con))
            {
                updateBadgeCmd.Parameters.AddWithValue("@userID", userID);
                updateBadgeCmd.Parameters.AddWithValue("@badgeID", badgeID);
                updateBadgeCmd.Parameters.AddWithValue("@badgeType", badgeType);
                updateBadgeCmd.ExecuteNonQuery();
            }
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