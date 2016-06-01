function init() {
    var fountainHub = $.connection.Fountain;
    // Add client-side hub methods that the server will call
    $.extend(fountainHub.client, {
        displayFountains: function (fountainsList) {
            fountains = [];
            for (var i = 0; i < fountainsList.length; i++) {
                var fountain = fountainsList[i];
                fountains.push({
                    x: fountain.Long,
                    y: fountain.Lat,
                    images: fountain.Images,
                    found: fountain.Found,
                    antoine: fountain.Antoine,
                    camille: fountain.Camille,
                    loic: fountain.Loic,
                    xavier:fountain.Xavier,
                    rafaela:fountain.Rafaela
            });
            }

            map.on('zoomend', function (eventLayer) {
                mapBounds = map.getBounds();
                north = mapBounds.getNorth();
                south = mapBounds.getSouth();
                east = mapBounds.getEast();
                west = mapBounds.getWest();

                bounds = new rectangle(north, south, east, west);
                root = initCluster(fountains, bounds, 0, null);
                if (root)
                    findMarkers(root);
            });

            map.on('dragend', function (eventLayer) {
                mapBounds = map.getBounds();
                north = mapBounds.getNorth();
                south = mapBounds.getSouth();
                east = mapBounds.getEast();
                west = mapBounds.getWest();

                bounds = new rectangle(north, south, east, west);
                root = initCluster(fountains, bounds, 0, null);
                if (root)
                    findMarkers(root);
            });

            mapBounds = map.getBounds();
            north = mapBounds.getNorth();
            south = mapBounds.getSouth();
            east = mapBounds.getEast();
            west = mapBounds.getWest();

            bounds = new rectangle(north, south, east, west);
            root = initCluster(fountains, bounds, 0, null);
            if (root)
                findMarkers(root);
        }
    });

    $.connection.hub.start()
        .done(function (state) {
            fountainHub.server.getFountains($("#n").text());
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