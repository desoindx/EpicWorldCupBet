﻿<%@ Page Title="Position" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Positions.aspx.cs" Inherits="Positions" %>

<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <% if (Context.User.Identity.IsAuthenticated)
       { %>
    <link rel="stylesheet" href="Content/slick.grid.css" type="text/css" />
    <link rel="stylesheet" href="Content/slick-default-theme.css" type="text/css" />
    <script src="Scripts/SlickGrid/Plugins/slick.cellrangedecorator.js"></script>
    <script src="Scripts/SlickGrid/Plugins/slick.cellrangeselector.js"></script>
    <script src="Scripts/SlickGrid/Plugins/slick.cellnewcopymanager.js"></script>
    <script src="Scripts/SlickGrid/Plugins/slick.cellselectionmodel.js"></script>
    <script src="JavaScript/Positions.js"></script>
    <div style="margin-top: 50px;" id='<%: "Div-" + Master.GetCompetitionId() %>'>
        <div style="position: absolute; left: 50%; margin-left: -320px;">
            <div style="float: left; width: 690px; height: 900px;" id='<%: "PositionsDiv-" + Master.GetCompetitionId() %>'>
            </div>
        </div>
    </div>
    <script type='text/javascript'>
        drawPositionGrid(<%: Master.GetCompetitionId()%>,<%: GetPositions(Master.GetCompetitionId())%>);
    </script>
    <% } %>
</asp:Content>
