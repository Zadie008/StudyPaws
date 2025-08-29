using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Default2 : System.Web.UI.Page
{
    private List<FriendRequest> pendingFriendRequests = new List<FriendRequest>();
    private int currentRequestIndex = 0;
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

                if (!string.IsNullOrEmpty(userID))
                {
                    int userXP = GetUserXP(cs, userID);
                    LoadFriends();
                    GetLevelInformation(cs, userID);
                    GetUserStats(cs, userID);
                    GetUserProfileIcon(cs, userID);
                    LoadPendingInvitesFromDB();
                    LoadUpcomingSessions();
                    Tuple<int, int, int> levelInfo = GetLevelInformation(cs, userID);
                    int currentLevel = levelInfo.Item1;
                    int currentLevelXpAmount = levelInfo.Item2; // XP needed to reach current level
                    int nextLevelXpAmount = levelInfo.Item3; // XP needed to reach next level

                    lblLevelNumber.Text = currentLevel.ToString();
                   
                    // Calculate progress for the progress bar
                    CalculateXPProgressBar(userXP, currentLevelXpAmount, nextLevelXpAmount);
                }
               
            }
            else
            {
                Response.Redirect("Landing-page.aspx");
            }
            ShowNextInvite(false);
        }
        ScriptManager1.RegisterAsyncPostBackControl(GridView1);
    }

    // start: friends code
    protected void btnSearchFriends_Click(object sender, EventArgs e)
    {
        Response.Redirect("C700_Search-Friends.aspx");
    }
    private DataTable LoadPendingFriendRequests()
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        DataTable dt = new DataTable();

        using (MySqlConnection con = new MySqlConnection(cs))
        {
            string query = @"
        SELECT friendshipID, userIDfrom, u.username
        FROM FriendsList f
        JOIN Users u ON f.userIDfrom = u.userID
        WHERE f.userIDto = @userID AND f.requestStatus = 'Pending'"; // Fixed column name

            MySqlCommand cmd = new MySqlCommand(query, con);
            cmd.Parameters.AddWithValue("@userID", Session["userID"]);

            con.Open();
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }
        }

        return dt;
    }

    protected void btnMail_Click(object sender, EventArgs e)
    {
        DataTable pendingRequests = LoadPendingFriendRequests();
        DataTable pendingGifts = LoadPendingGifts();

        // Clear any existing popups first
        pnlFriendRequests.Visible = false;
        pnlGiftNotifications.Visible = false;

        // Reset viewstate to ensure fresh data
        ViewState["PendingFriendRequests"] = pendingRequests;
        ViewState["PendingGifts"] = pendingGifts;

        if (pendingRequests.Rows.Count > 0)
        {
            ViewState["CurrentRequestIndex"] = 0;
            ShowFriendRequest(0);
        }
        else if (pendingGifts.Rows.Count > 0)
        {
            ViewState["CurrentGiftIndex"] = 0;
            ShowGiftNotification(0);
        }
        else
        {
            // Show the no notifications popup
            ScriptManager.RegisterStartupScript(this, this.GetType(), "showNoNotifications",
                "document.getElementById('popupNoNotifications').style.display = 'block';", true);
        }

        updFriendRequests.Update();
    }
    private DataTable LoadPendingGifts()
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        DataTable dt = new DataTable();
        string currentUserID = Session["userID"].ToString();

        using (MySqlConnection con = new MySqlConnection(cs))
        {
            string query = @"
        SELECT 
            CASE 
                WHEN f.userIDfrom = @userID THEN f.userIDto
                ELSE f.userIDfrom
            END as friendID,
            u.username
        FROM FriendsList f
        JOIN Users u ON 
            (f.userIDfrom = @userID AND u.userID = f.userIDto) OR
            (f.userIDto = @userID AND u.userID = f.userIDfrom)
        WHERE f.giftAvailable = 1
        AND f.requestStatus = 'Accepted'";

            MySqlCommand cmd = new MySqlCommand(query, con);
            cmd.Parameters.AddWithValue("@userID", currentUserID);

            con.Open();
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }
        }

        return dt;
    }
    private void ShowGiftNotification(int index)
    {
        DataTable pendingGifts = ViewState["PendingGifts"] as DataTable;

        if (pendingGifts != null && index >= 0 && index < pendingGifts.Rows.Count)
        {
            DataRow row = pendingGifts.Rows[index];

            hiddenGiftFriendID.Value = row["friendID"].ToString();
            lblGiftMessage.Text = "You received a gift of 10 coins from <span style='font-weight:bold;'>" + row["username"].ToString() + "</span>!";

            pnlGiftNotifications.Visible = true;
            ViewState["CurrentGiftIndex"] = index;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "showGiftPopup",
                "document.getElementById('popupGiftNotifications').style.display = 'block';", true);
            updFriendRequests.Update();
        }
    }
    protected void btnCollectGift_Click(object sender, EventArgs e)
    {
        string friendID = hiddenGiftFriendID.Value;
        string userID = Session["userID"].ToString();

        if (!string.IsNullOrEmpty(friendID))
        {
            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (MySqlConnection con = new MySqlConnection(cs))
            {
                con.Open();

                // Update current user's coin count (add 10 coins)
                string updateCoinsQuery = @"
            UPDATE Users 
            SET userCoinCount = userCoinCount + 10 
            WHERE userID = @userID";

                MySqlCommand updateCoinsCmd = new MySqlCommand(updateCoinsQuery, con);
                updateCoinsCmd.Parameters.AddWithValue("@userID", userID);
                updateCoinsCmd.ExecuteNonQuery();

                // Reset gift status
                string updateGiftQuery = @"
            UPDATE FriendsList 
            SET giftAvailable = 0, lastGiftSender = NULL
            WHERE (userIDfrom = @friendID AND userIDto = @userID)
               OR (userIDfrom = @userID AND userIDto = @friendID)";

                MySqlCommand updateGiftCmd = new MySqlCommand(updateGiftQuery, con);
                updateGiftCmd.Parameters.AddWithValue("@friendID", friendID);
                updateGiftCmd.Parameters.AddWithValue("@userID", userID);
                updateGiftCmd.ExecuteNonQuery();
            }

           
            GetUserStats(cs, userID);
            LoadFriends();

        
            pnlGiftNotifications.Visible = false;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "hideGiftPopup",
                "document.getElementById('popupGiftNotifications').style.display = 'none';", true);

         
            updFriendRequests.Update();

           

            ShowNextGiftOrClose();
        }
    }
    protected void btnLaterGift_Click(object sender, EventArgs e)
    {
        ShowNextGiftOrClose();
    }
    private void ShowNextGiftOrClose()
    {
        DataTable pendingGifts = ViewState["PendingGifts"] as DataTable;
        int currentIndex = ViewState["CurrentGiftIndex"] != null ? (int)ViewState["CurrentGiftIndex"] : 0;

        if (pendingGifts != null && pendingGifts.Rows.Count > 0)
        {
            pendingGifts.Rows.RemoveAt(currentIndex);
            ViewState["PendingGifts"] = pendingGifts;

            if (pendingGifts.Rows.Count > 0)
            {
                ShowGiftNotification(0);
            }
            else
            {
                pnlGiftNotifications.Visible = false;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "hideGiftPopup",
                    "document.getElementById('popupGiftNotifications').style.display = 'none';", true);
                updFriendRequests.Update();
            }
        }
        else
        {
            pnlGiftNotifications.Visible = false;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "hideGiftPopup",
                "document.getElementById('popupGiftNotifications').style.display = 'none';", true);
            updFriendRequests.Update();
        }
    }
    private void ShowFriendRequest(int index)
    {
        DataTable pendingRequests = ViewState["PendingFriendRequests"] as DataTable;

        if (pendingRequests != null && index >= 0 && index < pendingRequests.Rows.Count)
        {
            DataRow row = pendingRequests.Rows[index];

            hiddenFriendRequestID.Value = row["friendshipID"].ToString();
            hiddenRequesterID.Value = row["userIDfrom"].ToString();
            lblFriendRequestMessage.Text = "You have a friend request from <span style='font-weight:bold;'>" + row["username"].ToString() + "</span>";

            pnlFriendRequests.Visible = true;
            ViewState["CurrentRequestIndex"] = index;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "showFriendRequestPopup",
                "document.getElementById('popupFriendRequests').style.display = 'block';", true);
            updFriendRequests.Update();
        }
    }
    protected void btnAcceptFriendRequest_Click(object sender, EventArgs e)
    {
        string friendshipID = hiddenFriendRequestID.Value;

        if (!string.IsNullOrEmpty(friendshipID))
        {
            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (MySqlConnection con = new MySqlConnection(cs))
            {
                string query = "UPDATE friendslist SET requestStatus = 'Accepted' WHERE friendshipID = @friendshipID";
                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.Parameters.AddWithValue("@friendshipID", friendshipID);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            // INSTANT UPDATE: Reload friends and update the panel
            LoadFriends();

            // Hide the friend request panel
            pnlFriendRequests.Visible = false;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "hideFriendRequestPopup",
                "document.getElementById('popupFriendRequests').style.display = 'none';", true);

            // Update the UpdatePanel
            updFriendRequests.Update();

            ShowNextRequestOrClose();
        }
    }


    protected void btnDeclineFriendRequest_Click(object sender, EventArgs e)
    {
        string friendshipID = hiddenFriendRequestID.Value;

        if (!string.IsNullOrEmpty(friendshipID))
        {
            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (MySqlConnection con = new MySqlConnection(cs))
            {
                string deleteQuery = "DELETE FROM friendslist WHERE friendshipID = @friendshipID";
                MySqlCommand cmd = new MySqlCommand(deleteQuery, con);
                cmd.Parameters.AddWithValue("@friendshipID", friendshipID);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            // Reload friends immediately
            LoadFriends();

            ShowNextRequestOrClose();
        }

        updFriendRequests.Update();
    }

    private void RemoveProcessedRequest()
    {
        if (ViewState["PendingFriendRequests"] != null)
        {
            pendingFriendRequests = (List<FriendRequest>)ViewState["PendingFriendRequests"];
            currentRequestIndex = (int)ViewState["CurrentRequestIndex"];
        }

        if (pendingFriendRequests.Count > 0 && currentRequestIndex < pendingFriendRequests.Count)
        {
            pendingFriendRequests.RemoveAt(currentRequestIndex);
            ViewState["PendingFriendRequests"] = pendingFriendRequests;
        }
    }

    private void ShowNextRequestOrClose()
    {
        DataTable pendingRequests = ViewState["PendingFriendRequests"] as DataTable;
        int currentIndex = ViewState["CurrentRequestIndex"] != null ? (int)ViewState["CurrentRequestIndex"] : 0;

        if (pendingRequests != null && pendingRequests.Rows.Count > 0)
        {
            pendingRequests.Rows.RemoveAt(currentIndex);
            ViewState["PendingFriendRequests"] = pendingRequests;

            if (pendingRequests.Rows.Count > 0)
            {
                ShowFriendRequest(0);
            }
            else
            {
                pnlFriendRequests.Visible = false;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "hideFriendRequestPopup",
                    "document.getElementById('popupFriendRequests').style.display = 'none';", true);
                updFriendRequests.Update();
            }
        }
        else
        {
            pnlFriendRequests.Visible = false;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "hideFriendRequestPopup",
                "document.getElementById('popupFriendRequests').style.display = 'none';", true);
            updFriendRequests.Update();
        }
    }

    protected void btnSendGift_Click(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        string friendID = btn.CommandArgument.ToString();
        string userID = Session["userID"].ToString();

        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        using (MySqlConnection con = new MySqlConnection(cs))
        {
            con.Open();

            // Check if there's already an uncollected gift
            string checkQuery = @"
        SELECT giftAvailable, lastGiftSender 
        FROM FriendsList 
        WHERE (userIDfrom = @currentUserID AND userIDto = @friendID)
           OR (userIDfrom = @friendID AND userIDto = @currentUserID)";

            MySqlCommand checkCmd = new MySqlCommand(checkQuery, con);
            checkCmd.Parameters.AddWithValue("@currentUserID", userID);
            checkCmd.Parameters.AddWithValue("@friendID", friendID);

            bool canSendGift = true;

            using (MySqlDataReader reader = checkCmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    bool giftAvailable = ConvertToBoolean(reader["giftAvailable"]);
                    object lastGiftSender = reader["lastGiftSender"];

                    // Can't send if there's already an uncollected gift
                    canSendGift = !giftAvailable;
                }
            }

            if (!canSendGift)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "giftAlreadySent",
                    "alert('There is already an uncollected gift for this friend.');", true);
                return;
            }

            // Update receiver's coin count (add 10 coins)
            string updateCoinsQuery = @"
        UPDATE Users 
        SET userCoinCount = userCoinCount + 10 
        WHERE userID = @receiverID";

            MySqlCommand updateCoinsCmd = new MySqlCommand(updateCoinsQuery, con);
            updateCoinsCmd.Parameters.AddWithValue("@receiverID", friendID);
            updateCoinsCmd.ExecuteNonQuery();

            // Mark gift as available and set who sent it
            string updateFriendQuery = @"
        UPDATE FriendsList 
        SET giftAvailable = 1, lastGiftSender = @currentUserID
        WHERE (userIDfrom = @currentUserID AND userIDto = @friendID)
           OR (userIDfrom = @friendID AND userIDto = @currentUserID)";

            MySqlCommand updateFriendCmd = new MySqlCommand(updateFriendQuery, con);
            updateFriendCmd.Parameters.AddWithValue("@currentUserID", userID);
            updateFriendCmd.Parameters.AddWithValue("@friendID", friendID);
            updateFriendCmd.ExecuteNonQuery();
        }

      

        LoadFriends();
        GetUserStats(cs, userID);
    }

    private void LoadFriends()
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        string currentUserID = Session["userID"].ToString();

        using (MySqlConnection con = new MySqlConnection(cs))
        {
            string command = @"
        SELECT 
            u.userID, 
            u.username, 
            u.iconNum, 
            f.giftAvailable,
            f.lastGiftSender,
            f.userIDfrom,
            f.userIDto,
            CASE 
                WHEN f.userIDfrom = @currentUserID THEN u.userID
                ELSE f.userIDfrom
            END as friendUserID
        FROM Users u 
        JOIN friendslist f ON (u.userID = f.userIDto AND f.userIDfrom = @currentUserID) OR (u.userID = f.userIDfrom AND f.userIDto = @currentUserID) 
        WHERE u.userID != @currentUserID AND f.requestStatus = 'Accepted'";

            MySqlCommand cmd = new MySqlCommand(command, con);
            cmd.Parameters.AddWithValue("@currentUserID", currentUserID);

            con.Open();
            DataTable dt = new DataTable();
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            GridView1.DataSource = dt;
            GridView1.DataBind();
        }
    }

    public string GetProfileImageUrl(object iconNum)
    {
        int num = Convert.ToInt32(iconNum);
        switch (num)
        {
            case 1: return "Images/ProfilePictures/CatPfp.png";
            case 2: return "Images/ProfilePictures/DogPfp.png";
            case 3: return "Images/ProfilePictures/BunnyPfp.png";
            case 4: return "Images/ProfilePictures/CowPfp.png";
            case 5: return "Images/ProfilePictures/UnicornPfp.png";
            default: return "Images/ProfilePictures/CatPfp.png";
        }
    }
    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "DeleteFriend")
        {
            string friendID = e.CommandArgument.ToString();
            hiddenFriendToDelete.Value = friendID; // Store in hidden field

            // Show the confirmation popup
            pnlDeleteFriend.Visible = true;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "showDeletePopup",
                "document.getElementById('popup-blue-box').style.display = 'block';", true);

            // Update the UpdatePanel
            updFriendRequests.Update();
        }
    }
    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Button btnSendGift = (Button)e.Row.FindControl("btnSendGift");
            ImageButton btnDeleteFriend = (ImageButton)e.Row.FindControl("btnDeleteFriend");

            if (btnSendGift != null)
            {
                DataRowView rowView = (DataRowView)e.Row.DataItem;
                bool giftAvailable = ConvertToBoolean(rowView["giftAvailable"]);

                if (giftAvailable)
                {
                    btnSendGift.Enabled = false;
                    btnSendGift.Text = "Gift Sent";
                    btnSendGift.CssClass = "btn btn-secondary";
                    btnSendGift.ToolTip = "There's already an uncollected gift";
                }
                else
                {
                    btnSendGift.Enabled = true;
                    btnSendGift.Text = "Send Gift";
                    btnSendGift.CssClass = "btn btn-primary";
                    btnSendGift.ToolTip = "Send 10 coins to this friend";
                }
            }

            if (btnDeleteFriend != null)
            {
                DataRowView rowView = (DataRowView)e.Row.DataItem;
                string friendID = rowView["userID"].ToString(); 
                btnDeleteFriend.CommandArgument = friendID;
                btnDeleteFriend.CommandName = "DeleteFriend";
            }
        }
    }
    // Helper method to handle various MySQL boolean representations
    private bool ConvertToBoolean(object value)
    {
        if (value == null || value == DBNull.Value)
            return false;

        if (value is int)
        {
            return (int)value == 1;
        }
        else if (value is bool)
        {
            return (bool)value;
        }
        else if (value is string)
        {
            string strValue = value.ToString().ToLower();
            return strValue == "1" || strValue == "true" || strValue == "yes";
        }

        return false;
    }
    private void SendGiftToFriend(string friendID)
    {
        string userID = Session["userID"].ToString();
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        using (MySqlConnection con = new MySqlConnection(cs))
        {
            con.Open();

            // Check if current user has already sent a gift to this friend
            string checkQuery = @"
            SELECT userIDfrom, userIDto, giftFromUser, giftToUser 
            FROM FriendsList 
            WHERE (userIDfrom = @currentUserID AND userIDto = @friendID)
               OR (userIDfrom = @friendID AND userIDto = @currentUserID)";

            MySqlCommand checkCmd = new MySqlCommand(checkQuery, con);
            checkCmd.Parameters.AddWithValue("@currentUserID", userID);
            checkCmd.Parameters.AddWithValue("@friendID", friendID);

            bool canSendGift = true;
            string userIDFrom = "";
            string userIDTo = "";

            using (MySqlDataReader reader = checkCmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    userIDFrom = reader["userIDfrom"].ToString();
                    userIDTo = reader["userIDto"].ToString();

                    // Get the string values and convert to boolean logic
                    string giftFromUserStr = reader["giftFromUser"].ToString();
                    string giftToUserStr = reader["giftToUser"].ToString();

                    // Determine which column to check based on who initiated the friendship
                    if (userIDFrom == userID)
                    {
                        // Current user is the initiator, check giftFromUser
                        // Treat "true", "1", "yes" as true, everything else as false
                        canSendGift = !(giftFromUserStr.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                                       giftFromUserStr == "1" ||
                                       giftFromUserStr.Equals("yes", StringComparison.OrdinalIgnoreCase));
                    }
                    else
                    {
                        // Current user is the receiver, check giftToUser
                        canSendGift = !(giftToUserStr.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                                       giftToUserStr == "1" ||
                                       giftToUserStr.Equals("yes", StringComparison.OrdinalIgnoreCase));
                    }
                }
            }

            if (!canSendGift)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "giftAlreadySent",
                    "alert('You have already sent a gift to this friend. You can only send one gift per friend.');", true);
                return;
            }

            // Insert gift transaction with amount 10
            string insertGiftQuery = @"
            INSERT INTO GiftTransactions (senderID, receiverID, giftAmount, sentDate)
            VALUES (@senderID, @receiverID, 10, NOW())";

            MySqlCommand insertCmd = new MySqlCommand(insertGiftQuery, con);
            insertCmd.Parameters.AddWithValue("@senderID", userID);
            insertCmd.Parameters.AddWithValue("@receiverID", friendID);
            insertCmd.ExecuteNonQuery();

            // Update receiver's coin count (add 10 coins)
            string updateCoinsQuery = @"
            UPDATE Users 
            SET userCoinCount = userCoinCount + 10 
            WHERE userID = @receiverID";

            MySqlCommand updateCoinsCmd = new MySqlCommand(updateCoinsQuery, con);
            updateCoinsCmd.Parameters.AddWithValue("@receiverID", friendID);
            updateCoinsCmd.ExecuteNonQuery();

            // Update the gift sent status in FriendsList (set to "true" string)
            string updateFriendQuery = @"
            UPDATE FriendsList 
            SET 
                giftFromUser = CASE 
                    WHEN userIDfrom = @currentUserID THEN 'true' 
                    ELSE giftFromUser 
                END,
                giftToUser = CASE 
                    WHEN userIDto = @currentUserID THEN 'true' 
                    ELSE giftToUser 
                END
            WHERE (userIDfrom = @currentUserID AND userIDto = @friendID)
               OR (userIDfrom = @friendID AND userIDto = @currentUserID)";

            MySqlCommand updateFriendCmd = new MySqlCommand(updateFriendQuery, con);
            updateFriendCmd.Parameters.AddWithValue("@currentUserID", userID);
            updateFriendCmd.Parameters.AddWithValue("@friendID", friendID);
            updateFriendCmd.ExecuteNonQuery();
        }

       

        // Refresh the GridView to update button states
        LoadFriends();

        // Refresh the user's coin display
        GetUserStats(cs, userID);
    }
    private void ResetGiftStatus()
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        using (MySqlConnection con = new MySqlConnection(cs))
        {
            // Reset both gift flags to false
            string query = @"
            UPDATE FriendsList 
            SET giftFromUser = false, 
                giftToUser = false 
            WHERE requestStatus = 'Accepted'";

            MySqlCommand cmd = new MySqlCommand(query, con);
            con.Open();
            cmd.ExecuteNonQuery();
        }
    }
    protected void btnCancelDeleteFriend_Click(object sender, EventArgs e)
    {
        pnlDeleteFriend.Visible = false;
        ScriptManager.RegisterStartupScript(this, this.GetType(), "hideDeletePopup",
            "document.getElementById('popup-blue-box').style.display = 'none';", true);
    }


    protected void btnConfirmDeleteFriend_Click(object sender, EventArgs e)
    {
        string friendID = hiddenFriendToDelete.Value;
        string userID = Session["userID"] as string;

        if (!string.IsNullOrEmpty(friendID) && !string.IsNullOrEmpty(userID))
        {
            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (MySqlConnection con = new MySqlConnection(cs))
            {
                con.Open();

                string deleteFriendQuery = @"
            DELETE FROM friendslist 
            WHERE (userIDfrom = @userID AND userIDto = @friendID)
               OR (userIDfrom = @friendID AND userIDto = @userID)";

                using (MySqlCommand cmd = new MySqlCommand(deleteFriendQuery, con))
                {
                    cmd.Parameters.AddWithValue("@userID", userID);
                    cmd.Parameters.AddWithValue("@friendID", friendID);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        
                        LoadFriends();
                    }
                }
            }

            // Update the UpdatePanel
            updFriendRequests.Update();
        }

        // Hide the popup
        pnlDeleteFriend.Visible = false;
        ScriptManager.RegisterStartupScript(this, this.GetType(), "hideDeletePopup",
            "document.getElementById('popup-blue-box').style.display = 'none';", true);
    }
    // end: friends code

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

    private void ShowNextInvite(bool userInitiated = false)
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

            // Only show popup if user clicked the bell
            if (userInitiated)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showPopup", "showNotificationPopup();", true);
            }
        }
        else
        {
            imgNotificationRinging.Visible = false;
            imgNotificationNormal.Visible = true;
            notificationBadge.Visible = false;

            // Only show "no notifications" popup if user clicked the bell
            if (userInitiated)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showPopupNone", "showNotificationPopup(false);", true);
            }
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
        Response.Redirect("C600_View-friend-list.aspx");
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
        Response.Redirect("C600_View-friend-list.aspx");
    }

    protected void btnOkayDeclined_Click(object sender, EventArgs e)
    {
        Response.Redirect("C600_View-friend-list.aspx");
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
public class FriendRequest
{
    public int FriendshipID { get; set; }
    public int RequesterID { get; set; }
    public string RequesterName { get; set; }
}