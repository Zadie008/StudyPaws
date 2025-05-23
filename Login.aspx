<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" Runat="Server">
    Login
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" Runat="Server">
    <div class="curved-header">
        <svg viewBox="0 0 700 150" xmlns="http://www.w3.org/2000/svg">
        <defs>
      <path id="curve" d="M50,120 Q350,20 650,120" />
    </defs>
    <text>
      <textPath href="#curve" startOffset="50%" text-anchor="middle">
        StudyPaws
      </textPath>
    </text>
  </svg>

  <h2>purrfectly productive</h2>
</div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="mainContentPlaceHolder" Runat="Server">

   <table>
    <tr>
        <td><asp:Label ID="lblUsername" runat="server" CssClass="label" Text="Username: "></asp:Label></td>
        <td><asp:TextBox ID="txtUsername" runat="server" CssClass="textbox"></asp:TextBox></td>
    </tr>
    <tr>
        <td><asp:Label ID="lblPassword" runat="server" CssClass="label" Text="Password:"></asp:Label></td>
        <td><asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="textbox"></asp:TextBox></td>
    </tr>
</table>
       <div class="buttonSection">
    <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="button" />

</div>

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="navContent" Runat="Server">

    <h1>tHIS IS TO TEST</h1>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">

</asp:Content>

