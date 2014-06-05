$(function () {
    var betHub = $.connection.Bet;

    // Add client-side hub methods that the server will call
    $.extend(betHub.client, {
        newOrders: function (orders) {
            drawOrdersGrid(orders);
        },
        newMessage: function (message) {
            alert(message);
        }
});

$.connection.hub.start()
    .done(function (state) {
        betHub.server.getTeam($("#UserId").text());

        $("#SendOrder").click(function () {
            betHub.server.sendOrder($("#UserId").text(), $("#TeamOrder").text(), $("#QuantityOrder").val(), $("#PriceOrder").val(), $("#SideOrder").text());
        });
    });
});