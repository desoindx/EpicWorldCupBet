$(function () {
    var betHub = $.connection.Bet;

    // Add client-side hub methods that the server will call
    $.extend(betHub.client, {
        newOrders: function (orders) {
            drawOrdersGrid(orders);
        }
    });

    $.connection.hub.start()
        .done(function (state) {
            betHub.server.getTeam();
        });
});