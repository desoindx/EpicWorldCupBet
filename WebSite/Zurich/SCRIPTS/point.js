coordinates = [];
xBounds = 449;
yBounds = 141;
precision = 0.000000001;
markers = [];
customPolygons = [];
customCircles = [];

function getColor(d) {
    return d > 500 ? '#BD0026' :
	       d > 200 ? '#FC4E2A' :
	       d > 100 ? '#FD8D3C' :
	       d > 50 ? '#FEB24C' :
	       d > 20 ? '#FED976' :
		   d > 10 ? '#FFEDA0' :
			           '#EAEAEA';
}

function initWind() {
    var startTime = new Date().getTime();
    var elapsedTime = 0;
    for (var i = 0; i < m3Wind.length; i++) {
        var point = m3Wind[i];
        if (point.PP > 1) {
            var bottomleft = new L.LatLng(point.y - 0.005, point.x - 0.005);
            var topright = new L.LatLng(point.y + 0.005, point.x + 0.005);
            var bounds2 = new L.LatLngBounds(bottomleft, topright);

            var currentColor = getColor(point.PP);
            var rect1 = L.rectangle(bounds2, { color: currentColor, fillColor: currentColor, weight: 0, opacity: 1, fillOpacity: 0.5, clickable: false });
            rect1.addTo(map);
        }
    }

    elapsedTime = new Date().getTime() - startTime;
    console.log('Wind' + elapsedTime);
}

function CreateCoordinates() {
    coordinates = [];
    for (var i = 0; i < portfolio.length; i++) {
        var xVal = portfolio[i][0];
        var yVal = portfolio[i][1];
        coordinates.push({ x: xVal, y: yVal, index: portfolio[i][2], value: portfolio[i][3] });
    }
}

function cluster(depth, bounds, leftChild, rightChild, marker, coordinatesPoint, parent) {
    this.depth = depth;
    this.bounds = bounds;
    this.leftChild = leftChild;
    this.rightChild = rightChild;
    this.marker = marker;
    this.coordinatesPoint = coordinatesPoint;
    this.parent = parent;

    this.numberOfChilds = numberOfChilds;
    function numberOfChilds() {
        if (this.leftChild == null && this.rightChild == null)
            return 1;

        if (this.leftChild == null)
            return this.rightChild.numberOfChilds();

        if (this.rightChild == null)
            return this.leftChild.numberOfChilds();

        return this.leftChild.numberOfChilds() + this.rightChild.numberOfChilds();
    }

    this.valueOfChilds = valueOfChilds;
    function valueOfChilds() {
        if (this.leftChild == null && this.rightChild == null)
            return this.marker.value;

        if (this.leftChild == null)
            return this.rightChild.valueOfChilds();

        if (this.rightChild == null)
            return this.leftChild.valueOfChilds();

        return this.leftChild.valueOfChilds() + this.rightChild.valueOfChilds();
    }

    this.maxDepth = maxDepth;
    function maxDepth() {
        if (this.leftChild == null && this.rightChild == null)
            return depth;

        if (this.leftChild == null)
            return this.rightChild.maxDepth();

        if (this.rightChild == null)
            return this.leftChild.maxDepth();

        return Math.max(this.leftChild.maxDepth(), this.rightChild.maxDepth());
    }
}

function rectangle(north, south, east, west) {
    this.north = north;
    this.south = south;
    this.east = east;
    this.west = west;
}

function intersects(rect1, rect2) {
    if (rect1.north < rect2.south || rect1.south > rect2.north || rect1.west > rect2.east || rect1.east < rect2.west)
        return false;

    return true;
}

function rootCluster() {
    return initCluster(portfolio, null, 0, null);
}

