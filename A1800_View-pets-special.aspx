<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="A1800_View-pets-special.aspx.cs" Inherits="Default2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" Runat="Server">
    Inventory
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" Runat="Server">
                        <!--account info to copy and paste-->
<div class="accountInfoDiv">
    <div class="profileDiv">
        <a href="C100-C500_Profile.aspx" class="profileIconLink">
            <div class="profileIcon">
                <div id="profileCircle"></div>
                <img id="profilePet" src="Images/Cat 1.png" width="120"/>
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
    <a href="Default.aspx"><img class="curvedHeaderPaw" src="Icons/icons8-cat-footprint-filled-white-96.png" alt="paw" /></a>
    <h2>purrfectly productive</h2>
</div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="navContent" Runat="Server">
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
<asp:Content ID="Content4" ContentPlaceHolderID="mainContentPlaceHolder" Runat="Server">
        <div id="viewInventoryMainContent">
        <h2>Your inventory</h2>
        <div class="backgroundColorContainer">
            <div class="leftCategorySection">
                <asp:Button ID="btnCats" runat="server" CssClass="button" Text="Cats" OnClick="btnCats_Click" />
                <asp:Button ID="btnDogs" runat="server" CssClass="button" Text="Dogs" OnClick="btnDogs_Click" />
                <asp:Button ID="btnFuzzy" runat="server" CssClass="button" Text="Fuzzy" OnClick="btnFuzzy_Click" />
                <asp:Button ID="btnFarm" runat="server" CssClass="button" Text="Farm" OnClick="btnFarm_Click" />
                <asp:Button ID="btnSpecial" runat="server" CssClass="button buttonSelected" Text="Special" OnClick="btnSpecial_Click" />
            </div>
            <div class="rightPetsSection">
                <table>
                    <tr>
                        <td><div class="petIcon">
                                <div class="petCircle"></div>
                                <img class="petImage" src="Images/Special 1.png" width="120"/>
                            </div>
                        </td>
                        <td>
                            <div class="petIcon">
                                <div class="petCircle"></div>
                                <img class="petImage" src="Images/Special 2.png" width="120"/>
                            </div>
                        </td>
                        <td>
                            <div class="petIcon">
                                <div class="petCircle"></div>
                                <img class="petImage" src="Images/Special 3.png" width="120"/>
                            </div>
                        </td>
                        <td>
                            <div class="petIcon">
                                <div class="petCircle"></div>
                                <img class="petImage" src="Images/Special 4.png" width="120"/>
                            </div>
                        </td>
                        <td>
                            <div class="petIcon">
                                <div class="petCircle"></div>
                                <img class="petImage" src="Images/Special 5.png" width="120"/>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td><asp:Button ID="btnSelect1" runat="server" CssClass="button" Text="Select" /></td>
                        <td><asp:Button ID="btnSelect2" runat="server" CssClass="button" Text="Select" /></td>
                        <td><asp:Button ID="btnSelect3" runat="server" CssClass="button" Text="Select" /></td>
                        <td><asp:Button ID="btnSelect4" runat="server" CssClass="button" Text="Select" /></td>
                        <td><asp:Button ID="btnSelect5" runat="server" CssClass="button" Text="Select" /></td>
                    </tr>
                </table>
            </div>
        </div>
        <div class="buttonSection">
            <div class="leftSection">
            </div>
            <div class="middleSection">
                <asp:Button ID="btnSell" class="button" runat="server" Text="Sell" />
                <asp:Button ID="btnEquip" class="button" runat="server" Text="Equip" />
            </div>
            <div class="rightSection">
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">

</asp:Content>

