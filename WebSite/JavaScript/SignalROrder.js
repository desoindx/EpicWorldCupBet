function openOrderPopup() {
    $("#newOrderDiv").bPopup({
        onOpen: function () {
        },
        follow: [false, false],
        speed: 250,
        transition: 'slideIn',
        transitionClose: 'slideBack'
    });
    $("#PriceOrder").focus();
}

function openBookPopup(teamName) {
    $.connection.Bet.server.getOrderBook(teamName, competitionUniverseId, competitionId);
    $("#orderBookDiv").bPopup({
        onOpen: function () {
        },
        follow: [false, false],
        speed: 250,
        transition: 'slideIn',
        transitionClose: 'slideBack'
    });
}

$(function () {
    var betHub = $.connection.Bet;

    // Add client-side hub methods that the server will call
    $.extend(betHub.client, {
        showOrderBook: function (team, bids, asks, lastTradedPriceValue, midPrice, position) {
            drawOrderBook(team, bids, asks);
            $("#LastTradedPrice")[0].innerHTML = "Last traded price : " + lastTradedPriceValue;
            $("#MidPrice")[0].innerHTML = "Current price : " + midPrice;
            if (position > 0)
                $("#Position")[0].innerHTML = "Your position : <span class='glyphicon glyphicon-plus' aria-hidden='true' style='color: green;'>" + position + "</span>";
            else if (position < 0)
                $("#Position")[0].innerHTML = "Your position : <span class='glyphicon glyphicon-minus' aria-hidden='true' style='color: red;'>" + -position + "</span>";
            else
                $("#Position")[0].innerHTML = "Your position : <span style='color: blue;'>" + position + "</span>";
        },
        newPrice: function (order, isMine, competitionId) {
            if ($.grids[competitionId] == null)
                return;
            var datas = $.bidasks[competitionId];
            for (var i = 0; i < datas.length; i++) {
                if (datas[i].TeamName == order.Team) {
                    var cellToFlash = [];
                    $.grids[competitionId].invalidateRow(i);
                    setUpOrder(order, isMine, i, true, cellToFlash, competitionId);
                    $.grids[competitionId].render();
                    for (var j = 0; j < cellToFlash.length; j++) {
                        $.grids[competitionId].flashCell(cellToFlash[j].row, cellToFlash[j].cell, 100, cellToFlash[j].className, 20);
                    }
                    break;
                }
            }
        },
        newOrders: function (orders) {
            drawOrdersGrid(orders);
        },
        newMessage: function (message, newClass) {
            $("#myAlert").removeClass("hiddenAlert");
            $("#myAlert").addClass(newClass);
            $("#myAlert").addClass("visibleAlert");
            $("#cashInfos").addClass("hiddenAlert");
            $("#alertMessage").html(message);
            setTimeout(function () {
                $("#myAlert").removeClass(newClass);
                $("#myAlert").removeClass("visibleAlert");
                $("#cashInfos").removeClass("hiddenAlert");
                $("#myAlert").addClass("hiddenAlert");
            }, 5000);
        },
        updateCashInfos: function (cashAvailable, maxExposition, cashToInvest) {
            $("#UserMoney").text(cashAvailable);
            $("#cashAvailable").text(cashAvailable);
            $("#maxExposition").text(maxExposition);
            $("#cashToInvest").text(cashToInvest);
        }
    });

    $.connection.hub.start()
        .done(function (state) {
            competitionId = $("#currentCompetitionId").text();
            universeId = $("#currentUniverseId").text();
            competitionUniverseId = $("#currentCompetitionUniverseId").text();
            betHub.server.getCashInfos(universeId, competitionId, competitionUniverseId);

            $("#SendOrder").click(function () {
                var side = "SELL";
                if ($("#BuySide").prop("checked"))
                    side = "BUY";
                betHub.server.sendOrder($("#TeamOrder").val(), $("#QuantityOrder").val(), $("#PriceOrder").val(), side, universeId, competitionId, competitionUniverseId);
                $("#newOrderDiv").bPopup().close();
            });

            $("#CancelOrder").click(function () {
                var side = "SELL";
                if ($("#BuySide").prop("checked"))
                    side = "BUY";

                betHub.server.cancelOrder(side, $("#TeamOrder").val(), universeId, competitionId, competitionUniverseId);
                $("#newOrderDiv").bPopup().close();
            });

            $("#OpenPopUp").click(function () {
                showTeamSelectPicker();
                $("#BuySide").prop("checked", true);
                openOrderPopup();
            });

            $('#OrderTab a').click(function (e) {
                e.preventDefault();
            });
        });
});