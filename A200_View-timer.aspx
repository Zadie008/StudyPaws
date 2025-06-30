<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="A200_View-timer.aspx.cs" Inherits="A200_View_timer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" Runat="Server">
    Timer
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" Runat="Server">
            <!--account info to copy and paste-->
<div class="accountInfoDiv">
    <div class="profileDiv">
        <a href="C100-C500_Profile.aspx" class="profileIconLink">
            <div class="profileIcon">
                <div id="profileCircle"></div>
                <img id="profilePet" src="Images/Farm%204%20Cow%20White%20and%20Black.png" width="120"/>
            </div>
        </a>
        <div class="profileDetails">
            <table>
                <tr>
                    <td><asp:Label ID="lblLevel" CssClass="accountInfoTableLabel" runat="server" Text="Label">Level</asp:Label></td>
                    <td><asp:Label ID="lblLevelNumber" CssClass="accountInfoTableLabelRight" runat="server" Text="Label">16</asp:Label></td> <!--CHANGE: has to be their level-->
                </tr>
                <tr>
                    <td><asp:Label ID="lblXP" CssClass="accountInfoTableLabel" runat="server" Text="Label">XP</asp:Label></td>
                    <td><asp:Label ID="lblXPAmount" CssClass="accountInfoTableLabelRight" runat="server" Text="Label">65</asp:Label></td> <!--CHANGE: has to be total xp-->
                </tr>
                <tr>
                    <td>
                        <div class="pawIcon">
                            <img class="circle" src="Icons/icons8-circle-white-96.png" width="50" />
                            <img class="paw" src="Icons/icons8-cat-footprint-filled-white-96.png" width="30" />
                        </div>
                    </td>
                    <td><asp:Label ID="lblPaws" CssClass="accountInfoTableLabelRight" runat="server" Text="Label">190</asp:Label></td> <!--CHANGE: has to be total paws currency-->
                </tr>
            </table>
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
    <img class="curvedHeaderPaw" src="Icons/icons8-cat-footprint-filled-white-96.png" alt="paw" />
    <h2>purrfectly productive</h2>
</div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="navContent" Runat="Server">

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="mainContentPlaceHolder" Runat="Server">
        <div class="viewTimerMainContent">
    <div class="timeSection">
        <div class="leftSection">
        </div>
        <div class="middleSection">
            <div class="timerCircleWrapper">
                <svg class="progress-ring" width="350" height="350">
                    <circle class="progress-ring-bg" stroke="#90A8C3" stroke-width="30" fill="transparent" r="210" cx="175" cy="175"/>
                    <circle class="progress-ring-fill" stroke="#F4CAE0" stroke-width="30" fill="transparent" r="210" cx="175" cy="175" stroke-dasharray="1319" stroke-dashoffset="0"/>
                </svg>

                <div class="timerInnerContent">
                    <div id="mainContentPlaceHolder_lblCountdown" class="timerText"></div>
                    <div class="homePagePet">
                        <img id="pet" src="Images/Cat%201%20Brown%20and%20White.png" /> <!--CHANGE: has to be chosen home page pet-->
                        <img id="glow" src="Images/Glow(cropped).png" />
                    </div>
                </div>
            </div>
        </div>
        <div class="rightSection">
            <div class="toDoListSection">
                <h1>To-do List</h1>
            </div>
        </div>
    </div>
    <div class="buttonSection">
        <div class="leftSection">
            <asp:TextBox ID="txtSessionTitle" CssClass="textbox" runat="server" ReadOnly="True" ></asp:TextBox>
        </div>
        <div class="middleSection">
            <div id="extraTimeButtons" class="extraTimeRow">
    <asp:Button ID="btnPlus5" runat="server" CssClass="button" Text="+5" OnClientClick="return addExtraTime(5);" UseSubmitBehavior="false" />
    <asp:Button ID="btnPlus10" runat="server" CssClass="button" Text="+10" OnClientClick="return addExtraTime(10);" UseSubmitBehavior="false" />
    <asp:Button ID="btnPlus15" runat="server" CssClass="button" Text="+15" OnClientClick="return addExtraTime(15);" UseSubmitBehavior="false" />
</div>
            <div class="buttonRow">
                <asp:Button ID="btnToggleAddExtra" CssClass="button" runat="server" Text="Add" OnClientClick="return toggleExtraButtons();" UseSubmitBehavior="false" />
                <asp:Button ID="btnStop" CssClass="button" runat="server" Text="Stop" OnClientClick="return stopTimer();" UseSubmitBehavior="false" />
            </div>
            
        </div>
        <div class="rightSection">
            <asp:Button ID="btnViewPastTimers" CssClass="button" runat="server" Text="View past timers" Visible="False" /> <!--invisible but for correct spacing of other buttons-->
        </div>
    </div>
     <div id="popup" class="simple-popup" style="display: none;">
    <div class="popup-blue-box">
        <p>Are you sure you want to stop the timer? All XP and coins earned will be lost!</p>
        <img src="Images/Notification%20Sad%20Hamster.png" />
        <br />
        <div class="buttonSection">
            <asp:Button ID="btnYes" CssClass="popup-button" runat="server" Text="Yes, I'm sure!" OnClientClick="hidePopup(); return false;" />
            <asp:Button ID="btnNo" CssClass="popup-button-best-blue" runat="server" Text="No, not sure!" OnClientClick="hidePopup(); return false;" />
        </div>
    </div>
</div>
            </div>
    <audio id="alarmSound" src="Audio/alarm.mp3" preload="auto"></audio> <!--add real audio-->
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">

</asp:Content>

