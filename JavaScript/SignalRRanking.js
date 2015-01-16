$(function () {
    var betHub = $.connection.Bet;

    // Add client-side hub methods that the server will call
    $.extend(betHub.client, {
        newRanking: function (user, rank) {
            drawRankingGrid(user, rank);
        },
        newMessage: function (message) {
            alert(message);
        },
        newMoney: function (money) {
            $("#UserMoney").text(money + '€');
        }

});

$.connection.hub.start()
    .done(function (state) {
        $.connection.Bet.server.getMoney();
        $.connection.Bet.server.getRanking();
    });
});