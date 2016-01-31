$(function () {
    var betHub = $.connection.Bet;
    $.extend(betHub.client, {
        newTrades: function (trades, nbTrades, hasNew, id) {
            if (id == -1 || id != competitionUniverseId) {
                return;
            }
            if (hasNew) {
                $("#NumberOfNewTrades").addClass("showBadge");
            }
            if (nbTrades == 1) {
                $("#ListTrade")[0].innerHTML = "<li><a>" + trades + "</a></li>";
            } else {
                var listTrades = "";
                for (var i = 0; i < nbTrades; i++) {
                    listTrades += "<li><a>" + trades[i] + "</a></li>";
                }
                $("#ListTrade")[0].innerHTML = listTrades;
            }
        }
    });

    $.connection.hub.start()
        .done(function (state) {
            competitionId = $("#currentCompetitionId").text();
            universeId = $("#currentUniverseId").text();
            competitionUniverseId = $("#currentCompetitionUniverseId").text();
            betHub.server.getTradeHistory($("#currentCompetitionId").text(), $("#currentCompetitionUniverseId").text());

            $('#ListTradeButton').click(function (e) {
                $("#NumberOfNewTrades").removeClass("showBadge");
                betHub.server.showTradeHistory($("#currentCompetitionUniverseId").text());
            });
        });
});