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
         <br/>
         <br/>
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
            </tr>
        </table>
        <br/>
        <div class="buttonSection">
           <asp:Button ID="btnRegister" class="button" runat="server" Text="Register" OnClick="btnRegister_Click1" />
        </div>
     <asp:Panel ID="pnlPopup" runat="server" Visible="false">
         <div id="popup" class="simple-popup">
        <div class="popup-pink-box">
            <p>You have been registered!</p>
            <img src="Images/Notification%20Happy.png" />
            <br />
            <div class="buttonSection">
                <asp:Button ID="btnOkay" CssClass="popup-button" runat="server" Text="Okay!" OnClientClick="hidePopup(); return false;" />
            </div>
        </div>
    </div>
</asp:Panel>
</div>
  
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="navContent" Runat="Server">

</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">

</asp:Content>