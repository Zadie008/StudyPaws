<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="C100_Register.aspx.cs" Inherits="C100_Register" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" Runat="Server">
    Register
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" Runat="Server"> 
    <div class="loginRegisterCurvedHeader">
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
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="mainContentPlaceHolder" Runat="Server">
    <div id="registerPageDiv">
        <table>
            <tr>
                <td><asp:Label ID="lblUsername" class="label" runat="server" Text="Username"></asp:Label></td>
                <td><asp:TextBox ID="txtUsername" class="textbox" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td><asp:Label ID="lblPassword" class="label" runat="server" Text="Password"></asp:Label></td>
                <td><asp:TextBox ID="txtPassword" class="textbox" runat="server" TextMode="Password"></asp:TextBox></td>
            </tr>
            <tr>
                <td><asp:Label ID="lblConfirmPassword" class="label" runat="server" Text="Confirm Password"></asp:Label></td>
                <td><asp:TextBox ID="txtConfirmPassword" class="textbox" runat="server" TextMode="Password"></asp:TextBox></td>
                <td><asp:Label ID="Label1" class="label" runat="server" Text="test"></asp:Label></td>
            </tr>
        </table>
        <br />
        <div class="buttonSection">
            <asp:Button ID="btnRegister" class="button" runat="server" Text="Register" />
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="navContent" Runat="Server">

</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">

</asp:Content>