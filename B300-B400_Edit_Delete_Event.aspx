<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="B300-B400_Edit_Delete_Event.aspx.cs" Inherits="B300_B400_Edit_Delete_Event" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" Runat="Server">
    Edit or Delete Calendar Event
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
<div id="editDeleteEvent" class="createEventMainContent">
    <div class="eventTitleTagSection">
        <div class="leftSection">
        </div>
        <div class="middleSection">
            <table>
                <tr>
                    <td><asp:Label ID="lblEventTitle" class="label" runat="server" Text="Title"></asp:Label></td>
                    <td><asp:TextBox ID="txtEventTitle" class="textbox" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td><asp:Label ID="lblEventTag" class="label" runat="server" Text="Tag"></asp:Label></td>
                    <td>
                        <asp:DropDownList ID="dropdownEventTag" ClientIDMode="Static" class="dropDownListEditEvents" runat="server" BackColor="#446791" DataTextField="tagName" DataValueField="tagID">
                        </asp:DropDownList>
                        <asp:HiddenField ID="hiddenSelectedTagID" runat="server" />
                    </td>
                    <td>
                        <div class="tagControls">
                            <asp:ImageButton ID="btnAddTag" runat="server" CommandName="AddTag" CausesValidation="false" class="addTagBtn" OnClick="btnNewTag_Click" ImageUrl="~/Icons/icons8-add-new-white-96.png" />
                            <asp:ImageButton ID="btnEditTag" runat="server" CausesValidation="false" CssClass="addTagBtn" ImageUrl="~/Icons/icons8-edit-white-96.png" OnClick="btnEditTag_Click"/>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td><asp:Label ID="lblEventDate" class="label" runat="server" Text="Date"></asp:Label></td>
                    <td class="rightWords"><div class="customDateWrapper"><asp:TextBox ID="txtEventDate" runat="server" CssClass="eventDateBox" TextMode="Date"></asp:TextBox></div></td>
                    <%--<td><asp:TextBox ID="txtEventDate" class="textbox" runat="server" TextMode="Date"></asp:TextBox></td>--%>
                </tr>
            </table>
        </div>
        <div class="rightSection">
            <table>
                <tr>
                    <td><asp:RequiredFieldValidator ID="errorTitle" class="validationError" runat="server" ErrorMessage="Please enter a Title" EnableClientScript="true" ControlToValidate="txtEventTitle" ValidationGroup="EventValidation"></asp:RequiredFieldValidator></td>
                </tr>
                <tr>
                    <td><asp:RequiredFieldValidator ID="errorDropDown" class="validationError" runat="server" ErrorMessage="Please select a Tag" EnableClientScript="true" ControlToValidate="dropdownEventTag" InitialValue="" ValidationGroup="EventValidation"></asp:RequiredFieldValidator></td>
                </tr>
                <tr>
                    <td><asp:RequiredFieldValidator ID="errorDate" class="validationError" runat="server" ErrorMessage="Please select a Date" EnableClientScript="true" ControlToValidate="txtEventDate" ValidationGroup="EventValidation"></asp:RequiredFieldValidator></td>
                </tr>
            </table>
        </div>
    </div>
    <div class="buttonSection">
        <div class="leftSection">
        </div>
        <div class="middleSection">
            <asp:Button ID="btnBack" class="button" runat="server" Text="Back" OnClick="btnBack_Click" CausesValidation="False" />
            <asp:Button ID="btnDelete" class="button" runat="server" Text="Delete" OnClick="btnDelete_Click" CausesValidation="False" OnClientClick="showPopupDelete(); return false;" />
            <asp:Button ID="btnSave" class="button" runat="server" Text="Save" OnClick="btnSave_Click" ValidationGroup="EventValidation"/>
        </div>
       <div class="rightSection">
            <asp:Button ID="btnViewPastTimers" class="button" runat="server" Text="View past timers" Visible="False" /> <!--invisible but for correct spacing of other buttons-->
        </div>
    </div>
    <asp:HiddenField ID="hiddenSelectedDate" runat="server" />

    <div id="popupDeleteEvent" class="simple-popup" style="display: none;">
        <div class="popup-blue-box">
            <p>Are you sure you want to delete this event?</p>
            <img src="Images/Notification%20Sad%20Hamster.png" />
            <div class="buttonSection">
                <asp:Button ID="btnYesDelete" CssClass="popup-button" runat="server" Text="Yes, I'm sure!" OnClick="btnYesDelete_Click" />
                <asp:Button ID="btnNo" CssClass="popup-button-best-blue" runat="server" Text="No, not sure!" OnClick="btnNoDelete_Click"  />
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
                

    <script type="text/javascript">
        window.addEventListener('DOMContentLoaded', function () {
            const dropdownList = document.getElementById('dropdownEventTag');
            const btnAdd = document.getElementById('<%= btnAddTag.ClientID%>');
            const btnEdit = document.getElementById('<%= btnEditTag.ClientID%>');
            const tagValidator = document.getElementById('<%= errorDropDown.ClientID%>');

            function updateButtonVisibility() {
                const selectedValue = dropdownList.value;
                if (selectedValue && selectedValue !== "") {
                    if (tagValidator) {
                        tagValidator.style.display = 'none';
                        if (typeof ValidatorEnable !== 'undefined') {
                            ValidatorEnable(tagValidator, false);
                        }
                    }
                }
                if (!selectedValue || selectedValue === "") {
                    btnAdd.style.display = 'inline-block';
                    btnEdit.style.display = 'none';
                }
                else if (selectedValue === "2") {
                    btnAdd.style.display = 'none';
                    btnEdit.style.display = 'none';
                }
                else {
                    btnAdd.style.display = 'none';
                    btnEdit.style.display = 'inline-block';
                }
            }
            dropdownList.addEventListener('change', updateButtonVisibility);
            updateButtonVisibility();
        });
        function showPopupDelete() {
            document.getElementById('popupDeleteEvent').style.display = 'flex';
        }
        function hideDeletePopup() {
            document.getElementById('popupDeleteEvent').style.display = 'none';
        }

        // JOIN STUDY SESSION POPUP
        function showJoinPopup() {
            document.getElementById("popup").style.display = "flex";

            // Auto-refresh after 1 minute only if popup is still visible
            setTimeout(function () {
                var popup = document.getElementById("popup");
                if (popup && popup.style.display === "flex") {
                    location.reload();
                }
            }, 60000); // 1 minute
        }

        function hideJoinPopup() {
            document.getElementById("popup").style.display = "none";
        }

        // Auto-show join popup when page loads if there's a session to join
        window.addEventListener('load', function () {
            var joinSessionID = document.getElementById("mainContentPlaceHolder_hiddenJoinSessionID");
            if (joinSessionID && joinSessionID.value) {
                setTimeout(function () {
                    showJoinPopup();
                }, 1000);
            }
        });
    </script>
</div>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">
</asp:Content>

