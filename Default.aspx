<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" Runat="Server">
    StudyPaws
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" Runat="Server">
    <div class="accountInfoDiv">
        <div class="profileDiv">
            <div class="profileIcon">
                <div id="profileCircle"></div>
                <img id="profilePet" src="Images/Farm%20Cow%20White%20and%20Black.png" width="120"/>
            </div>
            <div class="profileDetails">
                <table>
                    <tr>
                        <td colspan="2"><asp:Label ID="lblLevel" runat="server" Text="Label">Level 16</asp:Label></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td colspan="2"><asp:Label ID="lblXP" runat="server" Text="Label">XP 65</asp:Label></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td>
                            <div class="pawIcon">
                                <img class="circle" src="Icons/icons8-circle-white-96.png" width="50" />
                                <img class="paw" src="Icons/icons8-cat-footprint-filled-white-96.png" width="30" />
                            </div>
                        </td>
                        <td><asp:Label ID="lblPaws" runat="server" Text="Label">190</asp:Label></td>
                    </tr>
                </table>
            </div>
        </div>

        <div class="timeDateDiv">
            <table>
                <tr>
                    <td colspan="2"><asp:Label ID="lblTime" runat="server" Text="09:52" Font-Size="65"></asp:Label></td>
                    <td></td>
                </tr>
                <tr>
                    <td><asp:Label ID="lblDay" runat="server" Text="Friday"></asp:Label></td>
                    <td><asp:Label ID="lblDate" runat="server" Text="18 April"></asp:Label></td>
                </tr>
            </table>
        </div>
    </div>

    <div class="curved-header">
        <svg viewBox="0 0 700 150" xmlns="http://www.w3.org/2000/svg">
            <defs>
                <path id="curve" d="M50,120 Q350,20 650,120" />
            </defs>
            <text>
                <textPath href="#curve" startOffset="50%" text-anchor="middle">StudyPaws</textPath>
            </text>
        </svg>
        <h2>purrfectly productive</h2>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="mainContentPlaceHolder" Runat="Server">
    <div class="homePagePet">
        <img id="pet" src="Images/Cat%20Brown%20and%20White.gif" width="400" />
        <img id="glow" src="Images/Glow.png" width="500" />
    </div>
    <div class="welcomeBackTextDiv">
        <h2>Welcome back</h2>
        <h1>zadie!</h1>
    </div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="navContent" Runat="Server">
    <asp:Menu ID="Menu1" runat="server" ForeColor="#F4CAE0" StaticHoverStyle-ForeColor="white" Orientation="Horizontal">
        <Items>
            <asp:MenuItem NavigateUrl="~\B__View-dashboard.aspx" Text="Dashboard" Value="Dashboard"></asp:MenuItem>
            <asp:MenuItem NavigateUrl="~\B100_View-calendar.aspx" Text="Calendar" Value="Calendar"></asp:MenuItem>
            <asp:MenuItem NavigateUrl="~\B900_View-to-do-list.aspx" Text="To-do List" Value="To-do List"></asp:MenuItem>
            <asp:MenuItem NavigateUrl="~\A100_Create-timer.aspx" Text="Timer" Value="Timer"></asp:MenuItem>
            <asp:MenuItem NavigateUrl="~\A700_Create-study-session.aspx" Text="Study Session" Value="Study Session"></asp:MenuItem>
            <asp:MenuItem NavigateUrl="~\B1400_View-shop.aspx" Text="Pet Shop" Value="Pet Shop"></asp:MenuItem>
            <asp:MenuItem NavigateUrl="~\A1800_View-pets.aspx" Text="Inventory" Value="Inventory"></asp:MenuItem>
            <asp:MenuItem NavigateUrl="~\C600_View-friend-list.aspx" Text="Friends" Value="Friends"></asp:MenuItem>
            <asp:MenuItem NavigateUrl="~\C1400_View-badges.aspx" Text="Badges" Value="Badges"></asp:MenuItem>
            <asp:MenuItem NavigateUrl="~\About.aspx" Text="About" Value="About"></asp:MenuItem>
        </Items>
        <StaticMenuItemStyle HorizontalPadding="1em" VerticalPadding="1em" />
    </asp:Menu>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">

</asp:Content>