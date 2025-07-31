<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="B900_View-to-do-list.aspx.cs" Inherits="Default2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" Runat="Server">
    To-do list
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" Runat="Server">
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
                        <td>
                            <asp:Label ID="lblLevel" CssClass="accountInfoTableLabel" runat="server" Text="Level"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblLevelNumber" CssClass="accountInfoTableLabelRight" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <div class="xpProgressContainer">
                                <asp:Label ID="lblXP" CssClass="accountInfoLabel" runat="server" Text="XP"></asp:Label>
                                <div class="progressBarBackground">
                                    <asp:Panel ID="xpProgressBar" runat="server" CssClass="progressBarFill"></asp:Panel>
                                </div>
                                <asp:Label ID="lblXPPercentage" CssClass="accountInfoLabel xpPercentage" runat="server" Text="0%"></asp:Label>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div class="pawIcon">
                                <img class="circle" src="Icons/icons8-circle-white-96.png" width="50" />
                                <img class="paw" src="Icons/icons8-cat-footprint-filled-white-96.png" width="30" />
                            </div>
                        </td>
                        <td>
                            <asp:Label ID="lblPaws" CssClass="accountInfoTableLabelRight" runat="server" Text="---"></asp:Label>
                        </td>
                    </tr>
                </table>
            </div>
        </div>

        <div class="rightInfoDiv">
            <div class="timeNotificationWrapper">
                <div class="notificationDetails">
                    <asp:ImageButton ID="imgNotificationRinging" CssClass="notificationIcon" runat="server" ImageUrl="~/Icons/icons8-notification-bell-ringing-white-96.png" OnClientClick="showNotificationPopup(true); return false;" />
                    <asp:ImageButton ID="imgNotificationNormal" CssClass="notificationIcon" runat="server" ImageUrl="~/Icons/icons8-notification-bell-white-96.png" OnClientClick="showNotificationPopup(false); return false;" />
                    <div id="notificationBadge" runat="server" class="notificationBadge"></div>
                </div>

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
        <h2><a href="Default.aspx">purrfectly productive</a></h2>
    </div>

    <script>
        document.body.classList.add('is-default');
    </script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="navContent" Runat="Server">
    <!--navigation to copy and paste-->
<div class="collapsedNav">
    <div class="navbar">
        <asp:Menu ID="MenuLeft" runat="server" Orientation="Vertical" CssClass="nav-left" StaticDisplayLevels="1" StaticMenuItemStyle-CssClass="menu-item">
            <Items>
                <asp:MenuItem NavigateUrl="~\B1600_View-dashboard.aspx" Text="Dashboard" Value="Dashboard"></asp:MenuItem>
                <asp:MenuItem NavigateUrl="~\B100_View-calendar.aspx" Text="Calendar" Value="Calendar"></asp:MenuItem>
                <asp:MenuItem NavigateUrl="~\B900_View-to-do-list.aspx" Text="To-do List" Value="To-do List"></asp:MenuItem>
                <asp:MenuItem NavigateUrl="~\A100_Create-timer.aspx" Text="Timer" Value="Timer"></asp:MenuItem>
                <asp:MenuItem NavigateUrl="~\A700_Create-study-session.aspx" Text="Study Session" Value="Study Session"></asp:MenuItem>
            </Items>
        </asp:Menu>
        <asp:Menu ID="MenuRight" runat="server" Orientation="Vertical" CssClass="nav-right" StaticDisplayLevels="1" StaticMenuItemStyle-CssClass="menu-item">
            <Items>
                <asp:MenuItem NavigateUrl="~\B1400_View-shop.aspx" Text="Pet Shop" Value="Pet Shop"></asp:MenuItem>
                <asp:MenuItem NavigateUrl="~\A1800_View-pets.aspx" Text="Inventory" Value="Inventory"></asp:MenuItem>
                <asp:MenuItem NavigateUrl="~\C600_View-friend-list.aspx" Text="Friends" Value="Friends"></asp:MenuItem>
                <asp:MenuItem NavigateUrl="~\C1400_View-badges.aspx" Text="Badges" Value="Badges"></asp:MenuItem>
                <asp:MenuItem NavigateUrl="~\About.aspx" Text="About" Value="About"></asp:MenuItem>
            </Items>
        </asp:Menu>
    </div>
