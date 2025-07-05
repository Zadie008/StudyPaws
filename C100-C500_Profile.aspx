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
        <asp:Button ID="btnChangeIcon" class="button" runat="server" Text="Change icon" OnClick="btnChangeIcon_Click" />
    </div>
        
        <table>
            <tr>
                <td><asp:Label ID="lblUsername" class="label" runat="server" Text="Username"></asp:Label></td>
                <td><asp:TextBox ID="txtUsername" class="textbox" runat="server" ReadOnly ="true" OnTextChanged="txtUsername_TextChanged"></asp:TextBox></td>
                <td>
                <asp:Button ID="btnEditUser" CssClass="button" runat="server" Text="Edit" OnClick="btnEditUser_Click" />
                <asp:Button ID="btnSaveUser" CssClass="button" runat="server" Text="Save" Visible="false" OnClick="btnSaveUser_Click" />
                <asp:Button ID="btnCancelUser" CssClass="button" runat="server" Text="Cancel" Visible="false" OnClick="btnCancelUser_Click" />
                </td>
                
                </tr>
            <tr>
                <td><asp:Label ID="lblPassword" class="label" runat="server" Text="Password"></asp:Label></td>
                <td><asp:TextBox ID="txtPassword" class="textbox" runat="server" TextMode="Password" ReadOnly ="true" OnTextChanged="txtPassword_TextChanged"></asp:TextBox></td>
                <td>
                <asp:Button ID="btnEditPass" CssClass="button" runat="server" Text="Edit" OnClick="btnEditPass_Click" />
                <asp:Button ID="btnSavePass" CssClass="button" runat="server" Text="Save" Visible="false" OnClick="btnSavePass_Click" />
                <asp:Button ID="btnCancelPass" CssClass="button" runat="server" Text="Cancel" Visible="false" OnClick="btnCancelPass_Click" />
                </td>
            </tr>
            <tr>
                <td><asp:Label ID="lblEmail" class="label" runat="server" Text="Email"></asp:Label></td>
                <td><asp:TextBox ID="txtEmail" class="textbox" runat="server" ReadOnly ="true"></asp:TextBox></td>
                <td>
                <asp:Button ID="btnEditEmail" CssClass="button" runat="server" Text="Edit" OnClick="btnEditEmail_Click" />
                <asp:Button ID="btnSaveEmail" CssClass="button" runat="server" Text="Save" Visible="false" OnClick="btnSaveEmail_Click" />
                <asp:Button ID="btnCancelEmail" CssClass="button" runat="server" Text="Cancel" Visible="false" OnClick="btnCancelEmail_Click" />
                </td>
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
             <td><asp:Label ID="lblDelete" CssClass="label" runat="server" Text="Delete Profile"></asp:Label></td>
                <td>
                    <div style="text-align: right;">
                        <img src="Icons/icons8-delete-white-96.png" alt="Delete Icon" class="deleteIconSmall" />
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

