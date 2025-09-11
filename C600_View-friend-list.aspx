<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="C600_View-friend-list.aspx.cs" Inherits="Default2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" runat="Server">
    Friends
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" runat="Server">
    <div class="accountInfoDiv">
    <div class="profileDiv">
        <a href="C100-C500_Profile.aspx" class="profileIconLink">
            <div class="profileIcon">
                <div id="profileCircle" runat="server" ClientIDMode="Static"></div>
                <asp:Image ID="profilePet" runat="server" />
            </div>
        </a>
        <div class="profileDetails">
            <table>
                <tr>
                    <td>
                        <asp:Label ID="lblLevel" CssClass="accountInfoTableLabel" runat="server" Text="Level"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblLevelNumber" CssClass="accountInfoTableLabelRight" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div class="xpProgressContainer">
                            <asp:Label ID="lblXP" CssClass="accountInfoLabel" runat="server" Text="XP"></asp:Label>
                            <div class="progressBarBackground">
                                <asp:Panel ID="xpProgressBar" runat="server" CssClass="progressBarFill"></asp:Panel>
                            </div>
                            <asp:Label ID="lblXPPercentage" CssClass="accountInfoLabel xpPercentage" runat="server" Text="0%"></asp:Label>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div class="pawIcon">
                            <img class="circle" src="Icons/icons8-circle-white-96.png" width="50" />
                            <img class="paw" src="Icons/icons8-cat-footprint-filled-white-96.png" width="30" />
                        </div>
                    </td>
                    <td>
                        <asp:Label ID="lblPaws" CssClass="accountInfoTableLabelRight" runat="server" Text="---"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
    </div>

    <div class="rightInfoDiv">
        <div class="timeNotificationWrapper">
            <div class="notificationDetails">
                <asp:ImageButton ID="imgNotificationRinging" CssClass="notificationIcon" runat="server" ImageUrl="~/Icons/icons8-notification-bell-ringing-white-96.png" OnClientClick="showNotificationPopup(true); return false;" />
                <asp:ImageButton ID="imgNotificationNormal" CssClass="notificationIcon" runat="server" ImageUrl="~/Icons/icons8-notification-bell-white-96.png" OnClientClick="showNotificationPopup(false); return false;" />
                <div id="notificationBadge" runat="server" class="notificationBadge"></div>
            </div>

            <div class="timeDateDiv">
                <asp:Label ID="lblTime" CssClass="accountInfoLabel currentTime" runat="server" Text="09:52"></asp:Label>
                <div class="dateContainer">
                    <asp:Label ID="lblDay" CssClass="accountInfoLabel currentDate" runat="server" Text="Friday"></asp:Label>
                    <span class="dateSeparator">|</span>
                    <asp:Label ID="lblDate" CssClass="accountInfoLabel currentDate" runat="server" Text="18 April"></asp:Label>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="curved-header">
    <svg viewBox="0 0 700 150" xmlns="http://www.w3.org/2000/svg">
        <defs>
            <path id="curve" d="M50,120 Q350,20 650,120" />
        </defs>
        <text>
            <textPath href="#curve" startOffset="50%" text-anchor="middle">
                <a href="Default.aspx" class="curvedHeaderLink">
                    StudyP<tspan dx="0.7em">w</tspan>s
                </a>
            </textPath>
        </text>
    </svg>
    <a href="Default.aspx"><img class="curvedHeaderPaw" src="Icons/icons8-cat-footprint-filled-white-96.png" alt="paw" /></a>
    <h2><a href="Default.aspx">purrfectly productive</a></h2>
