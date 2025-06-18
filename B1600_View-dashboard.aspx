<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="B1600_View-dashboard.aspx.cs" Inherits="Default2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" Runat="Server">
    Dashboard
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" Runat="Server">
        <!--account info to copy and paste-->
<div class="accountInfoDiv">
    <div class="profileDiv">
        <div class="profileIcon">
            <div id="profileCircle"></div> <!--CHANGE: has to be corresponding background colour-->
            <img id="profilePet" src="Images/Farm%204%20Cow%20White%20and%20Black.png" width="120"/> <!--CHANGE: has to be chosen profile pic-->
        </div>
        <div class="profileDetails">
            <table>
                <tr>
                    <td colspan="2"><asp:Label ID="lblLevel" runat="server" Text="Label">Level 16</asp:Label></td> <!--CHANGE: has to be their level-->
                    <td></td>
                </tr>
                <tr>
                    <td><asp:Label ID="lblXP" runat="server" Text="Label">XP</asp:Label></td>
                    <td><asp:Label ID="lblXPAmount" runat="server" Text="Label">65</asp:Label></td> <!--CHANGE: has to be total xp-->
                </tr>
                <tr>
                    <td>
                        <div class="pawIcon">
                            <img class="circle" src="Icons/icons8-circle-white-96.png" width="50" />
                            <img class="paw" src="Icons/icons8-cat-footprint-filled-white-96.png" width="30" />
                        </div>
                    </td>
                    <td><asp:Label ID="lblPaws" runat="server" Text="Label">190</asp:Label></td> <!--CHANGE: has to be total paws currency-->
                </tr>
            </table>
        </div>
    </div>

    <div class="timeDateDiv">
        <table>
            <tr>
                <td colspan="2"><asp:Label ID="lblTime" runat="server" Text="--:--" Font-Size="65"></asp:Label></td>
                <td></td>
            </tr>
            <tr>
                <td><asp:Label ID="lblDay" runat="server" Text="Day"></asp:Label></td>
                <td><asp:Label ID="lblDate" runat="server" Text="Date"></asp:Label></td>
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
            <textPath href="#curve" startOffset="50%" text-anchor="middle"><a href="Default.aspx">StudyPaws</a></textPath> <!--link to home page-->
        </text>
    </svg>
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
                <p class="calendarTitle">Calendar</p>
                <div class="controlsCalendar">
                    <button class="calendarFilters"><img src="Icons/icons8-filter-bars-white-96.png" /></button>
                    <button class="prevMonth"><img src="Icons/icons8-arrow-left-white-96.png" /></button>
                    <p class="dateCalendar">Month</p>
                    <button class="nextMonth"><img src="Icons/icons8-arrow-right-white-96.png" /></button>
                </div>
            </div>
            
            <div class="calendarBox">
                <ul class="weeks">
                    <li>Mon</li>
                    <li>Tue</li>
                    <li>Wed</li>
                    <li>Thu</li>
                    <li>Fri</li>
                    <li>Sat</li>
                    <li>Sun</li>
                </ul>
                <ul class="days">
                    <li class="noncurrentMonth">31</li>
                    <li>1</li>
                    <li>2</li>
                    <li>3</li>
                    <li>4</li>
                    <li>5</li>
                    <li>6</li>
                    <li>7</li>
                    <li>8</li>
                    <li>9</li>
                    <li>10</li>
                    <li>11</li>
                    <li>12</li>
                    <li>13</li>
                    <li>14</li>
                    <li>15</li>
                    <li>16</li>
                    <li>17</li>
                    <li>18</li>
                    <li>19</li>
                    <li>20</li>
                    <li>21</li>
                    <li>22</li>
                    <li>23</li>
                    <li>24</li>
                    <li>25</li>
                    <li>26</li>
                    <li>27</li>
                    <li>28</li>
                    <li>29</li>
                    <li>30</li>
                    <li class="noncurrentMonth">1</li>
                    <li class="noncurrentMonth">2</li>
                    <li class="noncurrentMonth">3</li>
                    <li class="noncurrentMonth">4</li>

                </ul>
            </div>
        </div>

        <div class="toDoListDashboard">
            <div class="toDoListControls">
                <p class="toDoListHeading">To-do List</p>
                <div class ="toDoListFilters">
                    <span id="allTasks" class="active">All</span>
                    <span id="pendingTasks">In Progress</span>
                    <span id="completedTasks">Completed</span>
                </div>
            </div>
            <ul class="taskBox">
                <li class="task">
                    <label>
                        <input type="checkbox" />
                        <span class="taskText">study maths</span>
                    </label>
            <!--<div class="taskListSettings">
                <i class="dotdotdot"></i>
                <ul class="taskEditMenu">
                    <li><i class="editTask"></i>Edit</li>
                    <li><i class="deleteTask"></i>Delete</li>
                </ul>
            </div>-->
                </li>
                <li class="addTask">
                    <span class="plusBtn">+</span>
                    <input type="text" placeholder="Add a new task" class="taskInput"/>
                </li>
            </ul>
        </div>
    </div>
    
    
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">

</asp:Content>

