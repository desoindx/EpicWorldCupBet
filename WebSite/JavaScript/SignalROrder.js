function openOrderPopup() {
    $("#newOrderDiv").bPopup({
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
            $('.TeamCell').hoverIntent({
                over: onTeamCellMouseOver,
                out: onTeamCellMouseOut
            });
        },
        newOrders: function (orders) {
            drawOrdersGrid(orders);
        },
        newMessage: function (message, newClass) {
            $("#myAlert").removeClass("hiddenAlert");
            $("#myAlert").addClass(newClass);
            $("#myAlert").addClass("visibleAlert");
            $("#alertMessage").html(message);
            setTimeout(function () {
                $("#myAlert").removeClass(newClass);
                $("#myAlert").removeClass("visibleAlert");
                $("#myAlert").addClass("hiddenAlert");
            }, 5000);
        },
        newMoney: function (money) {
            $("#UserMoney").text(money + '€');
        },
        lastTrades: function (trades) {
            for (var i = 1; i <= trades.length; i++) {
                $("#Trade" + i).text(trades[i - 1]);
            }
        },
        chat: function (message) {
            $("#ChatDiv")[0].innerHTML += "<p><label class='chatName'>" + message[0] + "</label><label>" + message[1] + "</label></p>";
            $("#ChatDiv").scrollTop(1E10);
        }
    });

    $.connection.hub.start()
        .done(function (state) {
            $("#ChatDiv").scrollTop(1E10);

            $("#SendOrder").click(function () {
                betHub.server.sendOrder($("#TeamOrder-" + currentCompetitionId).val(), $("#QuantityOrder").val(), $("#PriceOrder").val(), $("#SideOrder").val(), universeId, currentCompetitionId);
                openOrderPopup();
            });

            $("#CancelOrder").click(function () {
                betHub.server.cancelOrder($("#SideOrder").val(), $("#TeamOrder-" + currentCompetitionId).val(), universeId, currentCompetitionId);
                openOrderPopup();
            });

            $("#SendMessage").click(function () {
                betHub.server.sendMessage(universeId, $("#Message").val());
                $("#Message").val('');
            });

            $("#OpenPopUp").click(function () {
                showTeamSelectPicker();
                $("#BuySide").prop("checked", true);
                openOrderPopup();
                $("#PriceOrder").focus();
            });

            $('#OrderTab a').click(function (e) {
                e.preventDefault();
            });

            $("#Message").keypress(function (event) {
                if (event.which == 13) {
                    event.preventDefault();
                    $("#SendMessage").trigger("click");
                }
            });

            $('#CompetitionTabMenu a').click(function (e) {
                $("#CompetitionDropDownButton")[0].innerHTML = this.innerText + " <span class='caret'></span>";
                showTeamSelectPicker();
            });
        });
});