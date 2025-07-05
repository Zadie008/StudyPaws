<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="ChangeProfilePhoto.aspx.cs" Inherits="Default2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" Runat="Server">
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

<div class="iconLayoutWrapper">
    <!-- Top row with 3 icons -->
    <div class="iconRow topIconRow">
        <div class="iconItem"><img src="ProfilePictures/CatPfp.png" alt="cat" /></div>
        <div class="iconItem"><img src="ProfilePictures/DogPfp.png" alt="dog" /></div>
        <div class="iconItem"><img src="ProfilePictures/BunnyPfp.png" alt="rabbit" /></div>
    </div>

    <!-- Bottom row with 2 icons -->
    <div class="iconRow bottomIconRow">
        <div class="iconItem"><img src="ProfilePictures/CowPfp.png" alt="cow" /></div>
        <div class="iconItem"><img src="ProfilePictures/UnicornPfp.png" alt="unicorn" /></div>
    </div>
</div>
   </asp:Content> 
<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">
</asp:Content>

