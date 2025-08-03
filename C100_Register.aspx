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
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
    <div id="registerPageDiv">
        <asp:Panel ID="registerPanel" runat="server" DefaultButton="btnRegister">
            <table>
                <tr>
                    <td><asp:Label ID="lblUsername" class="label" runat="server" Text="Username"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="txtUsername" class="textbox" runat="server" AutoPostBack="false" onkeyup="checkUsernameAvailability()"></asp:TextBox>
                        <asp:Label ID="lblUsernameAvailability" runat="server" CssClass="errorLabel" Style="display:none;"></asp:Label>
                        <asp:CustomValidator ID="cvUsername" runat="server" ControlToValidate="txtUsername" OnServerValidate="cvUsername_ServerValidate" EnableClientScript="false" ErrorMessage="Sorry! this username already exists" ForeColor="Red" Display="Dynamic"></asp:CustomValidator>
                    </td>
                </tr>
                <tr>
                    <td><asp:Label ID="lblPassword" class="label" runat="server" Text="Password"></asp:Label></td>
                    <td><asp:TextBox ID="txtPassword" class="textbox" runat="server" TextMode="Password"></asp:TextBox></td>
                </tr>
                <tr>
                    <td><asp:Label ID="lblConfirmPassword" class="label" runat="server" Text="Confirm Password"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="txtConfirmPassword" class="textbox" runat="server" TextMode="Password"></asp:TextBox>
                        <asp:Label ID="lblPasswordMismatch" runat="server" Text="Password does not match" CssClass="errorLabel" Visible="false" />
                    </td>
                </tr>
            </table>
            <br />
            <div class="buttonSection">
                <asp:Button ID="btnBack" class="button" runat="server" Text="Back" OnClick="btnBack_Click" />
                <asp:Button ID="btnRegister" class="button" runat="server" Text="Register" OnClick="btnRegister_Click1" />
            </div>
        </asp:Panel>

        <%-- Panel for successful registration confirmation --%>
        <asp:Panel ID="pnlConfirm" runat="server" Visible="false">
            <div id="popupConfirm" class="simple-popup">
                <div class="popup-pink-box">
                    <p>You have been registered!</p>
                    <img src="Images/Notification%20Happy.png" />
                    <br />
                    <div class="buttonSection">
                        <asp:Button ID="btnOkay" CssClass="popup-button" runat="server" Text="Okay!" OnClick="btnOkay_Click" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <%-- Panel for tutorial prompt --%>
        <asp:Panel ID="pnlTut" runat="server" Visible="false">
            <div id="popupTut" class="simple-popup">
                <div class="popup-pink-box">
                    <p>Would you like to learn how to use the home page?</p>
                    <img src="Images/Notification%20Happy.png" />
                    <br />
                    <div class="buttonSection">
                        <asp:Button ID="btnWatchtut" CssClass="popup-button-best-pink" runat="server" Text="Yes, please!" OnClientClick="hidePanel(pnlTutClientID);" OnClick="btnWatchtut_Click" />
                        <asp:Button ID="BtnNotut" CssClass="popup-button" runat="server" Text="No, thank you!" OnClientClick="hidePanel(pnlTutClientID);" OnClick="BtnNotut_Click" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <%-- Panel for "Username Already Exists" error --%>
        <asp:Panel ID="pnlProfileExists" runat="server" Visible="false">
            <div id="popupProfileExists" class="simple-popup">
                <div class="popup-blue-box">
                    <p>Sorry! This user already exists</p>
                    <img src="Images/Notification%20Sad%20Hamster.png" />
                    <br />
                    <div class="buttonSection">
                       <asp:Button ID="btnUnderstandExists" CssClass="popup-button" runat="server" Text="I understand :(" OnClick="btnUnderstandExists_Click" />
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>

  
    <script type="text/javascript">
        const txtUsernameClientID = '<%= txtUsername.ClientID %>';
        const lblUsernameAvailabilityClientID = '<%= lblUsernameAvailability.ClientID %>';
        const pnlConfirmClientID = '<%= pnlConfirm.ClientID %>';
        const pnlTutClientID = '<%= pnlTut.ClientID %>';
        const pnlProfileExistsClientID = '<%= pnlProfileExists.ClientID %>';
        const btnUnderstandExistsClientID = '<%= btnUnderstandExists.ClientID %>'; 
    </script>
    <script type="text/javascript" src="Scripts/your_main_script_file.js"></script>

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="navContent" Runat="Server">
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">
</asp:Content>