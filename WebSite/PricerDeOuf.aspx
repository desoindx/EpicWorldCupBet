<%@ Page Title="PricerDeOuf" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="PricerDeOuf.aspx.cs" Inherits="PricerDeOuf" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <link rel="stylesheet" href="Content/slick.grid.css" type="text/css" />
    <link rel="stylesheet" href="Content/slick-default-theme.css" type="text/css" />
    <script src="Scripts/SlickGrid/Plugins/slick.cellrangedecorator.js"></script>
    <script src="Scripts/SlickGrid/Plugins/slick.cellrangeselector.js"></script>
    <script src="Scripts/SlickGrid/Plugins/slick.cellnewcopymanager.js"></script>
    <script src="Scripts/SlickGrid/Plugins/slick.cellselectionmodel.js"></script>
    <script src="JavaScript/Pricer.js"></script>
    <div style="width: 525px; height: 1000px; margin-top: 50px" id="PricingDiv">
    </div>
    <script type='text/javascript'>
        drawPricingGrid(<%: GetPricing()%>);
    </script>
</asp:Content>
