<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="B1600_View-dashboard.aspx.cs" Inherits="Default2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" Runat="Server">
    Dashboard
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" Runat="Server">
                    <!--account info to copy and paste-->
<div class="accountInfoDiv">
    <div class="profileDiv">
        <a href="C100-C500_Profile.aspx" class="profileIconLink">
            <div class="profileIcon">
                <div id="profileCircle"></div>
                <img id="profilePet" src="Images/Cat 1.png" width="120"/>
            </div>
        </a>
        <div class="profileDetails">
            <table>
                <tr>
                    <td><asp:Label ID="lblLevel" CssClass="accountInfoTableLabel" runat="server" Text="Label">Level</asp:Label></td>
                    <td><asp:Label ID="lblLevelNumber" CssClass="accountInfoTableLabelRight" runat="server" Text="Label">16</asp:Label></td> <!--CHANGE: has to be their level-->
                </tr>
                <tr>
                    <td><asp:Label ID="lblXP" CssClass="accountInfoTableLabel" runat="server" Text="Label">XP</asp:Label></td>
                    <td><asp:Label ID="lblXPAmount" CssClass="accountInfoTableLabelRight" runat="server" Text="Label">65</asp:Label></td> <!--CHANGE: has to be total xp-->
                </tr>
                <tr>
                    <td>
                        <div class="pawIcon">
                            <img class="circle" src="Icons/icons8-circle-white-96.png" width="50" />
                            <img class="paw" src="Icons/icons8-cat-footprint-filled-white-96.png" width="30" />
                        </div>
                    </td>
                    <td><asp:Label ID="lblPaws" CssClass="accountInfoTableLabelRight" runat="server" Text="Label">190</asp:Label></td> <!--CHANGE: has to be total paws currency-->
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
                <a href="Default.aspx" class="curvedHeaderLink">
                    StudyP<tspan dx="0.7em">w</tspan>s
                </a>
            </textPath>
        </text>
    </svg>
    <a href="Default.aspx"><img class="curvedHeaderPaw" src="Icons/icons8-cat-footprint-filled-white-96.png" alt="paw" /></a>
    <h2>purrfectly productive</h2>
</div>
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
    <div class="dashboardContainer">
        <div class="calendarDashboard">
            <div class="calendarHeader">
                <asp:Label ID="lblMonthYear" runat="server" Text="" CssClass="calendarTitle"></asp:Label>
                <div class="controlsCalendar">
                    <button class="calendarFilters"><img src="Icons/icons8-filter-bars-white-96.png" /></button>
                    <asp:ImageButton ID="btnPrevMonth" class="prevMonth" runat="server" OnClick="btnPrevMonth_Click" ImageUrl ="~/Icons/icons8-arrow-left-white-96.png" />
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
                <p class="toDoListHeading">To-do List</p>
                <div class ="toDoListFilters">
                    <button class="toDoFilterBtn"><img src="Icons/icons8-filter-bars-white-96.png" /></button>
                    <!--<asp:LinkButton ID="btnAll" runat="server" CssClass="filterBtn" OnClick="filter_Click">All</asp:LinkButton>
                    <asp:LinkButton ID="btnProgress" runat="server" CssClass="filterBtn" OnClick="filter_Click">In Progress</asp:LinkButton>
                    <asp:LinkButton ID="btnCompleted" runat="server" CssClass="filterBtn" OnClick="filter_Click">Completed</asp:LinkButton>  --> 
                </div>
            </div>

            <asp:Repeater ID="rptTasks" runat="server">
                <ItemTemplate>
                    <li class="task">
                        <asp:CheckBox ID="chkComplete" runat="server" AutoPostBack="true" OnCheckedChanged="chkComplete_CheckedChange" 
                            Checked='<%# Convert.ToBoolean(Eval("taskStatus"))%>'
                            ToolTip='<%# Eval("taskID")%>' />
                        <span class="taskText <%# Eval("taskStatus").ToString() == "Completed" ?"done":"" %>">
                            <%# Eval("taskDesc") %>
                            </span>
                    </li>
                </ItemTemplate>
            </asp:Repeater>

             <div class="newTask">
                 <button ID="addTaskBtn" onclick="btnAddTask_Click"><img src="Icons/icons8-add-new-white-96.png" /></button>
                 <asp:TextBox ID="txtNewTask" runat="server" CssClass="taskInput"></asp:TextBox>
             </div>
            <!--<ul class="taskBox">
                <li class="task">
                    <label>
                        <input type="checkbox" />
                        <span class="taskText">study maths</span>
                    </label>
            <div class="taskListSettings">
                <i class="dotdotdot"></i>
                <ul class="taskEditMenu">
                    <li><i class="editTask"></i>Edit</li>
                    <li><i class="deleteTask"></i>Delete</li>
                </ul>
            </div>
                </li>-->
            <!--</ul>-->
        </div>
    </div>
    
    
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">

</asp:Content>

