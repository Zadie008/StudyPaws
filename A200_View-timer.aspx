<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="A200_View-timer.aspx.cs" Inherits="Default2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" Runat="Server">
    Timer
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" Runat="Server">
            <!--account info to copy and paste-->
<div class="accountInfoDiv">
    <div class="profileDiv">
        <div class="profileIcon">
            <div id="profileCircle"></div> <!--CHANGE: has to be corresponding background colour-->
            <img id="profilePet" src="Images/Farm%204%20Cow%20White%20and%20Black.png" width="120"/> <!--CHANGE: has to be chosen profile pic-->
        </div>
        <div class="profileDetails">
            <table>
                <tr>
                    <td><asp:Label ID="lblLevel" class="accountInfoTableLabel" runat="server" Text="Label">Level</asp:Label></td>
                    <td><asp:Label ID="lblLevelNumber" class="accountInfoTableLabelRight" runat="server" Text="Label">16</asp:Label></td> <!--CHANGE: has to be their level-->
                </tr>
                <tr>
                    <td><asp:Label ID="lblXP" class="accountInfoTableLabel" runat="server" Text="Label">XP</asp:Label></td>
                    <td><asp:Label ID="lblXPAmount" class="accountInfoTableLabelRight" runat="server" Text="Label">65</asp:Label></td> <!--CHANGE: has to be total xp-->
                </tr>
                <tr>
                    <td>
                        <div class="pawIcon">
                            <img class="circle" src="Icons/icons8-circle-white-96.png" width="50" />
                            <img class="paw" src="Icons/icons8-cat-footprint-filled-white-96.png" width="30" />
                        </div>
                    </td>
                    <td><asp:Label ID="lblPaws" class="accountInfoTableLabelRight" runat="server" Text="Label">190</asp:Label></td> <!--CHANGE: has to be total paws currency-->
                </tr>
            </table>
        </div>
    </div>

    <div class="timeDateDiv">
        <table>
            <tr>
                <td colspan="2"><asp:Label ID="lblTime" class="accountInfoLabel" runat="server" Text="--:--" Font-Size="65"></asp:Label></td>
                <td></td>
            </tr>
            <tr>
                <td><asp:Label ID="lblDay" class="accountInfoLabel" runat="server" Text="Day"></asp:Label></td>
                <td><asp:Label ID="lblDate" class="accountInfoLabel" runat="server" Text="Date"></asp:Label></td>
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
            <textPath href="#curve" startOffset="50%" text-anchor="middle"><a href="Default.aspx">StudyPaws</a></textPath> <!--link to home page-->
        </text>
    </svg>
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
            <asp:Label ID="lblCountdown" class="label" runat="server"></asp:Label>
            <div class="homePagePet">
                <img id="pet" src="Images/Cat%201%20Brown%20and%20White.png" width="400" /> <!--CHANGE: has to be chosen home page pet-->
                <img id="glow" src="Images/Glow(cropped).png" width="400" />
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
            <asp:TextBox ID="txtSessionTitle" class="textbox" runat="server" ReadOnly="True" ></asp:TextBox>
        </div>
        <div class="middleSection">
            <asp:Button ID="btnAdd" class="button" runat="server" Text="Add" OnClick="btnAdd_Click" />
            <asp:Button ID="btnStop" class="button" runat="server" Text="Stop" OnClick="btnStop_Click" />
        </div>
        <div class="rightSection">
            <asp:Button ID="btnViewPastTimers" class="button" runat="server" Text="View past timers" Visible="False" /> <!--invisible but for correct spacing of other buttons-->
        </div>
    </div>
</div>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">

</asp:Content>

