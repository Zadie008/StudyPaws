<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="C100-C500_Profile.aspx.cs" Inherits="Default2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" Runat="Server">
    Profile
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" Runat="Server">
    <div class="profileHeader">
        <!--Back Button -->
        <div class="headerLeft">
            <asp:Button ID="btnBackProfile" class="profilebutton" runat="server" Text="Back" OnClick=" btnBackProfile_Click" />
        </div>
        
        <!-- pfpIcon -->
         <div class="headerCenter">
            <div class="profileIcon">
                <div class="profileImageContainer">
                   <div id="profileCircle" runat="server" ClientIDMode="Static"></div>
                    <asp:Image ID="profilePet" runat="server" />
                </div>
            </div>
        </div>
        
        <!-- Time -->
       <div class="rightInfoDiv">
    <div class="timeNotificationWrapper">
        <div class="timeDateDiv">
            <asp:Label ID="lblTime" CssClass="accountInfoLabel currentTime" runat="server" Text="09:52"></asp:Label>
            <div class="dateContainer">
                <asp:Label ID="lblDay" CssClass="accountInfoLabel currentDate" runat="server" Text="Friday"></asp:Label>
                <span class="dateSeparator">|</span>
                <asp:Label ID="lblDate" CssClass="accountInfoLabel currentDate" runat="server" Text="18 April"></asp:Label>
            </div>
        </div>
    </div>
</div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="navContent" Runat="Server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="mainContentPlaceHolder" Runat="Server">
    <div id="profileMainContent">
        <table>
            <tr>
                <td><asp:Label ID="lblUsername" class="label" runat="server" Text="Username"></asp:Label></td>
                <td><asp:TextBox ID="txtUsername" class="textbox" runat="server" ReadOnly ="true" OnTextChanged="txtUsername_TextChanged"></asp:TextBox></td>
                <td>
                    <asp:Button ID="btnEditUser" CssClass="profilebutton" runat="server" Text="Edit" OnClick="btnEditUser_Click" />
                    <asp:Button ID="btnSaveUser" CssClass="profilebutton" runat="server" Text="Save" Visible="false" OnClick="btnSaveUser_Click" />
                    <asp:Button ID="btnCancelUser" CssClass="profilebutton" runat="server" Text="Cancel" Visible="false" OnClick="btnCancelUser_Click" />
                </td>
            </tr>
            <tr>
                <td><asp:Label ID="lblPassword" class="label" runat="server" Text="Password"></asp:Label></td>
                <td><asp:TextBox ID="txtPassword" class="textbox" runat="server" TextMode="Password" ReadOnly ="true" OnTextChanged="txtPassword_TextChanged"></asp:TextBox></td>
                <td>
                    <asp:Button ID="btnEditPass" CssClass="profilebutton" runat="server" Text="Edit" OnClick="btnEditPass_Click" />
                    <asp:Button ID="btnSavePass" CssClass="profilebutton" runat="server" Text="Save" Visible="false" OnClick="btnSavePass_Click" />
                    <asp:Button ID="btnCancelPass" CssClass="profilebutton" runat="server" Text="Cancel" Visible="false" OnClick="btnCancelPass_Click" />
                </td>
            </tr>
            <tr>
                <td><asp:Label ID="lblEmail" class="label" runat="server" Text="Email"></asp:Label></td>
                <td><asp:TextBox ID="txtEmail" class="textbox" runat="server" ReadOnly ="true"></asp:TextBox></td>
                <td>
                    <asp:Button ID="btnEditEmail" CssClass="profilebutton" runat="server" Text="Edit" OnClick="btnEditEmail_Click" />
                    <asp:Button ID="btnSaveEmail" CssClass="profilebutton" runat="server" Text="Save" Visible="false" OnClick="btnSaveEmail_Click" />
                    <asp:Button ID="btnCancelEmail" CssClass="profilebutton" runat="server" Text="Cancel" Visible="false" OnClick="btnCancelEmail_Click" />
                </td>
            </tr>
             <tr>
                <td><asp:Label ID="lblDelete" CssClass="label" runat="server" Text="Delete Profile"></asp:Label></td>
                <td>
                    <div class="deleteIconContainer">
                        <asp:ImageButton ID="deleteImageButton" runat="server" ImageUrl="~/Icons/icons8-delete-white-96.png" CssClass="deleteIcon" AlternateText="Delete Profile" OnClick="deleteImageButton_Click"/>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    <div class="buttonSection">
                        <asp:Button ID="btnLogout" CssClass="button" runat="server" Text="Logout" OnClick="btnLogout_Click" />
                    </div>
                </td>
            </tr>
        </table>

        <asp:Panel ID="pnlDeleteProfile" runat="server" Visible="false">
            <div id="popup" class="simple-popup">
                <div class="popup-blue-box">
                    <p>Are you sure you want to delete your profile? All progress will be lost!</p>
                    <img src="Images/Notification%20Sad%20Hamster.png" />
                    <br />
                    <div class="buttonSection">
                       <asp:Button ID="btnConfirmDeleteProfile" CssClass="popup-button" runat="server" Text="Yes, I am sure!" OnClick="btnConfirmDeleteProfile_Click" />
                        <asp:Button ID="btnCancelDelete" CssClass="popup-button-best-blue" runat="server" Text="No, not sure!" OnClientClick="hidePopup(); return false;" />
                    </div>
                </div>
            </div>
        </asp:Panel>
            
        <asp:Panel ID="pnlLogout" runat="server" Visible="false">
            <div id="popup" class="simple-popup">
                <div class="popup-pink-box">
                    <p>Hope you have a purrfect day!</p>
                    <img src="Images/Notification%20Happy.png" />
                    <br />
                    <div class="buttonSection">
                       <asp:Button ID="btnGoodbye" CssClass="popup-button" runat="server" Text="Goodbye!" OnClick="btnGoodbye_Click" />
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">
</asp:Content>

