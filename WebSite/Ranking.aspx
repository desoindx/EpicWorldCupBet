<%@ Page Title="Ranking" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Ranking.aspx.cs" Inherits="Ranking" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <link rel="stylesheet" href="Content/slick.grid.css" type="text/css" />
    <link rel="stylesheet" href="Content/slick-default-theme.css" type="text/css" />
    <script src="Scripts/SlickGrid/Plugins/slick.cellrangedecorator.js"></script>
    <script src="Scripts/SlickGrid/Plugins/slick.cellrangeselector.js"></script>
    <script src="Scripts/SlickGrid/Plugins/slick.cellnewcopymanager.js"></script>
    <script src="Scripts/SlickGrid/Plugins/slick.cellselectionmodel.js"></script>
    <script src="JavaScript/Ranking.js"></script>
    <h3>Please note that the Profit is only an estimation and is subject to a reevaluation as the competition goes on !!!</h3>
    <div style="width: 600px; height: 1000px; margin-top: 50px" id="RankingDiv">
    </div>
    <script type='text/javascript'>
        drawRankingGrid(<%: GetRanks()%>);
    </script>
</asp:Content>
