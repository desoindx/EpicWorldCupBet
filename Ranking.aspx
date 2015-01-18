<%@ Page Title="Ranking" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Ranking.aspx.cs" Inherits="Ranking" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <link rel="stylesheet" href="Content/slick.grid.css" type="text/css" />
    <link rel="stylesheet" href="Content/slick-default-theme.css" type="text/css" />
    <script src="Scripts/jquery-1.7.2.min.js"></script>
    <script src="Scripts/jquery.event.drag.js"></script>
    <script src="Scripts/SlickGrid/slick.core.js"></script>
    <script src="Scripts/SlickGrid/slick.grid.js"></script>
    <script src="Scripts/SlickGrid/Plugins/slick.cellrangedecorator.js"></script>
    <script src="Scripts/SlickGrid/Plugins/slick.cellrangeselector.js"></script>
    <script src="Scripts/SlickGrid/Plugins/slick.cellnewcopymanager.js"></script>
    <script src="Scripts/SlickGrid/Plugins/slick.cellselectionmodel.js"></script>
    <script type="text/javascript" src="Scripts/jquery.signalR-2.0.3.js"></script>
    <script type="text/javascript" src="../signalr/hubs"></script>
    <script src="JavaScript/Ranking.js"></script>

    <div style="width: 400px; height: 1000px;" id="RankingDiv">
    </div>
    <script type='text/javascript'>
        drawRankingGrid(<%: GetRanks()%>);
    </script>
</asp:Content>
