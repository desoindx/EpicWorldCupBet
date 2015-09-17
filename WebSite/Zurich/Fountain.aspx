<%@ Page Title="You gotta find them all !" Language="C#" AutoEventWireup="true" CodeFile="Fountain.aspx.cs" Inherits="Fountain" %>

<!DOCTYPE html>
<html>
<head>
    <title>You gotta find them all !</title>
    <meta charset="utf-8" />

    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />

    <link href="css/smoothness/jquery-ui-1.10.3.custom.css" rel="stylesheet">
    <script src="SCRIPTS/jquery-1.9.1.js"></script>
    <script src="SCRIPTS/jquery-ui-1.10.3.custom.js"></script>

    <link rel="stylesheet" href="css/leaflet.css" />
    <link rel="stylesheet" href="css/catExposure.css" />

    <script src="SCRIPTS/leaflet.js"></script>

    <script src="SCRIPTS/leaflet_numbered_markers.js"></script>
    <script src="SCRIPTS/point.js"></script>
    <script src="SCRIPTS/GeoSearch/l.control.geosearch.js"></script>
    <script src="SCRIPTS/GeoSearch/l.geosearch.provider.openstreetmap.js"></script>

    <link rel="stylesheet" href="css/MarkerCluster.css" />
    <link rel="stylesheet" href="css/MarkerCluster.Default.css" />
    <!--[if lte IE 8]><link rel="stylesheet" href="../dist/MarkerCluster.Default.ie.css" /><![endif]-->

    <link rel="stylesheet" href="css/leaflet.draw.css" />
    <!--[if lte IE 8]><link rel="stylesheet" href="css/leaflet.draw.ie.css" /><![endif]-->
    <script src="SCRIPTS/Draw/Leaflet.draw.js"></script>
    <script src="SCRIPTS/Draw/draw/handler/Draw.Feature.js"></script>
    <script src="SCRIPTS/Draw/draw/handler/Draw.Marker.js"></script>
    <script src="SCRIPTS/Draw/ext/LatLngUtil.js"></script>
    <script src="SCRIPTS/Draw/ext/GeometryUtil.js"></script>
    <script src="SCRIPTS/Draw/ext/LineUtil.Intersect.js"></script>
    <script src="SCRIPTS/Draw/ext/Polyline.Intersect.js"></script>
    <script src="SCRIPTS/Draw/ext/Polygon.Intersect.js"></script>
    <script src="SCRIPTS/Draw/Control.Draw.js"></script>
    <script src="SCRIPTS/Draw/Tooltip.js"></script>
    <script src="SCRIPTS/Draw/Toolbar.js"></script>
    <script src="SCRIPTS/Draw/draw/DrawToolbar.js"></script>
    <script src="SCRIPTS/Draw/edit/EditToolbar.js"></script>
    <script type="text/javascript" src="../Scripts/jquery.signalR-2.0.3.min.js"></script>
    <script type="text/javascript" src="../../signalr/hubs"></script>
    <script src="Fountain.js"></script>
    <script src="../Scripts/bPopUp.js"></script>
