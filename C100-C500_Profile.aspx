<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="C100-C500_Profile.aspx.cs" Inherits="Default2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" Runat="Server">
    Profile
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" Runat="Server">
    
    <div class="profileHeader">
        <!--Back Button -->
        <div class="headerLeft">
            <asp:Button ID="btnBackProfile" class="button" runat="server" Text="Back" OnClick=" btnBackProfile_Click" />
        </div>
        
        <!-- pfpIcon -->
        <div class="headerCenter">
            <div class="profileIcon">
                <div class="profileImageContainer">
                    <div id="profileCircle"></div>
                    <img id="profilePet" src="Images/Farm%204%20Cow%20White%20and%20Black.png"/>
                </div>
              
            </div>
        </div>
        
        <!-- Time -->
        <div class="headerRight">
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
        </div>
    
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="navContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="mainContentPlaceHolder" Runat="Server">

    <div class="changeIconContainer">
        <asp:Button ID="btnChangeIcon" class="button" runat="server" Text="Change icon" />
    </div>
        
        <table>
            <tr>
                <td><asp:Label ID="lblUsername" class="label" runat="server" Text="Username"></asp:Label></td>
                <td><asp:TextBox ID="txtUsername" class="textbox" runat="server" ReadOnly ="true" OnTextChanged="txtUsername_TextChanged"></asp:TextBox></td>
                   <td> <asp:Button ID="btnEditUser" class="button" runat="server" Text="Edit" OnClientClick="enableEdit('txtUsername'); return false;" /></td>
                
                </tr>
            <tr>
                <td><asp:Label ID="lblPassword" class="label" runat="server" Text="Password"></asp:Label></td>
                <td><asp:TextBox ID="txtPassword" class="textbox" runat="server" TextMode="Password" ReadOnly ="true"></asp:TextBox></td>
                   <td> <asp:Button ID="btnEditPass" class="button" runat="server" Text="Edit" OnClientClick="enableEdit('txtPassword'); return false;" /></td>
            </tr>
            <tr>
                <td><asp:Label ID="lblEmail" class="label" runat="server" Text="Email"></asp:Label></td>
                <td><asp:TextBox ID="txtEmail" class="textbox" runat="server" ReadOnly ="true"></asp:TextBox></td>
                 <td> <asp:Button ID="btnEditEmail" class="button" runat="server" Text="Edit" OnClientClick="enableEdit('txtEmail'); return false;" /></td>
            </tr>
            <tr>
                <td><asp:Label ID="lblMode" class="label" runat="server" Text="Light mode"></asp:Label></td>
                <td>
                     <div style="text-align: right;">
                      <label class="switch">
                      <input type="checkbox" id="toggleLightMode">
                      <span class="slider"></span>
                      </label>
                     </div>
                </td>
            </tr>
           <tr>
               <td><asp:Label ID="lblDelete" class="label" runat="server" Text="Delete Profile"></asp:Label></td>
             <td>
                    <div style="text-align: right;">
                       <img src="Icons/icons8-delete-white-96.png" alt="Delete Icon" CssClass="deleteIconSmall" />
                    </div>
                </td>
          </tr>
        </table>

        <br/>
        <div class="buttonSection">
            <asp:Button ID="btnLogout" class="button" runat="server" Text="Logout" />
        </div>
   

</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">
</asp:Content>

