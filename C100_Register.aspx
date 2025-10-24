<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="C100_Register.aspx.cs" Inherits="C100_Register" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" Runat="Server">
    Register
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" Runat="Server">
    <div class="loginRegisterCurvedHeader">
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
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="mainContentPlaceHolder" Runat="Server">
    <br />
    <br />
    
    <!-- JavaScript Functions -->
    <script type="text/javascript">
        function showPanel(panelId) {
            var panel = document.getElementById(panelId);
            if (panel) {
                panel.style.display = 'flex';
                console.log('Showing panel: ' + panelId);
            } else {
                console.log('Panel not found: ' + panelId);
            }
        }

        function hidePanel(panelId) {
            var panel = document.getElementById(panelId);
            if (panel) {
                panel.style.display = 'none';
                console.log('Hiding panel: ' + panelId);
            }
        }

        function hideAllPanels() {
            hidePanel('<%= pnlConfirm.ClientID %>');
            hidePanel('<%= pnlTut.ClientID %>');
            hidePanel('<%= pnlProfileExists.ClientID %>');
            var mismatchLabel = document.getElementById('<%= lblPasswordMismatch.ClientID %>');
            if (mismatchLabel) {
                mismatchLabel.style.display = 'none';
            }
        }

        function showPasswordMismatch() {
            var mismatchLabel = document.getElementById('<%= lblPasswordMismatch.ClientID %>');
            if (mismatchLabel) {
                mismatchLabel.style.display = 'inline';
                mismatchLabel.style.visibility = 'visible';
            }
        }

        function hidePasswordMismatch() {
            var mismatchLabel = document.getElementById('<%= lblPasswordMismatch.ClientID %>');
            if (mismatchLabel) {
                mismatchLabel.style.display = 'none';
            }
        }

        // Client-side click handlers for popup buttons
        function onOkayClick() {
            hidePanel('<%= pnlConfirm.ClientID %>');
            showPanel('<%= pnlTut.ClientID %>');
            return false; // Prevent postback
        }

        function onUnderstandExistsClick() {
            hidePanel('<%= pnlProfileExists.ClientID %>');
            return false; // Prevent postback
        }

        function onWatchTutClick() {
            hidePanel('<%= pnlTut.ClientID %>');
            // Allow the postback to happen for redirect
            return true;
        }

        function onNoTutClick() {
            hidePanel('<%= pnlTut.ClientID %>');
            // Allow the postback to happen for redirect
            return true;
        }

        // Password visibility toggle functions
        function togglePasswordVisibility(fieldId, iconId) {
            var passwordField = document.getElementById(fieldId);
            var eyeIcon = document.getElementById(iconId);

            if (passwordField.type === 'password') {
                // Show password
                passwordField.type = 'text';
                eyeIcon.src = 'Icons/icons8-invisible-white-96.png'; 
                eyeIcon.style.opacity = '1';
            } else {
                // Hide password
                passwordField.type = 'password';
                eyeIcon.src = 'Icons/icons8-eye-white-96.png';
                eyeIcon.style.opacity = '0.6';
            }
        }

        function togglePassword() {
            togglePasswordVisibility('<%= txtPassword.ClientID %>', 'passwordToggle');
        }

        function toggleConfirmPassword() {
            togglePasswordVisibility('<%= txtConfirmPassword.ClientID %>', 'confirmPasswordToggle');
        }
    </script>

    <div id="registerPageDiv">
        <div class="leftSection">
            <p class="backgroundColorText">invisible p</p>
        </div>
        <div class="middleSection">
            <asp:Panel ID="registerPanel" runat="server" DefaultButton="btnRegister">
                <table>
                    <tr>
                        <td><asp:Label ID="lblUsername" class="label" runat="server" Text="Username"></asp:Label></td>
                        <td>
                            <asp:TextBox ID="txtUsername" class="textbox" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td><asp:Label ID="lblPassword" class="label" runat="server" Text="Password"></asp:Label></td>
                        <td>
                            <div style="position: relative; display: inline-block;">
                                <asp:TextBox ID="txtPassword" class="textbox" runat="server" TextMode="Password" style="padding-right: 40px; width: 100%;"></asp:TextBox>
                                <img id="passwordToggle" src="Icons/icons8-eye-white-96.png" 
                                     style="position: absolute; right: 10px; top: 50%; transform: translateY(-50%); cursor: pointer; width: 24px; height: 24px; opacity: 0.6;" 
                                     onclick="togglePassword()" 
                                     title="Show Password" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td><asp:Label ID="lblConfirmPassword" class="label" runat="server" Text="Confirm Password"></asp:Label></td>
                        <td>
                            <div style="position: relative; display: inline-block;">
                                <asp:TextBox ID="txtConfirmPassword" class="textbox" runat="server" TextMode="Password" style="padding-right: 40px; width: 100%;"></asp:TextBox>
                                <img id="confirmPasswordToggle" src="Icons/icons8-eye-white-96.png" 
                                     style="position: absolute; right: 10px; top: 50%; transform: translateY(-50%); cursor: pointer; width: 24px; height: 24px; opacity: 0.6;" 
                                     onclick="toggleConfirmPassword()" 
                                     title="Show Password" />
                            </div>
                        </td>
                    </tr>
                </table>
                <br />
                <div class="buttonSection">
                    <asp:Button ID="btnBack" class="loginButton" runat="server" Text="Back" OnClick="btnBack_Click" CausesValidation="false" />
                    <asp:Button ID="btnRegister" class="loginButton" runat="server" Text="Register" OnClick="btnRegister_Click1" />
                </div>
            </asp:Panel>
        </div>

        <div class="rightSection">
            <div class="validationSection">
                <asp:RequiredFieldValidator ID="rfvUsername" runat="server" ControlToValidate="txtUsername" ErrorMessage="Username is required" CssClass="validationErrorCustom" ForeColor="white" Display="Dynamic"></asp:RequiredFieldValidator>
                <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword" ErrorMessage="Password is required" CssClass="validationErrorCustom" ForeColor="white" Display="Dynamic"></asp:RequiredFieldValidator>
                <asp:Label ID="lblPasswordMismatch" runat="server" Text="Password does not match" CssClass="validationErrorCustom" style="display: none; color: white;" />
                <asp:RequiredFieldValidator ID="rfvConfirmPassword" runat="server" ControlToValidate="txtConfirmPassword" 
                    ErrorMessage="Confirm Password is required" CssClass="validationErrorCustom" ForeColor="white" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>
        </div>
        

        <%-- Panel for successful registration confirmation --%>
        <asp:Panel ID="pnlConfirm" runat="server" CssClass="simple-popup" style="display: none;">
            <div class="popup-pink-box">
                <p>You have been registered!</p>
                <img src="Images/Notification%20Happy.png" alt="Success" />
                <br />
                <div class="buttonSection">
                    <%-- Use OnClientClick instead of OnClick for client-side handling --%>
                    <asp:Button ID="btnOkay" CssClass="popup-button" runat="server" Text="Okay!" OnClientClick="return onOkayClick();" />
                </div>
            </div>
        </asp:Panel>

        <%-- Panel for tutorial prompt --%>
        <asp:Panel ID="pnlTut" runat="server" CssClass="simple-popup" style="display: none;">
            <div class="popup-pink-box">
                <p>Would you like to learn how to use the home page?</p>
                <img src="Images/Notification%20Happy.png" alt="Tutorial" />
                <br />
                <div class="buttonSection">
                    <asp:Button ID="btnWatchtut" CssClass="popup-button-best-pink" runat="server" Text="Yes, please!" OnClick="btnWatchtut_Click" OnClientClick="return onWatchTutClick();" />
                    <asp:Button ID="BtnNotut" CssClass="popup-button" runat="server" Text="No, thank you!" OnClick="BtnNotut_Click" OnClientClick="return onNoTutClick();" />
                </div>
            </div>
        </asp:Panel>

        <%-- Panel for "Username Already Exists" error --%>
        <asp:Panel ID="pnlProfileExists" runat="server" CssClass="simple-popup" style="display: none;">
            <div class="popup-blue-box">
                <p>Sorry! This user already exists</p>
                <img src="Images/Notification%20Sad%20Hamster.png" alt="Error" />
                <div class="buttonSection">
                   <%-- Use OnClientClick instead of OnClick for client-side handling --%>
                   <asp:Button ID="btnUnderstandExists" CssClass="popup-button" runat="server" Text="Okay" OnClientClick="return onUnderstandExistsClick();" />
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="navContent" Runat="Server">
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">
</asp:Content>