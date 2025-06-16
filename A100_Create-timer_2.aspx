<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="A100_Create-timer_2.aspx.cs" Inherits="Default2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" Runat="Server">
    Create timer
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
                    <td colspan="2"><asp:Label ID="lblLevel" runat="server" Text="Label">Level 16</asp:Label></td> <!--CHANGE: has to be their level-->
                    <td></td>
                </tr>
                <tr>
                    <td><asp:Label ID="lblXP" runat="server" Text="Label">XP</asp:Label></td>
                    <td><asp:Label ID="lblXPAmount" runat="server" Text="Label">65</asp:Label></td> <!--CHANGE: has to be total xp-->
                </tr>
                <tr>
                    <td>
                        <div class="pawIcon">
                            <img class="circle" src="Icons/icons8-circle-white-96.png" width="50" />
                            <img class="paw" src="Icons/icons8-cat-footprint-filled-white-96.png" width="30" />
                        </div>
                    </td>
                    <td><asp:Label ID="lblPaws" runat="server" Text="Label">190</asp:Label></td> <!--CHANGE: has to be total paws currency-->
                </tr>
            </table>
        </div>
    </div>

    <div class="timeDateDiv">
        <table>
            <tr>
                <td colspan="2"><asp:Label ID="lblTime" runat="server" Text="--:--" Font-Size="65"></asp:Label></td>
                <td></td>
            </tr>
            <tr>
                <td><asp:Label ID="lblDay" runat="server" Text="Day"></asp:Label></td>
                <td><asp:Label ID="lblDate" runat="server" Text="Date"></asp:Label></td>
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
                            <asp:DropDownList ID="dropdownTag" class="dropDownList" runat="server" BackColor="#446791">
                            <asp:ListItem></asp:ListItem>
                            <asp:ListItem>Studying</asp:ListItem>
                            <asp:ListItem>Working</asp:ListItem>
                            <asp:ListItem>Self care</asp:ListItem>
                            <asp:ListItem>Cleaning</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="rightSection">
            </div>
        </div>
        <div class="buttonSection">
            <div class="leftSection">
            </div>
            <div class="middleSection">
                <asp:Button ID="btnBack" class="button" runat="server" Text="Back" OnClick="btnBack_Click" />
                <asp:Button ID="btnContinue" class="button" runat="server" Text="Continue" OnClick="btnContinue_Click" />
            </div>
            <div class="rightSection">
                <asp:Button ID="btnViewPastTimers" class="button" runat="server" Text="View past timers" Visible="False" /> <!--invisible but for correct spacing of other buttons-->
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">

</asp:Content>

