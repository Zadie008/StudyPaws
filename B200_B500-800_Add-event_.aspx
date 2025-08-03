<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="B200_B500-800_Add-event_.aspx.cs" Inherits="Default2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" Runat="Server">
    Add event
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" Runat="Server">
        <!--account info to copy and paste-->
    <div class="accountInfoDiv">
        <div class="profileDiv">
            <a href="C100-C500_Profile.aspx" class="profileIconLink">
                <div class="profileIcon">
                    <div id="profileCircle"></div>
                    <asp:Image ID="profilePet" runat="server" ImageUrl="~/Images/Cat 1.png" />
                </div>
            </a>
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

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="mainContentPlaceHolder" Runat="Server">
    <div class="createEventMainContent">
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
                        <asp:DropDownList ID="dropdownEventTag" ClientIDMode="Static" class="dropDownList" runat="server" BackColor="#446791" DataTextField="tagName" DataValueField="tagID">
                        </asp:DropDownList>
                        <asp:HiddenField ID="hiddenSelectedTagID" runat="server" />
                    </td>
                </tr>
            </table>
        </div>
        <div class="rightSection">
            <table>
                <tr>
                    <td></td>
                    <td><asp:RequiredFieldValidator ID="errorTitle" class="validationError" runat="server" ErrorMessage="Please enter a Title" EnableClientScript="true" ControlToValidate="txtEventTitle"></asp:RequiredFieldValidator></td>
                </tr>
                <tr>
                    <td><asp:ImageButton ID="btnAddTag" runat="server" CommandName="AddTag" CausesValidation="false" class="addTagBtn" OnClick="btnNewTag_Click" ImageUrl="~/Icons/icons8-add-new-white-96.png" /></td>
                    <td><asp:ImageButton ID="btnEditTag" runat="server" CausesValidation="false" CommandArgument='<%# Eval ("tagID") %>' CssClass="addTagBtn" ImageUrl="~/Icons/icons8-edit-white-96.png" OnClick="btnEditTag_Click"/></td>
                    <td><asp:RequiredFieldValidator ID="errorDropDown" class="validationError" runat="server" ErrorMessage="Please select a Tag" EnableClientScript="true" ControlToValidate="dropdownEventTag"></asp:RequiredFieldValidator></td>
                </tr>
            </table>
        </div>
    </div>
    <div class="buttonSection">
        <div class="leftSection">
        </div>
        <div class="middleSection">
            <asp:Button ID="btnBack" class="button" runat="server" Text="Back" OnClick="btnBack_Click" CausesValidation="False" />
            <asp:Button ID="btnAdd" class="button" runat="server" Text="Add" OnClick="btnAdd_Click" />
        </div>
        <div class="rightSection">
            <asp:Button ID="btnViewPastTimers" class="button" runat="server" Text="View past timers" Visible="False" /> <!--invisible but for correct spacing of other buttons-->
        </div>
    </div>

    <div id="popup1" class="simple-popup" style="display: none;">
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
            <asp:Button ID="btnBackNewTag" class="button" runat="server" Text="Back" CausesValidation="False" OnClientClick="hidePopup(); return false;" />
            <asp:Button ID="btnAddNewTag" class="button" runat="server" Text="Add" CausesValidation="true" OnClick="btnAddTag_Click" ValidationGroup="tagPopup" />
        </div>
    </div>
        <asp:HiddenField ID="hiddenSelectedTagColour" runat="server" />     
