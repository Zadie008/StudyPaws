<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="B1400_View-shop.aspx.cs" Inherits="Default2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" Runat="Server">
    Pet Shop Cats
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
    <!--navigation to copy and paste-->
    <div class="collapsedNav">
        <div class="navbar">
            <asp:Menu ID="MenuLeft" runat="server" Orientation="Vertical" CssClass="nav-left" StaticDisplayLevels="1" StaticMenuItemStyle-CssClass="menu-item">
                <Items>
                    <asp:MenuItem NavigateUrl="~\B1600_View-dashboard.aspx" Text="Dashboard" Value="Dashboard"></asp:MenuItem>
                    <asp:MenuItem NavigateUrl="~\B100_View-calendar.aspx" Text="Calendar" Value="Calendar"></asp:MenuItem>
                    <asp:MenuItem NavigateUrl="~\B900_View-to-do-list.aspx" Text="To-do List" Value="To-do List"></asp:MenuItem>
                    <asp:MenuItem NavigateUrl="~\A100_Create-timer.aspx" Text="Timer" Value="Timer"></asp:MenuItem>
                    <asp:MenuItem NavigateUrl="~\A700_Create-study-session.aspx" Text="Study Session" Value="Study Session"></asp:MenuItem>
                </Items>
            </asp:Menu>
            <asp:Menu ID="MenuRight" runat="server" Orientation="Vertical" CssClass="nav-right" StaticDisplayLevels="1" StaticMenuItemStyle-CssClass="menu-item">
                <Items>
                    <asp:MenuItem NavigateUrl="~\B1400_View-shop.aspx" Text="Pet Shop" Value="Pet Shop"></asp:MenuItem>
                    <asp:MenuItem NavigateUrl="~\A1800_View-pets.aspx" Text="Inventory" Value="Inventory"></asp:MenuItem>
                    <asp:MenuItem NavigateUrl="~\C600_View-friend-list.aspx" Text="Friends" Value="Friends"></asp:MenuItem>
                    <asp:MenuItem NavigateUrl="~\C1400_View-badges.aspx" Text="Badges" Value="Badges"></asp:MenuItem>
                    <asp:MenuItem NavigateUrl="~\About.aspx" Text="About" Value="About"></asp:MenuItem>
                </Items>
            </asp:Menu>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="mainContentPlaceHolder" Runat="Server">
    <div id="viewShopMainContent">
        <asp:HiddenField ID="hfCurrentCategory" runat="server" Value="CAT" />
        <asp:HiddenField ID="hfSelectedPetID" runat="server"/>
        <asp:HiddenField ID="hfSelectedPetPrice" runat="server" />
        <h2>Welcome to the Pet Shop!</h2>
        <div class="backgroundColorContainer">
                <div class="leftCategorySection">
        <asp:Button ID="btnCats" runat="server" CssClass="button buttonSelected" Text="Cats" OnClick="btnCats_Click" />
        <asp:Button ID="btnDogs" runat="server" CssClass="button" Text="Dogs" OnClick="btnDogs_Click" />
        <asp:Button ID="btnFuzzy" runat="server" CssClass="button" Text="Fuzzy" OnClick="btnFuzzy_Click" />
        <asp:Button ID="btnFarm" runat="server" CssClass="button" Text="Farm" OnClick="btnFarm_Click" />
        <asp:Button ID="btnSpecial" runat="server" CssClass="button" Text="Special" OnClick="btnSpecial_Click" />
    </div>
    <div class="rightPetsSection">
        <table>
            <tr>
                <td><div class="petIconShop">
                        <div id="circle1" class="petCircleShop" runat="server"></div>
                        <asp:Image ID="imgPet1" runat="server" CssClass="petImageShop" ImageUrl="Images/Cat 1.png" Width="130" />
                    </div>
                </td>
                <td>
                    <div class="petIconShop">
                        <div id="circle2" class="petCircleShop" runat="server"></div>
                        <asp:Image ID="imgPet2" runat="server" CssClass="petImageShop" ImageUrl="Images/Cat 2.png" Width="130" />
                    </div>
                </td>
                <td>
                    <div class="petIconShop">
                        <div id="circle3" class="petCircleShop" runat="server"></div>
                        <asp:Image ID="imgPet3" runat="server" CssClass="petImageShop" ImageUrl="Images/Cat 3.png" Width="130" />
                    </div>
                </td>
                <td>
                    <div class="petIconShop">
                        <div id="circle4" class="petCircleShop" runat="server"></div>
                        <asp:Image ID="imgPet4" runat="server" CssClass="petImageShop" ImageUrl="Images/Cat 4.png" Width="130" />
                    </div>
                </td>
                <td>
                    <div class="petIconShop">
                        <div id="circle5" class="petCircleShop" runat="server"></div>
                        <asp:Image ID="imgPet5" runat="server" CssClass="petImageShop" ImageUrl="Images/Cat 5.png" Width="130" />
                    </div>
                </td>
            </tr>
            <tr>
                <td><asp:Button ID="btnSelect1" runat="server" CssClass="button" UseSubmitBehavior="false" OnClick="btnSelect_Clicked" /></td>
                <td><asp:Button ID="btnSelect2" runat="server" CssClass="button" UseSubmitBehavior="false" OnClick="btnSelect_Clicked" /></td>
                <td><asp:Button ID="btnSelect3" runat="server" CssClass="button" UseSubmitBehavior="false" OnClick="btnSelect_Clicked" /></td>
                <td><asp:Button ID="btnSelect4" runat="server" CssClass="button" UseSubmitBehavior="false" OnClick="btnSelect_Clicked" /></td>
                <td><asp:Button ID="btnSelect5" runat="server" CssClass="button" UseSubmitBehavior="false" OnClick="btnSelect_Clicked" /></td>
            </tr>
        </table>
    </div>