</div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="mainContentPlaceHolder" Runat="Server">
    <div class="toDoListPage">

        <div class="toDoListControls">
    <div class="toDoListHeaderRow">
        <p class="toDoListHeading">To Do List</p>
        <div class="toDoListFilters">
            <asp:DropDownList ID="ddlFilter" BackColor="#ADA7C9" runat="server" AutoPostBack="true" class="toDoFilterDropDownList" OnSelectedIndexChanged="ddlFilter_SelectedIndexChanged">
                <asp:ListItem Text="All Tasks" Value="All" />
                <asp:ListItem Text="In Progress" Value="InProgress" />
                <asp:ListItem Text="Completed" Value="Completed" />
            </asp:DropDownList>
            <asp:ImageButton ID="filterButton" runat="server" class="toDoFilterBtn" ClientIDMode="Static" ImageUrl="~/Icons/icons8-filter-bars-white-96.png" OnClick="toDoFilterBtn_Click"/>
        </div>
    </div>
</div>
<div class="newTaskContainer">
    <asp:ImageButton ID="btnAdd" runat="server" CommandName="Add" class="addTaskBtn" OnClick="btnAdd_Click" ImageUrl="~/Icons/icons8-add-new-white-96.png"/>
    <asp:TextBox ID="txtNewTask" runat="server" CssClass="addTaskText" AutoPostBack="true" OnTextChanged="txtNewTask_TextChanged" Placeholder="Add a new task..."></asp:TextBox>
</div>
<div class="scrollableTasksContainer">
    
    <asp:Repeater ID="rptTasks" runat="server" EnableViewState="false" OnItemCommand="rptTasks_ItemCommand" OnItemDataBound="rptTasks_ItemDataBound">
        <ItemTemplate>
            <div class="task">
                <asp:Button runat="server" CommandName="Toggle" CommandArgument='<%# Eval("taskID") %>' CssClass='<%# (bool)Eval("taskStatus") ? "checkbox checked" : "checkbox" %>' Text=" " />
                <asp:TextBox ID="txtEditDesc" runat="server"  ReadOnly="true" Text='<%# Eval("taskDesc") %>' CssClass='<%# (bool)Eval("taskStatus") ? "taskCompleted" : "taskUncompleted" %>' />
                <div class="taskControls">
                    <asp:ImageButton ID="editBtn" runat="server" class="editBtn" CommandName="Edit" CommandArgument='<%#Eval("taskID") %>' ImageUrl="~/Icons/icons8-edit-white-96.png" />
                    <asp:ImageButton ID="saveEditBtn" runat="server" visible="false" class="saveEditBtn" CommandName="Save" CommandArgument='<%#Eval("taskID") %>' ImageUrl="~/Icons/icons8-check-white-96.png"/>
                    <asp:ImageButton ID="deleteBtn" runat="server" class="deleteBtn" CommandName="Delete" CommandArgument='<%# Eval("taskID") %>' ImageUrl="~/Icons/icons8-delete-white-96.png"/>
                </div>
                <asp:HiddenField ID="taskIDHidden" runat="server" Value='<%# Eval("taskID") %>' />
            </div>
        </ItemTemplate>
    </asp:Repeater>
    <asp:HiddenField ID="userIDHidden" runat="server" />
</div>

                <div id="popupTaskComplete" class="simple-popup" style="display: none;">
    <div class="popup-pink-boxSmaller">
        <p>Congrats! You earned:</p> 
        <p>XP   +10</p>
        <img src="Images/Notification%20Happy.png" />
        <br />
        <div class="buttonSection">
            <asp:Button ID="btnThankYou" CssClass="popup-button-best-pink" runat="server" Text="Thank you!" OnClick="btnThankYou_Click" />
        </div>
    </div>
