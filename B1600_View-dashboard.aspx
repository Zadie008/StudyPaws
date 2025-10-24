<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="B1600_View-dashboard.aspx.cs" Inherits="Default2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" runat="Server">
    Dashboard
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" runat="Server">
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
            <!--<div class="notificationDetails">
                <asp:ImageButton ID="imgNotificationRinging" CssClass="notificationIcon" runat="server" ImageUrl="~/Icons/icons8-notification-bell-ringing-white-96.png" OnClientClick="showNotificationPopup(true); return false;" />
                <asp:ImageButton ID="imgNotificationNormal" CssClass="notificationIcon" runat="server" ImageUrl="~/Icons/icons8-notification-bell-white-96.png" OnClientClick="showNotificationPopup(false); return false;" />
                <div id="notificationBadge" runat="server" class="notificationBadge"></div>
            </div>-->

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
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="navContent" runat="Server">
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

<asp:Content ID="Content4" ContentPlaceHolderID="mainContentPlaceHolder" runat="Server">
    <div class="dashboardContainer">
        <div class="calendarDashboard">
            <div class="calendarHeader">
                <asp:Label ID="lblMonthYear" runat="server" Text="" CssClass="calendarTitle"></asp:Label>
                <div class="controlsCalendar">
                    <asp:ImageButton ID="btnPrevMonth" class="prevMonth" runat="server" OnClick="btnPrevMonth_Click" ImageUrl="~/Icons/icons8-arrow-left-white-96.png" />
                    <asp:Button ID="btnToday" class="dateCalendar" runat="server" Text="Today" OnClick="btnToday_Click" />
                    <asp:ImageButton ID="btnNextMonth" class="nextMonth" runat="server" OnClick="btnNextMonth_Click" ImageUrl="~/Icons/icons8-arrow-right-white-96.png" />
                    <asp:HiddenField ID="hfYear" runat="server" />
                    <asp:HiddenField ID="hfMonth" runat="server" />
                </div>
            </div>

            <div class="calendarBox">
                <asp:Literal ID="literalCalendar" runat="server"></asp:Literal>
            </div>
        </div>

        <div class="toDoListDashboard">
            <div class="toDoListControls">
                <div class="toDoListHeaderRow">
                    <p class="toDoListHeading">To Do List</p>
                    <div class="toDoListFilters">
                        <asp:DropDownList ID="ddlFilter" BackColor="#ADA7C9" runat="server" AutoPostBack="true" class="toDoFilterDropDownList" OnSelectedIndexChanged="ddlFilter_SelectedIndexChanged">
                            <asp:ListItem Text="All Tasks" Value="All" />
                            <asp:ListItem Text="In Progress" Value="InProgress" />
                            <asp:ListItem Text="Completed" Value="Completed" />
                        </asp:DropDownList>
                        <asp:ImageButton ID="filterButton" runat="server" class="toDoFilterBtn" ClientIDMode="Static" ImageUrl="~/Icons/icons8-filter-bars-white-96.png" OnClick="toDoFilterBtn_Click" />
                    </div>
                </div>
            </div>
            <div class="newTaskContainer">
                <asp:ImageButton ID="btnAdd" runat="server" CommandName="Add" class="addTaskBtn" OnClick="btnAdd_Click" ImageUrl="~/Icons/icons8-add-new-white-96.png" />
                <asp:TextBox ID="txtNewTask" runat="server" CssClass="addTaskText" OnTextChanged="txtNewTask_TextChanged" Placeholder="Add a new task..."></asp:TextBox>
            </div>
            <div class="scrollableTasksContainerDashboard">

                <asp:Repeater ID="rptTasks" runat="server" EnableViewState="false" OnItemCommand="rptTasks_ItemCommand" OnItemDataBound="rptTasks_ItemDataBound">
                    <ItemTemplate>
                        <div class="task">
                            <asp:Button runat="server" CommandName="Toggle" CommandArgument='<%# Eval("taskID") %>' CssClass='<%# (bool)Eval("taskStatus") ? "checkbox checked" : "checkbox" %>' Text=" " />
                            <asp:TextBox ID="txtEditDesc" runat="server" ReadOnly="true" Text='<%# Eval("taskDesc") %>' CssClass='<%# (bool)Eval("taskStatus") ? "taskCompleted" : "taskUncompleted" %>' />
                            <div class="taskControls">
                                <asp:ImageButton ID="editBtn" runat="server" class="editBtn" CommandName="Edit" CommandArgument='<%#Eval("taskID") %>' ImageUrl="~/Icons/icons8-edit-white-96.png" />
                                <asp:ImageButton ID="saveEditBtn" runat="server" Visible="false" class="saveEditBtn" CommandName="Save" CommandArgument='<%#Eval("taskID") %>' ImageUrl="~/Icons/icons8-check-white-96.png" />
                                <asp:ImageButton ID="deleteBtn" runat="server" class="deleteBtn" CommandName="Delete" CommandArgument='<%# Eval("taskID") %>' ImageUrl="~/Icons/icons8-delete-white-96.png" />
                            </div>
                            <asp:HiddenField ID="taskIDHidden" runat="server" Value='<%# Eval("taskID") %>' />
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:HiddenField ID="userIDHidden" runat="server" />
            </div>
        </div>

        <div id="popupTaskComplete" class="simple-popup" style="display: none;">
            <div class="popup-pink-boxSmaller">
                <p>Congrats! You earned:</p>
                <p>XP   +1</p>
                <img src="Images/Notification%20Happy.png" />
                <div class="buttonSection">
                    <asp:Button ID="btnThankYou" CssClass="popup-button" runat="server" Text="Thank you!" OnClick="btnThankYou_Click" />
                </div>
            </div>
        </div>

        <div id="popupDeleteTask" class="simple-popup" style="display: none;">
            <div class="popup-blue-box">
                <p>Are you sure you want to delete this task?</p>
                <img src="Images/Notification%20Sad%20Hamster.png" />
                <div class="buttonSection">
                    <asp:Button ID="btnYesDelete" CssClass="popup-button" runat="server" Text="Yes, I'm sure!" OnClick="btnYesDelete_Click" />
                    <asp:Button ID="btnNo" CssClass="popup-button-best-blue" runat="server" Text="No, not sure!" OnClick="btnNoDelete_Click" />
                </div>
            </div>
        </div>

        <!--does not have notification-->
        <%--<div id="popupNoNotifications" class="simple-popup" style="display: none;">
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
                <br />
                <br />
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
        </div>--%>


        <div id="popupLevelUp" class="simple-popup" style="display: none;">
            <div class="popup-pink-box">
                <h2>🎉 Level Up! 🎉</h2>
                <p>Congratulations! You've reached Level <span id="newLevelSpan"></span>!</p>
                <img src="Images/Notification%20Happy.png" />
                <br />
                <div class="buttonSection">
                    <asp:Button ID="btnYayLevelUp" CssClass="popup-button" runat="server" Text="Awesome!" OnClientClick="hideLevelUpPopup(); return false;" />
                </div>
            </div>
        </div>
        <script src="https://cdn.jsdelivr.net/npm/canvas-confetti@1.6.0/dist/confetti.browser.min.js"></script>
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

            // Level Up Popup with Confetti
            function showLevelUpPopup(newLevel) {
                document.getElementById('newLevelSpan').innerText = newLevel;
                document.getElementById('popupLevelUp').style.display = 'flex';

                // Trigger confetti explosion
                triggerConfetti();
            }

            function hideLevelUpPopup() {
                document.getElementById('popupLevelUp').style.display = 'none';
                // Stop any ongoing confetti
                confetti.reset();
            }

            function triggerConfetti() {
                // Major explosion
                confetti({
                    particleCount: 300,
                    spread: 100,
                    origin: { y: 0.6 },
                    colors: ['#F4CAE0', '#D7B9D5', '#ADA7C9', '#90A8C3', '#64A6BD', '#FFFFFF']
                });
                // old colors: ['#FFD700', '#FFA500', '#FF8C00', '#FF6347', '#00FF7F', '#1E90FF']
                
                // Continuous falling confetti for 5 seconds
                const duration = 5000;
                const end = Date.now() + duration;

                (function frame() {
                    confetti({
                        particleCount: 5,
                        angle: 60,
                        spread: 55,
                        origin: { x: 0 },
                        colors: ['#F4CAE0', '#D7B9D5', '#ADA7C9']
                    });
                    // old colors: ['#FFD700', '#FFA500', '#FF8C00']
                    confetti({
                        particleCount: 5,
                        angle: 120,
                        spread: 55,
                        origin: { x: 1 },
                        colors: ['#90A8C3', '#64A6BD', '#FFFFFF']
                    });
                    // old colors: ['#1E90FF', '#00FF7F', '#FF6347']

                    if (Date.now() < end) {
                        requestAnimationFrame(frame);
                    }
                }());
            }

            // Enhanced confetti version (more spectacular)
            function triggerEnhancedConfetti() {
                // Multiple explosions from different positions
                const end = Date.now() + 3000;
                const colors = ['#F4CAE0', '#D7B9D5', '#ADA7C9', '#90A8C3', '#64A6BD', '#FFFFFF', '#446791', '#5c7b9f'];
                // old colors: ['#FFD700', '#FFA500', '#FF8C00', '#FF6347', '#00FF7F', '#1E90FF', '#9370DB', '#FF69B4']

                (function frame() {
                    confetti({
                        particleCount: 10,
                        angle: 60,
                        spread: 70,
                        origin: { x: 0, y: 0.7 },
                        colors: colors
                    });
                    confetti({
                        particleCount: 10,
                        angle: 120,
                        spread: 70,
                        origin: { x: 1, y: 0.7 },
                        colors: colors
                    });
                    confetti({
                        particleCount: 15,
                        spread: 100,
                        origin: { y: 0.6 },
                        colors: colors
                    });

                    if (Date.now() < end) {
                        requestAnimationFrame(frame);
                    }
                }());

                // Big explosion in the center
                setTimeout(() => {
                    confetti({
                        particleCount: 200,
                        spread: 150,
                        origin: { y: 0.6 },
                        colors: colors
                    });
                }, 500);
            }

            // Update the existing showLevelUp function to use the new confetti version
            function showLevelUp() {
                var popup = document.getElementById('popupLevelUp');
                if (popup) {
                    // Get the current level from server-side
                    var currentLevel = <%= Session["CurrentLevel"] != null ? Session["CurrentLevel"] : "0" %>;
                    showLevelUpPopup(currentLevel);
                }
            }
            window.onload = function () {
                updateTime();
                setInterval(updateTime, 60000);

            // Check for level up
            <% if (Session["ShowLevelUpPopup"] != null && (bool)Session["ShowLevelUpPopup"])
            { %>
                showLevelUpPopup(<%= Session["CurrentLevel"] %>);
                <% Session["ShowLevelUpPopup"] = false; %>
                <% } %>
            };
        </script>
    </div>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" runat="Server">
</asp:Content>


