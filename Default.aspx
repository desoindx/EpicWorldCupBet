<%@ Page Title="Home Page" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%@ MasterType VirtualPath="Site.Master" %>

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
    <% if (Master.UniverseHasMultipleCompetition())
       {%>
    <div role="tabpanel">
        <ul class="nav nav-tabs" id="CompetitionTab" role="tablist" style="width: 600px;">
            <li role="presentation" class="dropdown active">
                <a id="CompetitionDropDownButton" class="dropdown-toggle active" data-toggle="dropdown" href="#" role="button" aria-expanded="false" aria-controls="CompetitionTabMenu">Dropdown <span class="caret"></span>
                </a>
                <ul class="dropdown-menu" role="menu" id="CompetitionTabMenu">
                    <% bool hasActive = false;
                       foreach (var competition in Master.UniverseCompetitions)
                       {%>
                    <li role="presentation" class="<%: hasActive ? "" : "active" %>">
                        <a title="<%: competition.Id %>" href="#<%: "Div-" + competition.Id %>" aria-controls="<%: "Div-" + competition.Id %>" role="tab" data-toggle="tab">
                            <%: competition.Name %>
                        </a>
                    </li>
                    <% if (!hasActive)
                       {%>
                    <script type='text/javascript'> 
                        $("#CompetitionDropDownButton")[0].innerHTML = '<%: competition.Name %>' + " <span class='caret'></span>";
                        currentCompetitionId = <%: competition.Id %>;
                    </script>
                    <%}
                       hasActive = true;
                       }%>
                </ul>
            </li>
        </ul>
        <div id="CompetitionTabContent" class="tab-content" style="width: 600px; height: 900px;">
            <% hasActive = false;
               foreach (var competition in Master.UniverseCompetitions)
               {%>
            <div role="tabpanel" class="tab-pane <%: hasActive ? "" : "active" %>" id='<%: "Div-" + competition.Id %>'>
                <div id='<%: "BidAskDiv-" + competition.Id %>' style="width: 600px; height: 900px;"></div>
                <div style="margin-top: -850px; margin-left: 650px; width: 500px;">
                    <p>
                        <label class="h4">Last Trades : </label>
                    </p>
                    <% var i = 1;
                       foreach (var trade in GetLastTrade(competition.Id))
                       { %>
                    <p>
                        <label id='<%: "Trade" + i + "-"+ competition.Id %>'><%: trade %></label>
                    </p>
                    <% i++;
                       } %>
                </div>
            </div>
            <%
                       hasActive = true;
               }%>
        </div>
    </div>
    <script type='text/javascript'> 
        <% foreach (var competition in Master.UniverseCompetitions)
           {%>
        drawOrdersGrid(<%: GetOrders(competition.Id)%>, <%:competition.Id%>);
        <%}%>
    </script>
    <% }
       else
       { %>
    <label class="h3" style="margin-top: 0;"><%: Master.GetUniverseCompetition() %></label>
    <div style="width: 600px; height: 900px;" id='<%: "BidAskDiv-" + Master.GetCompetitionId() %>'>
    </div>
    <div style="margin-top: 25px; margin-left: 650px; width: 500px;">
        <p>
            <label class="h4">Last Trades : </label>
        </p>
        <% var i = 1;
           foreach (var trade in GetLastTrade(Master.GetCompetitionId()))
           { %>
        <p>
            <label id='<%: "Trade" + i %>'><%: trade %></label>
        </p>
        <% i++;
           } %>
    </div>
    <script type='text/javascript'>
        drawOrdersGrid(<%: GetOrders()%>, <%: Master.GetCompetitionId()%>);
    </script>
    <% } %>
    <div style="margin-top: -900px; margin-left: 700px;">
        <input style="margin-left: 10px" type="button" id="OpenPopUp" value="Place A New Order" class="btn btn-default" />
    </div>
    <div style="margin-top: 210px; margin-left: 650px; width: 500px;">
        <p>
            <label class="h4">Chat : </label>
        </p>
        <div id="ChatDiv" style="overflow: auto; width: 450px; height: 245px;">
            <% foreach (var message in GetMessages())
               { %>
            <p>
                <label class="chatName"><%: message[0] %></label>
                <label><%: message[1] %></label>
            </p>
            <% } %>
            <p>
        </div>
        <p>
            <input style="margin-left: 13px; width: 400px" type='text' id='Message' />
            <input style="margin-left: 10px" type="button" id="SendMessage" value="Send" class="btn btn-default" />
        </p>
    </div>
    <div id="blanket" style="display: none"></div>
    <div id="newOrderDiv" style="display: none">
        <span class="glyphicon glyphicon-remove" aria-hidden="true" id="ClosePopUp" style="color: red; position: absolute; top: 5px; right: 5px;"></span>
        <div role="tabpanel">
            <ul class="nav nav-tabs" id="OrderTab" role="tablist">
                <li role="presentation" class="active"><a href="#home" aria-controls="home" role="tab" data-toggle="tab">Simple</a></li>
                <li role="presentation"><a href="#swap" aria-controls="swap" role="tab" data-toggle="tab">Swap</a></li>
            </ul>
            <div id="OrderTabContent" class="tab-content">
                <div role="tabpanel" class="tab-pane active" id="home">
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
                                    <% var hasDisplayed = false;
                                       foreach (var competition in Master.UniverseCompetitions)
                                       {%>
                                    <div class="teamSelectPicker" style="height: 0; <%: hasDisplayed ? "display:none" : "" %>" id='<%:"TeamOrderDiv" + competition.Id%>'>
                                        <select class="selectpicker" id='<%:"TeamOrder-" + competition.Id%>'>
                                            <% foreach (var team in GetTeamFor(competition.Id))
                                               {%>
                                            <option><%: team %></option>
                                            <% }%>
                                        </select>
                                    </div>
                                    <%
                                               hasDisplayed = true;
                                       }%>
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
                <div role="tabpanel" class="tab-pane" id="swap">ToDo !!!</div>
            </div>
        </div>
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
        universeId = <%: Master.SelectedUniverseId%>;
    </script>
    <% }
       else
       {%>
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
