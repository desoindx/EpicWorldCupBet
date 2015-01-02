<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    
    <link rel="stylesheet" href="Content/slick.grid.css" type="text/css" />
    <link rel="stylesheet" href="Content/slick-default-theme.css" type="text/css" />
    <link rel="stylesheet" href="Scripts/bootstrap-select.css" type="text/css" />
    <script src="Scripts/jquery-1.7.2.min.js"></script>
    <script src="Scripts/jquery.event.drag.js"></script>
    <script src="Scripts/SlickGrid/slick.core.js"></script>
    <script src="Scripts/SlickGrid/slick.grid.js"></script>
    <script src="Scripts/bootstrap-select.js"></script>
    <script type="text/javascript" src="Scripts/jquery.signalR-2.0.3.js"></script>
    <script type="text/javascript" src="../signalr/hubs"></script>
    <script src="JavaScript/Order.js"></script>
    <script src="JavaScript/SignalROrder.js"></script>
    <script src="JavaScript/PopUp.js"></script>
    
    <div id="myAlert" class="alert hiddenAlert" role="alert">
       <label id ="alertMessage">Better check yourself, you're not looking too good.</label>
    </div>
    <div style="width: 600px; height: 900px;" id="BidAskDiv">
        <label>Loading in progress</label>
    </div>
    <div style="margin-top: -900px; margin-left: 700px;">
        <input style="margin-left: 10px" type="button" id="OpenPopUp" value="Place A New Order" class="btn btn-default"/>
    </div>
    <div style="margin-top: 25px; margin-left: 650px; width: 500px;">
        <p>
            <label class="h4">Last Trades : </label>
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
    <div style="margin-top: 25px; margin-left: 650px; width: 500px;">
        <p>
            <label class="h4">Chat : </label>
        </p>
        <p>
            <label id="Name1" class="chatName"></label>
            <label id="Message1"></label>
        </p>
        <p>
            <label id="Name2" class="chatName"></label>
            <label id="Message2"></label>
        </p>
        <p>
            <label id="Name3" class="chatName"></label>
            <label id="Message3"></label>
        </p>
        <p>
            <label id="Name4" class="chatName"></label>
            <label id="Message4"></label>
        </p>
        <p>
            <label id="Name5" class="chatName"></label>
            <label id="Message5"></label>
        </p>
        <p>
            <label id="Name6" class="chatName"></label>
            <label id="Message6"></label>
        </p>
        <p>
            <label id="Name7" class="chatName"></label>
            <label id="Message7"></label>
        </p>
        <p>
            <label id="Name8" class="chatName"></label>
            <label id="Message8"></label>
        </p>
        <p>
            <label id="Name9" class="chatName"></label>
            <label id="Message9"></label>
        </p>
        <p>
            <label id="Name10" class="chatName"></label>
            <label id="Message10"></label>
        </p>
        <p>
            <input style="margin-left: 13px; width:400px" type='textbox' id='Message' />
            <input style="margin-left: 10px" type="button" id="SendMessage" value="Send" class="btn btn-default"/>
        </p>
    </div>
    <div id="blanket" style="display:none"></div>
    <div id="newOrderDiv" style="display:none">
        <span class="glyphicon glyphicon-remove" aria-hidden="true" id="ClosePopUp"style="color:red;position:absolute; top:5px; right: 5px;">
        </span>
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
                                    <div class="btn-group" role="group" style="margin-left:37px;">
                                        <button type="button" class="btn btn-default SelectedSide" id="SideOrderBuy">BUY</button>
                                        <button type="button" class="btn btn-default" id="SideOrderSell">SELL</button>
                                    </div>
                                    <input type="hidden" id="SideOrder" value="BUY" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label style="margin-top:17px">Team : </label>
                                    <select class="selectpicker" id="TeamOrder">
                                    </select>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label style="">Price : </label>
                                    <input style="width:220px;margin-left: 35px; margin-top: 10px;" type='textbox' id='PriceOrder' />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label style="">Quantity : </label>
                                    <input style="width:220px;margin-left: 13px; margin-top: 10px;" type='textbox' id='QuantityOrder' />
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
    <div id="orderBookDiv" style="text-align:center;display:none">
        <span class="glyphicon glyphicon-remove" aria-hidden="true" id="ClosePopOrderBook" style="color:red;position:absolute; top:5px; right: 5px;">
        </span>
        <label class="h4" id="orderBookTeamName"></label>
        <p>
            <label>Bids</label>
            <label style="margin-left:220px;">Asks</label>
        </p>
        <div id="bidGrid" style ="width: 200px;height: 200px;margin-left:5px;">
        </div>
        <div id="askGrid" style ="width: 200px;height: 200px; margin-top:-200px; margin-left:250px;">
        </div>
    </div>
</asp:Content>
