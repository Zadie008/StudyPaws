<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" Runat="Server">
    StudyPaws
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" Runat="Server"> 
    <div class="curved-header">
        <svg viewBox="0 0 700 150" xmlns="http://www.w3.org/2000/svg">
        <defs>
      <path id="curve" d="M50,120 Q350,20 650,120" />
    </defs>
    <text>
      <textPath href="#curve" startOffset="50%" text-anchor="middle">
        StudyPaws
      </textPath>
    </text>
  </svg>
        <div style="height: 50px;"></div>
                <h2 class="subtitle">purrfectly productive</h2>
        </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="mainContentPlaceHolder" Runat="Server">
     <img src="Images/Notification%20Happy.png" id="logo" width="470" />
     <img src="Icons/icons8-cursor-white-96.png" class="icon" width="30" />
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="navContent" Runat="Server">
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">

</asp:Content>