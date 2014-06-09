$(function () {
    var betHub = $.connection.Bet;

    // Add client-side hub methods that the server will call
    $.extend(betHub.client, {
        newOrders: function (orders) {
            drawOrdersGrid(orders);
        },
        newMessage: function (message) {
            alert(message);
        },
        newMoney: function (money) {
            $("#UserMoney").text(money + '€');
        },
        lastTrades: function (trades) {
            for (var i = 1; i <= trades.length; i++) {
                $("#Trade" + i).text(trades[i - 1]);
            }
        },
        chat: function (messages) {
            for (var i = 1; i <= messages.length; i++) {
                $("#Message" + i).text(messages[i - 1]);
            }
        }
    });

    $.connection.hub.start()
        .done(function (state) {
            $.connection.Bet.server.getMoney($("#UserId").text());
            $.connection.Bet.server.getTeam($("#UserId").text());
            $.connection.Bet.server.getLastTrades();

            $("#SendOrder").click(function () {
                betHub.server.sendOrder($("#UserId").text(), $("#TeamOrder").text(), $("#QuantityOrder").val(), $("#PriceOrder").val(), $("#SideOrder").text());
            });

            $("#CancelOrder").click(function () {
                betHub.server.cancelOrder($("#UserId").text(), $("#SideOrder").text(), $("#TeamOrder").text());
            });

            $("#SendMessage").click(function () {
                betHub.server.sendMessage($("#UserId").text(), $("#Message").val());
                $("#Message").val('');
            });
        });
});