</head>
<body>
    <div id="newFountainDiv">
        <span class="button b-close"><span>X</span></span><form runat="server">
            <fieldset>
                <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                    <p class="text-danger">
                        <asp:Literal runat="server" ID="FailureText" />
                    </p>
                </asp:PlaceHolder>
                <div class="form-group">
                    <asp:TextBox runat="server" ID="Longitude" Text="" Style="display: none" />
                    <asp:TextBox runat="server" ID="Lattitude" Text="" Style="display: none" />
                </div>
                <div class="form-group">
                    <asp:CheckBox runat="server" ID="Antoine" Text="Antoine" />
                    <asp:CheckBox runat="server" ID="Camille" Text="Camille" />
                    <asp:CheckBox runat="server" ID="Loic" Text="Loic" />
                    <asp:CheckBox runat="server" ID="Xavier" Text="Xavier" />
                </div>
                <div class="form-group">
                    <asp:FileUpload type="file" runat="server" capture="camera" accept="image/*" ID="Upload" name="cameraInput" />
                </div>
                <!-- Change this to a button or input when using this as a form -->
                <asp:Button runat="server" OnClick="Save" Text="Save Fountain" CssClass="btn btn-primary" />
            </fieldset>
        </form>
    </div>
    <div id="map"></div>
    <div id="Menu">
        <ul>
            <li><a href="#Summary">Foutain</a></li>
        </ul>
        <div id="Summary">
            <table>
                <tr>
                    <td><b>Colloc</b></td>
                    <td><b>First Found</b></td>
                    <td><b>Found</b></td>
                </tr>
                <% LoadStats();
                   foreach (var user in Fountains)
                   {
                %>
                <tr>
                    <td><%:user.Key %></td>
                    <td><%:user.Value.Item1 %></td>
                    <td><%:user.Value.Item2 %></td>
                </tr>
                <%  } %>
                <tr>
                    <td>Total</td>
                    <td>-</td>
                    <td><%:FountainsCount %></td>
                </tr>
            </table>
        </div>
    </div>
    <script type="text/javascript">
        $("#Menu").tabs();
        var menu = L.control({ position: 'bottomleft' });
        menu.onAdd = function (map) {
            this._div = document.getElementById('Menu');
            return this._div;
        };

        // control that shows state info on hover
        var info = L.control({ position: 'bottomleft' });
        //var infoEQ = L.control({position: 'topleft'});

        info.onAdd = function (map) {
            this._div = L.DomUtil.create('div', 'info');
            this.update();
            return this._div;
        };


        function highlightFeature(e) {
            //layer is the object that fired the event (May be a multypolygon in that case)
            var layer = e.target;

            layer.setStyle({
                weight: 5,
                //Grey color of the line
                color: '#666',
                dashArray: '',
                fillOpacity: 0.7
            });

            if (!L.Browser.ie && !L.Browser.opera) {
                layer.bringToFront();
            }

            info.update(layer.feature.properties);
        }

        var geojson;

        function resetHighlight(e) {
            var layer = e.target;
            layer.setStyle({
                weight: 2,
                color: 'white',
                dashArray: '3',
                fillOpacity: 0.7
            });
            //With the resetStyle function there were problems
            //It was not taking into account the modification made to the fill color
            //geojson.resetStyle(e.target);
            info.update();
        }

        function zoomToFeature(e) {
            var layer = e.target;
            map.fitBounds(e.target.getBounds());
            //Update the information on click for Iphone/Ipad
            info.update(layer.feature.properties);
        }

        function onEachFeature(feature, layer) {
            layer.on({
                mouseover: highlightFeature,
                mouseout: resetHighlight,
                click: zoomToFeature
            });
        }

        function updatePosition(pos) {
            map = new L.Map('map', { center: new L.LatLng(pos.coords.latitude, pos.coords.longitude), zoom: 18, minZoom: 6 });
            posmarker = [];
            refreshPos(pos);
            createMap();
        }

        function refreshPos(pos) {
            for (var i = 0; i < posmarker.length; i++) {
                map.removeLayer(posmarker[i]);
            }

            posmarker = [];
            posmarker.push(L.marker([pos.coords.latitude, pos.coords.longitude], {
                icon: new L.DivIcon({ html: '<div><img src="Icons/ico_curr_pos.png"></img></div>', iconSize: new L.Point(30, 30) })
            }).on('click', refreshPosition));

            L.layerGroup(posmarker).addTo(map);
        }

        function refreshPosition(e) {
            navigator.geolocation.getCurrentPosition(refreshPos, refreshPositionError);
        }

        function refreshPositionError(pos) {
        }

        function positionError(pos) {
            map = new L.Map('map', { center: new L.LatLng(47.39, 8.545), zoom: 18, minZoom: 6 });
            createMap();
        }

        navigator.geolocation.getCurrentPosition(updatePosition, positionError);

        function createMap() {
            L.tileLayer('https://api.tiles.mapbox.com/v4/{id}/{z}/{x}/{y}.png?access_token={accessToken}', {
                attribution: 'Gotta find \'em all ! By Antoine, Camille, Loic and Xavier',
                maxZoom: 22,
                id: 'djidane.ciebm5cmz001ttdmakbfuuv2x',
                accessToken: 'pk.eyJ1IjoiZGppZGFuZSIsImEiOiJiNTU1NWU3YmYxNGRmMzhjNGQ5MjFjMWNkODQwMTM4OCJ9.m_HYDHlcIL6WRIwRwWApTA'
            }).addTo(map);

            map.on('overlayremove', function (eventLayer) {
                this.removeControl(legend);
                this.removeControl(info);

            });

            map.on('overlayadd', function (eventLayer) {
                legend.addTo(this);
                info.addTo(this);
            });

            map.on('geosearch_showlocation', function (result) {
                map.setZoom(17);
            });

            menu.addTo(map);

            new L.Control.GeoSearch({
                provider: new L.GeoSearch.Provider.OpenStreetMap(),
                showMarker: false
            }).addTo(map);
            var drawnItems = new L.FeatureGroup();
            map.addLayer(drawnItems);

            var drawControl = new L.Control.Draw({
                draw: {
                    marker: {
                        shapeOptions: {
                            color: '#662d91'
                        }
                    },
                    custom: {
                        shapeOptions: {
                            color: '#662d91'
                        }
                    }
                }
            });
            map.addControl(drawControl);

            map.on('draw:created', function (e) {
                drawnItems.addLayer(e.layer);
                $("#Longitude").val(e.layer._latlng.lng);
                $("#Lattitude").val(e.layer._latlng.lat);
                $("#newFountainDiv").bPopup({
                    onOpen: function () {
                    },
                    follow: [false, false],
                    speed: 250,
                    transition: 'slideIn',
                    transitionClose: 'slideBack'
                });
            });

            init();
        }
    </script>
</body>
</html>