function initCluster(coordinatesList, bounds, depth, parent) {
    var currentCluster = new cluster(depth, bounds, null, null, null, null, parent);
    if (coordinatesList.length == 0) {
        return currentCluster;
    }
    else if (coordinatesList.length == 1) {
        var fount = coordinatesList[0];
        currentCluster.coordinatesPoint = { x: fount.x, y: fount.y };
        currentCluster.marker = fount;
    }
    else {
        var XSum = 0;
        var YSum = 0;
        for (var i = 0; i < coordinatesList.length; i++) {
            XSum += coordinatesList[i].x;
            YSum += coordinatesList[i].y;
        }
        var XMean = XSum / coordinatesList.length;
        var YMean = YSum / coordinatesList.length;

        var sumXsquared = 0.0;
        var sumYsquared = 0.0;
        var sumXY = 0.0;

        for (var i = 0; i < coordinatesList.length; i++) {
            var x = coordinatesList[i].x - XMean;
            var y = coordinatesList[i].y - YMean;
            sumXsquared += x * x;
            sumYsquared += y * y;
            sumXY += x * y;
        }

        var aX = 0.0;
        var aY = 0.0;

        if (Math.abs(sumXY) / coordinatesList.length > precision) {
            aX = sumXY;
            var lambda = 0.5 * ((sumXsquared + sumYsquared) + Math.sqrt((sumXsquared + sumYsquared) * (sumXsquared + sumYsquared) + 4 * sumXY * sumXY));
            aY = lambda - sumXsquared;
        }
        else {
            aX = sumXsquared > sumYsquared ? 1.0 : 0.0;
            aY = sumXsquared > sumYsquared ? 0.0 : 1.0;
        }

        var leftAnnotations = [];
        var rightAnnotations = [];

        if (Math.abs(sumXsquared) / coordinatesList.length < precision || Math.abs(sumYsquared) / coordinatesList.length < precision) { // all X and Y are the same => same coordinates
            // then every x equals XMean and we have to arbitrarily choose where to put the pivotIndex
            var pivotIndex = coordinatesList.length / 2;
            for (var i = 0; i < pivotIndex; ++i)
                leftAnnotations.push(coordinatesList[i]);
            for (var i = Math.ceil(pivotIndex) ; i < coordinatesList.length; i++)
                rightAnnotations.push(coordinatesList[i]);
        }
        else {
            // compute scalar product between the vector of this regression line and the vector
            // (x - x(mean))
            // (y - y(mean))
            // the sign of this scalar product determines which cluster the point belongs to
            for (var i = 0; i < coordinatesList.length; i++) {
                var positivityConditionOfScalarProduct = (coordinatesList[i].x - XMean) * aX + (coordinatesList[i].y - YMean) * aY > 0.0;
                if (positivityConditionOfScalarProduct) {
                    leftAnnotations.push(coordinatesList[i]);
                } else {
                    rightAnnotations.push(coordinatesList[i]);
                }
            }
        }
        var leftBounds = getBounds(leftAnnotations);
        var rightBounds = getBounds(rightAnnotations);

        currentCluster.coordinatesPoint = { x: XMean, y: YMean };
        currentCluster.leftChild = initCluster(leftAnnotations, leftBounds, depth + 1, currentCluster);
        currentCluster.rightChild = initCluster(rightAnnotations, rightBounds, depth + 1, currentCluster);
    }
    return currentCluster;
}

function getBounds(coordinatesList) {
    var north = -100;
    var south = 100;
    var east = -250;
    var west = 250;
    for (var i = 0; i < coordinatesList.length; i++) {
        var point = coordinatesList[i];
        if (point.y > north) {
            north = point.y;
        }
        if (point.x > east) {
            east = point.x;
        }
        if (point.y < south) {
            south = point.y;
        }
        if (point.x < west) {
            west = point.x;
        }
    }
    return new rectangle(north, south, east, west);
}

function getLevel(currentCluster, size) {
    var currentClusterSize = Math.max(currentCluster.leftChild.bounds.east, currentCluster.rightChild.bounds.east) -
							Math.min(currentCluster.leftChild.bounds.west, currentCluster.rightChild.bounds.west);

    var depth = currentCluster.maxDepth();
    for (var i = 0; i < depth; i++) {
        currentClusterSize /= 2;
    }

    while (currentClusterSize * 5 < size) {
        currentClusterSize *= 2;
        depth--;
    }

    return Math.max(0, depth);
}

function getNumberOfPointToDisplay(zoom) {
    if (zoom == 18)
        return 100;
    if (zoom > 17)
        return 50;
    if (zoom > 16)
        return 25;
    if (zoom > 11)
        return 10;

    return 1;
}

