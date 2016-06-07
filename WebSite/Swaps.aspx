<%@ Page Title="Swaps" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Swaps.aspx.cs" Inherits="_Default" %>

<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="Content/slick.grid.css" type="text/css">
    <link rel="stylesheet" href="Content/slick-default-theme.css" type="text/css">
    <link rel="stylesheet" href="Content/bootstrap-select.css" type="text/css">
    <script src="Scripts/SlickGrid/Plugins/slick.cellrangedecorator.js"></script>
    <script src="Scripts/SlickGrid/Plugins/slick.cellrangeselector.js"></script>
    <script src="Scripts/SlickGrid/Plugins/slick.cellnewcopymanager.js"></script>
    <script src="Scripts/SlickGrid/Plugins/slick.cellselectionmodel.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <% if (Context.User.Identity.IsAuthenticated)
       { %>
    <script src="Scripts/SlickGrid/Plugins/slick.checkboxselectcolumn.js"></script>
    <script src="JavaScript/Swaps.js"></script>
    <script src="JavaScript/SignalRSwaps.js"></script>
    <script src="Scripts/bPopUp.js"></script>

    <% if (Master.UserHasUniverse)
       { %>
    <div id="myAlert" class="alert hiddenAlert" role="alert">
        <label id="alertMessage">These is not the message you are looking for</label>
    </div>
    <div id="cashInfos">
        <table>
            <tr>
                <td id="cashAvailable"></td>
                <td id="maxExposition"></td>
                <td id="cashToInvest">Please wait while your position is being updated ...</td>
            </tr>
        </table>
    </div>
    <div style="position: absolute; left: 50%; margin-left: -138px; z-index: 100; top: 45px">
        <input style="margin-top: 20px; margin-bottom: 20px; width: 250px;" type="button" id="OpenPopUp" value="Place New Swap" class="btn btn-primary btn-lg" />
    </div>
    <div class="BidAskDiv">
        <div style="margin-top: 80px; width: 920px; height: 900px;" id='<%: "BidAskDiv-" + Master.GetCompetitionId() %>'>
        </div>
    </div>
    <script type='text/javascript'>
        drawSwapsGrid(<%: GetSwaps() %>, <%: Master.GetCompetitionId() %>);
    </script>
    <div id="blanket" style="display: none"></div>
    <div id="newOrderDiv">
        <span class="button b-close"><span>X</span></span>
        <table style="margin-left: 50px">
            <tbody>
                <tr>
                    <td>
                        <label>I want to buy :</label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <input class="orderInfo" type='number' min="1" value="10" id='QuantityBuyOrder' />
                    </td>
                    <td>
                        <div class="teamSelectPicker" id="TeamBuyDiv">
                            <select class="selectpicker" id="TeamBuy">
                                <% foreach (var team in Master.GetTeamFor(Master.GetCompetitionId()))
                                   { %>
                                <option><%: team %></option>
                                <% } %>
                            </select>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>And sell :</label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <input class="orderInfo" type='number' min="1" value="10" id='QuantitySellOrder' />
                    </td>
                    <td>
                        <div class="teamSelectPicker" id="TeamSellDiv">
                            <select class="selectpicker" id="TeamSell">
                                <% foreach (var team in Master.GetTeamFor(Master.GetCompetitionId()))
                                   { %>
                                <option><%: team %></option>
                                <% } %>
                            </select>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>For a premium of (For the swap, not by contract !!):</label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <input class="orderInfo" type='number' value="0" id='PriceOrder' />
                        <span style="position: relative; right: 30px;">$</span>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>You will buy <span id="SwapBuyQuantity">10</span> <span id="SwapBuyTeam">Albania</span> and sell <span id="SwapSellQuantity">10</span> <span id="SwapSellTeam">Albania</span> and <span id="SwapWay">receive</span> <span id="SwapPrice">0</span> $ (in total)</label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <input style="margin-left: -50px; margin-top: 30px" type="button" id="SendOrder" class="btn btn-primary" value="Send New Swap" />
                        <input style="position: absolute; right: 20px; display: none;" type="button" class="btn btn-danger" id="CancelOrder" value="Cancel Existing Swap" />
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    <script type='text/javascript'>
        universeId = <%: Master.SelectedUniverseId %>;
    </script>
    <% }
       else
       { %>
    <div style="text-align: center">
        <p>
            <label class="h2">You don't have any universe configured !</label>
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
