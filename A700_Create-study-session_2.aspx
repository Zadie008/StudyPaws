<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="A700_Create-study-session_2.aspx.cs" Inherits="Default2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" Runat="Server">
    Create study session
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" Runat="Server">
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

<asp:Content ID="Content3" ContentPlaceHolderID="navContent" Runat="Server">

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="mainContentPlaceHolder" Runat="Server">
    <div class="createTimer2MainContent">
        <div class="titleTagSection">
            <div class="leftSection">
            </div>
            <div class="middleSection">
                <table>
                    <tr>
                        <td><asp:Label ID="lblTitle" class="label" runat="server" Text="Title"></asp:Label></td>
                        <td><asp:TextBox ID="txtTitle" class="textbox" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td><asp:Label ID="lblTag" class="label" runat="server" Text="Tag"></asp:Label></td>
                        <td>
                            <asp:DropDownList ID="dropdownTag" ClientIDMode="Static" class="dropDownList" runat="server" BackColor="#446791">
                            <asp:ListItem></asp:ListItem>
                            <asp:ListItem>Studying</asp:ListItem>
                            <asp:ListItem>Assignments</asp:ListItem>
                            <asp:ListItem>Reading</asp:ListItem>
                            <asp:ListItem>Break</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="rightSection">
                <table>
                    <tr>
                        <td><asp:RequiredFieldValidator ID="errorTitle" class="validationError" runat="server" ErrorMessage="Please enter a Title for your Study session" EnableClientScript="true" ControlToValidate="txtTitle"></asp:RequiredFieldValidator></td>
                    </tr>
                    <tr>
                        <td><asp:RequiredFieldValidator ID="errorDropDown" class="validationError" runat="server" ErrorMessage="Please select a Tag for your Study session" EnableClientScript="true" ControlToValidate="dropdownTag"></asp:RequiredFieldValidator></td>
                    </tr>
                </table>
            </div>
        </div>
        <div class="buttonSection">
            <div class="leftSection">
            </div>
            <div class="middleSection">
                <asp:Button ID="btnBack" class="button" runat="server" Text="Back" OnClick="btnBack_Click" CausesValidation="False" />
                <asp:Button ID="btnContinue" class="button" runat="server" Text="Continue" OnClick="btnContinue_Click" />
            </div>
            <div class="rightSection">
                <asp:Button ID="btnViewPastTimers" class="button" runat="server" Text="View past sessions" Visible="False" /> <!--invisible but for correct spacing of other buttons-->
            </div>
        </div>

            <!--does not have notification-->
            <div id="popupNoNotifications" class="simple-popup" style="display: none;">
                <div class="popup-blue-box">
                    <p>You do not have any notifications at the moment!</p>
                    <img src="Images/Notification%20Sad%20Hamster.png" />
                    <br />
                    <div class="buttonSection">
                        <asp:Button ID="btnOkay" CssClass="popup-button" runat="server" Text="Okay!" OnClientClick="hideNotificationPopup(); return false;" />
                    </div>
                </div>
            </div>

            <!--has notifications-->
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
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">

</asp:Content>