</div>

        <!-- EDIT AND DELETE TAG POPUP-->
            <div id="popup2" class="simple-popup" style="display: none;">
                <div class="popup-pink-boxTagEdit">
                    <table class="popupTagEditing">
                        <tr>
                            <td class="tableLabel"><asp:Label ID="lblTagTitleEdit" class="label" runat="server" Text="Tag Title"></asp:Label></td>
                            <td class="tableInput" colspan="5"><asp:TextBox ID="txtTagTitleEdit" class="textbox" runat="server" ValidationGroup="tagPopup"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td class="tabelLabel"></td>
                            <td class="tableValidation" colspan="5"><asp:RequiredFieldValidator ID="errorTitleTagEdit" class="validationError" runat="server" ErrorMessage="Please enter a Title" ValidationGroup="tagPopup" Display="Static" EnableClientScript="true" ControlToValidate="txtTagTitle"></asp:RequiredFieldValidator></td>
                        </tr>
                        <tr>
                            <asp:HiddenField ID="hfEditTagColourNum" runat="server" />
                            <td class="tableLabel"><asp:Label ID="lblTagColourEdit" class="label" runat="server" Text="Tag Colour" UseSubmitBehaviour="false" OnClientClick="return false;"></asp:Label></td>
                            <td class="tableInput"><asp:Button ID="tagColourOneEdit" class="tagOne" runat="server" Text="" UseSubmitBehaviour="false" OnClientClick="selectTagColour(1); return false;"/></td>
                            <td class="tableInput"><asp:Button ID="tagColourTwoEdit" class="tagTwo" runat="server" Text="" UseSubmitBehaviour="false" OnClientClick="selectTagColour(2); return false;"/></td>
                            <td class="tableInput"><asp:Button ID="tagColourThreeEdit" class="tagThree" runat="server" Text="" UseSubmitBehaviour="false" OnClientClick="selectTagColour(3); return false;"/></td>
                            <td class="tableInput"><asp:Button ID="tagColourFourEdit" class="tagFour" runat="server" Text="" UseSubmitBehaviour="false" OnClientClick="selectTagColour(4); return false;"/></td>
                            <td class="tableInput"><asp:Button ID="tagColourFiveEdit" class="tagFive" runat="server" Text="" UseSubmitBehaviour="false" OnClientClick=" selectTagColour(5);return false;"/></td>
                        </tr>
                        <tr>
                            <td class="tabelLabel"></td>
                            <td class="tableValidation" colspan="5"><asp:CustomValidator ID="validatorTagColourEdit" runat="server" ErrorMessage="Please select a Tag Colour" ClientValidationFunction="validateTagColour" ValidationGroup="tagPopup" Display="Static" CssClass="validationError" OnServerValidate="validatorTagColour_ServerValidate"/></td>
                        </tr>
                    </table>
                    <div class="buttonSection">
                        <asp:Button ID="btnBackEditTag" class="button" runat="server" Text="Back" CausesValidation="False" OnClientClick="hidePopup2(); return false;" />
                        <asp:Button ID="btnDeleteTag" class="button" runat="server" Text="Delete" CausesValidation="false" OnClick="btnDeleteTag_Click" />
                        <asp:Button ID="btnSaveEditTag" class="button" runat="server" Text="Save" CausesValidation="true" OnClick="btnAddTag_Click" ValidationGroup="tagPopup" />
                    </div>
                </div>
                <asp:HiddenField ID="hiddenSelectedTagColourEdit" runat="server" />     
            </div>

        <!--DELETE TAG POP-UP-->
        <div id="popupDeleteTag" class="simple-popup" style="display: none;">
            <div class="popup-blue-box">
                <p>Are you sure you want to delete this tag?</p>
                <img src="Images/Notification%20Sad%20Hamster.png" />
                <br />
                <div class="buttonSection">
                    <asp:Button ID="btnYesDelete" CssClass="popup-button" runat="server" Text="Yes, I'm sure!" OnClientClick="hideDeletePopup(); return false;" OnClick="btnYesDelete_Click" />
                    <asp:Button ID="btnNo" CssClass="popup-button-best-blue" runat="server" Text="No, not sure!" OnClientClick="hideDeletePopup(); return false;"  />
                </div>
            </div>
            </div>
                    

        <script type="text/javascript">
            function showPopup1() {
                document.getElementById('popup1').style.display = 'flex';
                clearPopup();
            }

            function hidePopup() {
                document.getElementById('popup').style.display = 'none';
            }
            function hidePopup2() {
                document.getElementById('popup2').style.display = 'none';
            }
            function hideDeletePopup() {
                document.getElementById('popupDeleteTag').style.display = 'none';
            }
            function clearPopup() {
                document.getElementById('<%=txtTagTitle.ClientID%>').value = "";
        const hiddenField = document.getElementById('<%= hiddenSelectedTagColour.ClientID %>');
                const tagButtons = document.querySelectorAll('.tagOne, .tagTwo, .tagThree, .tagFour, .tagFive');
                tagButtons.forEach(btn => {
                    btn.classList.remove('selectedTag');
                    hiddenField.value = "";
                });
            }
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
    window.addEventListener('DOMContentLoaded', function () {
        const dropdownList = document.getElementById('dropdownEventTag');
        const btnAdd = document.getElementById('<%= btnAddTag.ClientID%>');
        const btnEdit = document.getElementById('<%= btnEditTag.ClientID%>');

        dropdownList.addEventListener('change', function () {
            if (dropdownList.selectedIndex === 0 || dropdownList.value === "") {
                btnAdd.style.display = 'inline-block';
                btnEdit.style.display = 'none';
            }
            else {
                btnAdd.style.display = 'none';
                btnEdit.style.display = 'inline-block';
            }
        });
        dropdownList.dispatchEvent(new Event('change'));
    });
            function showPopup2() {
                document.getElementById('popup2').style.display = 'flex';
            }
            function showDeletePopup() {
                document.getElementById('popupDeleteTag').style.display = 'flex';
            }

        </script>
</div>


</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">

</asp:Content>

