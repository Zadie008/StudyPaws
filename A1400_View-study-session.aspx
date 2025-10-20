<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="A1400_View-study-session.aspx.cs" Inherits="View_study_session" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" runat="Server">
    Study Session
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" runat="Server">
    <div class="accountInfoDiv">
        <div class="profileDiv">
            <div class="profileIcon">
                <div id="profileCircle" runat="server" clientidmode="Static"></div>
                <asp:Image ID="profilePet" runat="server" />
            </div>
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
                    <asp:ImageButton ID="imgNotificationNormal" CssClass="notificationIcon" runat="server" ImageUrl="~/Icons/icons8-notification-bell-white-96.png" CausesValidation="False" Enabled="False" />
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
                    StudyP<tspan dx="0.7em">w</tspan>s
                </textPath>
            </text>
        </svg>
        <img class="curvedHeaderPaw" src="Icons/icons8-cat-footprint-filled-white-96.png" alt="paw" />
        <h2>purrfectly productive</h2>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="navContent" runat="Server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="mainContentPlaceHolder" runat="Server">
    <div class="viewTimerMainContent">
        <div class="timeSection">
            <div class="leftSection">
                <div class="inSessionDiv">
                    <h2 class="inSessionHeading">In session</h2>
                    <div class="scrollableTableContainer">
                        <asp:GridView ID="GridView1" runat="server" GridLines="None" CssClass="searchFriendsTable" AutoGenerateColumns="False" OnRowCommand="GridView1_RowCommand">
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <div class="friendRow">
                                            <a href="C600_View-friend-list.aspx" class="friendProfileIconLink">
                                                <div class="friendProfileIcon">
                                                    <div class="friendProfileCircle"></div>
                                                    <img class="friendProfileImage" src='<%# GetProfileImagePath(Convert.ToInt32(Eval("iconNum"))) %>' />
                                                </div>
                                            </a>
                                            <span class="friendUsername"><%# Eval("username") %></span>
                                            <asp:ImageButton ID="btnAddFriend" runat="server" CssClass="addFriendBtn" 
                                                CommandName="SendFriendRequest" 
                                                CommandArgument='<%# Eval("userID") + "|" + Eval("username") %>' 
                                                ImageUrl='<%# IsFriend(Convert.ToInt32(Eval("userID"))) ? "Icons/icons8-check-white-96.png" : "Icons/icons8-add-new-white-96.png" %>'
                                                Visible='<%# !IsCurrentUser(Convert.ToInt32(Eval("userID"))) && !IsFriend(Convert.ToInt32(Eval("userID"))) %>' />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
            <div class="middleSection">
                <div class="timerCircleWrapper">
                    <svg class="progress-ring" width="350" height="350">
                        <circle class="progress-ring-bg" stroke="#90A8C3" stroke-width="30" fill="transparent" r="210" cx="175" cy="175" />
                        <circle class="progress-ring-fill" stroke="#F4CAE0" stroke-width="30" fill="transparent" r="210" cx="175" cy="175" stroke-dasharray="1319" stroke-dashoffset="0" />
                    </svg>

                    <div class="timerInnerContent">
                        <div id="mainContentPlaceHolder_lblCountdown" class="timerText"></div>
                        <div class="homePagePet">
                            <img id="glow" src="Images/Glow(cropped).png" />
                            <asp:Image ID="pet" runat="server" Width="400" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="rightSection">
            </div>
        </div>
        <div class="buttonSection">
            <div class="leftSection">
                <asp:TextBox ID="txtSessionTitle" CssClass="textbox" runat="server" ReadOnly="True"></asp:TextBox>
            </div>
            <div class="middleSection">
                <!--leaving this extraTimeButtons here for spacing reasons-->
                <div id="extraTimeButtons" class="extraTimeRow">
                    <asp:Button ID="btnPlus5" runat="server" CssClass="button" Text="+5 min" OnClientClick="return addExtraTime(5);" UseSubmitBehavior="false" />
                    <asp:Button ID="btnPlus10" runat="server" CssClass="button" Text="+10 min" OnClientClick="return addExtraTime(10);" UseSubmitBehavior="false" />
                    <asp:Button ID="btnPlus15" runat="server" CssClass="button" Text="+15 min" OnClientClick="return addExtraTime(15);" UseSubmitBehavior="false" />
                </div>
                <div class="buttonRow">
                    <asp:Button ID="btnStop" CssClass="button" runat="server" Text="Stop" OnClientClick="return stopTimer();" UseSubmitBehavior="false" />
                </div>
            </div>
            <div class="rightSection">
                <asp:Button ID="btnViewPastTimers" CssClass="button" runat="server" Text="View past timers" Visible="False" />
                <!--invisible but for correct spacing of other buttons-->
            </div>
        </div>

        <div id="popup" class="simple-popup" style="display: none;">
            <div class="popup-blue-box">
                <p>Are you sure you want to stop the study session?<br />
                    All XP and coins earned will be lost!<br />
                    (Your friends will stay in the study session even if you decide to leave!)</p>
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
                <p>Time's up! You earned:</p>
                <!--CHANGED: from "Congrats!" to "Time's up!"-->
                <table id="popupTimeUpTable">
                    <tr>
                        <td>XP</td>
                        <td id="xpEarned">+10</td>
                    </tr>
                    <tr>
                        <td>
                            <div class="pawIcon">
                                <img class="circle" src="Icons/icons8-circle-white-96.png" width="50" />
                                <img class="paw" src="Icons/icons8-cat-footprint-filled-white-96.png" width="30" />
                            </div>
                        </td>
                        <td id="coinsEarned">+10</td>
                    </tr>
                </table>
                <img src="Images/Notification%20Happy.png" />
                <br />
                <div class="buttonSection">
                    <asp:Button ID="btnThankYou" CssClass="popup-button" runat="server" Text="Thank you!" OnClientClick="hideStudySessionTimeUpPopup(); return false;" />
                </div>
            </div>
        </div>
    </div>

    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
    <audio id="timerStartSound" src="Audio/timerStartSound.mp3" preload="auto"></audio>
    <audio id="timerEndSound" src="Audio/timerEndSound.mp3" preload="auto"></audio>

    <script type="text/javascript">
        function loadInSessionUsers() {
            PageMethods.GetJoinedUsers(function (users) {
                const table = document.getElementById('<%= GridView1.ClientID %>');
                if (!table) return;

                let html = "";
                users.forEach(u => {
                    const imgSrc = getProfileImagePath(u.iconNum);
                    html += `
                    <div class="friendRow">
                        <div class="friendProfileIcon">
                            <img class="friendProfileImage" src="${imgSrc}" />
                        </div>
                        <span class="friendUsername">${u.username}</span>
                    </div>`;
                });

                table.innerHTML = html;
            });
        }

        function getProfileImagePath(iconNum) {
            switch (iconNum) {
                case 1: return "Images/ProfilePictures/CatPfp.png";
                case 2: return "Images/ProfilePictures/DogPfp.png";
                case 3: return "Images/ProfilePictures/BunnyPfp.png";
                case 4: return "Images/ProfilePictures/CowPfp.png";
                case 5: return "Images/ProfilePictures/UnicornPfp.png";
                default: return "Images/ProfilePictures/CatPfp.png";
            }
        }

        setInterval(loadInSessionUsers, 5000); // refresh every 5 seconds
    </script>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" runat="Server">
</asp:Content>