</div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="navContent" runat="Server">
    <!--navigation to copy and paste-->
    <div class="collapsedNav">
        <div class="navbar">
            <asp:Menu ID="MenuLeft" runat="server" Orientation="Vertical" CssClass="nav-left" StaticDisplayLevels="1" StaticMenuItemStyle-CssClass="menu-item">
                <Items>
                    <asp:MenuItem NavigateUrl="~\B1600_View-dashboard.aspx" Text="Dashboard" Value="Dashboard"></asp:MenuItem>
                    <asp:MenuItem NavigateUrl="~\B100_View-calendar.aspx" Text="Calendar" Value="Calendar"></asp:MenuItem>
                    <asp:MenuItem NavigateUrl="~\B900_View-to-do-list.aspx" Text="To-do List" Value="To-do List"></asp:MenuItem>
                    <asp:MenuItem NavigateUrl="~\A100_Create-timer.aspx" Text="Timer" Value="Timer"></asp:MenuItem>
                    <asp:MenuItem NavigateUrl="~\A700_Create-study-session.aspx" Text="Study Session" Value="Study Session"></asp:MenuItem>
                </Items>
            </asp:Menu>
            <asp:Menu ID="MenuRight" runat="server" Orientation="Vertical" CssClass="nav-right" StaticDisplayLevels="1" StaticMenuItemStyle-CssClass="menu-item">
                <Items>
                    <asp:MenuItem NavigateUrl="~\B1400_View-shop.aspx" Text="Pet Shop" Value="Pet Shop"></asp:MenuItem>
                    <asp:MenuItem NavigateUrl="~\A1800_View-pets.aspx" Text="Inventory" Value="Inventory"></asp:MenuItem>
                    <asp:MenuItem NavigateUrl="~\C600_View-friend-list.aspx" Text="Friends" Value="Friends"></asp:MenuItem>
                    <asp:MenuItem NavigateUrl="~\C1400_View-badges.aspx" Text="Badges" Value="Badges"></asp:MenuItem>
                    <asp:MenuItem NavigateUrl="~\About.aspx" Text="About" Value="About"></asp:MenuItem>
                </Items>
            </asp:Menu>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="mainContentPlaceHolder" runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <div id="viewFriendListMainContent">
        <asp:UpdatePanel ID="updFriendRequests" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:HiddenField ID="hiddenFriendToDelete" runat="server" />

                <!-- Header -->
                <div class="header-container">
                    <h2>Your friends</h2>
                    <asp:ImageButton ID="btnMail" runat="server"
                        ImageUrl="Icons/icons8-mail-white-96.png"
                        CssClass="mailIcon"
                        OnClick="btnMail_Click"
                        CausesValidation="false" />
                </div>


                <!-- Friend Request Notification Panel -->
                <asp:Panel ID="pnlFriendRequests" runat="server" Visible="false" CssClass="simple-popup" Style="display: none;">
                    <div class="popup-pink-box">
                        <asp:HiddenField ID="hiddenFriendRequestID" runat="server" />
                        <asp:HiddenField ID="hiddenRequesterID" runat="server" />
                        <asp:Label ID="lblFriendRequestMessage" runat="server" Text=""></asp:Label>
                        <img src="Images/Notification%20Happy.png" alt="Friend Request" />
                        <br />
                        <div class="buttonSection">
                            <asp:Button ID="btnAcceptFriendRequest" CssClass="popup-button-best-pink" runat="server"
                                Text="Accept!" OnClick="btnAcceptFriendRequest_Click" />
                            <asp:Button ID="btnDeclineFriendRequest" CssClass="popup-button" runat="server"
                                Text="Decline!" OnClick="btnDeclineFriendRequest_Click" />
                        </div>
                    </div>
                </asp:Panel>

                <!-- Gift Notification Panel -->
                <asp:Panel ID="pnlGiftNotifications" runat="server" Visible="false" CssClass="simple-popup" Style="display: none;">
                    <div class="popup-pink-box">
                        <asp:HiddenField ID="hiddenGiftFriendID" runat="server" />
                        <asp:Label ID="lblGiftMessage" runat="server" Text=""></asp:Label>
                        <img src="Images/Notification%20Happy.png" alt="Gift Notification" />
                        <br />
                        <div class="buttonSection">
                            <asp:Button ID="btnCollectGift" CssClass="popup-button-best-pink" runat="server"
                                Text="Collect!" OnClick="btnCollectGift_Click" />
                            <asp:Button ID="btnLaterGift" CssClass="popup-button" runat="server"
                                Text="Later" OnClick="btnLaterGift_Click" />
                        </div>
                    </div>
                </asp:Panel>
                <!-- No Notifications Panel -->
                <div id="popupNoNotifications" class="simple-popup" style="display: none;">
                    <div class="popup-blue-box">
                        <p>You do not have any notifications at the moment!</p>
                        <img src="Images/Notification%20Sad%20Hamster.png" alt="No notifications" />
                        <br />
                        <div class="buttonSection">
                            <asp:Button ID="Button1" CssClass="popup-button" runat="server" Text="Okay!" OnClientClick="hideAllPopups(); return false;" />
                        </div>
                    </div>
                </div>

                <!-- Gift Sent Confirmation Panel -->
                <div id="popupGiftSent" class="simple-popup" style="display: none;">
                    <div class="popup-pink-box">
                        <p>Gift sent successfully!</p>
                        <img src="Images/Notification%20Happy.png" alt="Gift sent" />
                        <br />
                        <div class="buttonSection">
                            <asp:Button ID="btnGiftSentOkay" CssClass="popup-button-best-pink" runat="server" Text="Okay!" OnClientClick="hideAllPopups(); return false;" />
                        </div>
                    </div>
                </div>
                <!-- Delete Confirmation Panel -->
                <asp:Panel ID="pnlDeleteFriend" runat="server" Visible="false">
                    <div id="popup-blue-box" class="simple-popup">
                        <div class="popup-blue-box">
                            <p>Are you sure you want to delete your friend? You will no longer be able to view them</p>
                            <img src="Images/Notification%20Sad%20Hamster.png" />
                            <br />
                            <div class="buttonSection">
                                <asp:Button ID="btnConfirmDeleteFriend" CssClass="popup-button"
                                    runat="server" Text="Yes, I am sure"
                                    OnClick="btnConfirmDeleteFriend_Click" />
                                <asp:Button ID="btnCancelDeleteFriend" CssClass="popup-button-best-blue"
                                    runat="server" Text="No, not sure!"
                                    OnClick="btnCancelDeleteFriend_Click" />
                            </div>
                        </div>
                    </div>
                </asp:Panel>

                <!-- Friends GridView -->
                <div class="backgroundColorContainer">
                    <div class="leftSection"></div>
                    <div class="middleSection">
                        <div class="friendsScrollableTableContainer">
                            <asp:GridView ID="GridView1" runat="server" GridLines="None" CssClass="searchFriendsTable"
                                AutoGenerateColumns="False" OnRowDataBound="GridView1_RowDataBound"
                                OnRowCommand="GridView1_RowCommand">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <div class="friendRow">
                                                <a href='<%# "ViewFriendProfile.aspx?friendID=" + Eval("userID") %>' class="friendProfileIconLink">
                                                    <div class="friendProfileIcon">
                                                        <div class='<%# "friendProfileCircle " + GetCircleColorClass(Convert.ToInt32(Eval("iconNum"))) %>'></div>
                                                        <img class="friendProfileImage" src='<%# GetProfileImageUrl(Eval("iconNum")) %>' alt="Profile" />
                                                    </div>
                                                </a>
                                                <span class="friendUsername"><%# Eval("username") %></span>

                                                <asp:Button ID="btnSendGift" CssClass="profilebutton" runat="server" Text="Send Gift"
                                                    OnClick="btnSendGift_Click" CommandArgument='<%# Eval("userID") %>' />
                                                <asp:ImageButton ID="btnDeleteFriend" runat="server"
                                                    ImageUrl="Icons/icons8-delete-white-96.png" CssClass="imageButton"
                                                    CommandName="DeleteFriend" CommandArgument='<%# Eval("userID") %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                    <div class="rightSection"></div>
                </div>

                <!-- Search Friends Button -->
                <div class="buttonSection">
                    <asp:Button ID="btnSearch" CssClass="button" runat="server" Text="Search friends" OnClick="btnSearchFriends_Click" />
                </div>
            </ContentTemplate>

            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnMail" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="btnAcceptFriendRequest" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="btnDeclineFriendRequest" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="btnCollectGift" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="btnLaterGift" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="btnConfirmDeleteFriend" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="btnCancelDeleteFriend" EventName="Click" />
                <%--<asp:AsyncPostBackTrigger ControlID="btnSendGift" EventName="Click" />--%>
                <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
    </div>

  

    <div id="popupHasNotifications" class="simple-popup" style="display: none;">
        <div class="popup-pink-box">
            <asp:HiddenField ID="hiddenSessionID" runat="server" />
            <asp:Literal ID="litNotificationText" runat="server" />
            <img src="Images/Notification%20Happy.png" />
            <br />
            <div class="buttonSection">
                <asp:Button ID="btnYes" CssClass="popup-button-best-pink" runat="server" Text="Accept!" OnClick="btnYes_Click" />
                <asp:Button ID="btnNo" CssClass="popup-button" runat="server" Text="Decline!" OnClick="btnNo_Click" />
            </div>
        </div>
    </div>

    <div id="popupCalendar" class="simple-popup" style="display: none;">
        <div class="popup-pink-box">
            <asp:HiddenField ID="hiddenShowCalendar" runat="server" />
            <p>Study Session has been added to your calendar!</p>
            <img src="Images/Notification%20Happy.png" />
            <br />
            <div class="buttonSection">
                <asp:Button ID="btnCalendar" CssClass="popup-button-best-pink" runat="server" Text="Calendar, GO!" OnClick="btnCalendar_Click" />
                <asp:Button ID="btnOk" CssClass="popup-button" runat="server" Text="Okay, thanks!" OnClick="btnOk_Click" />
            </div>
        </div>
    </div>

    <div id="popupConfirmDecline" class="simple-popup" style="display: none;">
        <div class="popup-blue-box">
            <asp:HiddenField ID="hiddenShowConfirmation" runat="server" />
            <p>Are you sure you want to decline the Study Session invitation?</p>
            <img src="Images/Notification%20Sad%20Hamster.png" />
            <br />
            <div class="buttonSection">
                <asp:Button ID="btnSure" CssClass="popup-button" runat="server" Text="Yes, I'm sure!" OnClick="btnSure_Click" />
                <asp:Button ID="btnNotSure" CssClass="popup-button-best-blue" runat="server" Text="No, not sure!" OnClick="btnNotSure_Click" />
            </div>
        </div>
    </div>

    <div id="popupIsDeclined" class="simple-popup" style="display: none;">
        <div class="popup-blue-box">
            <asp:HiddenField ID="hiddenShowDeclineConfirmed" runat="server" />
            <p>Study Session has been declined!</p>
            <img src="Images/Notification%20Sad%20Hamster.png" />
            <br />
            <div class="buttonSection">
                <asp:Button ID="btnOkayDeclined" CssClass="popup-button" runat="server" Text="Okay!" OnClick="btnOkayDeclined_Click" />
            </div>
        </div>
    </div>

    <div id="popup" class="simple-popup" style="display: none;">
        <div class="popup-pink-box">
            <asp:HiddenField ID="hiddenJoinSessionID" runat="server" />
            <p>Study Session has started!</p>
            <img src="Images/Notification%20Happy.png" />
            <br />
            <div class="buttonSection">
                <asp:Button ID="btnJoin" CssClass="popup-button" runat="server" Text="Join!" OnClick="btnJoin_Click" />
            </div>
        </div>
    </div>

    <script type="text/javascript">
        // Show/hide functions for popup panels
        function showFriendRequestPopup() {
            hideAllPopups();
            var panel = document.getElementById('<%= pnlFriendRequests.ClientID %>');
            if (panel) {
                panel.style.display = 'flex';
                console.log('Friend request popup shown');
            }
        }

        function showGiftPopup() {
            hideAllPopups();
            var panel = document.getElementById('<%= pnlGiftNotifications.ClientID %>');
            if (panel) {
                panel.style.display = 'flex';
                console.log('Gift notification popup shown');
            }
        }

        function showNoNotificationsPopup() {
            hideAllPopups();
            var panel = document.getElementById('popupNoNotifications');
            if (panel) {
                panel.style.display = 'flex';
                console.log('No notifications popup shown');
            }
        }

        function showGiftSentPopup() {
            hideAllPopups();
            var panel = document.getElementById('popupGiftSent');
            if (panel) {
                panel.style.display = 'flex';
                console.log('Gift sent confirmation popup shown');
            }
        }

        function hideAllPopups() {
            // Hide all popup panels
            var popups = document.querySelectorAll('.simple-popup');
            popups.forEach(function (popup) {
                popup.style.display = 'none';
            });

            // Also hide ASP.NET panels
            var aspNetPanels = document.querySelectorAll('[id*="pnlFriendRequests"], [id*="pnlGiftNotifications"]');
            aspNetPanels.forEach(function (panel) {
                panel.style.display = 'none';
            });
        }

        // Handle escape key to close popups
        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') {
                hideAllPopups();
            }
        });

        // Handle click outside to close popups
        document.addEventListener('click', function (e) {
            var visiblePopups = document.querySelectorAll('.simple-popup[style*="display: flex"]');
            visiblePopups.forEach(function (popup) {
                var content = popup.querySelector('.popup-pink-box') || popup.querySelector('.popup-blue-box');
                if (content && !content.contains(e.target)) {
                    hideAllPopups();
                }
            });
        });

        // Prevent click events from bubbling up from popup content
        document.querySelectorAll('.popup-pink-box, .popup-blue-box').forEach(function (content) {
            content.addEventListener('click', function (e) {
                e.stopPropagation();
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" runat="Server">
</asp:Content>
