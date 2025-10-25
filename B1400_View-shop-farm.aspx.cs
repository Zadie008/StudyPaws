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

public partial class B1400_View_shop_farm : System.Web.UI.Page
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
    protected void btnCats_Click(object sender, EventArgs e)
    {
        Response.Redirect("B1400_View-shop.aspx");
    }

    protected void btnDogs_Click(object sender, EventArgs e)
    {
        Response.Redirect("B1400_View-shop-dogs.aspx");
    }

    protected void btnFuzzy_Click(object sender, EventArgs e)
    {
        Response.Redirect("B1400_View-shop-fuzzy.aspx");
    }

    protected void btnFarm_Click(object sender, EventArgs e)
    {
        Response.Redirect("B1400_View-shop-farm.aspx");
    }

    protected void btnSpecial_Click(object sender, EventArgs e)
    {
        Response.Redirect("B1400_View-shop-special.aspx");
    }

    // start: view shop code

    private void LoadOwnedPets(string userID) // CHANGED
    {
        ContentPlaceHolder content = (ContentPlaceHolder)Master.FindControl("mainContentPlaceHolder");

        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        //get user XP level and coins
        int userXPLevel = GetXPLevel(cs, userID);
        int userCoins = GetUserCoins(cs, userID);
        //load all pets
        string allPetsCommand = "SELECT petID, colourNum, xpCost, coinCost FROM Pet WHERE Pet.petType = @petType";
        List<Pet> allPets = new List<Pet>();

        using (MySqlConnection con = new MySqlConnection(cs))
        using (MySqlCommand cmd = new MySqlCommand(allPetsCommand, con))
        {
            cmd.Parameters.AddWithValue("@petType", "Farm"); // PET TYPE~~~~

            try
            {
                con.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int petID = Convert.ToInt32(reader["petID"]);
                        int colourNum = Convert.ToInt32(reader["colourNum"]);
                        int xpCost = Convert.ToInt32(reader["xpCost"]);
                        int coinCost = Convert.ToInt32(reader["coinCost"]);
                        allPets.Add(new Pet(petID, colourNum, xpCost, coinCost));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading all pets: " + ex.Message);
            }
        }

        //load owned pets for this user
        string ownedPetsCommand = "SELECT UserPets.petID FROM UserPets INNER JOIN Pet ON UserPets.PetID = Pet.petID WHERE UserPets.userID = @userID AND Pet.petType = @petType";
        HashSet<int> ownedPetIDs = new HashSet<int>();

        using (MySqlConnection con = new MySqlConnection(cs))
        using (MySqlCommand cmd = new MySqlCommand(ownedPetsCommand, con))
        {
            cmd.Parameters.AddWithValue("@userID", userID);
            cmd.Parameters.AddWithValue("@petType", "Farm"); // PET TYPE~~~~

            try
            {
                con.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ownedPetIDs.Add(Convert.ToInt32(reader["petID"]));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading owned pets: " + ex.Message);
            }
        }
        bool allPetsLocked = false;
        //for (int i = 0; i <Math.Min(allPets.Count, 5); i++)
        //{
        //    Pet pet = allPets[i];
        //    bool isOwned = ownedPetIDs.Contains(pet.PetID);
        //    bool isLocked = userXP < pet.xpCost;
        //    if (!isOwned && !isLocked)
        //    {
        //        allPetsLocked = false;
        //        break;
        //    }
        //}

        //display pets
        for (int i = 0; i < 5; i++)
        {
            int displayIndex = i + 1;
            Pet pet = allPets[i];
            bool isOwned = ownedPetIDs.Contains(pet.PetID);
            bool isLocked = userXPLevel < 15;
            if (isLocked)
            {
                allPetsLocked = true;
            }

            System.Web.UI.WebControls.Image petImg = (System.Web.UI.WebControls.Image)content.FindControl("imgPet" + displayIndex);
            Button selectBtn = (Button)content.FindControl("btnSelect" + displayIndex);
            ImageButton pawIconBtn = (ImageButton)content.FindControl("btnPaw" + displayIndex);
            HtmlGenericControl circleDiv = (HtmlGenericControl)content.FindControl("circle" + displayIndex);

            if (petImg != null && selectBtn != null && circleDiv != null)
            {
                //image
                petImg.ImageUrl = string.Format("Images/Farm {0}.png", pet.ColourNum); // PET TYPE~~~~
                petImg.Visible = true;

                //circle
                string circleClass = "petCircleShop";
                if (isOwned)
                {
                    circleClass += " owned";
                }
                else
                {
                    circleClass += " unowned";
                }
                if (isLocked)
                {
                    circleClass += " locked";
                    petImg.CssClass = "locked";
                }
                circleDiv.Attributes["class"] = circleClass;

                //button
                selectBtn.Visible = true;
                pawIconBtn.Visible = true;
                selectBtn.CommandArgument = pet.PetID.ToString();
                selectBtn.Attributes["data-petid"] = pet.PetID.ToString();
                selectBtn.Attributes["data-price"] = pet.coinCost.ToString();
                pawIconBtn.Attributes["data-petid"] = pet.PetID.ToString();
                pawIconBtn.Attributes["data-price"] = pet.coinCost.ToString();

                if (isOwned)
                {
                    selectBtn.Text = "SOLD";
                    selectBtn.CssClass = "button soldButton";
                    pawIconBtn.Visible = false;
                }
                else
                {
                    selectBtn.Text = pet.coinCost.ToString();
                    selectBtn.CssClass = "button priceButton";
                    selectBtn.Enabled = !isLocked;
                    pawIconBtn.CssClass = "pawIconButton";
                    pawIconBtn.Enabled = !isLocked;
                }
                if (isLocked)
                {
                    selectBtn.CssClass += " locked";
                    pawIconBtn.ImageUrl = string.Format("Icons/icons8-lock-white-96.png");
                    pawIconBtn.CssClass += " locked";
                }
                circleDiv.Visible = true;
            }
        }

        if (allPetsLocked)
        {
            btnBuy.Visible = false;
            lblLocked.Visible = true;
            lblLocked.Text = "UNLOCKS AT LEVEL 15";
        }
        else
        {
            btnBuy.Visible = false;
            lblLocked.Visible = false;
        }
    }
    public void btnSelect_Clicked(object sender, EventArgs e) // CHANGED
    {
        Control clickedBtn = (Control)sender;
        ContentPlaceHolder content = (ContentPlaceHolder)Master.FindControl("mainContentPlaceHolder");
        Button buyBtn = (Button)content.FindControl("btnBuy");

        string controlID = clickedBtn.ID;
        string petNum = controlID.Replace("btnSelect", "").Replace("btnPaw", "");

        Button selectBtn = (Button)content.FindControl("btnSelect" + petNum);
        ImageButton pawBtn = (ImageButton)content.FindControl("btnPaw" + petNum);
        HtmlGenericControl circleDiv = (HtmlGenericControl)content.FindControl("circle" + petNum);

        for (int i = 1; i <= 5; i++)
        {
            if (i.ToString() != petNum)
            {
                Button otherSelectBtn = (Button)content.FindControl("btnSelect" + i);
                ImageButton otherPawBtn = (ImageButton)content.FindControl("btnPaw" + i);
                HtmlGenericControl otherCircleDiv = (HtmlGenericControl)content.FindControl("circle" + i);
                if (otherSelectBtn != null)
                {
                    otherSelectBtn.CssClass = otherSelectBtn.CssClass.Replace(" buttonSelected", "");
                }
                if (otherPawBtn != null)
                {
                    otherPawBtn.CssClass = otherPawBtn.CssClass.Replace(" buttonSelected", "");
                }
                if (otherCircleDiv != null)
                {
                    string circleClass = otherCircleDiv.Attributes["class"];
                    if (circleClass != null && circleClass.Contains(" selected"))
                    {
                        otherCircleDiv.Attributes["class"] = circleClass.Replace(" selected", "");
                    }
                }
            }
        }
        if (selectBtn != null && selectBtn.CssClass.Contains("soldButton"))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "showAlreadyOwnedPopup", "showAlreadyOwnedPopup();", true);
            buyBtn.Visible = false;
            hfSelectedPetID.Value = "";
            hfSelectedPetPrice.Value = "";
            return;
        }
        bool isCurrentlySelected = selectBtn != null && selectBtn.CssClass.Contains("buttonSelected");
        if (isCurrentlySelected)
        {
            if (selectBtn != null)
            {
                selectBtn.CssClass = selectBtn.CssClass.Replace(" buttonSelected", "");
            }
            if (pawBtn != null)
            {
                pawBtn.CssClass = pawBtn.CssClass.Replace(" buttonSelected", "");
            }
            buyBtn.Visible = false;
            hfSelectedPetID.Value = "";
            hfSelectedPetPrice.Value = "";
            if (circleDiv != null)
            {
                string circleClass = circleDiv.Attributes["class"];
                if (!string.IsNullOrEmpty(circleClass) && circleClass.Contains(" selected"))
                {
                    circleDiv.Attributes["class"] = circleClass.Replace(" selected", "");
                }
            }
        }
        else
        {
            if (selectBtn != null && !selectBtn.CssClass.Contains("buttonSelected"))
            {
                selectBtn.CssClass += " buttonSelected";
            }
            if (pawBtn != null && !pawBtn.CssClass.Contains("buttonSelected"))
            {
                pawBtn.CssClass += " buttonSelected";
            }
            buyBtn.Visible = true;
            string petID = "";
            string price = "";
            if (selectBtn != null && selectBtn.Attributes["data-petid"] != null)
            {
                petID = selectBtn.Attributes["data-petid"];
                price = selectBtn.Attributes["data-price"];
            }
            else if (pawBtn != null && pawBtn.Attributes["data-petid"] != null)
            {
                petID = pawBtn.Attributes["data-petid"];
                price = pawBtn.Attributes["data-price"];
            }
            hfSelectedPetID.Value = petID;
            hfSelectedPetPrice.Value = price;
            if (circleDiv != null)
            {
                string circleClass = circleDiv.Attributes["class"];
                if (!string.IsNullOrEmpty(circleClass) && !circleClass.Contains("locked") && !circleClass.Contains(" selected"))
                {
                    circleDiv.Attributes["class"] = circleClass + " selected";
                }
            }
        }
    }
    private int GetXPLevel(string cs, string userID) // CHANGED
    {
        int level = 0;
        string currentLevelQuery = "SELECT levelID FROM CurrentLevel WHERE userID = @userID";
        using (MySqlConnection con = new MySqlConnection(cs))
        using (MySqlCommand cmdCurrentLevel = new MySqlCommand(currentLevelQuery, con))
        {
            cmdCurrentLevel.Parameters.AddWithValue("@userID", userID);
            con.Open();
            object result = cmdCurrentLevel.ExecuteScalar();
            if (result != null && int.TryParse(result.ToString(), out level))
            {
                return level;
            }
            else
            {
                return 0;
            }
        }
    }
    private int GetUserCoins(string cs, string userID)
    {
        string query = "SELECT userCoinCount FROM Users WHERE userID = @userID";

        using (MySqlConnection con = new MySqlConnection(cs))
        using (MySqlCommand cmd = new MySqlCommand(query, con))
        {
            cmd.Parameters.AddWithValue("@userID", userID);
            try
            {
                con.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch
            {
                return 0;
            }
        }
    }
    protected void btnBuy_Click(object sender, EventArgs e) // CHANGED
    {
        String petID = hfSelectedPetID.Value;
        String priceStr = hfSelectedPetPrice.Value; // ADDED

        if (!string.IsNullOrEmpty(petID) && !string.IsNullOrEmpty(priceStr)) // CHANGED
        {
            int price = Convert.ToInt32(priceStr); // ADDED
            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString; //ADDED

            if (Session["Username"] != null) // ADDED
            {
                String username = Session["Username"].ToString();
                userID = GetUserID(username, cs);
            }

            int userCoins = GetUserCoins(cs, userID); // CHANGED

            lblSellPrice.Text = price.ToString(); // ADDED

            if (userCoins >= price) // CHANGED
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showConfirmBuyPopup", "showConfirmBuyPopup();", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showInsufficientCoinsPopup", "showInsufficientCoinsPopup();", true);
            }
        }
    }
    protected void btnYesBuy_Click(object sender, EventArgs e) // CHANGED
    {
        String petID = hfSelectedPetID.Value;
        String priceStr = hfSelectedPetPrice.Value;

        if (!string.IsNullOrEmpty(petID) && !string.IsNullOrEmpty(priceStr))
        {
            int price = Convert.ToInt32(priceStr); // CHANGED
            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            if (Session["Username"] != null) // ADDED
            {
                String username = Session["Username"].ToString();
                userID = GetUserID(username, cs);
            }
            int currentUserCoins = GetUserCoins(cs, userID); //ADDED
            if (currentUserCoins >= price) //ADDED
            {
                int newUserCoins = currentUserCoins - price; //CHANGED
                try
                {
                    using (MySqlConnection con = new MySqlConnection(cs))
                    {
                        con.Open();
                        using (MySqlTransaction transaction = con.BeginTransaction())
                        {
                            try
                            {
                                //Update user coin count
                                string updateCoinsQuery = "UPDATE Users SET userCoinCount = @newCoins WHERE userID = @userID"; // CHANGED
                                MySqlCommand updateCoinsCmd = new MySqlCommand(updateCoinsQuery, con, transaction);
                                updateCoinsCmd.Parameters.AddWithValue("@newCoins", newUserCoins); // CHANGED
                                updateCoinsCmd.Parameters.AddWithValue("@userID", userID);
                                updateCoinsCmd.ExecuteNonQuery();

                                //add pet to inventory
                                string addPetQuery = "INSERT INTO UserPets (userID, petID, equippedStatus) VALUES (@userID, @petID, 0)";
                                MySqlCommand addPetCmd = new MySqlCommand(addPetQuery, con, transaction);
                                addPetCmd.Parameters.AddWithValue("@userID", userID);
                                addPetCmd.Parameters.AddWithValue("@petID", petID);
                                addPetCmd.ExecuteNonQuery();

                                //updated collected pets badge
                                string collectedPetsQuery = "UPDATE Users SET collectedPets = collectedPets + 1 WHERE userID = @userID";
                                MySqlCommand updateCollectedPetsCmd = new MySqlCommand(collectedPetsQuery, con, transaction);
                                updateCollectedPetsCmd.Parameters.AddWithValue("@userID", userID);
                                updateCollectedPetsCmd.ExecuteNonQuery();

                                transaction.Commit();

                                hfSelectedPetID.Value = ""; // ADDED
                                hfSelectedPetPrice.Value = ""; // ADDED

                                CheckCollectedPetsBadge(con, Convert.ToInt32(userID));
                                //close pop-up
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "hideConfirmBuyPopup", "hideConfirmBuyPopup();", true); // ADDED
                                //show confirmation pop-up
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "showSuccessPopup", "showPurchaseSuccessPopup();", true); // ADDED
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                Console.WriteLine("Transaction error: " + ex.Message);
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "hideConfirmBuyPopup", "hideConfirmBuyPopup();", true); // ADDED
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "showInsufficientCoinsPopup", "showInsufficientCoinsPopup();", true);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Connection error: " + ex.Message);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "hideConfirmBuyPopup", "hideConfirmBuyPopup();", true); // ADDED
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showInsufficientCoinsPopup", "showInsufficientCoinsPopup();", true);
                }
            }
            else // ADDED
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "hideConfirmBuyPopup", "hideConfirmBuyPopup();", true); // ADDED
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showInsufficientCoinsPopup", "showInsufficientCoinsPopup();", true);
            }

        }
    }
    // CHECKING COLLECTED PETS BADGE
    private static void CheckCollectedPetsBadge(MySqlConnection con, int userID)
    {
        string getCountQuery = "SELECT collectedPets FROM Users WHERE userID = @userID";
        int collectedCount = 0;
        using (MySqlCommand getCountCmd = new MySqlCommand(getCountQuery, con))
        {
            getCountCmd.Parameters.AddWithValue("@userID", userID);
            object result = getCountCmd.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                collectedCount = Convert.ToInt32(result);
            }
        }

        if (collectedCount >= 25)
        {
            AwardBadgeStatic(con, userID, 11, "Gold");
        }
        else if (collectedCount >= 15)
        {
            AwardBadgeStatic(con, userID, 11, "Silver");
        }
        else if (collectedCount >= 5)
        {
            AwardBadgeStatic(con, userID, 11, "Bronze");
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
    protected void btnNoBuy_Click(object sender, EventArgs e) // CHANGED
    {
        hfSelectedPetID.Value = "";
        hfSelectedPetPrice.Value = "";
        ScriptManager.RegisterStartupScript(this, this.GetType(), "hideConfirmBuyPopup", "hideConfirmBuyPopup();", true);
        Response.Redirect(Request.RawUrl);
    }
    protected void btnSold_Click(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), "showAlreadyOwnedPopup", "showAlreadyOwnedPopup();", true);
    }
    protected void btnCloseAlreadyOwned_Click(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), "hideAlreadyOwnedPopup", "hideAlreadyOwnedPopup();", true);
    }
    protected void btnCloseInsufficientCoins_Click(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), "hideInsufficientCoinsPopup", "hideInsufficientCoinsPopup();", true);
    }
    protected void btnGoToInventory_Click(object sender, EventArgs e) // ADDED
    {
        Response.Redirect("A1800_View-pets.aspx");
    }
    protected void btnStayInShop_Click(object sender, EventArgs e) // ADDED
    {
        Response.Redirect(Request.RawUrl); // ADDED
    }
    // end: view shop code

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
        Response.Redirect("B1400_View-shop.aspx");
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
        Response.Redirect("B1400_View-shop.aspx");
    }

    protected void btnOkayDeclined_Click(object sender, EventArgs e)
    {
        Response.Redirect("B1400_View-shop.aspx");
    }

    protected void btnJoin_Click(object sender, EventArgs e)
    {
        if (Session["sessionID"] != null && Session["userID"] != null)
        {
            int sessionID = Convert.ToInt32(Session["sessionID"]);
            int userID = Convert.ToInt32(Session["userID"]);

            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            string checkTimeQuery = @"SELECT sessionStart FROM StudySession WHERE sessionID = @sessionID AND DATE_ADD(sessionStart, INTERVAL 1 MINUTE) < NOW()";

            string updateQuery = "UPDATE StudySessionParticipants SET joined = true WHERE sessionID = @sessionID AND userID = @userID";

            using (MySqlConnection conn = new MySqlConnection(cs))
            {
                conn.Open();

                using (MySqlCommand checkCmd = new MySqlCommand(checkTimeQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@sessionID", sessionID);
                    var result = checkCmd.ExecuteScalar();

                    // session started more than 1 minute ago
                    if (result != null)
                    {
                        Response.Redirect("Default.aspx");
                        return;
                    }
                }

                // session started less than 1 minute ago, so can join study session
                using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn))
                {
                    updateCmd.Parameters.AddWithValue("@sessionID", sessionID);
                    updateCmd.Parameters.AddWithValue("@userID", userID);
                    int rowsAffected = updateCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Response.Redirect("A1400_View-study-session.aspx");
                    }
                }
            }
        }
    }
    // end: notification bell code
}