</div>
<div class="buttonSection">
    <div class="leftSection">
    </div>
    <div class="middleSection buttonRow">
        <asp:Button ID="btnBuy" class="button" runat="server" Text="Buy" Visible="false" OnClick="btnBuy_Click" />
        <asp:HiddenField ID="hfSelectedColourNum" runat="server" />
        <asp:Label ID="lblLocked" class="button" runat="server" Text="UNLOCKS AT LEVEL 1" Visible="false" />
    </div>
    <div class="rightSection">
    </div>
</div>
        </div>

    <!-- pop ups for pets -->
    <div id="alreadyOwnedPopup" class="simple-popup" style="display: none;">
    <div class="popup-pink-box">
        <asp:HiddenField ID="HiddenField1" runat="server" />
        <p>You already own this pet!</p>
        <img src="Images/Notification%20Happy.png" />
        <br />
        <div class="buttonSection">
            <asp:Button ID="btnCloseAlreadyOwned" CssClass="popup-button" runat="server" Text="Okay!" OnClick="btnCloseAlreadyOwned_Click" />
        </div>
    </div>
</div>

    <div id="confirmBuyPopup" class="simple-popup" style="display: none;">
    <div class="popup-pink-box">
        <p>Are you sure you want to buy this pet for</p>
        <br />
        <table id="popupSellPriceTable">
            <tr>
                <td>
                    <div class="pawIcon">
                        <img class="circle" src="Icons/icons8-circle-white-96.png" width="50" />
                        <img class="paw" src="Icons/icons8-cat-footprint-filled-white-96.png" width="30" />
                    </div>
                </td>
                <td><asp:Label ID="lblSellPrice" runat="server" Text="100"></asp:Label></td>
                <td>?</td>
            </tr>
        </table>
        <br />
        <img src="Images/Notification%20Happy.png" />

        <div class="buttonSection">
            <asp:Button ID="btnYesBuy" CssClass="popup-button-best-pink" runat="server" Text="Yes, I'm sure!" OnClick="btnYesBuy_Click" />
            <asp:Button ID="btnNoBuy" CssClass="popup-button" runat="server" Text="No, not sure!" OnClick="btnNoBuy_Click" />
        </div>
    </div>
