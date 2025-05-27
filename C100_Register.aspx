<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="C100_Register.aspx.cs" Inherits="C100_Register" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" Runat="Server">
    Register
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" Runat="Server"> 
    <div class="curved-header">
    <svg viewBox="0 0 700 150" xmlns="http://www.w3.org/2000/svg">
        <defs>
            <path id="curve" d="M50,120 Q350,20 650,120" />
        </defs>
        <text>
            <textPath href="#curve" startOffset="50%" text-anchor="middle">StudyPaws</textPath>
        </text>
    </svg>
    <div style="height: 50px;"></div>
    <h2 class="subtitle">purrfectly productive</h2>
</div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="mainContentPlaceHolder" Runat="Server">
    <table>
        <tr>
            <td><asp:Label ID="lblUsername" runat="server" CssClass="label" Text="Username"></asp:Label></td>
            <td><asp:TextBox ID="txtUsername" runat="server" CssClass="textbox"></asp:TextBox></td>
        </tr>
        <tr>
            <td><asp:Label ID="lblPassword" runat="server" CssClass="label" Text="Password"></asp:Label></td>
            <td><asp:TextBox ID="TextBox1" runat="server" CssClass="textbox"></asp:TextBox></td>
        </tr>
        <tr>
            <td><asp:Label ID="lblConfirmPassword" runat="server" CssClass="label" Text="Confirm Password"></asp:Label></td>
            <td><asp:TextBox ID="TextBox2" runat="server" CssClass="textbox"></asp:TextBox></td>
        </tr>
    </table>
 
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="navContent" Runat="Server">
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">
</asp:Content>

