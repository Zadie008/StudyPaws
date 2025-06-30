<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Landing-page.aspx.cs" Inherits="Default2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" Runat="Server">
    StudyPaws
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" Runat="Server">
    <div class="loginRegisterCurvedHeader">
    <div class="curved-header">
        <svg viewBox="0 0 700 150" xmlns="http://www.w3.org/2000/svg">
            <defs>
                <path id="curve" d="M50,120 Q350,20 650,120" />
            </defs>
            <text>
                <textPath href="#curve" startOffset="50%" text-anchor="middle">
                    StudyP<tspan dx="0.7em">w</tspan>s
                </textPath>
            </text>
        </svg>
        <img class="curvedHeaderPaw" src="Icons/icons8-cat-footprint-filled-white-96.png" alt="paw" />
        <h2>purrfectly productive</h2>
    </div>
</div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="navContent" Runat="Server">

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="mainContentPlaceHolder" Runat="Server">
    <div id="landingPageDiv">
        <img src="Images/Notification%20Happy.png" />
        <br />
        <div class="buttonSection">
            <asp:Button ID="btnRegister" class="button" runat="server" Text="Register" OnClick="btnRegister_Click" />
            <asp:Button ID="btnLogin" class="button" runat="server" Text="Login" OnClick="btnLogin_Click" />
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">

</asp:Content>

