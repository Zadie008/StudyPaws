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
                    Session["userID"] = userID; // Ensure userID is in session
                    int userXP = GetUserXP(cs, userID);
                    LoadFriends();
                    Tuple<int, int, int> levelInfo = GetLevelInformation(cs, userID);
                    GetUserStats(cs, userID);
                    GetUserProfileIcon(cs, userID);
                    LoadPendingInvitesFromDB();
                    LoadUpcomingSessions();

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
        WHERE f.userIDto = @userID AND f.requestStatus = 'pending'";

            MySqlCommand cmd = new MySqlCommand(query, con);

            string userID = Session["userID"] != null ? Session["userID"].ToString() : null;
            if (string.IsNullOrEmpty(userID))
            {
                return dt;
            }

            cmd.Parameters.AddWithValue("@userID", userID);

            try
            {
                con.Open();
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
                System.Diagnostics.Debug.WriteLine("Loaded " + dt.Rows.Count + " pending friend requests");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading friend requests: " + ex.Message);
            }
        }
        return dt;
    }
    private DataTable LoadPendingGifts()
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        DataTable dt = new DataTable();

        // FIX: C# 5 compatible null checking
        string currentUserID = Session["userID"] != null ? Session["userID"].ToString() : null;
        if (string.IsNullOrEmpty(currentUserID))
        {
            System.Diagnostics.Debug.WriteLine("UserID is null or empty in session for gifts");
            return dt;
        }

        using (MySqlConnection con = new MySqlConnection(cs))
        {
            string query = @"
        SELECT 
            CASE 
                WHEN f.userIDfrom = @currentUserID THEN f.userIDto
                ELSE f.userIDfrom
            END as friendID,
            u.username,
            f.lastGiftSender
        FROM FriendsList f
        JOIN Users u ON (f.userIDfrom = u.userID OR f.userIDto = u.userID) 
            AND u.userID != @currentUserID
        WHERE (f.userIDfrom = @currentUserID OR f.userIDto = @currentUserID)
            AND f.requestStatus = 'Accepted'
            AND f.giftAvailable = 1
            AND f.lastGiftSender != @currentUserID";

            MySqlCommand cmd = new MySqlCommand(query, con);
            cmd.Parameters.AddWithValue("@currentUserID", currentUserID);

            try
            {
                con.Open();
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
                System.Diagnostics.Debug.WriteLine("Loaded " + dt.Rows.Count + " pending gifts for user: " + currentUserID);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading gifts: " + ex.Message);
            }
        }
        return dt;
    }
    protected void btnMail_Click(object sender, EventArgs e)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("Mail button clicked - starting processing");

            // Check if userID exists in session
            if (Session["userID"] == null)
            {
                System.Diagnostics.Debug.WriteLine("UserID is null in session!");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "noUserID",
                    "alert('Please log in again.');", true);
                return;
            }

            // Clear all popups first
            pnlFriendRequests.Visible = false;
            pnlGiftNotifications.Visible = false;

            DataTable pendingRequests = LoadPendingFriendRequests();
            DataTable pendingGifts = LoadPendingGifts();

            ViewState["PendingFriendRequests"] = pendingRequests;
            ViewState["PendingGifts"] = pendingGifts;

            System.Diagnostics.Debug.WriteLine("Requests: " + pendingRequests.Rows.Count + ", Gifts: " + pendingGifts.Rows.Count);

            // Show friend requests first, then gifts
            if (pendingRequests.Rows.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine("Showing friend request");
                ViewState["CurrentRequestIndex"] = 0;
                ShowFriendRequest(0);
            }
            else if (pendingGifts.Rows.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine("Showing gift notification");
                ViewState["CurrentGiftIndex"] = 0;
                ShowGiftNotification(0);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No notifications found - showing no notifications panel");
                // Show no notifications popup
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showNoNotifications",
                    "showNoNotificationsPopup();", true);
            }

            updFriendRequests.Update();
            System.Diagnostics.Debug.WriteLine("Mail button processing completed");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Exception in btnMail_Click: " + ex.ToString());
            ScriptManager.RegisterStartupScript(this, this.GetType(), "error",
                "alert('Error loading notifications: " + ex.Message.Replace("'", "\\'") + "');", true);
        }
    }

    private void ShowGiftNotification(int index)
    {
        DataTable pendingGifts = ViewState["PendingGifts"] as DataTable;
        string userID = Session["userID"].ToString();
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        if (pendingGifts != null && index >= 0 && index < pendingGifts.Rows.Count)
        {
            DataRow row = pendingGifts.Rows[index];

            hiddenGiftFriendID.Value = row["friendID"].ToString();
            lblGiftMessage.Text = "You received a gift of 10 coins from <span style='font-weight:bold;'>" + row["username"].ToString() + "</span>!";

            pnlGiftNotifications.Visible = true;
            ViewState["CurrentGiftIndex"] = index;

            // Update the panel first
            updFriendRequests.Update();

            // Then show the popup
            ScriptManager.RegisterStartupScript(this, this.GetType(), "showGiftPopup",
                "showGiftPopup();", true);
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("No gift found at index: " + index);
        }
        GetUserStats(cs, userID);
    }

    protected void btnCollectGift_Click(object sender, EventArgs e)
    {
        string friendID = hiddenGiftFriendID.Value;
        string userID = Session["userID"].ToString();
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        if (!string.IsNullOrEmpty(friendID))
        {
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

                // Reset gift status and clear last sender - FIXED COLUMN NAME
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
            LoadFriends(); // Refresh the friend list to update buttons
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
            }
        }
        else
        {
            pnlGiftNotifications.Visible = false;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "hideGiftPopup",
                "document.getElementById('popupGiftNotifications').style.display = 'none';", true);
        }
        updFriendRequests.Update();
    }

    protected void btnCancelDeleteFriend_Click(object sender, EventArgs e)
    {
        Response.Redirect("C600_View-friend-list.aspx");
    }

    private void ShowFriendRequest(int index)
    {
        DataTable pendingRequests = ViewState["PendingFriendRequests"] as DataTable;

        if (pendingRequests != null && index >= 0 && index < pendingRequests.Rows.Count)
        {
            DataRow row = pendingRequests.Rows[index];

            hiddenFriendRequestID.Value = row["friendshipID"].ToString();
            hiddenRequesterID.Value = row["userIDfrom"].ToString();

            string username = row["username"].ToString();
            lblFriendRequestMessage.Text = "You have a friend request from <span style='font-weight:bold;'>" + username + "</span>" + "!";

            pnlFriendRequests.Visible = true;
            ViewState["CurrentRequestIndex"] = index;

            // Update the panel first
            updFriendRequests.Update();

            // Then show the popup
            ScriptManager.RegisterStartupScript(this, this.GetType(), "showFriendRequestPopup",
                "showFriendRequestPopup();", true);
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("No friend request found at index: " + index);
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
                string query = "UPDATE FriendsList SET requestStatus = 'Accepted' WHERE friendshipID = @friendshipID";
                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.Parameters.AddWithValue("@friendshipID", friendshipID);

                try
                {
                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    System.Diagnostics.Debug.WriteLine("Friend request accepted. Rows affected: " + rowsAffected);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error accepting friend request: " + ex.Message);
                }
            }

            LoadFriends();
            ShowNextRequestOrClose();
        }
        updFriendRequests.Update();
    }

    protected void btnDeclineFriendRequest_Click(object sender, EventArgs e)
    {
        string friendshipID = hiddenFriendRequestID.Value;

        if (!string.IsNullOrEmpty(friendshipID))
        {
            string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (MySqlConnection con = new MySqlConnection(cs))
            {
                string deleteQuery = "DELETE FROM FriendsList WHERE friendshipID = @friendshipID";
                MySqlCommand cmd = new MySqlCommand(deleteQuery, con);
                cmd.Parameters.AddWithValue("@friendshipID", friendshipID);

                try
                {
                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    System.Diagnostics.Debug.WriteLine("Friend request declined. Rows affected: " + rowsAffected);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error declining friend request: " + ex.Message);
                }
            }

            LoadFriends();
            ShowNextRequestOrClose();
        }
        updFriendRequests.Update();
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
                    "hideAllPopups();", true);
            }
        }
        else
        {
            pnlFriendRequests.Visible = false;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "hideFriendRequestPopup",
                "hideAllPopups();", true);
        }
        updFriendRequests.Update();
    }
    protected void btnSendGift_Click(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        string friendID = btn.CommandArgument.ToString();
        string userID = Session["userID"] != null ? Session["userID"].ToString() : null;
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        System.Diagnostics.Debug.WriteLine("Sending gift from " + userID + " to " + friendID);

        if (string.IsNullOrEmpty(userID))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "notLoggedIn",
                "alert('Please log in to send gifts.');", true);
            return;
        }

        using (MySqlConnection con = new MySqlConnection(cs))
        {
            con.Open();

            // First check the current state
            string checkQuery = @"
        SELECT giftAvailable, lastGiftSender 
        FROM FriendsList
        WHERE ((userIDfrom = @currentUserID AND userIDto = @friendID)
            OR (userIDfrom = @friendID AND userIDto = @currentUserID))";

            MySqlCommand checkCmd = new MySqlCommand(checkQuery, con);
            checkCmd.Parameters.AddWithValue("@currentUserID", userID);
            checkCmd.Parameters.AddWithValue("@friendID", friendID);

            using (MySqlDataReader reader = checkCmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    bool giftAvailable = Convert.ToBoolean(reader["giftAvailable"]);
                    string lastSender = reader["lastGiftSender"] != DBNull.Value ? reader["lastGiftSender"].ToString() : null;

                    System.Diagnostics.Debug.WriteLine("Current state - GiftAvailable: " + giftAvailable + ", LastSender: " + lastSender);

                    // If gift is already available and current user was the last sender, prevent sending
                    if (giftAvailable && lastSender == userID)
                    {
                        System.Diagnostics.Debug.WriteLine("User already sent the last gift to " + friendID);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "giftAlreadySent",
                            "alert('You already sent the last gift. Wait for your friend to collect it.');", true);
                        return;
                    }
                }
            }

            // Update friendslist to mark gift as available and set last sender
            string updateFriendQuery = @"
        UPDATE FriendsList 
        SET giftAvailable = 1, lastGiftSender = @currentUserID
        WHERE (userIDfrom = @currentUserID AND userIDto = @friendID)
            OR (userIDfrom = @friendID AND userIDto = @currentUserID)";

            MySqlCommand updateFriendCmd = new MySqlCommand(updateFriendQuery, con);
            updateFriendCmd.Parameters.AddWithValue("@currentUserID", userID);
            updateFriendCmd.Parameters.AddWithValue("@friendID", friendID);

            int rowsAffected = updateFriendCmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                System.Diagnostics.Debug.WriteLine("Gift sent successfully to " + friendID);
                // Show gift sent confirmation popup instead of alert
                ScriptManager.RegisterStartupScript(this, this.GetType(), "giftSent",
                    "showGiftSentPopup();", true);

                // Reload friends to update button states
                LoadFriends();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Error sending gift to " + friendID);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "giftError",
                    "alert('Error sending gift. Please try again.');", true);
            }
        }
    }
    private void LoadFriends()
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        string currentUserID = Session["userID"] != null ? Session["userID"].ToString() : null;

        if (string.IsNullOrEmpty(currentUserID))
        {
            System.Diagnostics.Debug.WriteLine("UserID is null, cannot load friends");
            return;
        }

        using (MySqlConnection con = new MySqlConnection(cs))
        {
            string command = @"
        SELECT  
            u.userID, 
            u.username, 
            u.iconNum, 
            f.giftAvailable,
            f.lastGiftSender,
            CASE 
                WHEN f.userIDfrom = @currentUserID THEN f.userIDto
                ELSE f.userIDfrom
            END as friendUserID
        FROM Users u 
        JOIN friendslist f ON (u.userID = f.userIDto AND f.userIDfrom = @currentUserID) OR (u.userID = f.userIDfrom AND f.userIDto = @currentUserID) 
        WHERE f.requestStatus = 'Accepted'";

            MySqlCommand cmd = new MySqlCommand(command, con);
            cmd.Parameters.AddWithValue("@currentUserID", currentUserID);

            con.Open();
            DataTable dt = new DataTable();
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            // Debug: Check what data is loaded
            foreach (DataRow row in dt.Rows)
            {
                System.Diagnostics.Debug.WriteLine("Friend: " + row["friendUserID"] +
                    ", GiftAvailable: " + row["giftAvailable"] +
                    ", LastSender: " + (row["lastGiftSender"] != DBNull.Value ? row["lastGiftSender"].ToString() : "null"));
            }

            GridView1.DataSource = dt;
            GridView1.DataBind();
        }
    }
    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Button btnSendGift = (Button)e.Row.FindControl("btnSendGift");

            if (btnSendGift != null)
            {
                DataRowView rowView = (DataRowView)e.Row.DataItem;
                bool giftAvailable = Convert.ToBoolean(rowView["giftAvailable"]);
                string friendUserID = rowView["friendUserID"].ToString();
                string currentUserID = Session["userID"] != null ? Session["userID"].ToString() : null;

                if (string.IsNullOrEmpty(currentUserID))
                {
                    btnSendGift.Enabled = false;
                    btnSendGift.CssClass = "btn btn-secondary";
                    btnSendGift.ToolTip = "Not logged in";
                    return;
                }

                // DEBUG: Check the giftAvailable value
                System.Diagnostics.Debug.WriteLine("Friend: " + friendUserID + ", GiftAvailable: " + giftAvailable + ", CurrentUser: " + currentUserID);

                if (!giftAvailable) // giftAvailable = 0
                {
                    // No gift available - ENABLE the button with primary style
                    btnSendGift.CssClass = "btn btn-primary";
                    btnSendGift.Enabled = true;
                    btnSendGift.ToolTip = "Send gift";

                    System.Diagnostics.Debug.WriteLine("Button ENABLED - No gift available, can send to friend: " + friendUserID);
                }
                else // giftAvailable = 1
                {
                    string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                    string lastSender = GetLastGiftSender(cs, currentUserID, friendUserID);

                    System.Diagnostics.Debug.WriteLine("Last sender for friend " + friendUserID + ": " + (lastSender ?? "null"));

                    if (lastSender == currentUserID)
                    {
                        // User already sent gift - disable button
                        btnSendGift.CssClass = "btn btn-secondary";
                        btnSendGift.Enabled = false;
                        btnSendGift.ToolTip = "You already sent a gift";
                        System.Diagnostics.Debug.WriteLine("Button DISABLED - User already sent gift to " + friendUserID);
                    }
                    else
                    {
                        // Gift available but friend sent it - enable button to send back
                        btnSendGift.CssClass = "btn btn-primary";
                        btnSendGift.Enabled = true;
                        btnSendGift.ToolTip = "Send gift back";
                        System.Diagnostics.Debug.WriteLine("Button ENABLED - Friend sent gift, can send back to " + friendUserID);
                    }
                }
            }
        }
    }
    private string GetLastGiftSender(string connectionString, string userID1, string userID2)
    {
        string query = @"
    SELECT lastGiftSender
    FROM FriendsList 
    WHERE (userIDfrom = @userID1 AND userIDto = @userID2)
       OR (userIDfrom = @userID2 AND userIDto = @userID1)";

        using (MySqlConnection con = new MySqlConnection(connectionString))
        using (MySqlCommand cmd = new MySqlCommand(query, con))
        {
            cmd.Parameters.AddWithValue("@userID1", userID1);
            cmd.Parameters.AddWithValue("@userID2", userID2);

            con.Open();
            object result = cmd.ExecuteScalar();
            return result != null && result != DBNull.Value ? result.ToString() : null;
        }
    }
    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "DeleteFriend")
        {
            string userId = e.CommandArgument.ToString();
            // Show confirmation popup
            hiddenFriendToDelete.Value = userId;
            pnlDeleteFriend.Visible = true;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "showDeletePopup",
                "document.getElementById('popup-blue-box').style.display = 'block';", true);
        }
    }


    protected void btnCancelFriend_Click(object sender, EventArgs e)
    {
        pnlDeleteFriend.Visible = false;
    }

    protected void btnConfirmDeleteFriend_Click(object sender, EventArgs e)
    {
        string friendID = ViewState["FriendToDelete"] as string;
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
                    cmd.ExecuteNonQuery();
                }
            }
            LoadFriends();
        }
        pnlDeleteFriend.Visible = false;
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

    public string GetProfileImagePath(int iconNum)
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
    public string GetProfileImageUrl(object iconNum)
    {
        if (iconNum == null || iconNum == DBNull.Value)
            return "Images/ProfilePictures/CatPfp.png";

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
    public string GetCircleColorClass(int iconNum)
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