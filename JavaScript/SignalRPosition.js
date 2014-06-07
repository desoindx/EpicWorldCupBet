$(function () {
    var betHub = $.connection.Bet;

    // Add client-side hub methods that the server will call
    $.extend(betHub.client, {
        newPositions: function (teams, users, positions) {
            drawPositionGrid(teams, users, positions);
        },
        newMessage: function (message) {
            alert(message);
        }
});

$.connection.hub.start()
    .done(function (state) {
        $.connection.Bet.server.getPosition($("#UserId").text());

        $("#SendOrder").click(function () {
            betHub.server.sendOrder($("#UserId").text(), $("#TeamOrder").text(), $("#QuantityOrder").val(), $("#PriceOrder").val(), $("#SideOrder").text());
        });

        $("#CancelOrder").click(function () {
            betHub.server.cancelOrder($("#UserId").text(), $("#SideOrder").text(), $("#TeamOrder").text());
        });
    });
});