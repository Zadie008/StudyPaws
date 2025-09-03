<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="ViewFriendProfile.aspx.cs" Inherits="Default2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" runat="Server">
    Friends
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" runat="Server">
    <div class="accountInfoDiv">
        <div class="profileDiv">
            <a href="C100-C500_Profile.aspx" class="profileIconLink">
                <div class="profileIcon">
                    <div id="profileCircle" runat="server" clientidmode="Static"></div>
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
                    <asp:Label ID="lblTime" CssClass="accountInfoLabel currentTime" runat="server" Text="--:--"></asp:Label>
                    <div class="dateContainer">
                        <asp:Label ID="lblDay" CssClass="accountInfoLabel currentDate" runat="server" Text="Someday"></asp:Label>
                        <span class="dateSeparator">|</span>
                        <asp:Label ID="lblDate" CssClass="accountInfoLabel currentDate" runat="server" Text="Day Month"></asp:Label>
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
                    StudyP
<tspan dx="0.7em">w</tspan>s
                </a>
                </textPath>
            </text>
        </svg>
        <a href="Default.aspx">
            <img class="curvedHeaderPaw" src="Icons/icons8-cat-footprint-filled-white-96.png" alt="paw" /></a>
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

    <div class="friend-profile-page-container">
        <h2>Your friends</h2>

        <div class="friend-profile-content-container">
            <div class="friend-profile-details-card">
                <div class="friend-profile-avatar-container">
                    <div class="friend-profile-large-avatar">
                        <asp:Image ID="imgFriend" runat="server" CssClass="friendProfileImage" />
                    </div>
                </div>
                
                <h2 class="friend-profile-display-name">
                    <asp:Label ID="lblFriendName" runat="server" Text="Ica" />
                </h2>
                
                <div class="friend-profile-level-badge">
                    <asp:Label ID="lblFriendLevel" runat="server" Text="Level 12" />
                </div>
                <%--<div class="friend-profile-buttons-container">
                    <asp:Button ID="btnViewBadges" runat="server" Text="View badges" 
                        CssClass="friend-profile-badges-button" OnClick="btnViewBadges_Click" />--%>
            </div>
            <div class="rightSection"></div>
        </div>
        </div>
   

   
    <%--<div class="buttonSection">
        <asp:Button ID="btnViewFriendBadges" CssClass="button" runat="server"
            Text="Back" OnClick="btnViewFriendBadges_Click" />
    </div>--%>

    <script>
        document.addEventListener('DOMContentLoaded', function () {
            // If you really want client-side interactivity:
            const profile = document.querySelector('.friendProfileDetails');
            if (profile) {
                profile.addEventListener('click', function () {
                    const name = document.getElementById('<%= lblFriendName.ClientID %>').innerText;
                    alert(`Showing profile for ${name}`);
                });
            }

            const backButton = document.querySelector('.button');
            if (backButton) {
                backButton.addEventListener('click', function () {
                    alert('Going back to previous screen');
                });
            }
        });
    </script>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" runat="Server">
</asp:Content>