function findMarkers(currentCluster) {
    var mapBounds = map.getBounds();
    var north = mapBounds.getNorth();
    var south = mapBounds.getSouth();
    var east = mapBounds.getEast();
    var west = mapBounds.getWest();

    var maxPoint = getNumberOfPointToDisplay(map.getZoom());
    var bounds = new rectangle(north, south, east, west);
    for (var i = 0; i < markers.length; i++) {
        map.removeLayer(markers[i]);
    }
    // Start from the root (self)
    // Adopt a breadth-first search strategy
    // If MapRect intersects the bounds, then keep this element for next iteration
    // Stop if there are N elements or more
    // Or if the bottom of the tree was reached (d'oh!)
    markers = [];
    var clusters = [];

    clusters.push(currentCluster);
    var clustersDidChange = true; // prevents infinite loop at the bottom of the tree
    while (clusters.length + markers.length < maxPoint && clusters.length > 0 && clustersDidChange)
        // for (var level = 0; level < levelMax; level++)
    {
        clustersDidChange = false;
        var nextLevelClusters = [];
        for (var i = 0; i < clusters.length; i++) {
            var myCluster = clusters[i];
            if (myCluster.leftChild != null) {
                if (myCluster.leftChild.marker != null) {
                    var m = myCluster.leftChild.marker;
                    var images = '';
                    for (var j = 0; j < m.images.length; j++) {
                        images += '<img style="width:' + winWidth / 4 + 'px;height: ' + winHeight / 4 + 'px;" src="' + m.images[j] + '" runat="server"/>';
                    }
                    markers.push(L.marker([m.y, m.x], {
                        icon: createSingleIcon(m.found)
                    })
                    .on('click', onMarkerClick)
                    .bindPopup(images +
                            '<button onclick="checkExistingFountain(' + m.x + ',' + m.y + ')">I\'m here!</button>'));
                }
                else if (intersects(myCluster.leftChild.bounds, bounds)) {
                    nextLevelClusters.push(myCluster.leftChild);
                }
            }
            if (myCluster.rightChild != null) {
                if (myCluster.rightChild.marker != null) {
                    var m = myCluster.rightChild.marker;
                    var images = '';
                    for (var j = 0; j < m.images.length; j++) {
                        images += '<img style="width:' + winWidth / 4 + 'px;height: ' + winHeight / 4 + 'px;" src="' + m.images[j] + '" runat="server"/>';
                    }
                    markers.push(L.marker([m.y, m.x], {
                        icon: createSingleIcon(m.found)
                    })
                      .on('click', onMarkerClick)
                      .bindPopup(images +
                            '<button onclick="checkExistingFountain(' + m.x + ',' + m.y + ')">I\'m here!</button>'));
                }
                else if (intersects(myCluster.rightChild.bounds, bounds)) {
                    nextLevelClusters.push(myCluster.rightChild);
                }
            }

            if (myCluster.marker != null) {
                var m = myCluster.marker;
                var images = '';
                for (var j = 0; j < m.images.length; j++) {
                    images += '<img style="width:' + winWidth / 4 + 'px;height: ' + winHeight / 4 + 'px;" src="' + m.images[j] + '" runat="server"/>';
                }
                markers.push(L.marker([m.y, m.x], {
                    icon: createSingleIcon(m.found)
                })
                  .on('click', onMarkerClick)
                  .bindPopup(images +
                        '<button onclick="checkExistingFountain(' + m.x + ',' + m.y + ')">I\'m here!</button>'));
            }
        }

        if (nextLevelClusters.length > 0)// && nextLevelClusters.length + markers.length < maxPoint)
        {
            clustersDidChange = true;
        }
        clusters = nextLevelClusters;
    }

    for (var i = 0; i < clusters.length; i++) {
        var marker = new L.Marker([clusters[i].coordinatesPoint.y, clusters[i].coordinatesPoint.x], {
            icon: createIcon(clusters[i])
        });
        marker.on('click', onClusterClick);
        markers.push(marker);
    }

    L.layerGroup(markers).addTo(map);
}

function onMarkerClick(e) {
    map.setView(e.latlng, map.getZoom());
}

function checkExistingFountain(lng, lat) {
    $("#Longitude").val(lng);
    $("#Lattitude").val(lat);
    $("#newFountainDiv").bPopup({
        onOpen: function () {
        },
        follow: [false, false],
        speed: 250,
        transition: 'slideIn',
        transitionClose: 'slideBack'
    });
}

function onClusterClick(e) {
    if (map.getZoom() < 17)
        map.setView(this.getLatLng(), map.getZoom() + 1);
}

function makeArrayOf(value, length) {
    var arr = [], i = length;
    while (i--) {
        arr[i] = value;
    }
    return arr;
}

function updateSearchedValue(x, y) {
    var nb100 = 0;
    var nb200 = 0;
    var nb500 = 0;

    var value100 = 0;
    var value200 = 0;
    var value500 = 0;
    for (var i = 0; i < portfolio.length; i++) {
        var dist = getDist(portfolio[i].x, portfolio[i].y, x, y);
        if (dist < 100) {
            nb100++;
            value100 += portfolio[i].value;
            nb200++;
            value200 += portfolio[i].value;
            nb500++;
            value500 += portfolio[i].value;
        }
        else if (dist < 200) {
            nb200++;
            value200 += portfolio[i].value;
            nb500++;
            value500 += portfolio[i].value;
        }
        else if (dist < 500) {
            nb500++;
            value500 += portfolio[i].value;
        }
    }
    //	document.getElementById("nb100").innerHTML=addCommas(nb100.toString(), true);
    //	document.getElementById("nb200").innerHTML=addCommas(nb200.toString(), true);
    //	document.getElementById("nb500").innerHTML=addCommas(nb500.toString(), true);
    //	
    //	document.getElementById("value100").innerHTML=addCommas(value100.toString(), true);
    //	document.getElementById("value200").innerHTML=addCommas(value200.toString(), true);
    //	document.getElementById("value500").innerHTML=addCommas(value500.toString(), true);
}

