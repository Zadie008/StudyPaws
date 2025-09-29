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
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
        <ContentTemplate>
            <div id="registerPageDiv">
                <asp:Panel ID="registerPanel" runat="server" DefaultButton="btnRegister">
                    <table>
                        <tr>
                            <td><asp:Label ID="lblUsername" class="label" runat="server" Text="Username"></asp:Label></td>
                            <td>
                                <asp:TextBox ID="txtUsername" class="textbox" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvUsername" runat="server" ControlToValidate="txtUsername" 
                                    ErrorMessage="Username is required" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td><asp:Label ID="lblPassword" class="label" runat="server" Text="Password"></asp:Label></td>
                            <td>
                                <asp:TextBox ID="txtPassword" class="textbox" runat="server" TextMode="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword" 
                                    ErrorMessage="Password is required" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td><asp:Label ID="lblConfirmPassword" class="label" runat="server" Text="Confirm Password"></asp:Label></td>
                            <td>
                                <asp:TextBox ID="txtConfirmPassword" class="textbox" runat="server" TextMode="Password"></asp:TextBox>
                                <asp:Label ID="lblPasswordMismatch" runat="server" Text="Password does not match" CssClass="errorLabel" Visible="false" />
                                <asp:RequiredFieldValidator ID="rfvConfirmPassword" runat="server" ControlToValidate="txtConfirmPassword" 
                                    ErrorMessage="Confirm Password is required" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <div class="buttonSection">
                        <asp:Button ID="btnBack" class="button" runat="server" Text="Back" OnClick="btnBack_Click" CausesValidation="false" />
                        <asp:Button ID="btnRegister" class="button" runat="server" Text="Register" OnClick="btnRegister_Click1" />
                    </div>
                </asp:Panel>

                <%-- Panel for successful registration confirmation --%>
                <asp:Panel ID="pnlConfirm" runat="server" Visible="false">
                    <div id="popupConfirm" class="simple-popup">
                        <div class="popup-pink-box">
                            <p>You have been registered!</p>
                            <img src="Images/Notification%20Happy.png" alt="Success" />
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
                            <img src="Images/Notification%20Happy.png" alt="Tutorial" />
                            <br />
                            <div class="buttonSection">
                                <asp:Button ID="btnWatchtut" CssClass="popup-button-best-pink" runat="server" Text="Yes, please!" OnClick="btnWatchtut_Click" />
                                <asp:Button ID="BtnNotut" CssClass="popup-button" runat="server" Text="No, thank you!" OnClick="BtnNotut_Click" />
                            </div>
                        </div>
                    </div>
                </asp:Panel>

                <%-- Panel for "Username Already Exists" error --%>
                <asp:Panel ID="pnlProfileExists" runat="server" Visible="false">
                    <div id="popupProfileExists" class="simple-popup">
                        <div class="popup-blue-box">
                            <p>Sorry! This user already exists</p>
                            <img src="Images/Notification%20Sad%20Hamster.png" alt="Error" />
                            <br />
                            <div class="buttonSection">
                               <asp:Button ID="btnUnderstandExists" CssClass="popup-button" runat="server" Text="I understand :(" OnClick="btnUnderstandExists_Click" />
                            </div>
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnRegister" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnOkay" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnWatchtut" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="BtnNotut" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnUnderstandExists" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="navContent" Runat="Server">
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">
</asp:Content>