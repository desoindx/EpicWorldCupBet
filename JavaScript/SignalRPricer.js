$(function () {
    var betHub = $.connection.Bet;

    // Add client-side hub methods that the server will call
    $.extend(betHub.client, {
        pricingFinished: function (teams, price) {
            for (var i = 0; i < teams.length; i++) {
                $("#" + teams[i] + "Price").text(price[i]);
            }
        }
    });

    $.connection.hub.start()
        .done(function (state) {
            $("#Eliminate").click(function () {
                betHub.server.eliminateTeam($("#Password").val(), $("#Team").val(), $("#Value").val());
            });
            $("#Price").click(function () {
                betHub.server.price($("#Password").val(),
                    $("#Brazil").val(),
                    $("#Croatia").val(),
                    $("#Mexico").val(),
                    $("#Cameroon").val(),
                    $("#Australia").val(),
                    $("#Chile").val(),
                    $("#Netherlands").val(),
                    $("#Spain").val(),
                    $("#Colombia").val(),
                    $("#Greece").val(),
                    $("#IvoryCoast").val(),
                    $("#Japan").val(),
                    $("#CostaRica").val(),
                    $("#England").val(),
                    $("#Italy").val(),
                    $("#Uruguay").val(),
                    $("#Ecuador").val(),
                    $("#France").val(),
                    $("#Honduras").val(),
                    $("#Switzerland").val(),
                    $("#Argentina").val(),
                    $("#BosniaAndHerzgovina").val(),
                    $("#Iran").val(),
                    $("#Nigeria").val(),
                    $("#Germany").val(),
                    $("#Ghana").val(),
                    $("#Portugal").val(),
                    $("#UnitedStates").val(),
                    $("#Algeria").val(),
                    $("#Belgium").val(),
                    $("#Russia").val(),
                    $("#SouthKorea").val())
            });
        });
});