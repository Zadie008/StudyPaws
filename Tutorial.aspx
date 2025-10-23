<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Tutorial.aspx.cs" Inherits="Tutorial" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" Runat="Server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="navContent" Runat="Server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="mainContentPlaceHolder" Runat="Server">
    <div class="tutorial-container">
        <img src="Images/Tutorial/tutorial.png" class="fullscreen-img" alt="Tutorial Screen" />
        <div class="button-container">
            <a href="Default.aspx" class="tutorial-button">Skip</a>
            <a href="TipsAndTricks.aspx" class="tutorial-button">Next</a>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="footerContentPlaceHolder" Runat="Server">
</asp:Content>