</div>

        <div id="popupDeleteTask" class="simple-popup" style="display: none;">
    <div class="popup-blue-box">
        <p>Are you sure you want to delete this task?</p>
        <img src="Images/Notification%20Sad%20Hamster.png" />
        <br />
        <div class="buttonSection">
            <asp:Button ID="btnYesDelete" CssClass="popup-button" runat="server" Text="Yes, I'm sure!" OnClick="btnYesDelete_Click" />
            <asp:Button ID="btnNo" CssClass="popup-button-best-blue" runat="server" Text="No, not sure!" OnClick="btnNoDelete_Click"  />
        </div>
    </div>
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

<!--has notifications-->
<div id="popupHasNotifications" class="simple-popup" style="display: none;">
    <div class="popup-pink-box">
        <asp:HiddenField ID="hiddenSessionID" runat="server" />
        <asp:Literal ID="litNotificationText" runat="server" />
        <img src="Images/Notification%20Happy.png" />
        <br />
        <div class="buttonSection">
            <asp:Button ID="btnYes" CssClass="popup-button-best-pink" runat="server" Text="Accept!" OnClick="btnYes_Click" />
            <asp:Button ID="Button1" CssClass="popup-button" runat="server" Text="Decline!" OnClick="btnNo_Click" />
        </div>
    </div>
</div>

<div id="popupCalendar" class="simple-popup" style="display: none;">
    <div class="popup-pink-box">
        <asp:HiddenField ID="hiddenShowCalendar" runat="server" />
        <p>Study Session has been added to your calendar!</p>
        <img src="Images/Notification%20Happy.png" />
        <br />
        <div class="buttonSection">
            <asp:Button ID="btnCalendar" CssClass="popup-button-best-pink" runat="server" Text="Calendar, GO!" OnClick="btnCalendar_Click" />
            <asp:Button ID="btnOk" CssClass="popup-button" runat="server" Text="Okay, thanks!" OnClick="btnOk_Click" />
        </div>
    </div>
</div>

<div id="popupConfirmDecline" class="simple-popup" style="display: none;">
    <div class="popup-blue-box">
        <asp:HiddenField ID="hiddenShowConfirmation" runat="server" />
        <p>Are you sure you want to decline the Study Session invitation?</p>
        <img src="Images/Notification%20Sad%20Hamster.png" />
        <br />
        <div class="buttonSection">
            <asp:Button ID="btnSure" CssClass="popup-button" runat="server" Text="Yes, I'm sure!" OnClick="btnSure_Click" />
            <asp:Button ID="btnNotSure" CssClass="popup-button-best-blue" runat="server" Text="No, not sure!" OnClick="btnNotSure_Click" />
        </div>
    </div>
</div>

<div id="popupIsDeclined" class="simple-popup" style="display: none;">
    <div class="popup-blue-box">
        <asp:HiddenField ID="hiddenShowDeclineConfirmed" runat="server" />
        <p>Study Session has been declined!</p>
        <img src="Images/Notification%20Sad%20Hamster.png" />
        <br />
        <div class="buttonSection">
            <asp:Button ID="btnOkayDeclined" CssClass="popup-button" runat="server" Text="Okay!" OnClick="btnOkayDeclined_Click" />
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
</div>

             <script type="text/javascript">
                 function showPopup() {
                     document.getElementById('popupTaskComplete').style.display = 'flex';
                 }

                 function hidePopup() {
                     document.getElementById('popupTaskComplete').style.display = 'none';
                 }
                 function showPopupDelete() {
                     document.getElementById('popupDeleteTask').style.display = 'flex';
                 }
                 function hideDeletePopup() {
                     document.getElementById('popupDeleteTask').style.display = 'none';
                 }

             </script>
    </div>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">

</asp:Content>

