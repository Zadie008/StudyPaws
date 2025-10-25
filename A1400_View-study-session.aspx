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
                        <asp:GridView ID="GridView1" runat="server" GridLines="None" CssClass="searchFriendsTable" 
                        AutoGenerateColumns="False" OnRowCommand="GridView1_RowCommand">
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <div class="friendRow">
                                            <div class="friendProfileIcon">
                                                <div class="friendProfileCircle"></div>
                                                <img class="friendProfileImage" src='<%# GetProfileImageUrl(Eval("iconNum")) %>' />
                                            </div>
                                            <span class="friendUsername"><%# Eval("username") %></span>
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
                    All XP and pawprints earned will be lost!<br />
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

        <!-- Level Up Popup with Confetti -->
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

    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
    <audio id="timerStartSound" src="Audio/timerStartSound.mp3" preload="auto"></audio>
    <audio id="timerEndSound" src="Audio/timerEndSound.mp3" preload="auto"></audio>
    <script src="https://cdn.jsdelivr.net/npm/canvas-confetti@1.6.0/dist/confetti.browser.min.js"></script>
    
 
    <script type="text/javascript">
        // Global variables to store level up information
        var leveledUp = false;
        var newLevel = 0;

        document.addEventListener('DOMContentLoaded', function () {
            setCircleColors();
        });

        function setCircleColors() {
            const profileImages = document.querySelectorAll('.friendProfileImage');

            profileImages.forEach(img => {
                const circle = img.closest('.friendProfileIcon').querySelector('.friendProfileCircle');
                const src = img.getAttribute('src').toLowerCase();
                let colorClass = 'circle-cat';

                if (src.includes('cat')) colorClass = 'circle-cat';
                else if (src.includes('dog')) colorClass = 'circle-dog';
                else if (src.includes('bunny')) colorClass = 'circle-bunny';
                else if (src.includes('cow')) colorClass = 'circle-cow';
                else if (src.includes('unicorn')) colorClass = 'circle-unicorn';

                circle.className = 'friendProfileCircle';
                circle.classList.add(colorClass);
            });
        }

        function loadInSessionUsers() {
            PageMethods.GetJoinedUsers(function (users) {
                updateUserList(users);
            });
        }

        function updateUserList(users) {
            const gridView = document.getElementById('<%= GridView1.ClientID %>');
            if (!gridView) return;

            let tbody = gridView.querySelector('tbody');
            if (!tbody) {
                tbody = document.createElement('tbody');
                gridView.appendChild(tbody);
            }

            let html = '';
            users.forEach(u => {
                const imgSrc = getProfileImagePath(u.iconNum);
                const circleClass = getCircleColorClass(u.iconNum);

                html += `
            <tr>
                <td>
                    <div class="friendRow">
                        <div class="friendProfileIcon">
                            <div class="friendProfileCircle ${circleClass}"></div>
                            <img class="friendProfileImage" src="${imgSrc}" />
                        </div>
                        <span class="friendUsername">${u.username}</span>
                    </div>
                </td>
            </tr>`;
            });

            tbody.innerHTML = html;

            setTimeout(setCircleColors, 100);
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

        function getCircleColorClass(iconNum) {
            switch (iconNum) {
                case 1: return "circle-cat";
                case 2: return "circle-dog";
                case 3: return "circle-bunny";
                case 4: return "circle-cow";
                case 5: return "circle-unicorn";
                default: return "circle-cat";
            }
        }
        //TAMMY ADDED - START
        function showStudySessionTimeUpPopup() {
            const minutesStudied = Math.floor(initialTime / 60);
            const xpEarned = minutesStudied * 2;
            const coinsEarned = minutesStudied * 2;

            document.getElementById("xpEarned").textContent = '+' + xpEarned;
            document.getElementById("coinsEarned").textContent = '+' + coinsEarned;

            // Show the popup
            document.getElementById("popupTimeUp").style.display = "flex";

            // Mark session as completed
            PageMethods.MarkSessionAsCompleted(function (response) {
                console.log("Session marked as completed:", response);
            }, function (error) {
                console.error("Error marking session as completed:", error);
            });
        }
        function hideStudySessionTimeUpPopup() {
            const minutesStudied = Math.floor(initialTime / 60);
            PageMethods.UpdateStudySessionRewards(minutesStudied, function (response) {
                console.log("Study session rewards updated:", response);

                if (response.startsWith("LevelUp:")) {
                    const parts = response.split(":");
                    const newLevel = parts[1];
                    const xpEarned = parts[2];
                    const coinsEarned = parts[3];

                    document.getElementById("popupTimeUp").style.display = "none";
                    showLevelUpPopup(newLevel);
                } else if (response.startsWith("Success:")) {
                    document.getElementById("popupTimeUp").style.display = "none";
                    window.location.href = "Default.aspx";
                }
                else {
                    console.error("Error updating rewards:", error);
                    document.getElementById("popupTimeUp").style.display = "none";
                    window.location.href = "Default.aspx";
                } 
            }, function (error) {
                console.error("Error updating study session rewards:", error);
                document.getElementById("popupTimeUp").style.display = "none";
                window.location.href = "Default.aspx";
            });
        }
        function showLevelUpPopup(newLevel) {
            console.log('Showing level up popup for level:', newLevel);
            document.getElementById('newLevelSpan').innerText = newLevel;
            document.getElementById('popupLevelUp').style.display = 'flex';
            triggerConfettiTimer();
        }

        function hideLevelUpPopup() {
            console.log('Hiding level up popup');
            document.getElementById('popupLevelUp').style.display = 'none';
            confetti.reset();
            window.location.href = "Default.aspx";
        }
        // Confetti Functions
        function triggerConfetti() {
            console.log('Triggering confetti');
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
        function onStudySessionComplete() {
            showStudySessionTimeUpPopup();
        }
        // TAMMY ADDED - END



        // ===== STUDY SESSION TIMER COMPLETION FUNCTIONS =====

        // This function should be called when the study session timer completes
        //function onStudySessionComplete(minutesStudied) {
        //    console.log('Study session completed. Minutes studied:', minutesStudied);

        //    // Call the WebMethod to update rewards and check for level up
        //    PageMethods.UpdateStudySessionRewards(minutesStudied, function (response) {
        //        console.log('UpdateStudySessionRewards response:', response);

        //        if (response.startsWith("Success")) {
        //            // Parse the response
        //            // Format: "Success:XP_EARNED:COINS_EARNED:LEVELED_UP_FLAG:NEW_LEVEL"
        //            var parts = response.split(':');
        //            var xpEarned = parts[1];
        //            var coinsEarned = parts[2];
        //            var leveledUpFlag = parts[3] === "1";
        //            var achievedLevel = parts[4];

        //            console.log('Parsed response - XP:', xpEarned, 'Coins:', coinsEarned, 'Leveled up:', leveledUpFlag, 'New level:', achievedLevel);

        //            // Update the time-up popup with earned amounts
        //            document.getElementById('xpEarned').textContent = '+' + xpEarned;
        //            document.getElementById('coinsEarned').textContent = '+' + coinsEarned;

        //            // Store level up information for when user clicks "Thank you"
        //            leveledUp = leveledUpFlag;
        //            newLevel = achievedLevel;

        //            // Show the time-up popup
        //            document.getElementById('popupTimeUp').style.display = 'flex';

        //        } else if (response.startsWith("Error")) {
        //            console.error('Error updating rewards:', response);
        //            // Still show time-up popup with default values as fallback
        //            document.getElementById('xpEarned').textContent = '+10';
        //            document.getElementById('coinsEarned').textContent = '+10';
        //            leveledUp = false;
        //            document.getElementById('popupTimeUp').style.display = 'flex';
        //        } else {
        //            console.error('Unexpected response:', response);
        //            // Fallback: show time-up popup with default values
        //            document.getElementById('xpEarned').textContent = '+10';
        //            document.getElementById('coinsEarned').textContent = '+10';
        //            leveledUp = false;
        //            document.getElementById('popupTimeUp').style.display = 'flex';
        //        }
        //    }, function (error) {
        //        console.error('WebMethod call failed:', error);
        //        // Fallback: show time-up popup with default values
        //        document.getElementById('xpEarned').textContent = '+10';
        //        document.getElementById('coinsEarned').textContent = '+10';
        //        leveledUp = false;
        //        document.getElementById('popupTimeUp').style.display = 'flex';
        //    });
        //}

        //// Function to handle when user clicks "Thank you" in the time-up popup
        //function hideStudySessionTimeUpPopup() {
        //    console.log('Hiding time-up popup. Leveled up:', leveledUp, 'New level:', newLevel);

        //    document.getElementById('popupTimeUp').style.display = 'none';

        //    // Check if we need to show level up popup
        //    if (leveledUp) {
        //        console.log('Showing level up popup for level:', newLevel);
        //        showLevelUpPopup(newLevel);
        //    } else {
        //        console.log('No level up - redirecting to home page');
        //        // If no level up, redirect to front page immediately
        //        setTimeout(function () {
        //            window.location.href = 'Default.aspx';
        //        }, 500);
        //    }
        //}

        //// ===== LEVEL UP POPUP FUNCTIONS =====

        //function showLevelUpPopup(level) {
        //    console.log('Displaying level up popup for level:', level);
        //    document.getElementById('newLevelSpan').innerText = level;
        //    document.getElementById('popupLevelUp').style.display = 'flex';
        //    triggerConfetti();
        //}

        //function hideLevelUpPopup() {
        //    console.log('Hiding level up popup and redirecting');
        //    document.getElementById('popupLevelUp').style.display = 'none';
        //    confetti.reset();

        //    // Reset level up flags
        //    leveledUp = false;
        //    newLevel = 0;

        //    // Redirect to front page after level up popup is closed
        //    setTimeout(function () {
        //        window.location.href = 'Default.aspx';
        //    }, 500);
        //}

        //// ===== CONFETTI FUNCTIONS =====

        //function triggerConfetti() {
        //    console.log('Triggering confetti');
        //    // Major explosion
        //    confetti({
        //        particleCount: 300,
        //        spread: 100,
        //        origin: { y: 0.6 },
        //        colors: ['#FFD700', '#FFA500', '#FF8C00', '#FF6347', '#00FF7F', '#1E90FF']
        //    });

        //    // Continuous falling confetti for 5 seconds
        //    const duration = 5000;
        //    const end = Date.now() + duration;

        //    (function frame() {
        //        confetti({
        //            particleCount: 5,
        //            angle: 60,
        //            spread: 55,
        //            origin: { x: 0 },
        //            colors: ['#FFD700', '#FFA500', '#FF8C00']
        //        });
        //        confetti({
        //            particleCount: 5,
        //            angle: 120,
        //            spread: 55,
        //            origin: { x: 1 },
        //            colors: ['#1E90FF', '#00FF7F', '#FF6347']
        //        });

        //        if (Date.now() < end) {
        //            requestAnimationFrame(frame);
        //        }
        //    }());
        //}

        //function triggerEnhancedConfetti() {
        //    console.log('Triggering enhanced confetti');
        //    const end = Date.now() + 3000;
        //    const colors = ['#FFD700', '#FFA500', '#FF8C00', '#FF6347', '#00FF7F', '#1E90FF', '#9370DB', '#FF69B4'];

        //    (function frame() {
        //        confetti({
        //            particleCount: 10,
        //            angle: 60,
        //            spread: 70,
        //            origin: { x: 0, y: 0.7 },
        //            colors: colors
        //        });
        //        confetti({
        //            particleCount: 10,
        //            angle: 120,
        //            spread: 70,
        //            origin: { x: 1, y: 0.7 },
        //            colors: colors
        //        });
        //        confetti({
        //            particleCount: 15,
        //            spread: 100,
        //            origin: { y: 0.6 },
        //            colors: colors
        //        });

        //        if (Date.now() < end) {
        //            requestAnimationFrame(frame);
        //        }
        //    }());

        //    // Big explosion in the center
        //    setTimeout(() => {
        //        confetti({
        //            particleCount: 200,
        //            spread: 150,
        //            origin: { y: 0.6 },
        //            colors: colors
        //        });
        //    }, 500);
        //}

        // ===== OTHER POPUP FUNCTIONS =====

        function showPopup() {
            document.getElementById('popup').style.display = 'flex';
        }

        function hidePopup() {
            document.getElementById('popup').style.display = 'none';
        }

        function showPopupDelete() {
            document.getElementById('popupDeleteTask').style.display = 'flex';
        }

        function hideDeletePopup() {
            document.getElementById('popupDeleteTask').style.display = 'none';
        }

        // ===== TIMER FUNCTIONS (Placeholders - replace with your actual timer functions) =====

        // These are placeholder functions - you'll need to integrate with your actual timer code
        function addExtraTime(minutes) {
            console.log('Adding extra time:', minutes, 'minutes');
            // Add your extra time logic here
            return false; // Prevent postback
        }

        function stopTimer() {
            console.log('Stop timer clicked');
            showPopup(); // Show confirmation popup
            return false; // Prevent postback
        }

        function confirmStop() {
            console.log('Confirming stop');
            // Add your stop confirmation logic here
            return true; // Allow postback
        }

        // ===== INITIALIZATION =====

        setTimeout(loadInSessionUsers, 1000);
        setInterval(loadInSessionUsers, 3000);

        // Remove any old window.onload level up checks since we're handling it differently now
        // The level up detection now happens through the WebMethod response

    </script>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" runat="Server">
</asp:Content>