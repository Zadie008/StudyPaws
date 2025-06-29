<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="C100-C500_Profile.aspx.cs" Inherits="Default2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" Runat="Server">
    Profile
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" Runat="Server">
    
   <div class="buttonSection">
    <asp:Button ID="btnBackProfile" class="button" runat="server" Text="Back" OnClick="btnBackProfile_Click" />
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

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="navContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="mainContentPlaceHolder" Runat="Server">

    
    <div class="profileIcon">
    <div id="profileCircle"></div> <!--CHANGE: has to be corresponding background colour-->
    <img id="profilePet" src="Images/Farm%204%20Cow%20White%20and%20Black.png" width="120"/> <!--CHANGE: has to be chosen profile pic-->
        <asp:Button ID="btnIcon" class="button" runat="server" Text="Change icon" />
    </div>

       
        <table>
            <tr>
                <td><asp:Label ID="lblUsername" class="label" runat="server" Text="Username"></asp:Label></td>
                <td><asp:TextBox ID="txtUsername" class="textbox" runat="server"></asp:TextBox></td>
                   <td> <asp:Button ID="btnEditUser" class="button" runat="server" Text="Edit" /></td>

            </tr>
            <tr>
                <td><asp:Label ID="lblPassword" class="label" runat="server" Text="Password"></asp:Label></td>
                <td><asp:TextBox ID="txtPassword" class="textbox" runat="server" TextMode="Password"></asp:TextBox></td>
                   <td> <asp:Button ID="btnEditPass" class="button" runat="server" Text="Edit" /></td>
            </tr>
            <tr>
                <td><asp:Label ID="lblEmail" class="label" runat="server" Text="Email"></asp:Label></td>
                <td><asp:TextBox ID="txtEmail" class="textbox" runat="server"></asp:TextBox></td>
                 <td> <asp:Button ID="btnEditEmail" class="button" runat="server" Text="Edit" /></td>
            </tr>
            <tr>
    <td><asp:Label ID="lblMode" CssClass="label" runat="server" Text="Light mode"></asp:Label></td>
    <td>
        <label class="switch">
            <input type="checkbox" id="toggleLightMode">
            <span class="slider"></span>
        </label>
    </td>
</tr>
           <tr>
    <td><asp:Label ID="lblDelete" class="label" runat="server" Text="Delete Profile"></asp:Label></td>
<%--   <tr>
    <td><asp:Label ID="Label1" CssClass="label" runat="server" Text="Delete Profile"></asp:Label></td>
    <td>
        <asp:LinkButton ID="btnDeletePfp" runat="server" CssClass="icon-button" OnClick="btnDeletePfp_Click">
            <img src="Icons/icons8-cat-footprint-filled-white-96.png" alt="Delete" class="delete-icon" />
        </asp:LinkButton>
    </td>--%>
<%--</tr>--%>
</tr>
        </table>
        <br/>
        <div class="buttonSection">
            <asp:Button ID="btnLogout" class="button" runat="server" Text="Logout" />
        </div>
   

</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">
</asp:Content>

