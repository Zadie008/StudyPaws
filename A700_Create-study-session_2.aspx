<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="A700_Create-study-session_2.aspx.cs" Inherits="Default2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" Runat="Server">
    Create study session
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" Runat="Server">
    <!--account info to copy and paste-->
    <div class="accountInfoDiv">
        <div class="profileDiv">
            <a href="C100-C500_Profile.aspx" class="profileIconLink">
                <div class="profileIcon">
                    <div id="profileCircle"></div>
                    <asp:Image ID="profilePet" runat="server" ImageUrl="~/Images/Cat 1.png" />
                </div>
            </a>
            <div class="profileDetails">
                <table>
                    <tr>
                        <td><asp:Label ID="lblLevel" CssClass="accountInfoTableLabel" runat="server" Text="Level"></asp:Label></td>
                        <td><asp:Label ID="lblLevelNumber" CssClass="accountInfoTableLabelRight" runat="server" Text="16"></asp:Label></td>
                    </tr>
                    <tr>
                        <td><asp:Label ID="lblXP" CssClass="accountInfoTableLabel" runat="server" Text="XP"></asp:Label></td>
                        <td><asp:Label ID="lblXPAmount" CssClass="accountInfoTableLabelRight" runat="server" Text="65"></asp:Label></td>
                    </tr>
                    <tr>
                        <td>
                            <div class="pawIcon">
                                <img class="circle" src="Icons/icons8-circle-white-96.png" width="50" />
                                <img class="paw" src="Icons/icons8-cat-footprint-filled-white-96.png" width="30" />
                            </div>
                        </td>
                    <td><asp:Label ID="lblPaws" CssClass="accountInfoTableLabelRight" runat="server" Text="190"></asp:Label></td>
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
    <a href="Default.aspx"><img class="curvedHeaderPaw" src="Icons/icons8-cat-footprint-filled-white-96.png" alt="paw" /></a>
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
    </div>

</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">

</asp:Content>

