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

</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">

</asp:Content>