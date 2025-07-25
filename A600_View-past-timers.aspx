<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="A600_View-past-timers.aspx.cs" Inherits="Default2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" Runat="Server">
    Past timers
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" Runat="Server">
  <!--account info to copy and paste-->
 <div class="accountInfoDiv">
     <div class="profileDiv">
      
         <a href="C100-C500_Profile.aspx" class="profileIconLink">
             <div class="profileIcon">
                 <div id="profileCircle" runat="server" ClientIDMode="Static"></div>
                <asp:Image ID="profilePet" runat="server" />
             </div>
         </a>

         <div class="profileDetails">
             <table>
                 <tr>
                     <td><asp:Label ID="lblLevel" CssClass="accountInfoTableLabel" runat="server" Text="Level"></asp:Label></td>
                     <td><asp:Label ID="lblLevelNumber" CssClass="accountInfoTableLabelRight" runat="server" Text="--"></asp:Label></td>
                 </tr>
                 <tr>
                     <td><asp:Label ID="lblXP" CssClass="accountInfoTableLabel" runat="server" Text="XP"></asp:Label></td>
                     <td><asp:Label ID="lblXPAmount" CssClass="accountInfoTableLabelRight" runat="server" Text="--"></asp:Label></td>
                 </tr>
                 <tr>
                     <td>
                         <div class="pawIcon">
                             <img class="circle" src="Icons/icons8-circle-white-96.png" width="50" />
                             <img class="paw" src="Icons/icons8-cat-footprint-filled-white-96.png" width="30" />
                         </div>
                     </td>
                 <td><asp:Label ID="lblPaws" CssClass="accountInfoTableLabelRight" runat="server" Text="---"></asp:Label></td>
                 </tr>
             </table>
         </div>
         <div class="notificationDetails">
             <asp:ImageButton ID="imgNotificationRinging" CssClass="notificationIcon" runat="server" ImageUrl="~/Icons/icons8-notification-bell-ringing-white-96.png" OnClientClick="showNotificationPopup(true); return false;" />
             <asp:ImageButton ID="imgNotificationNormal" CssClass="notificationIcon" runat="server" ImageUrl="~/Icons/icons8-notification-bell-white-96.png" OnClientClick="showNotificationPopup(false); return false;" />
             <div id="notificationBadge" runat="server" class="notificationBadge"></div> <!--the coloured circle-->
         </div>
     </div>

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

<!--study paws header to copy and paste-->
<div class="curved-header">
    <svg viewBox="0 0 700 150" xmlns="http://www.w3.org/2000/svg">
        <defs>
            <path id="curve" d="M50,120 Q350,20 650,120" />
        </defs>
        <text>
            <textPath href="#curve" startOffset="50%" text-anchor="middle">
                <a href="Default.aspx" class="curvedHeaderLink">
                    StudyP<tspan dx="0.7em">w</tspan>s
                </a>
            </textPath>
        </text>
    </svg>
    <a href="Default.aspx"><img class="curvedHeaderPaw" src="Icons/icons8-cat-footprint-filled-white-96.png" alt="paw" /></a>
    <h2>purrfectly productive</h2>
</div></asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="navContent" Runat="Server">

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="mainContentPlaceHolder" Runat="Server">
       <%--Notification content for the bell--%>
 <div id="popupNoNotifications" class="simple-popup" style="display: none;">
     <div class="popup-blue-box">
         <p>You do not have any notifications at the moment!</p>
         <img src="Images/Notification%20Sad%20Hamster.png" />
         <br />
         <div class="buttonSection">
             <asp:Button ID="btnOkay" CssClass="popup-button" runat="server" Text="Okay!" OnClientClick="hideNotificationPopup(); return false;" />
         </div>
     </div>
 </div>
 <div id="popupHasNotifications" class="simple-popup" style="display: none;">
     <div class="popup-pink-box">
         <asp:HiddenField ID="hiddenSessionID" runat="server" />
         <asp:Literal ID="litNotificationText" runat="server" />
         <img src="Images/Notification%20Happy.png" />
         <br />
         <div class="buttonSection">
             <asp:Button ID="btnYes" CssClass="popup-button-best-pink" runat="server" Text="Accept!" OnClick="btnYes_Click" />
             <asp:Button ID="btnNo" CssClass="popup-button" runat="server" Text="Decline!" OnClick="btnNo_Click" />
         </div>
     </div>
 </div>
 <div id="popup" class="simple-popup" style="display: none;">
     <div class="popup-pink-box">
         <asp:HiddenField ID="hiddenJoinSessionID" runat="server" />
         <p>Study Session has started!</p>
         <img src="Images/Notification%20Happy.png" />
         <br />
         <div class="buttonSection">
             <asp:Button ID="btnJoin" CssClass="popup-button" runat="server" Text="Join!" OnClick="btnJoin_Click" />
         </div>
     </div>
 </div>
     <%--end of notification code--%>
    <div class="viewPastTimersMainContent">
        <div class="pastTimerStudySessionTableSection">
            <div class="viewPastTimersHeading">
                <div class="leftSection">
                </div>
                <div class="middleSection">
                    <h2>Past timers</h2>
                </div>
                <div class="rightSection">
                    <img id="filterIcon" src="Icons/icons8-filter-bars-white-96.png" style="cursor:pointer;" />
                </div>
                <div class="filterControls" id="filterControls" style="display: none;">
                    <asp:Label ID="lblFilterDate" for="txtFilterDate" runat="server" CssClass="label" Text="Date:"></asp:Label>
                    <div class="customDateWrapper">
                        <asp:TextBox ID="txtFilterDate" runat="server" CssClass="filterDateBox" TextMode="Date"></asp:TextBox>
                    </div>

                    <asp:Label ID="lblFilterTag" for="ddlFilterTag" runat="server" CssClass="label" Text="Tag:"></asp:Label>
                    <asp:DropDownList ID="ddlFilterTag" runat="server" CssClass="dropDownList" BackColor="#90A8C3">
                        <asp:ListItem></asp:ListItem>
                        <asp:ListItem>Studying</asp:ListItem>
                        <asp:ListItem>Assignments</asp:ListItem>
                        <asp:ListItem>Reading</asp:ListItem>
                        <asp:ListItem>Break</asp:ListItem>
                    </asp:DropDownList>

                    <asp:Button ID="btnApplyFilters" runat="server" Text="Apply" OnClick="btnApplyFilters_Click" CssClass="button" />
                </div>
            </div>
            <div class="scrollableTableContainer">
                <asp:GridView ID="GridView1" runat="server" GridLines="None" CssClass="pastTimerTable" AutoGenerateColumns="False">
                    <Columns>
                        <asp:BoundField DataField="Date Created" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                        <asp:BoundField DataField="Title" HeaderText="Title" />
                        <asp:BoundField DataField="Tag" HeaderText="Tag" />
                        <asp:TemplateField HeaderText="Duration">
                            <ItemTemplate>
                                <%# FormatDuration(Eval("Duration")) %>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
        <div class="buttonSection">
            <div class="leftSection">
            </div>
            <div class="middleSection">
                <asp:Button ID="btnBack" CssClass="button" runat="server" Text="Back" OnClick="btnBack_Click" />
            </div>
            <div class="rightSection">
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">

</asp:Content>

