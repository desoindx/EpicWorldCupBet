function openOrderPopup() {
    $("#newOrderDiv").bPopup({
        onOpen: function () {
        },
        follow: [false, false],
        speed: 250,
        transition: 'slideIn',
        transitionClose: 'slideBack'
    });
    $("#PriceOrder").focus();
}

$(function () {
    var betHub = $.connection.Bet;

    // Add client-side hub methods that the server will call
    $.extend(betHub.client, {
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
        updateCashInfos: function (cashAvailable, maxExposition, cashToInvest) {
            $("#UserMoney").text(cashAvailable);
            $("#cashAvailable").text(cashAvailable);
            $("#maxExposition").text(maxExposition);
            $("#cashToInvest").text(cashToInvest);
        }
    });

    $.connection.hub.start()
        .done(function (state) {
            competitionId = $("#currentCompetitionId").text();
            universeId = $("#currentUniverseId").text();
            competitionUniverseId = $("#currentCompetitionUniverseId").text();

            $("#SendOrder").click(function () {
                betHub.server.sendSwap($("#TeamBuy").val(), $("#QuantityBuyOrder").val(), $("#TeamSell").val(), $("#QuantitySellOrder").val(), $("#PriceOrder").val(), universeId, competitionId, competitionUniverseId);
                $("#newOrderDiv").bPopup().close();
            });

            $("#CancelOrder").click(function () {
                var side = "SELL";
                if ($("#BuySide").prop("checked"))
                    side = "BUY";

                betHub.server.cancelSwap(side, $("#TeamOrder").val(), universeId, competitionId, competitionUniverseId);
                $("#newOrderDiv").bPopup().close();
            });

            $("#OpenPopUp").click(function () {
                showTeamSelectPicker();
                $("#CancelOrder").hide();
                openOrderPopup();
            });

            $('#OrderTab a').click(function (e) {
                e.preventDefault();
            });
        });
});