function getDist(x1, y1, x2, y2) {
    var latLng = new L.LatLng(y1, x1);
    return latLng.distanceTo(new L.LatLng(y2, x2));
}

function addCommas(nStr, showDecimal) {
    nStr += '';
    x = nStr.split('.');
    x1 = x[0];
    x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    if (showDecimal)
        return x1 + x2;
    else
        return x1;
}

function coutPointsInCircle(circle) {
    var nb = 0;
    var value = 0;
    for (var i = 0; i < portfolio.length; i++) {
        var dist = circle.getLatLng().distanceTo(new L.LatLng(portfolio[i].y, portfolio[i].x));
        if (dist < circle.getRadius()) {
            nb++;
            value += portfolio[i].value;
        }
    }

    return addCommas(nb.toString(), true) + " Police(s) - " + addCommas(value.toString(), true) + " EUR";
}

function countPointsInPolygon(polygon) {
    var nb = 0;
    var value = 0;
    for (var i = 0; i < portfolio.length; i++) {
        if (isPointInPoly2(polygon.getLatLngs(), portfolio[i])) {
            nb++;
            value += portfolio[i].value;
        }
    }

    return addCommas(nb.toString(), true) + " Police(s) - " + addCommas(value.toString(), true) + " EUR";
}

function countPointsInCustomShape() {
    var latlngs = [];
    for (var i = 0; i < customPolygons.length; i++) {
        latlngs.push(customPolygons[i].getLatLngs());
    }

    var nb = 0;
    var value = 0;
    for (var i = 0; i < portfolio.length; i++) {
        var found = false;
        for (var j = 0; j < customCircles.length; j++) {
            var dist = customCircles[j].getLatLng().distanceTo(new L.LatLng(portfolio[i].y, portfolio[i].x));
            if (dist < customCircles[j].getRadius()) {
                found = true;
                nb++;
                value += portfolio[i].value;
                break;
            }
        }
        if (!found) {
            for (var j = 0; j < latlngs.length; j++) {
                if (isPointInPoly2(latlngs[j], portfolio[i])) {
                    nb++;
                    value += portfolio[i].value;
                    break;
                }
            }
        }
    }

    if (nb > 0) {
        document.getElementById("nbCustom").innerHTML = addCommas(nb.toString(), true);
        document.getElementById("valueCustom").innerHTML = addCommas(value.toString(), true);
    }
    else {
        document.getElementById("nbCustom").innerHTML = "-";
        document.getElementById("valueCustom").innerHTML = "-";
    }
}

function isPointInPoly(poly, pt) {
    for (var c = false, i = -1, l = poly.length, j = l - 1; ++i < l; j = i)
        ((poly[i].lat <= pt.lat && pt.lat < poly[j].lat) || (poly[j].lat <= pt.lat && pt.lat < poly[i].lat))
        && (pt.lng < (poly[j].lng - poly[i].lng) * (pt.lat - poly[i].lat) / (poly[j].lat - poly[i].lat) + poly[i].lng)
        && (c = !c);
    return c;
}

function isPointInPoly2(poly, pt) {
    for (var c = false, i = -1, l = poly.length, j = l - 1; ++i < l; j = i)
        ((poly[i].lat <= pt.y && pt.y < poly[j].lat) || (poly[j].lat <= pt.y && pt.y < poly[i].lat))
        && (pt.x < (poly[j].lng - poly[i].lng) * (pt.y - poly[i].lat) / (poly[j].lat - poly[i].lat) + poly[i].lng)
        && (c = !c);
    return c;
}

uploadCount = 0;
function readFile() {
    var file = document.getElementById('file');
    var fileName = file.files[0].name;
    uploadCount++;
    var previousText = document.getElementById("Uploading").innerHTML;
    document.getElementById("Uploading").innerHTML = "<p>Uploading " + fileName + "...</p>" + previousText;
    setTimeout(function () {
        document.getElementById("Uploading").innerHTML = "<p>" + fileName + " Uploaded</p><button id=\"UploadResultButton" + uploadCount + "\">Download Result</button>" + previousText;
        $("#UploadResultButton" + uploadCount)
      		.button()
      		.click(function (event) {
      		    event.preventDefault();
      		    alert('file' + fileName + ' downloaded');
      		});
    }, 1000);
}