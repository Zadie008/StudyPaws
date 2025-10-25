<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" Runat="Server">
    Login
</asp:Content>

<%--please copy this part if you guys want the study paws header--%>
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
<%--above is the header of studypaws--%>

<asp:Content ID="Content3" ContentPlaceHolderID="mainContentPlaceHolder" Runat="Server">
    <br />
    <br />
    <asp:Panel ID="loginPanel" runat="server" DefaultButton="btnLogin">
        <div id="loginPageDiv">
            <table>
                <tr>
                    <td><asp:Label ID="lblUsername" class="label" runat="server" Text="Username"></asp:Label></td>
                    <td><asp:TextBox ID="txtUsername" class="textbox" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td><asp:Label ID="lblPassword" class="label" runat="server" Text="Password"></asp:Label></td>
                    <td>
                        <div style="position: relative; display: inline-block;">
                            <asp:TextBox ID="txtPassword" class="textbox" runat="server" TextMode="Password" style="padding-right: 40px;"></asp:TextBox>
                            <img id="passwordToggle" src="Icons/icons8-eye-white-96.png" 
                                 style="position: absolute; right: 10px; top: 50%; transform: translateY(-50%); cursor: pointer; width: 24px; height: 24px; opacity: 0.6;" 
                                 onclick="togglePasswordVisibility()" 
                                  />
                        </div>
                    </td>
                </tr>
            </table>
            <br />
            <div class="buttonSection">
                <asp:Button ID="btnBack" class="loginButton" runat="server" Text="Back" OnClick="btnBack_Click" />
                <asp:Button ID="btnLogin" class="loginButton" runat="server" Text="Login" OnClick="btnLogin_Click" />
            </div>

            <div id="popup" class="simple-popup" style="display: none;">
                <div class="popup-blue-box">
                    <p>Sorry! Your username or password is incorrect</p>
                    <img src="Images/Notification%20Sad%20Hamster.png" alt="Sad hamster" />
                    <div class="buttonSection">
                        <asp:Button ID="btnOkay" CssClass="popup-button" runat="server" Text="Okay!" OnClientClick="hidePopup(); return false;" />
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>

    

    <script type="text/javascript">
    function showPopup() {
        document.getElementById('popup').style.display = 'flex';
    }
    
    function hidePopup() {
        document.getElementById('popup').style.display = 'none';
        }
        function togglePasswordVisibility() {
            var passwordField = document.getElementById('<%= txtPassword.ClientID %>');
         var eyeIcon = document.getElementById('passwordToggle');

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
    </script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="navContent" Runat="Server">
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">

</asp:Content>