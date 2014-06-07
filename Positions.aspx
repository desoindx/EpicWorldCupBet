<%@ Page Title="Position" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Positions.aspx.cs" Inherits="Positions" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <link rel="stylesheet" href="Content/slick.grid.css" type="text/css" />
    <script src="Scripts/jquery-1.7.2.min.js"></script>
    <script src="Scripts/jquery.event.drag.js"></script>
    <script src="Scripts/SlickGrid/slick.core.js"></script>
    <script src="Scripts/SlickGrid/slick.grid.js"></script>
    <script type="text/javascript" src="Scripts/jquery.signalR-2.0.3.js"></script>
    <script type="text/javascript" src="../signalr/hubs"></script>
    <script src="JavaScript/Positions.js"></script>
    <script src="JavaScript/SignalRPosition.js"></script>

    <div style="width: 5000px; height: 1200px;" id="PositionsDiv"><label>Loading in progress</label></div>
</asp:Content>
