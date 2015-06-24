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
    <script src="JavaScript/PopUp.js"></script>

    <% if (Master.UserHasUniverse)
       { %>
    <div id="myAlert" class="alert hiddenAlert" role="alert">
        <label id="alertMessage">These is not the message you are looking for</label>
    </div>
    <div class="col-md-4 col-md-offset-4">
        <input style="margin-top: 20px; margin-bottom: 20px; width: 250px;" type="button" id="OpenPopUp" value="Place New Order" class="btn btn-primary" />
    </div>
    <div style="margin-top: 10px; width: 880px; height: 900px;" id='<%: "BidAskDiv-" + Master.GetCompetitionId() %>'>
    </div>
    <script type='text/javascript'>
        drawOrdersGrid(<%: GetOrders() %>, <%: Master.GetCompetitionId() %>);
    </script>
    <div id="blanket" style="display: none"></div>
    <div id="newOrderDiv" style="display: none">
        <span class="glyphicon glyphicon-remove" aria-hidden="true" id="ClosePopUp" style="color: red; position: absolute; top: 5px; right: 5px;"></span>

        <table style="margin-left: 50px; margin-top: 50px">
            <tbody>
                <tr>
                    <td>
                        <label style="">Side : </label>
                        <div class="btn-group" role="group" style="margin-left: 37px;">
                            <button type="button" class="btn SelectedSide" id="SideOrderBuy">BUY</button>
                            <button type="button" class="btn UnselectedSide" id="SideOrderSell">SELL</button>
                        </div>
                        <input type="hidden" id="SideOrder" value="BUY" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label style="margin-top: 17px; margin-right: 30px;">Team : </label>
                        <div class="teamSelectPicker" style="height: 0ne" id="TeamOrderDiv">
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
                        <label style="">Price : </label>
                        <input style="width: 220px; margin-left: 35px; margin-top: 10px;" type='number' min="1" max="999" id='PriceOrder' />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label style="">Quantity : </label>
                        <input style="width: 220px; margin-left: 13px; margin-top: 10px;" type='number' min="1" value="10" id='QuantityOrder' />
                    </td>
                </tr>
                <tr>
                    <td>
                        <input style="margin-left: 10px; margin-top: 30px" type="button" id="SendOrder" class="btn btn-default" value="Send" />
                        <input style="margin-left: 80px; margin-top: 30px; display: none;" type="button" class="btn btn-default" id="CancelOrder" value="Cancel Buy Order" />
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    <div id="orderBookDiv" style="text-align: center; display: none">
        <span class="glyphicon glyphicon-remove" aria-hidden="true" id="ClosePopOrderBook" style="color: red; position: absolute; top: 5px; right: 5px;"></span>
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
        <div id="bidGrid" style="width: 200px; height: 200px; margin-left: 5px;">
        </div>
        <div id="askGrid" style="width: 200px; height: 200px; margin-top: -200px; margin-left: 250px;">
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
            <label class="h2">It appears that you don't have any universe configurated !</label>
        </p>
        <p>
            <a runat="server" href="~/Universe/Create">
                <input style="margin-left: 10px; margin-top: 30px" type="button" id="CreateUniverse" class="btn btn-default" value="I want to create my own" />
            </a>
            <a runat="server" href="~/Universe/Join">
                <input style="margin-left: 80px; margin-top: 30px;" type="button" class="btn btn-default" id="JoinUniverse" value="I want to join an existing universe" />
            </a>
        </p>
    </div>
    <% } %>
    <% } %>
</asp:Content>
