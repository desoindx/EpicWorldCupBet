﻿$(function () {
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
        }

});

$.connection.hub.start()
    .done(function (state) {
        $.connection.Bet.server.getMoney($("#UserId").text());
        $.connection.Bet.server.getTeam($("#UserId").text());

        $("#SendOrder").click(function () {
            betHub.server.sendOrder($("#UserId").text(), $("#TeamOrder").text(), $("#QuantityOrder").val(), $("#PriceOrder").val(), $("#SideOrder").text());
        });

        $("#CancelOrder").click(function () {
            betHub.server.cancelOrder($("#UserId").text(), $("#SideOrder").text(), $("#TeamOrder").text());
        });
    });
});