<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">


    <link rel="stylesheet" href="Content/slick.grid.css" type="text/css" />
    <script src="Scripts/jquery-1.7.2.min.js"></script>
    <script src="Scripts/jquery.event.drag.js"></script>
    <script src="Scripts/SlickGrid/slick.core.js"></script>
    <script src="Scripts/SlickGrid/slick.grid.js"></script>
    <script src="JavaScript/Order.js"></script>
    <script type="text/javascript" src="Scripts/jquery.signalR-2.0.3.js"></script>
    <script type="text/javascript" src="../signalr/hubs"></script>
    <script src="JavaScript/SignalR.js"></script>

    <div class="jumbotron">
        <h1>Epic World Cup Betting</h1>
    </div>

    <div style="width: 600px; height: 1200px;" id="BidAskDiv"></div>
    <div style="margin-top: -1100px; margin-left: 600px; width: 500px; height: 1200px;" id="OrderDiv">
        <table style="margin-left: 50px">
            <tbody>
                <tr>
                    <td>
                        <label style="margin-left: 50px">Place an order on </label>
                        <label style="margin-left: 5px" id="TeamOrder">Brazil</label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label style="">Side : </label>
                        <label style="margin-left: 75px" id="SideOrder">BUY </label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label style="">Price : </label>
                        <input style="margin-left: 35px" type='textbox' id='PriceOrder' />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label style="">Quantity : </label>
                        <input style="margin-left: 13px" type='textbox' id='QuantityOrder' />
                    </td>
                </tr>
                <tr>
                    <td>
                        <input style="margin-left: 110px"  type="button" id="SendOrder" value="Send" />
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</asp:Content>
