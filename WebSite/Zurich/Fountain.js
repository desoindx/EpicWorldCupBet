$(function () {
    var fountainHub = $.connection.Fountain;

    // Add client-side hub methods that the server will call
    $.extend(fountainHub.client, {
        displayFountains: function(fountainsList) {
            fountains = [];
            for (var i = 0; i < fountainsList.length; i++) {
                var fountain = fountainsList[i];
                fountains.push({
                    x: fountain.Long, y: fountain.Lat, images: fountain.Images, found:fountain.Found
            });
            }

            root = initCluster(fountains, null, 0, null);
            findMarkers(root);

            map.on('zoomend', function(eventLayer) {
                findMarkers(root);
            });

            map.on('dragend', function(eventLayer) {
                findMarkers(root);
            });
        }
    });

    $.connection.hub.start()
        .done(function (state) {
            fountainHub.server.getFountains();
            winWidth = $(window).width();
            winHeight = $(window).height();
        });

 });