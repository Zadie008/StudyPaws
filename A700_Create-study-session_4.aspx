<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="A700_Create-study-session_4.aspx.cs" Inherits="Default2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" Runat="Server">
    Create study session
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

<asp:Content ID="Content3" ContentPlaceHolderID="navContent" Runat="Server">

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="mainContentPlaceHolder" Runat="Server">
    <div id="createSession4MainContent" class="createTimer3MainContent">
            <div class="timeSection">
                <div class="leftSection">
                </div>
                <div id="selectDateTimeSection" class="middleSection">
                    <h2>Schedule your study session</h2>
                    <table>
                        <tr>
                            <td><asp:Label ID="lblFilterDate" for="txtFilterDate" runat="server" CssClass="label leftWords" Text="Date"></asp:Label></td>
                            <td class="rightWords"><div class="customDateWrapper"><asp:TextBox ID="txtFilterDate" runat="server" CssClass="filterDateBox" TextMode="Date"></asp:TextBox></div></td>
                        </tr>
                        <tr>
                            <td><asp:Label ID="lblStartTime" for="txtStartTimeHours" runat="server" CssClass="label leftWords" Text="Start Time"></asp:Label></td>
                            <td class="rightWords"><asp:TextBox ID="txtStartTimeHours" runat="server" CssClass="textbox timerInput" Text="00"></asp:TextBox><asp:Label ID="lblColon1" runat="server" CssClass="label" Text=":"></asp:Label><asp:TextBox ID="txtStartTimeMinutes" runat="server" CssClass="textbox timerInput" Text="00"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td><asp:Label ID="lblEndTime" for="txtEndTimeHours" runat="server" CssClass="label leftWords" Text="End Time"></asp:Label></td>
                            <td class="rightWords"><asp:TextBox ID="txtEndTimeHours" runat="server" CssClass="textbox timerInput" Text="00"></asp:TextBox><asp:Label ID="lblColon2" runat="server" CssClass="label" Text=":"></asp:Label><asp:TextBox ID="txtEndTimeMinutes" runat="server" CssClass="textbox timerInput" Text="00"></asp:TextBox></td>
                        </tr>
                    </table>
                </div>
                <div class="rightSection">
                    <div id="validationErrorStartSection" class="validationErrorSection">
                        <asp:RangeValidator ID="errorStartHour" CssClass="validationErrorCustom" runat="server" ControlToValidate="txtStartTimeHours" ErrorMessage="Hours have to be between 00 and 23" MinimumValue="0" MaximumValue="23" Type="Integer" Display="Dynamic" EnableClientScript="true" ValidationGroup="timerValidation" ValidateEmptyText="true" SetFocusOnError="true" />

                        <asp:RangeValidator ID="errorStartMinute" CssClass="validationErrorCustom" runat="server" ControlToValidate="txtStartTimeMinutes" ErrorMessage="Minutes have to be between 00 and 59" MinimumValue="0" MaximumValue="59" Type="Integer" Display="Dynamic" EnableClientScript="true" ValidationGroup="timerValidation" ValidateEmptyText="true" SetFocusOnError="true" />
                    </div>
                    <div id="validationErrorEndSection" class="validationErrorSection">
                        <asp:RangeValidator ID="errorEndHour" CssClass="validationErrorCustom" runat="server" ControlToValidate="txtEndTimeHours" ErrorMessage="Hours have to be between 00 and 23" MinimumValue="0" MaximumValue="23" Type="Integer" Display="Dynamic" EnableClientScript="true" ValidationGroup="timerValidation" ValidateEmptyText="true" SetFocusOnError="true" />

                        <asp:RangeValidator ID="errorEndMinute" CssClass="validationErrorCustom" runat="server" ControlToValidate="txtEndTimeMinutes" ErrorMessage="Minutes have to be between 00 and 59" MinimumValue="0" MaximumValue="59" Type="Integer" Display="Dynamic" EnableClientScript="true" ValidationGroup="timerValidation" ValidateEmptyText="true" SetFocusOnError="true" />

                        <asp:CustomValidator ID="minTotalTimeValidator" CssClass="validationErrorCustom" runat="server" ErrorMessage="Study Session must be at least 1 minute" ClientValidationFunction="validateMinTime" EnableClientScript="true" Display="Dynamic" ValidationGroup="timerValidation" OnServerValidate="minTotalTimeValidator_ServerValidate" ValidateEmptyText="true" SetFocusOnError="true" />
                        
                        <span id="dateValidationError" class="validationErrorCustom" style="display: none; color: white;"></span>
                    </div>
                </div>
            </div>

            <div class="buttonSection">
                <div class="leftSection"></div>
                <div class="middleSection">
                    <asp:Button ID="btnBack" CssClass="button" runat="server" Text="Back" OnClick="btnBack_Click" CausesValidation="False" />
                    <asp:Button ID="btnSchedule" CssClass="button" runat="server" Text="Schedule" OnClick="btnSchedule_Click" CausesValidation="True" ValidationGroup="timerValidation" OnClientClick="return validateDateNotInPast() && validateTimeNotInPast();" />
                </div>
                <div class="rightSection">
                    <asp:Button ID="btnViewPastTimers" CssClass="button" runat="server" Text="View past timers" Visible="False" />
                </div>
            </div>

        <div id="popupSuccess" class="simple-popup" style="display: none;">
            <div class="popup-pink-box">
                <p>Study Session scheduled!</p>
                <img src="Images/Notification%20Happy.png" />
                <div class="buttonSection">
                    <asp:Button ID="btnSuccess" CssClass="popup-button" runat="server" Text="Okay!" OnClientClick="hideSuccessPopup(); return false;" />
                </div>
            </div>
        </div>

        <!--does not have notification-->
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
                    <asp:Button ID="btnNo" CssClass="popup-button" runat="server" Text="Decline!" OnClick="btnNo_Click" />
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

        <!-- study session has started (popup stays for 1 minute) -->
        <div id="popup" class="simple-popup" style="display: none;">
            <div class="popup-pink-box">
                <asp:HiddenField ID="hiddenJoinSessionID" runat="server" />
                <p style="font-size: 2.3em;">Study Session has started!</p>
                <img style="margin-bottom: -2em;" src="Images/Notification%20Happy.png" />
                <div class="buttonSection">
                    <asp:Button ID="btnJoin" CssClass="popup-button" runat="server" Text="Join!" OnClick="btnJoin_Click" />
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        window.studySessionControlIds = {
            dateId: '<%= txtFilterDate.ClientID %>',
            startHoursId: '<%= txtStartTimeHours.ClientID %>',
            startMinutesId: '<%= txtStartTimeMinutes.ClientID %>',
            endHoursId: '<%= txtEndTimeHours.ClientID %>',
            endMinutesId: '<%= txtEndTimeMinutes.ClientID %>'
        };

        // Store current time for default values
        let currentTime = new Date();
        // Add 5 minutes to current time for start time
        let defaultStartTime = new Date(currentTime.getTime() + 5 * 60 * 1000);
        let currentHours = defaultStartTime.getHours().toString().padStart(2, '0');
        let currentMinutes = defaultStartTime.getMinutes().toString().padStart(2, '0');

        // Calculate default end time (1 hour from start time)
        let defaultEndTime = new Date(defaultStartTime.getTime() + 60 * 60 * 1000);
        let defaultEndHours = defaultEndTime.getHours().toString().padStart(2, '0');
        let defaultEndMinutes = defaultEndTime.getMinutes().toString().padStart(2, '0');

        function showSuccessPopup() {
            document.getElementById('popupSuccess').style.display = 'flex';
        }

        function hideSuccessPopup() {
            document.getElementById('popupSuccess').style.display = 'none';
            window.location.href = 'Default.aspx';
        }

        function validateDateNotInPast() {
            const dateInput = document.getElementById('<%= txtFilterDate.ClientID %>');
            const dateValidationError = document.getElementById('dateValidationError');

            if (!dateInput.value) {
                dateValidationError.textContent = "Please select a date";
                dateValidationError.style.display = 'inline';
                return false;
            }

            const inputDate = new Date(dateInput.value);

            // Get the current date
            const currentDate = new Date();

            inputDate.setHours(0, 0, 0, 0);
            currentDate.setHours(0, 0, 0, 0);

            if (inputDate < currentDate) {
                dateValidationError.textContent = 'Cannot schedule study session for a past date';
                dateValidationError.style.display = 'inline';
                return false;
            } else {
                dateValidationError.style.display = 'none';
                return validateTimeNotInPast();
            }
        }

        // function to validate time for today's date
        function validateTimeNotInPast() {
            const dateInput = document.getElementById('<%= txtFilterDate.ClientID %>');
            const startHoursInput = document.getElementById('<%= txtStartTimeHours.ClientID %>');
            const startMinutesInput = document.getElementById('<%= txtStartTimeMinutes.ClientID %>');
            const dateValidationError = document.getElementById('dateValidationError');

            const inputDate = new Date(dateInput.value);
            const currentDate = new Date();

            inputDate.setHours(0, 0, 0, 0);
            currentDate.setHours(0, 0, 0, 0);

            if (inputDate.getTime() === currentDate.getTime()) {
                const currentTime = new Date();
                const selectedStartTime = new Date();
                selectedStartTime.setHours(parseInt(startHoursInput.value) || 0, parseInt(startMinutesInput.value) || 0, 0, 0);

                if (selectedStartTime < currentTime) {
                    dateValidationError.textContent = 'Start time has already passed for today';
                    dateValidationError.style.display = 'inline';
                    return false;
                }
            }

            dateValidationError.style.display = 'none';
            return true;
        }

        document.addEventListener('DOMContentLoaded', function () {
            // Set default times to current time and 1 hour from now
            document.getElementById('<%= txtStartTimeHours.ClientID %>').value = currentHours;
            document.getElementById('<%= txtStartTimeMinutes.ClientID %>').value = currentMinutes;
            document.getElementById('<%= txtEndTimeHours.ClientID %>').value = defaultEndHours;
            document.getElementById('<%= txtEndTimeMinutes.ClientID %>').value = defaultEndMinutes;

            // Set today's date as default (using local time)
            const today = new Date();
            const localDate = today.toLocaleDateString('en-CA'); // YYYY-MM-DD format
            document.getElementById('<%= txtFilterDate.ClientID %>').value = localDate;

            validateAllFields();

            document.getElementById('<%= txtFilterDate.ClientID %>').addEventListener('change', function () {
                validateDateNotInPast();
                validateAllFields();
            });

            document.getElementById('<%= txtStartTimeHours.ClientID %>').addEventListener('input', function () {
                validateTimeNotInPast();
                validateAllFields();
            });

            document.getElementById('<%= txtStartTimeMinutes.ClientID %>').addEventListener('input', function() {
                validateTimeNotInPast();
                validateAllFields();
            });
        
            document.getElementById('<%= txtEndTimeHours.ClientID %>').addEventListener('input', validateAllFields);
            document.getElementById('<%= txtEndTimeMinutes.ClientID %>').addEventListener('input', validateAllFields);
        });

        function validateAllFields() {
            if (typeof Page_ClientValidate === 'function') {
                Page_ClientValidate('timerValidation');
            }

            validateMinTime(null, null);
            resetInvalidTimeInputs();
            showValidationErrors();
        }

        function showValidationErrors() {
            const validators = [
                '<%= errorStartHour.ClientID %>',
                '<%= errorStartMinute.ClientID %>',
                '<%= errorEndHour.ClientID %>',
                '<%= errorEndMinute.ClientID %>',
                '<%= minTotalTimeValidator.ClientID %>'
            ];
    
            validators.forEach(id => {
                const validator = document.getElementById(id);
                if (validator) {
                    validator.style.display = validator.isvalid ? 'none' : 'inline';
                }
            });
        }

        // Reset time inputs to "00" if they contain invalid data
        function resetInvalidTimeInputs() {
            const timeInputs = [
                '<%= txtStartTimeHours.ClientID %>',
                '<%= txtStartTimeMinutes.ClientID %>',
                '<%= txtEndTimeHours.ClientID %>',
                '<%= txtEndTimeMinutes.ClientID %>'
            ];

            timeInputs.forEach(inputId => {
                const input = document.getElementById(inputId);
                if (input) {
                    const value = input.value.trim();
                    // If empty or contains non-digits or value is invalid, reset to "00"
                    if (value === '' || !/^\d+$/.test(value) || isNaN(parseInt(value))) {
                        input.value = '00';
                    }
                }
            });
        }

        document.getElementById('<%= txtStartTimeHours.ClientID %>').addEventListener('blur', function () {
            if (this.value === '') this.value = '00';
        });
        document.getElementById('<%= txtStartTimeMinutes.ClientID %>').addEventListener('blur', function() {
            if (this.value === '') this.value = '00';
        });
        document.getElementById('<%= txtEndTimeHours.ClientID %>').addEventListener('blur', function() {
            if (this.value === '') this.value = '00';
        });
        document.getElementById('<%= txtEndTimeMinutes.ClientID %>').addEventListener('blur', function () {
            if (this.value === '') this.value = '00';
        });

        function validateMinTime(source, args) {
            // Always get values, even if they're "00"
            var startHours = parseInt(document.getElementById('<%= txtStartTimeHours.ClientID %>').value) || 0;
            var startMinutes = parseInt(document.getElementById('<%= txtStartTimeMinutes.ClientID %>').value) || 0;
            var endHours = parseInt(document.getElementById('<%= txtEndTimeHours.ClientID %>').value) || 0;
            var endMinutes = parseInt(document.getElementById('<%= txtEndTimeMinutes.ClientID %>').value) || 0;

            var startTotal = (startHours * 3600) + (startMinutes * 60);
            var endTotal = (endHours * 3600) + (endMinutes * 60);
    
            // Handle sessions that span across midnight
            var duration;
            if (endTotal < startTotal) {
                duration = (endTotal + (24 * 3600)) - startTotal;
            } else {
                duration = endTotal - startTotal;
            }
    
            var isValid = duration >= 60;

            var validator = document.getElementById('<%= minTotalTimeValidator.ClientID %>');
            if (validator) {
                validator.style.display = isValid ? 'none' : 'inline';
                validator.innerHTML = "Study Session must be at least <br />1 minute in duration";
            }

            if (args) {
                args.IsValid = isValid;
            }
            return isValid;
        }
    </script>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">

</asp:Content>