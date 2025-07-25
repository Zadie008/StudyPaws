<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="ChangeProfilePhoto.aspx.cs" Inherits="Default2" %>

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
    <div id="changeProfilePhotoPage">
            <div class="changeIconContainer">
       <asp:Button ID="btnChangeIcon" class="profilebutton" runat="server" Text="Change icon" OnClick="btnChangeIcon_Click" />
        <asp:HiddenField ID="selectedIcon" runat="server" ClientIDMode="Static" />
   </div>

<div class="iconLayoutWrapper">
    <div class="iconRow topIconRow">
        <asp:ImageButton ID="btnCat" runat="server" CssClass="iconItem circle-cat"
            ImageUrl="Images/ProfilePictures/CatPfp.png"
            AlternateText="Cat"
            CommandArgument="1"
            OnClick="SelectIcon_Click" />
        
        <asp:ImageButton ID="btnDog" runat="server" CssClass="iconItem circle-dog"
            ImageUrl="Images/ProfilePictures/DogPfp.png"
            AlternateText="Dog"
            CommandArgument="2"
            OnClick="SelectIcon_Click" />
        
        <asp:ImageButton ID="btnBunny" runat="server" CssClass="iconItem circle-bunny"
            ImageUrl="Images/ProfilePictures/BunnyPfp.png"
            AlternateText="Bunny"
            CommandArgument="3"
            OnClick="SelectIcon_Click" />
    </div>

    <!-- Bottom row -->
    <div class="iconRow bottomIconRow">
        <asp:ImageButton ID="btnCow" runat="server" CssClass="iconItem circle-cow"
            ImageUrl="Images/ProfilePictures/CowPfp.png"
            AlternateText="Cow"
            CommandArgument="4"
            OnClick="SelectIcon_Click" />

        <asp:ImageButton ID="btnUnicorn" runat="server" CssClass="iconItem circle-unicorn"
            ImageUrl="Images/ProfilePictures/UnicornPfp.png"
            AlternateText="Unicorn"
            CommandArgument="5"
            OnClick="SelectIcon_Click" />
    </div>

    <asp:Panel ID="pnlConfirmPfpf" runat="server" Visible="false">
     <div id="popup" class="simple-popup">
    <div class="popup-pink-box">
        <p>Your profile photo has been updated!</p>
        <img src="Images/Notification%20Happy.png" />
        <br />
        <div class="buttonSection">
            <asp:Button ID="btnConfirmChange" CssClass="popup-button" runat="server" Text="Okay!" OnClientClick="hidePopup(); return false;" />
        </div>
    </div>
</div>
</asp:Panel>
    </div>
    </div>
</asp:Content> 

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">
</asp:Content>

