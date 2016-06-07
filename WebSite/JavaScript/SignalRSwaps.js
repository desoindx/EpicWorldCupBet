function openOrderPopup() {
    $("#newOrderDiv").bPopup({
        onOpen: function () {
        },
        follow: [false, false],
        speed: 250,
        transition: 'slideIn',
        transitionClose: 'slideBack'
    });

    $("#SendOrder").show();
    $('#QuantityBuyOrder').prop('readonly', false);
    $('#QuantitySellOrder').prop('readonly', false);
    $('#PriceOrder').prop('readonly', false);
    $('#TeamBuyDiv').css('pointer-events', '');
    $('#TeamSellDiv').css('pointer-events', '');
    $("#PriceOrder").focus();
    $("#SendOrder").prop('value', 'Send New Swap');
    isNewMenu = true;
}

$(function () {
    var betHub = $.connection.Bet;

    // Add client-side hub methods that the server will call
    $.extend(betHub.client, {
        newMessage: function (message, newClass) {
            $("#myAlert").removeClass("hiddenAlert");
            $("#myAlert").addClass(newClass);
            $("#myAlert").addClass("visibleAlert");
            $("#cashInfos").addClass("hiddenAlert");
            $("#alertMessage").html(message);
            setTimeout(function () {
                $("#myAlert").removeClass(newClass);
                $("#myAlert").removeClass("visibleAlert");
                $("#myAlert").addClass("hiddenAlert");
                $("#cashInfos").removeClass("hiddenAlert");
            }, 5000);
        },
        newMessageAndRefresh: function (message, newClass) {
            $("#myAlert").removeClass("hiddenAlert");
            $("#myAlert").addClass(newClass);
            $("#myAlert").addClass("visibleAlert");
            $("#cashInfos").addClass("hiddenAlert");
            $("#alertMessage").html(message);
            setTimeout(function () {
                $("#myAlert").removeClass(newClass);
                $("#myAlert").removeClass("visibleAlert");
                $("#myAlert").addClass("hiddenAlert");
                $("#cashInfos").removeClass("hiddenAlert");
                window.location.reload(false);
            }, 500);
        },
        updateCashInfos: function (cashAvailable, maxExposition, cashToInvest) {
            $("#UserMoney").text(cashAvailable);
            $("#cashAvailable").text(cashAvailable);
            $("#maxExposition").text(maxExposition);
            $("#cashToInvest").text(cashToInvest);
        }
    });

    $("#TeamBuy").change(function () {
        $("#SwapBuyTeam").html($("#TeamBuy").val());
    });

    $("#TeamSell").change(function () {
        $("#SwapSellTeam").html($("#TeamSell").val());
    });

    $("#QuantityBuyOrder").change(function () {
        $("#SwapBuyQuantity").html($("#QuantityBuyOrder").val());
    });

    $("#QuantitySellOrder").change(function () {
        $("#SwapSellQuantity").html($("#QuantitySellOrder").val());
    });

    $("#PriceOrder").change(function () {
        if (parseInt($("#PriceOrder").val()) > 0) {
            $("#SwapPrice").html($("#PriceOrder").val());
            $("#SwapWay").html("receive");
        } else {
            $("#SwapPrice").html(-parseInt($("#PriceOrder").val()));
            $("#SwapWay").html("pay");
        }
    });
//    $("#TeamSell").selectpicker('val', item.SellTeam);
//    $("#QuantityBuyOrder").val(item.BuyQuantity);
//    $("#QuantitySellOrder").val(item.SellQuantity);
//    $("#PriceOrder").val(item.Price);

    $.connection.hub.start()
        .done(function (state) {
            competitionId = $("#currentCompetitionId").text();
            universeId = $("#currentUniverseId").text();
            competitionUniverseId = $("#currentCompetitionUniverseId").text();
            betHub.server.getCashInfos(universeId, competitionId, competitionUniverseId);

            $("#SendOrder").click(function () {
                if (isNewMenu) {
                    betHub.server.sendSwap($("#TeamBuy").val(), $("#QuantityBuyOrder").val(), $("#TeamSell").val(), $("#QuantitySellOrder").val(), $("#PriceOrder").val(), universeId, competitionId, competitionUniverseId);
                } else {
                    betHub.server.matchSwap(selectedItem, universeId, competitionId, competitionUniverseId);
                }
                $("#newOrderDiv").bPopup().close();
            });

            $("#CancelOrder").click(function () {
                betHub.server.cancelSwap(selectedItem);
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