</div>

    <div id="insufficientCoinsPopup" class="simple-popup" style="display: none;">
    <div class="popup-blue-box">
        <p>Sorry, you don't have enough pawprints!</p>
        <img src="Images/Notification%20Sad%20Hamster.png" />
        <br />
        <div class="buttonSection">
            <asp:Button ID="btnCloseInsufficientCoins" CssClass="popup-button" runat="server" Text="Okay!" OnClick="btnCloseInsufficientCoins_Click" />
        </div>
    </div>
</div>

    <!-- purchase success pop-up -->
    <div id="purchaseSuccessPopup" class="simple-popup" style="display: none;">
        <div class="popup-pink-box">
            <p>Congrats! You now own this pet!</p>
            <p>Do you want to go to your inventory?</p>
            <img src="Images/Notification%20Happy.png" />
            <br />
            <div class="buttonSection">
                <asp:Button ID="btnGoToInventory" CssClass="popup-button-best-pink" runat="server" Text="Inventory, GO!" OnClick="btnGoToInventory_Click" />
                <asp:Button ID="btnStayInShop" CssClass="popup-button" runat="server" Text="No, thanks!" OnClick="btnStayInShop_Click" />
            </div>
        </div>
    </div>

        <!--does not have notification-->
        <div id="popupNoNotifications" class="simple-popup" style="display: none;">
            <div class="popup-blue-box">
                <p>You do not have any notifications at the moment!</p>
                <img src="Images/Notification%20Sad%20Hamster.png" />
                <div class="buttonSection">
                    <asp:Button ID="Button1" CssClass="popup-button" runat="server" Text="Okay!" OnClientClick="hideNotificationPopup(); return false;" />
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
                    <asp:Button ID="Button2" CssClass="popup-button-best-pink" runat="server" Text="Accept!" OnClick="btnYes_Click" />
                    <asp:Button ID="Button3" CssClass="popup-button" runat="server" Text="Decline!" OnClick="btnNo_Click" />
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

        <div id="popup" class="simple-popup" style="display: none;">
            <div class="popup-pink-box">
                <asp:HiddenField ID="hiddenJoinSessionID" runat="server" />
                <p>Study Session has started!</p>
                <img src="Images/Notification%20Happy.png" />
                <br />
                <div class="buttonSection">
                    <asp:Button ID="btnJoin" CssClass="popup-button" runat="server" Text="Join!" OnClick="btnJoin_Click" />
                </div>
            </div>
        </div>
    <script>
        function showAlreadyOwnedPopup() {
            var popup = document.getElementById('alreadyOwnedPopup');
            if (popup) popup.style.display = 'flex';
        }
        function hideAlreadyOwnedPopup() {
            var popup = document.getElementById('alreadyOwnedPopup');
            if (popup) popup.style.display = 'none';
        }
        function showConfirmBuyPopup() {
            var popup = document.getElementById('confirmBuyPopup');
            if (popup) popup.style.display = 'flex';
        }
        function hideConfirmBuyPopup() {
            var popup = document.getElementById('confirmBuyPopup');
            if (popup) popup.style.display = 'none';
        }
        function showInsufficientCoinsPopup() {
            var popup = document.getElementById('insufficientCoinsPopup');
            if (popup) popup.style.display = 'flex';
        }
        function hideInsufficientCoinsPopup() {
            var popup = document.getElementById('insufficientCoinsPopup');
            if (popup) popup.style.display = 'none';
        }
        function showPurchaseSuccessPopup() {
            var popup = document.getElementById('purchaseSuccessPopup');
            if (popup) popup.style.display = 'flex';
        }
        function hidePurchaseSuccessPopup() {
            var popup = document.getElementById('purchaseSuccessPopup');
            if (popup) popup.style.display = 'none';
        }
    </script>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">

</asp:Content>

