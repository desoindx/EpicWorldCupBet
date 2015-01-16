$(function () {
    var betHub = $.connection.Bet;

    // Add client-side hub methods that the server will call
    $.extend(betHub.client, {
        showOrderBook: function (team, bids, asks) {
            drawOrderBook(team, bids, asks);
        },
        newPrice: function (order, isMine, competitionId) {
            if ($.grids[competitionId] == null)
                return;
            for (var i = 0; i < $.bidasks.length; i++) {
                if ($.bidasks[competitionId][i].TeamName == order.Team) {
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
                popup('newOrderDiv');
            });

            $("#CancelOrder").click(function () {
                betHub.server.cancelOrder($("#SideOrder").val(), $("#TeamOrder-" + currentCompetitionId).val(), universeId, currentCompetitionId);
                popup('newOrderDiv');
            });

            $("#SendMessage").click(function () {
                betHub.server.sendMessage(universeId, $("#Message").val());
                $("#Message").val('');
            });

            $("#SideOrderBuy").click(function () {
                clickBuySide();
            });

            $("#SideOrderSell").click(function () {
                clickSellSide();
            });

            $("#OpenPopUp").click(function () {
                showTeamSelectPicker(currentCompetitionId);
                popup('newOrderDiv');
                $("#PriceOrder").focus();
            });

            $("#ClosePopUp").click(function () {
                popup('newOrderDiv');
            });

            $("#ClosePopOrderBook").click(function () {
                $.manuallyCloseorderBookDiv = true;
                popup('orderBookDiv');
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
                currentCompetitionId = this.title;
                showTeamSelectPicker(currentCompetitionId);
            });

            $(document).keyup(function (e) {
                if (e.keyCode == 27) {// esc
                    if ($("#orderBookDiv")[0].style.display != 'none') {
                        $.manuallyCloseorderBookDiv = true;
                        popup('orderBookDiv');
                    }

                    if ($("#newOrderDiv")[0].style.display != 'none') {
                        popup('newOrderDiv');
                    }
                }
            });
        });
});