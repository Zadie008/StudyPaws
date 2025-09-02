using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class View_Pets_dogs : System.Web.UI.Page
{
    private string userID;
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

                userID = GetUserID(username, cs);

                if (!string.IsNullOrEmpty(userID))
                {
                    LoadOwnedPets(userID);

                    int userXP = GetUserXP(cs, userID);
                    Tuple<int, int, int> levelInfo = GetLevelInformation(cs, userID);
                    int currentLevel = levelInfo.Item1;
                    int currentLevelXpAmount = levelInfo.Item2;
                    int nextLevelXpAmount = levelInfo.Item3;

                    lblLevelNumber.Text = currentLevel.ToString();
                    CalculateXPProgressBar(userXP, currentLevelXpAmount, nextLevelXpAmount);
                    GetUserStats(cs, userID);
                    GetUserProfileIcon(cs, userID);
                    LoadPendingInvitesFromDB();
                    LoadUpcomingSessions();
                }

                ShowNextInvite(); // always show latest invite
            }
            else
            {
                Response.Redirect("Landing-page.aspx");
            }
        }
    }

    // start: view pets code
    private void LoadOwnedPets(string userID)
    {
        ContentPlaceHolder content = (ContentPlaceHolder)Master.FindControl("mainContentPlaceHolder");

        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        string command = "SELECT Pet.colourNum, UserPets.equippedStatus FROM UserPets INNER JOIN Pet ON UserPets.petID = Pet.petID WHERE UserPets.userID = @userID AND Pet.petType = @petType";

        List<OwnedPet> ownedPets = new List<OwnedPet>();

        using (MySqlConnection con = new MySqlConnection(cs))
        using (MySqlCommand cmd = new MySqlCommand(command, con))
        {
            cmd.Parameters.AddWithValue("@userID", userID);
            cmd.Parameters.AddWithValue("@petType", "Dog"); // PET TYPE~~~~

            try
            {
                con.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int colour = Convert.ToInt32(reader["colourNum"]);
                        bool equipped = (reader["equippedStatus"] != DBNull.Value) && Convert.ToBoolean(reader["equippedStatus"]);
                        ownedPets.Add(new OwnedPet(colour, equipped));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading owned pets: " + ex.Message);
            }
        }

        for (int i = 1; i <= 5; i++)
        {
            HtmlGenericControl circleDiv = (HtmlGenericControl)content.FindControl("circle" + i);
            if (circleDiv != null)
            {
                string classes = circleDiv.Attributes["class"];
                if (!string.IsNullOrEmpty(classes) && classes.Contains("equipped"))
                {
                    circleDiv.Attributes["class"] = classes.Replace("equipped", "").Trim();
                }
            }
        }

        int displayIndex = 1;

        foreach (OwnedPet pet in ownedPets)
        {
            Image petImg = (Image)content.FindControl("imgPet" + displayIndex);
            Button selectBtn = (Button)content.FindControl("btnSelect" + displayIndex);
            HtmlGenericControl circleDiv = (HtmlGenericControl)content.FindControl("circle" + displayIndex);

            if (petImg != null && selectBtn != null && circleDiv != null)
            {
                petImg.ImageUrl = string.Format("Images/Dog {0}.png", pet.ColourNum); // PET TYPE~~~~
                petImg.Visible = true;

                selectBtn.Visible = true;
                selectBtn.CommandArgument = pet.ColourNum.ToString();
                selectBtn.Attributes["data-colour"] = pet.ColourNum.ToString();

                if (pet.IsEquipped)
                {
                    string existingClass = circleDiv.Attributes["class"];
                    if (!existingClass.Contains("equipped"))
                    {
                        circleDiv.Attributes["class"] = existingClass + " equipped";
                    }
                }
            }

            displayIndex++;
        }

        // hide remaining pets
        for (int i = displayIndex; i <= 5; i++)
        {
            Image petImg = (Image)content.FindControl("imgPet" + i);
            Button selectBtn = (Button)content.FindControl("btnSelect" + i);
            HtmlGenericControl circleDiv = (HtmlGenericControl)content.FindControl("circle" + i);

            if (petImg != null) petImg.Visible = false;
            if (selectBtn != null) selectBtn.Visible = false;
            if (circleDiv != null) circleDiv.Visible = false;
        }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        btnEquip.Click += new EventHandler(btnEquip_Click);
    }

    protected void btnEquip_Click(object sender, EventArgs e)
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        string selectedColourNum = hfSelectedColourNum.Value;
        string userID = Session["UserID"] as string;

        if (string.IsNullOrEmpty(selectedColourNum) || string.IsNullOrEmpty(userID))
        {
            return;
        }

        using (MySqlConnection con = new MySqlConnection(cs))
        {
            con.Open();

            // 1. uncheck all current equipped pets for this user
            string unequipQuery = "UPDATE UserPets SET equippedStatus = FALSE WHERE userID = @userID";
            using (MySqlCommand cmdUnequip = new MySqlCommand(unequipQuery, con))
            {
                cmdUnequip.Parameters.AddWithValue("@userID", userID);
                cmdUnequip.ExecuteNonQuery();
            }

            // 2. get petID
            int petID = -1;
            string getPetIDQuery = "SELECT petID FROM Pet WHERE petType = 'Dog' AND colourNum = @colourNum"; // PET TYPE~~~~

            using (MySqlCommand getPetIDCmd = new MySqlCommand(getPetIDQuery, con))
            {
                getPetIDCmd.Parameters.AddWithValue("@colourNum", selectedColourNum);
                object result = getPetIDCmd.ExecuteScalar();
                if (result != null)
                {
                    petID = Convert.ToInt32(result);
                }
                else
                {
                    return;
                }
            }

            // 3. equip the selected one (check the checkbox)
            string equipQuery = "UPDATE UserPets SET equippedStatus = TRUE WHERE userID = @userID AND petID = @petID";
            using (MySqlCommand equipCmd = new MySqlCommand(equipQuery, con))
            {
                equipCmd.Parameters.AddWithValue("@userID", userID);
                equipCmd.Parameters.AddWithValue("@petID", petID);
                equipCmd.ExecuteNonQuery();
            }
        }

        // update session variable
        Session["EquippedPetImagePath"] = string.Format("Images/Dog {0}.png", selectedColourNum); // PET TYPE~~~~

        // reload pets to reflect new equipped status
        LoadOwnedPets(userID);
    }

    // SELL PET (DELETING THE USERPETS ENTRY)
    protected void btnSell_Click(object sender, EventArgs e)
    {
        string selectedColourNum = hfSelectedColourNum.Value;
        userID = Session["UserID"] as string;

        if (string.IsNullOrEmpty(selectedColourNum) || string.IsNullOrEmpty(userID))
            return;

        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        int petID = -1;
        int sellPrice = 0;
        string petType = "";

        using (MySqlConnection con = new MySqlConnection(cs))
        {
            con.Open();

            string queryPet = "SELECT petID, sellPrice, petType FROM Pet WHERE petType = @petType AND colourNum = @colourNum";
            using (MySqlCommand cmd = new MySqlCommand(queryPet, con))
            {
                cmd.Parameters.AddWithValue("@petType", "Dog"); // PET TYPE~~~~
                cmd.Parameters.AddWithValue("@colourNum", selectedColourNum);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        petID = Convert.ToInt32(reader["petID"]);
                        sellPrice = Convert.ToInt32(reader["sellPrice"]);
                        petType = reader["petType"].ToString();
                    }
                }
            }
        }

        Session["petID"] = petID;
        Session["sellPrice"] = sellPrice;
        Session["colourNum"] = selectedColourNum;

        lblSellPrice.Text = sellPrice.ToString(); // UPDATE COIN LABEL
        ScriptManager.RegisterStartupScript(this, GetType(), "showPopup", "showPopup();", true);
    }

    protected void btnConfirmSell_Click(object sender, EventArgs e)
    {
        string userID = Session["UserID"] as string;
        if (Session["petID"] == null || Session["sellPrice"] == null || string.IsNullOrEmpty(userID))
            return;

        int petID = Convert.ToInt32(Session["petID"]);
        int sellPrice = Convert.ToInt32(Session["sellPrice"]);
        string colourNum = Session["colourNum"] as string;

        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        int currentCoins = 0;
        bool wasEquipped = false;

        using (MySqlConnection con = new MySqlConnection(cs))
        {
            con.Open();

            // CHECK IF SOLD PET WAS EQUIPPED
            string checkEquippedQuery = "SELECT equippedStatus FROM UserPets WHERE userID = @userID AND petID = @petID";
            using (MySqlCommand cmd = new MySqlCommand(checkEquippedQuery, con))
            {
                cmd.Parameters.AddWithValue("@userID", userID);
                cmd.Parameters.AddWithValue("@petID", petID);
                object result = cmd.ExecuteScalar();
                wasEquipped = result != null && Convert.ToBoolean(result);
            }

            // 1. DELETE PET FROM UserPets
            string deleteCommand = "DELETE FROM UserPets WHERE userID = @userID AND petID = @petID";
            using (MySqlCommand cmd = new MySqlCommand(deleteCommand, con))
            {
                cmd.Parameters.AddWithValue("@userID", userID);
                cmd.Parameters.AddWithValue("@petID", petID);
                cmd.ExecuteNonQuery();
            }

            // 2. GET CURRENT coin count
            string getCoinsQuery = "SELECT userCoinCount FROM Users WHERE userID = @userID";
            using (MySqlCommand cmd = new MySqlCommand(getCoinsQuery, con))
            {
                cmd.Parameters.AddWithValue("@userID", userID);
                object result = cmd.ExecuteScalar();
                currentCoins = result != null ? Convert.ToInt32(result) : 0;
            }

            // 3. UPDATE coin count
            int updatedCoins = currentCoins + sellPrice;
            string updateCoins = "UPDATE Users SET userCoinCount = @coins WHERE userID = @userID";
            using (MySqlCommand cmd = new MySqlCommand(updateCoins, con))
            {
                cmd.Parameters.AddWithValue("@coins", updatedCoins);
                cmd.Parameters.AddWithValue("@userID", userID);
                cmd.ExecuteNonQuery();
            }

            if (wasEquipped)
            {
                // GET petID of Cat 1
                int cat1PetID = -1;
                string getCat1ID = "SELECT petID FROM Pet WHERE petType = 'Cat' AND colourNum = 1";
                using (MySqlCommand cmd = new MySqlCommand(getCat1ID, con))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                        cat1PetID = Convert.ToInt32(result);
                }

                // ENSURE CAT 1 IS OWNED BY USER
                bool ownsCat1 = false;
                string checkOwnership = "SELECT COUNT(*) FROM UserPets WHERE userID = @userID AND petID = @petID";
                using (MySqlCommand cmd = new MySqlCommand(checkOwnership, con))
                {
                    cmd.Parameters.AddWithValue("@userID", userID);
                    cmd.Parameters.AddWithValue("@petID", cat1PetID);
                    ownsCat1 = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }

                if (ownsCat1)
                {
                    string unequipAll = "UPDATE UserPets SET equippedStatus = FALSE WHERE userID = @userID";
                    using (MySqlCommand cmd = new MySqlCommand(unequipAll, con))
                    {
                        cmd.Parameters.AddWithValue("@userID", userID);
                        cmd.ExecuteNonQuery();
                    }

                    string equipCat1 = "UPDATE UserPets SET equippedStatus = TRUE WHERE userID = @userID AND petID = @petID";
                    using (MySqlCommand cmd = new MySqlCommand(equipCat1, con))
                    {
                        cmd.Parameters.AddWithValue("@userID", userID);
                        cmd.Parameters.AddWithValue("@petID", cat1PetID);
                        cmd.ExecuteNonQuery();
                    }

                    // UPDATE SESSION FOR HOME PAGE PET PATH
                    Session["EquippedPetImagePath"] = "Images/Cat 1.png";
                }
            }
        }

        Session.Remove("petID");
        Session.Remove("sellPrice");
        Session.Remove("colourNum");

        lblPaws.Text = (currentCoins + sellPrice).ToString();
        LoadOwnedPets(userID);
    }

    protected void btnCats_Click(object sender, EventArgs e)
    {
        Response.Redirect("A1800_View-pets.aspx");
        hfCurrentCategory.Value = "CAT";
    }

    protected void btnDogs_Click(object sender, EventArgs e)
    {
        Response.Redirect("A1800_View-pets-dogs.aspx");
        hfCurrentCategory.Value = "DOG";
    }

    protected void btnFuzzy_Click(object sender, EventArgs e)
    {
        Response.Redirect("A1800_View-pets-fuzzy.aspx");
        hfCurrentCategory.Value = "FUZZY";
    }

    protected void btnFarm_Click(object sender, EventArgs e)
    {
        Response.Redirect("A1800_View-pets-farm.aspx");
        hfCurrentCategory.Value = "FARM";
    }

    protected void btnSpecial_Click(object sender, EventArgs e)
    {
        Response.Redirect("A1800_View-pets-special.aspx");
        hfCurrentCategory.Value = "SPECIAL";
    }
    // end: view pets code

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

    // start: notification bell code
    private void LoadPendingInvitesFromDB()
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        List<SessionInvite> pendingInvites = new List<SessionInvite>();
        string query = "SELECT StudySession.sessionID, StudySession.sessionTitle, StudySession.sessionTag, StudySession.sessionStart, StudySession.sessionEnd, Users.username FROM (StudySessionParticipants INNER JOIN StudySession ON StudySessionParticipants.sessionID = StudySession.sessionID) INNER JOIN Users ON StudySession.leaderID = Users.userID WHERE StudySessionParticipants.userID = @userID AND StudySessionParticipants.accepted = false ORDER BY StudySession.sessionID ASC";

        using (MySqlConnection conn = new MySqlConnection(cs))
        using (MySqlCommand cmd = new MySqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@userID", Session["userID"]);
            conn.Open();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    pendingInvites.Add(new SessionInvite
                    {
                        sessionID = Convert.ToInt32(reader["sessionID"]),
                        leaderUsername = reader["username"].ToString(),
                        title = reader["sessionTitle"].ToString(),
                        tag = reader["sessionTag"].ToString(),
                        startTime = Convert.ToDateTime(reader["sessionStart"]),
                        endTime = Convert.ToDateTime(reader["sessionEnd"])
                    });
                }
            }
        }

        Session["PendingInvites"] = pendingInvites;
    }

    private void ShowNextInvite()
    {
        List<SessionInvite> invites = Session["PendingInvites"] as List<SessionInvite>;
        if (invites != null && invites.Count > 0)
        {
            var invite = invites[0];

            litNotificationText.Text = "<p><span style='text-decoration:underline;'>Study session invitation</span></p><table><tr><td><p>From:</p></td><td><p><span style='font-weight:bold;'>" + invite.leaderUsername + "</span></p></td></tr>" + "<tr><td><p>Title:</p></td><td><p><span style='font-weight:bold;'>" + invite.title + "</span></p></td></tr>" + "<tr><td><p>Tag:</p></td><td><p><span style='font-weight:bold;'>" + invite.tag + "</span></p></td></tr>" + "<tr><td><p>Starts:</p></td><td><p><span style='font-weight:bold;'>" + invite.startTime.ToString("dddd, dd MMMM yyyy @ HH:mm") + "</span></p></td></tr><tr><td><p>Ends:</p></td><td><p><span style='font-weight:bold;'>" + invite.endTime.ToString("dddd, dd MMMM yyyy @ HH:mm") + "</span></p></td></tr></table>";

            hiddenSessionID.Value = invite.sessionID.ToString();

            imgNotificationRinging.Visible = true;
            imgNotificationNormal.Visible = false;
            notificationBadge.Visible = true;

            ScriptManager.RegisterStartupScript(this, this.GetType(), "showPopup", "showNotificationPopup();", true);
        }
        else
        {
            imgNotificationRinging.Visible = false;
            imgNotificationNormal.Visible = true;
            notificationBadge.Visible = false;

            ScriptManager.RegisterStartupScript(this, this.GetType(), "showPopupNone", "showNotificationPopup(false);", true);
        }
    }

    protected void btnYes_Click(object sender, EventArgs e)
    {
        int sessionID = int.Parse(hiddenSessionID.Value);
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        string updateQuery = "UPDATE StudySessionParticipants SET accepted = true WHERE sessionID = @sessionID AND userID = @userID";
        using (MySqlConnection conn = new MySqlConnection(cs))
        using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
        {
            cmd.Parameters.AddWithValue("@sessionID", sessionID);
            cmd.Parameters.AddWithValue("@userID", Session["userID"]);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        List<SessionInvite> invites = Session["PendingInvites"] as List<SessionInvite>;
        SessionInvite invite = null;

        if (invites != null)
        {
            foreach (SessionInvite i in invites)
            {
                if (i.sessionID == sessionID)
                {
                    invite = i;
                    break;
                }
            }
        }

        if (invite != null)
        {
            string insertQuery = "INSERT INTO CalendarEvent (eventDesc, eventDate, tagID, userID) VALUES (@eventDesc, @eventDate, @tagID, @userID)";
            using (MySqlConnection conn = new MySqlConnection(cs))
            using (MySqlCommand cmd2 = new MySqlCommand(insertQuery, conn))
            {
                string eventDesc = invite.title + " (From: " + invite.leaderUsername + ")";
                DateTime eventDate = invite.startTime;
                int tagID = 1;
                int userID = Convert.ToInt32(Session["userID"]);

                cmd2.Parameters.AddWithValue("@eventDesc", eventDesc);
                cmd2.Parameters.AddWithValue("@eventDate", eventDate);
                cmd2.Parameters.AddWithValue("@tagID", tagID);
                cmd2.Parameters.AddWithValue("@userID", userID);

                conn.Open();
                cmd2.ExecuteNonQuery();
            }
        }

        hiddenShowCalendar.Value = "true";

        RemoveInviteAndShowNext(sessionID);
    }

    protected void btnNo_Click(object sender, EventArgs e)
    {
        hiddenShowConfirmation.Value = "true";
    }

    private void RemoveInviteAndShowNext(int sessionID)
    {
        List<SessionInvite> invites = Session["PendingInvites"] as List<SessionInvite>;
        if (invites != null)
        {
            var currentInvite = invites.Find(i => i.sessionID == sessionID);
            if (currentInvite != null)
            {
                invites.Remove(currentInvite);
            }
            Session["PendingInvites"] = invites;
            ShowNextInvite();
        }
    }

    private void LoadUpcomingSessions()
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        string query = "SELECT StudySession.sessionID, StudySession.sessionStart FROM StudySession INNER JOIN StudySessionParticipants ON StudySession.sessionID = StudySessionParticipants.sessionID WHERE StudySessionParticipants.userID = @userID AND StudySessionParticipants.accepted = true";

        List<string> jsSessionTimes = new List<string>();

        using (MySqlConnection conn = new MySqlConnection(cs))
        using (MySqlCommand cmd = new MySqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@userID", Session["userID"]);
            conn.Open();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int foundSessionID = Convert.ToInt32(reader["sessionID"]);
                    DateTime sessionStart = Convert.ToDateTime(reader["sessionStart"]);

                    string jsObject = "{ sessionID: " + foundSessionID + ", time: '" + sessionStart.ToString("yyyy-MM-ddTHH:mm:ss") + "' }";
                    jsSessionTimes.Add(jsObject);

                    TimeSpan timeUntilStart = sessionStart - DateTime.Now;
                    if (timeUntilStart.TotalMinutes >= 0 && timeUntilStart.TotalMinutes <= 10)
                    {
                        Session["sessionID"] = foundSessionID;
                    }
                }
            }
        }

        if (jsSessionTimes.Count > 0)
        {
            string jsArray = "[" + string.Join(",", jsSessionTimes.ToArray()) + "]";
            ClientScript.RegisterStartupScript(this.GetType(), "registerSessions", "var upcomingSessions = " + jsArray + ";", true);
        }
    }

    protected void btnCalendar_Click(object sender, EventArgs e)
    {
        Response.Redirect("B100_View-calendar.aspx");
    }

    protected void btnOk_Click(object sender, EventArgs e)
    {
        Response.Redirect("A1800_View-pets-dogs.aspx");
    }

    protected void btnSure_Click(object sender, EventArgs e)
    {
        int sessionID = int.Parse(hiddenSessionID.Value);
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        string deleteQuery = "DELETE FROM StudySessionParticipants WHERE sessionID = @sessionID AND userID = @userID AND accepted = false";
        using (MySqlConnection conn = new MySqlConnection(cs))
        using (MySqlCommand cmd = new MySqlCommand(deleteQuery, conn))
        {
            cmd.Parameters.AddWithValue("@sessionID", sessionID);
            cmd.Parameters.AddWithValue("@userID", Session["userID"]);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        hiddenShowDeclineConfirmed.Value = "true";

        RemoveInviteAndShowNext(sessionID);
    }

    protected void btnNotSure_Click(object sender, EventArgs e)
    {
        Response.Redirect("A1800_View-pets-dogs.aspx");
    }

    protected void btnOkayDeclined_Click(object sender, EventArgs e)
    {
        Response.Redirect("A1800_View-pets-dogs.aspx");
    }

    protected void btnJoin_Click(object sender, EventArgs e)
    {
        if (Session["sessionID"] != null && Session["userID"] != null)
        {
            int sessionID = Convert.ToInt32(Session["sessionID"]);
            int userID = Convert.ToInt32(Session["userID"]);

            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string updateQuery = "UPDATE StudySessionParticipants SET joined = true WHERE sessionID = @sessionID AND userID = @userID";

            using (MySqlConnection conn = new MySqlConnection(cs))
            using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
            {
                cmd.Parameters.AddWithValue("@sessionID", sessionID);
                cmd.Parameters.AddWithValue("@userID", userID);
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Response.Redirect("A1400_View-study-session.aspx");
                }
            }
        }
    }
    // end: notification bell code
}