<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="B600_Add_Tags.aspx.cs" Inherits="B500_B800_Tags" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" Runat="Server">
    Add Tags
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
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="navContent" Runat="Server">

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="mainContentPlaceHolder" Runat="Server">
        <div id="popup1" runat="server" ClientIDMode="static" class="simple-popup" >
    <div class="popup-pink-boxTagEdit">
        <table class="popupTagEditing">
            <tr>
                <td class="tableLabel"><asp:Label ID="lblTagTitle" class="label" runat="server" Text="Tag Title"></asp:Label></td>
                <td class="tableInput" colspan="5"><asp:TextBox ID="txtTagTitle" class="textbox" runat="server" ValidationGroup="tagPopup"></asp:TextBox></td>
            </tr>
            <tr>
                <td></td>
                <td class="tableValidation" colspan="5"><asp:RequiredFieldValidator ID="errorTagTitle" class="validationError" runat="server" ErrorMessage="Please enter a Title" ValidationGroup="tagPopup" Display="Static" EnableClientScript="true" ControlToValidate="txtTagTitle"></asp:RequiredFieldValidator></td>
            </tr>
            <tr>
                <asp:HiddenField ID="hfTagColourNum" runat="server" />
                <td class="tableLabel"><asp:Label ID="lblTagColour" class="label" runat="server" Text="Tag Colour" UseSubmitBehaviour="false" OnClientClick="return false;"></asp:Label></td>
                <td class="tableInput"><asp:Button ID="tagColourOne" class="tagOne" runat="server" Text="" UseSubmitBehaviour="false" OnClientClick="selectTagColour(1); return false;"/></td>
                <td class="tableInput"><asp:Button ID="tagColourTwo" class="tagTwo" runat="server" Text="" UseSubmitBehaviour="false" OnClientClick="selectTagColour(2); return false;"/></td>
                <td class="tableInput"><asp:Button ID="tagColourThree" class="tagThree" runat="server" Text="" UseSubmitBehaviour="false" OnClientClick="selectTagColour(3); return false;"/></td>
                <td class="tableInput"><asp:Button ID="tagColourFour" class="tagFour" runat="server" Text="" UseSubmitBehaviour="false" OnClientClick="selectTagColour(4); return false;"/></td>
                <td class="tableInput"><asp:Button ID="tagColourFive" class="tagFive" runat="server" Text="" UseSubmitBehaviour="false" OnClientClick=" selectTagColour(5);return false;"/></td>
                
            </tr>
            <tr>
                <td>
                </td>
                <td class="tableValidation" colspan="5"><asp:CustomValidator ID="validatorTagColour" runat="server" ErrorMessage="Please select a Tag Colour" ClientValidationFunction="validateTagColour" ValidationGroup="tagPopup" Display="Static" CssClass="validationError" OnServerValidate="validatorTagColour_ServerValidate"/></td>
            </tr>
        </table>

        <div class="buttonSection">
            <asp:Button ID="btnBackNewTag" class="button" runat="server" Text="Back" CausesValidation="False" OnClick="btnBack_Click" />
            <asp:Button ID="btnAddNewTag" class="button" runat="server" Text="Add" CausesValidation="true" OnClick="btnAddTag_Click" ValidationGroup="tagPopup" />
        </div>
    </div>
        <asp:HiddenField ID="hiddenSelectedTagColour" runat="server" />     


<script type="text/javascript">
    window.addEventListener('DOMContentLoaded', function () {
        const tagButtons = document.querySelectorAll('.tagOne, .tagTwo, .tagThree, .tagFour, .tagFive');
        const hiddenField = document.getElementById('<%= hiddenSelectedTagColour.ClientID %>');
        tagButtons.forEach(btn => {
            btn.addEventListener('click', function () {
                tagButtons.forEach(b => b.classList.remove('selectedTag'));
                this.classList.add('selectedTag');
                hiddenField.value = this.id;
            });
        });
    });

    function validateTagColour(sender, args) {
        var selected = document.getElementById('<%=hiddenSelectedTagColour.ClientID%>').value;
        args.IsValid = selected !== "";
    }
    function selectTagColour(colourNum) {
        document.getElementById('<%= hfTagColourNum.ClientID%>').value = colourNum;
}
</script>
</div>

</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">

</asp:Content>

