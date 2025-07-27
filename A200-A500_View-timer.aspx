<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="A200-A500_View-timer.aspx.cs" Inherits="A200_View_timer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" Runat="Server">
    Timer
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" Runat="Server">
    <!--account info to copy and paste-->
    <div class="accountInfoDiv">
        <div class="profileDiv">
                <div class="profileIcon">
                    <div id="profileCircle"></div>
                    <asp:Image ID="profilePet" runat="server" ImageUrl="~/Images/Cat 1.png" />
                </div>
            <div class="profileDetails">
                <table>
                    <tr>
                        <td><asp:Label ID="lblLevel" CssClass="accountInfoTableLabel" runat="server" Text="Level"></asp:Label></td>
                        <td><asp:Label ID="lblLevelNumber" CssClass="accountInfoTableLabelRight" runat="server" Text="16"></asp:Label></td>
                    </tr>
                    <tr>
                        <td><asp:Label ID="lblXP" CssClass="accountInfoTableLabel" runat="server" Text="XP"></asp:Label></td>
                        <td><asp:Label ID="lblXPAmount" CssClass="accountInfoTableLabelRight" runat="server" Text="65"></asp:Label></td>
                    </tr>
                    <tr>
                        <td>
                            <div class="pawIcon">
                                <img class="circle" src="Icons/icons8-circle-white-96.png" width="50" />
                                <img class="paw" src="Icons/icons8-cat-footprint-filled-white-96.png" width="30" />
                            </div>
                        </td>
                    <td><asp:Label ID="lblPaws" CssClass="accountInfoTableLabelRight" runat="server" Text="190"></asp:Label></td>
                    </tr>
                </table>
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
                StudyP<tspan dx="0.7em">w</tspan>s
            </textPath>
        </text>
    </svg>
    <img class="curvedHeaderPaw" src="Icons/icons8-cat-footprint-filled-white-96.png" alt="paw" />
    <h2>purrfectly productive</h2>
</div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="navContent" Runat="Server">

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="mainContentPlaceHolder" Runat="Server">
    <div class="viewTimerMainContent">
        <div class="timeSection">
            <div class="leftSection">
            </div>

            <div class="middleSection">
                <div class="timerCircleWrapper">
                    <svg class="progress-ring" width="350" height="350">
                        <circle class="progress-ring-bg" stroke="#90A8C3" stroke-width="30" fill="transparent" r="210" cx="175" cy="175"/>
                        <circle class="progress-ring-fill" stroke="#F4CAE0" stroke-width="30" fill="transparent" r="210" cx="175" cy="175" stroke-dasharray="1319" stroke-dashoffset="0"/>
                    </svg>

                    <div class="timerInnerContent">
                        <div id="mainContentPlaceHolder_lblCountdown" class="timerText"></div>
                        <div class="homePagePet">
                            <img id="glow" src="Images/Glow(cropped).png" />
                            <asp:Image id="pet" runat="server" width="400" />
                        </div>
                    </div>
                </div>
            </div>

            <div class="rightSection">
                        <asp:ImageButton ID="toggleToDoList" runat="server" OnClick="toggleToDoList_Click" class="toggleToDoListBtn"/>
    <asp:Panel ID="toDoListPanel" runat="server" CssClass="toDoListSection" Visible="true" >
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
    </asp:Panel>

          
                
            </div>
        </div>
        <div class="buttonSection">
            <div class="leftSection">
                <asp:TextBox ID="txtSessionTitle" CssClass="textbox" runat="server" ReadOnly="True" ></asp:TextBox>
            </div>
            <div class="middleSection">
                <div id="extraTimeButtons" class="extraTimeRow">
                    <asp:Button ID="btnPlus5" runat="server" CssClass="button" Text="+5 min" OnClientClick="return addExtraTime(5);" UseSubmitBehavior="false" />
                    <asp:Button ID="btnPlus10" runat="server" CssClass="button" Text="+10 min" OnClientClick="return addExtraTime(10);" UseSubmitBehavior="false" />
                    <asp:Button ID="btnPlus15" runat="server" CssClass="button" Text="+15 min" OnClientClick="return addExtraTime(15);" UseSubmitBehavior="false" />
                </div>
                <div class="buttonRow">
                    <asp:Button ID="btnToggleAddExtra" CssClass="button" runat="server" Text="Add" OnClientClick="return toggleExtraButtons();" UseSubmitBehavior="false" />
                    <asp:Button ID="btnStop" CssClass="button" runat="server" Text="Stop" OnClientClick="return stopTimer();" UseSubmitBehavior="false" />
                </div>
            </div>
            <div class="rightSection">
                <asp:Button ID="btnViewPastTimers" CssClass="button" runat="server" Text="View past timers" Visible="False" /> <!--invisible but for correct spacing of other buttons-->
            </div>
        </div>

        <div id="popup" class="simple-popup" style="display: none;">
            <div class="popup-blue-box">
                <p>Are you sure you want to stop the timer?<br />All XP and coins earned will be lost!</p>
                <img src="Images/Notification%20Sad%20Hamster.png" />
                <br />
                <div class="buttonSection">
                    <asp:Button ID="btnYes" CssClass="popup-button" runat="server" Text="Yes, I'm sure!" OnClick="btnYes_Click" OnClientClick="return confirmStop();" />
                    <asp:Button ID="btnNo" CssClass="popup-button-best-blue" runat="server" Text="No, not sure!" OnClientClick="hidePopup(); return false;" />
                </div>
            </div>
        </div>

        <div id="popupTimeUp" class="simple-popup" style="display: none;">
            <div class="popup-pink-box">
                <p>Time's up! You earned:</p> <!--CHANGED: from "Congrats!" to "Time's up!"-->
                <table id="popupTimeUpTable">
                    <tr>
                        <td>XP</td>
                        <td>+10</td>
                    </tr>
                    <tr>
                        <td>
                            <div class="pawIcon">
                                <img class="circle" src="Icons/icons8-circle-white-96.png" width="50" />
                                <img class="paw" src="Icons/icons8-cat-footprint-filled-white-96.png" width="30" />
                            </div>
                        </td>
                        <td>+10</td>
                    </tr>
                </table>
                <img src="Images/Notification%20Happy.png" />
                <br />
                <div class="buttonSection">
                    <asp:Button ID="btnThankYou" CssClass="popup-button" runat="server" Text="Thank you!" OnClientClick="hideTimeUpPopup(); return false;" />
                </div>
            </div>
        </div>
    </div>

    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
    <audio id="timerStartSound" src="Audio/timerStartSound.mp3" preload="auto"></audio>
    <audio id="timerEndSound" src="Audio/timerEndSound.mp3" preload="auto"></audio>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">

</asp:Content>

