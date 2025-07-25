<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="A100_Create-timer_3.aspx.cs" Inherits="Default2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" Runat="Server">
    Create timer
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" Runat="Server">
   <!--account info to copy and paste-->
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
                     <td><asp:Label ID="lblLevel" CssClass="accountInfoTableLabel" runat="server" Text="Level"></asp:Label></td>
                     <td><asp:Label ID="lblLevelNumber" CssClass="accountInfoTableLabelRight" runat="server" Text="--"></asp:Label></td>
                 </tr>
                 <tr>
                     <td><asp:Label ID="lblXP" CssClass="accountInfoTableLabel" runat="server" Text="XP"></asp:Label></td>
                     <td><asp:Label ID="lblXPAmount" CssClass="accountInfoTableLabelRight" runat="server" Text="--"></asp:Label></td>
                 </tr>
                 <tr>
                     <td>
                         <div class="pawIcon">
                             <img class="circle" src="Icons/icons8-circle-white-96.png" width="50" />
                             <img class="paw" src="Icons/icons8-cat-footprint-filled-white-96.png" width="30" />
                         </div>
                     </td>
                 <td><asp:Label ID="lblPaws" CssClass="accountInfoTableLabelRight" runat="server" Text="---"></asp:Label></td>
                 </tr>
             </table>
         </div>
         <div class="notificationDetails">
             <asp:ImageButton ID="imgNotificationRinging" CssClass="notificationIcon" runat="server" ImageUrl="~/Icons/icons8-notification-bell-ringing-white-96.png" OnClientClick="showNotificationPopup(true); return false;" />
             <asp:ImageButton ID="imgNotificationNormal" CssClass="notificationIcon" runat="server" ImageUrl="~/Icons/icons8-notification-bell-white-96.png" OnClientClick="showNotificationPopup(false); return false;" />
             <div id="notificationBadge" runat="server" class="notificationBadge"></div> <!--the coloured circle-->
         </div>
     </div>

    <div class="timeDateDiv">
        <table>
            <tr>
                <td colspan="2"><asp:Label ID="lblTime" CssClass="accountInfoLabel" runat="server" Text="--:--" Font-Size="65"></asp:Label></td>
                <td></td>
            </tr>
            <tr>
                <td><asp:Label ID="lblDay" CssClass="accountInfoLabel" runat="server" Text="Day"></asp:Label></td>
                <td><asp:Label ID="lblDate" CssClass="accountInfoLabel" runat="server" Text="Date"></asp:Label></td>
            </tr>
        </table>
    </div>
</div>

<!--study paws header to copy and paste-->
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
    <h2>purrfectly productive</h2>
</div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="navContent" Runat="Server">

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="mainContentPlaceHolder" Runat="Server">
       <%--Notification content for the bell--%>
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
     <%--end of notification code--%>
    <div class="createTimer3MainContent">
    <div class="timeSection">
        <div class="leftSection">
        </div>
        <div class="middleSection">
            <div class="timeHeadings">
                <asp:Label ID="lblHours" CssClass="label" runat="server" Text="Hours"></asp:Label>
                <asp:Label ID="lblMinutes" CssClass="label" runat="server" Text="Minutes"></asp:Label>
                <asp:Label ID="lblSeconds" CssClass="label" runat="server" Text="Seconds"></asp:Label>
            </div>
            <asp:TextBox ID="txtTimeHours" ClientIDMode="Static" CssClass="textbox timerInput" runat="server" Text="00"></asp:TextBox>
            <asp:Label ID="lblTimeColon1" CssClass="label" runat="server" Text=":"></asp:Label>
            <asp:TextBox ID="txtTimeMinutes" ClientIDMode="Static" CssClass="textbox timerInput" runat="server" Text="00"></asp:TextBox>
            <asp:Label ID="lblTimeColon2" CssClass="label" runat="server" Text=":"></asp:Label>
            <asp:TextBox ID="txtTimeSeconds" ClientIDMode="Static" CssClass="textbox timerInput" runat="server" Text="00"></asp:TextBox>
            <asp:TextBox ID="dummyInput" runat="server" CssClass="hiddenDummyInput" Style="display:none;"></asp:TextBox>
            <div class="validationErrorSection">
                <asp:RangeValidator ID="errorHour" CssClass="validationError" runat="server" ErrorMessage="Hours have to be between 00 and 99" MinimumValue="0" MaximumValue="99" Type="Integer" Display="Dynamic" EnableClientScript="true" ControlToValidate="txtTimeHours" ValidationGroup="timerValidation"></asp:RangeValidator>
                <asp:RangeValidator ID="errorMinute" CssClass="validationError" runat="server" ErrorMessage="Minutes have to be between 00 and 59" MinimumValue="0" MaximumValue="59" Type="Integer" Display="Dynamic" EnableClientScript="true" ControlToValidate="txtTimeMinutes" ValidationGroup="timerValidation"></asp:RangeValidator>
                <asp:RangeValidator ID="errorSecond" CssClass="validationError" runat="server" ErrorMessage="Seconds have to be 00" MinimumValue="0" MaximumValue="0" Type="Integer" Display="Dynamic" EnableClientScript="true" ControlToValidate="txtTimeSeconds" ValidationGroup="timerValidation"></asp:RangeValidator>
                <asp:CustomValidator ID="minTotalTimeValidator" CssClass="validationErrorCustom" runat="server" ErrorMessage="Timer must be at least 1 minute" ClientValidationFunction="validateMinTime" EnableClientScript="true" Display="Dynamic" ValidationGroup="timerValidation"></asp:CustomValidator>
            </div>
        </div>
        <div class="rightSection">
        </div>
    </div>
    <div class="buttonSection">
        <div class="leftSection">
        </div>
        <div class="middleSection">
            <asp:Button ID="btnBack" CssClass="button" runat="server" Text="Back" OnClick="btnBack_Click" CausesValidation="False" />
            <asp:Button ID="btnStart" CssClass="button" runat="server" Text="Start" OnClick="btnStart_Click" CausesValidation="true" ValidationGroup="timerValidation" />
        </div>
        <div class="rightSection">
            <asp:Button ID="btnViewPastTimers" CssClass="button" runat="server" Text="View past timers" Visible="False" /> <!--invisible but for correct spacing of other buttons-->
        </div>
    </div>
</div>
    <script>
    window.timerControlIds = {
        hoursId: '<%= txtTimeHours.ClientID %>',
        minutesId: '<%= txtTimeMinutes.ClientID %>',
        secondsId: '<%= txtTimeSeconds.ClientID %>'
    };
    </script>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">

</asp:Content>

