$(function () {
    var betHub = $.connection.Bet;

    // Add client-side hub methods that the server will call
    $.extend(betHub.client, {
        showOrderBook : function (team, bids, asks){
            drawOrderBook(team, bids, asks);
        },
        newPrice: function (order, isMine) {
            if ($.grid == null)
                return;
            for (var i = 0; i < $.bidasks.length; i++) {
                if ($.bidasks[i].TeamName == order.Team) {
                    cellToFlash = [];
                    $.grid.invalidateRow(i);
                    setUpOrder(order, isMine, i, true, cellToFlash);
                    $.grid.render();
                    for (var j = 0; j < cellToFlash.length; j++) {
                        $.grid.flashCell(cellToFlash[j].row, cellToFlash[j].cell, 100, cellToFlash[j].className, 20);
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
        allTeams: function (teams) {
            $.teams = teams;
            for (var i = 0; i < teams.length; i++) {
                var team = teams[i];
                var o = new Option(team, team);
                $(o).html(team);
                $("#TeamOrder").append(o);
            }
            $("#TeamOrder").selectpicker('setStyle', 'teamOrderSelect', 'add');
            $("#TeamOrder").selectpicker('refresh');
        },
        chat: function (names, messages) {
            for (var i = 1; i <= names.length; i++) {
                $("#Name" + i).text(names[i - 1]);
            }
            for (var i = 1; i <= messages.length; i++) {
                $("#Message" + i).text(messages[i - 1]);
            }
        }
    });

    $.connection.hub.start()
        .done(function (state) {
            $.connection.Bet.server.getLastTrades();
            $.connection.Bet.server.getMessages();

            $("#SendOrder").click(function () {
                betHub.server.sendOrder($("#TeamOrder").val(), $("#QuantityOrder").val(), $("#PriceOrder").val(), $("#SideOrder").val());
                popup('newOrderDiv');
            });

            $("#CancelOrder").click(function () {
                betHub.server.cancelOrder($("#SideOrder").val(), $("#TeamOrder").val());
                popup('newOrderDiv');
            });

            $("#SendMessage").click(function () {
                betHub.server.sendMessage($("#Message").val());
                $("#Message").val('');
            });

            $("#SideOrderBuy").click(function () {
                clickBuySide();
            });

            $("#SideOrderSell").click(function () {
                clickSellSide();
            });

            $("#OpenPopUp").click(function () {
                popup('newOrderDiv');
            }); 

            $("#ClosePopUp").click(function () {
                popup('newOrderDiv');
            });

            $("#ClosePopOrderBook").click(function () {
                popup('orderBookDiv');
            });

            $('#OrderTab a').click(function (e) {
                e.preventDefault();
            })

            $("#Message").keypress(function (event) {
                if (event.which == 13) {
                    event.preventDefault();
                    $("#SendMessage").trigger("click");
                }
            })
        });
});