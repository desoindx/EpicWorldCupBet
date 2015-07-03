<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="Content/slick.grid.css" type="text/css">
    <link rel="stylesheet" href="Content/slick-default-theme.css" type="text/css">
    <link rel="stylesheet" href="Content/bootstrap-select.css" type="text/css">
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <% if (Context.User.Identity.IsAuthenticated)
       { %>
    <script src="JavaScript/Order.js"></script>
    <script src="JavaScript/SignalROrder.js"></script>
    <script src="Scripts/bPopUp.js"></script>

    <% if (Master.UserHasUniverse)
       { %>
    <div id="myAlert" class="alert hiddenAlert" role="alert">
        <label id="alertMessage">These is not the message you are looking for</label>
    </div>
    <div class="col-md-4 col-md-offset-4">
        <input style="margin-top: 20px; margin-bottom: 20px; width: 250px;" type="button" id="OpenPopUp" value="Place New Order" class="btn btn-primary btn-lg" />
    </div>
    <div style="margin-top: 10px; width: 890px; height: 900px;" id='<%: "BidAskDiv-" + Master.GetCompetitionId() %>'>
    </div>
    <script type='text/javascript'>
        drawOrdersGrid(<%: GetOrders() %>, <%: Master.GetCompetitionId() %>);
    </script>
    <div id="blanket" style="display: none"></div>
    <div id="newOrderDiv">
        <span class="button b-close"><span>X</span></span>
        <table style="margin-left: 50px">
            <tbody>
                <tr>
                    <td>
                        <div class="btn-group" role="group" style="margin-left: 37px;">
                            <input id="BuySide" type="radio" name="action" value="buy">Buy
                                <input id="SellSide" type="radio" name="action" value="sell" style="margin-left: 90px;">Sell
                            <br />
                        </div>
                        <input type="hidden" id="SideOrder" value="BUY" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <div class="teamSelectPicker" id="TeamOrderDiv">
                            <select class="selectpicker" id="TeamOrder">
                                <% foreach (var team in GetTeamFor(Master.GetCompetitionId()))
                                   { %>
                                <option><%: team %></option>
                                <% } %>
                            </select>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <input class="orderInfo" type='number' min="1" max="999" id='PriceOrder' />
                        <span style="position: relative; right: 30px;">$</span>
                    </td>
                </tr>
                <tr>
                    <td>
                        <input class="orderInfo" type='number' min="1" value="10" id='QuantityOrder' />
                    </td>
                </tr>
                <tr>
                    <td>
                        <input style="margin-left: -50px; margin-top: 30px" type="button" id="SendOrder" class="btn btn-primary" value="Send New Order" />
                        <input style="margin-left: 50px; margin-top: 30px; display: none;" type="button" class="btn btn-danger" id="CancelOrder" value="Cancel Buy Order" />
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    <div id="orderBookDiv" style="text-align: center;">
        <span class="button b-close"><span>X</span></span>
        <label class="h4" id="orderBookTeamName"></label>
        <p>
            <label style="margin-right: 15px" id="LastTradedPrice">Last traded price : </label>
            <label style="margin-right: 15px" id="MidPrice">Current price : </label>
            <label id="Position">Your position : </label>
        </p>
        <p>
            <label>Bids</label>
            <label style="margin-left: 220px;">Asks</label>
        </p>
        <div id="bidGrid" style="width: 200px; height: 200px; margin-left: 25px;">
        </div>
        <div id="askGrid" style="width: 200px; height: 200px; margin-top: -200px; margin-left: 280px;">
        </div>
    </div>
    <script type='text/javascript'>
        universeId = <%: Master.SelectedUniverseId %>;
    </script>
    <% }
       else
       { %>
    <div style="text-align: center">
        <p>
            <label class="h2">You don't have any universe configurated !</label>
        </p>
        <p>
            <a runat="server" href="~/Create">
                <input style="margin-left: 10px; margin-top: 30px" type="button" id="CreateUniverse" class="btn btn-info" value="I want to create my own" />
            </a>
            <a runat="server" href="~/Join">
                <input style="margin-left: 80px; margin-top: 30px;" type="button" class="btn btn-info" id="JoinUniverse" value="I want to join an existing universe" />
            </a>
        </p>
    </div>
    <% } %>
    <% } %>
</asp:Content>
