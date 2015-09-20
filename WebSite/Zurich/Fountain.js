function init() {
    var fountainHub = $.connection.Fountain;
    // Add client-side hub methods that the server will call
    $.extend(fountainHub.client, {
        displayFountains: function (fountainsList) {
            fountains = [];
            for (var i = 0; i < fountainsList.length; i++) {
                var fountain = fountainsList[i];
                fountains.push({
                    x: fountain.Long, y: fountain.Lat, images: fountain.Images, found: fountain.Found
                });
            }

            map.on('zoomend', function (eventLayer) {
                findMarkers(root);
            });

            map.on('dragend', function (eventLayer) {
                findMarkers(root);
            });

            root = initCluster(fountains, null, 0, null);
            findMarkers(root);
        }
    });

    $.connection.hub.start()
        .done(function (state) {
            fountainHub.server.getFountains();
            winWidth = $(window).width();
            winHeight = $(window).height();
        });
};

function openIphoneMenu() {
    $("#Menu").bPopup({
        onOpen: function () {
        },
        follow: [false, false],
        speed: 250,
        transition: 'slideIn',
        transitionClose: 'slideBack'
    });
}