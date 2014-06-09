<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <link rel="stylesheet" href="Content/slick.grid.css" type="text/css" />
    <script src="Scripts/jquery-1.7.2.min.js"></script>
    <script src="Scripts/jquery.event.drag.js"></script>
    <script src="Scripts/SlickGrid/slick.core.js"></script>
    <script src="Scripts/SlickGrid/slick.grid.js"></script>
    <script type="text/javascript" src="Scripts/jquery.signalR-2.0.3.js"></script>
    <script type="text/javascript" src="../signalr/hubs"></script>
    <script src="JavaScript/Order.js"></script>
    <script src="JavaScript/SignalROrder.js"></script>

    <div style="width: 600px; height: 1200px;" id="BidAskDiv">
        <label>Loading in progress</label></div>
    <div style="margin-top: -1200px; margin-left: 650px; width: 500px;">
        <p>
            <label>Last Trades : </label>
        </p>
        <p>
            <label id="Trade1"></label>
        </p>
        <p>
            <label id="Trade2"></label>
        </p>
        <p>
            <label id="Trade3"></label>
        </p>
        <p>
            <label id="Trade4"></label>
        </p>
        <p>
            <label id="Trade5"></label>
        </p>
    </div>
    <div style="margin-top: 40px; margin-left: 600px; width: 500px; height: 200px;" id="OrderDiv">
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
                        <label style="margin-left: 75px" id="SideOrder">BUY</label>
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
                        <input style="margin-left: 10px" type="button" id="SendOrder" value="Send" />
                        <input style="margin-left: 20px; display: none;" type="button" id="CancelOrder" value="Cancel Buy Order" />
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    <div style="margin-top: 20px; margin-left: 650px; width: 500px;">
        <p>
            <label>Chat : </label>
        </p>
        <p>
            <label id="Message1"></label>
        </p>
        <p>
            <label id="Message2"></label>
        </p>
        <p>
            <label id="Message3"></label>
        </p>
        <p>
            <label id="Message4"></label>
        </p>
        <p>
            <label id="Message5"></label>
        </p>
        <p>
            <label id="Message6"></label>
        </p>
        <p>
            <label id="Message7"></label>
        </p>
        <p>
            <label id="Message8"></label>
        </p>
        <p>
            <label id="Message9"></label>
        </p>
        <p>
            <label id="Message10"></label>
        </p>
        <p>
            <input style="margin-left: 13px; width:400px" type='textbox' id='Message' />
            <input style="margin-left: 10px" type="button" id="SendMessage" value="Send" />
        </p>
    </div>
</asp:Content>
