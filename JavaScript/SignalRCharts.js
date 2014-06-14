$(function () {
    var betHub = $.connection.Bet;

    // Add client-side hub methods that the server will call
    $.extend(betHub.client, {
        newMessage: function (message) {
            alert(message);
        },
        newMoney: function (money) {
            $("#UserMoney").text(money + '€');
        },
        newCharts:function (charts)
        {
            chartsData = charts;
            drawChart();
        }
});

$.connection.hub.start()
    .done(function (state) {
        $.connection.Bet.server.getMoney($("#UserId").text());
        $.connection.Bet.server.getCharts();
    });
});