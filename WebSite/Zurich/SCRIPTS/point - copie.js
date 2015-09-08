coordinates = [];
xBounds = 449;
yBounds = 141;
//markers = [];

function CreateCoordinates(nbOfPoint)
{
	coordinates = [];
	for (var i = 0; i < nbOfPoint; i++)
	{
		var xVal = Math.random() * xBounds;
		var yVal = Math.random() * yBounds;
		coordinates.push({x: xVal, y: yVal});
	}
}

function DrawCoordinates(levelOfZoom)
{
	/*for (var i = 0; i < markers.length; i++)
	{
		map.removeLayer(markers[i]);
	}
	*/
    var north = map.getBounds().getNorth() + 58;
    var south = map.getBounds().getSouth() + 58;
    var east = map.getBounds().getEast() + 222;
    var west = map.getBounds().getWest() + 222;
    var size;
	switch(levelOfZoom)
	{
		case 2 :
			size = 5;
			break;
		case 3 :
			size = 10;
			break;
		case 4 :
			size = 15;
			break;
		case 5 :
			size = 20;
			break;
		default:
			drawAllPoints(north, south, east, west);
			return;
	}
	
	var x = xBounds / size;
	var y = yBounds / size;
	
	var point = makeArrayOf(0,size*size);
	
	for (var i = 0; i < coordinates.length; i++)
	{
		var xCoor = coordinates[i].x;
		var yCoor = coordinates[i].y;
		if (xCoor > west && xCoor < east && yCoor < north && yCoor > south)
			point[Math.floor(xCoor / x) + size * Math.floor(yCoor / y)] += 1;
	}
	
	markers = [];
	for (var i = 0; i < point.length; i++)
	{
		if (point[i] > 0)
		{
			var marker = new L.Marker([Math.floor(i/size) * y + 0.5 * y - 58, x * (i % size) + 0.5 * x - 222], {
    			icon:	new L.NumberedDivIcon({number: point[i].toString()})
			});
			marker.on('click', onMarkerClick)
			markers.push(marker);
		}
	}
	L.layerGroup(markers).addTo(map);
}

function drawAllPoints(north, south, east, west)
{	
	/*markers = [];
	for (var i = 0; i < coordinates.length; i++)
	{
		var xCoor = coordinates[i].x;
		var yCoor = coordinates[i].y;
		if (xCoor > west && xCoor < east && yCoor < north && yCoor > south)
		{
			markers.push(L.marker([yCoor - 58, xCoor - 222]).on('click', onMarkerClick));
		}
	}
	L.layerGroup(markers).addTo(map);*/

  var markers = L.markerClusterGroup();

  for (var i = 0; i < coordinates.length; i++) {
		var xCoor = coordinates[i].x;
		var yCoor = coordinates[i].y;
    var marker = L.marker([yCoor - 58, xCoor - 222]);
    markers.addLayer(marker);
  }

  map.addLayer(markers);
}

function onMarkerClick(e) {
	if (map.getZoom() < 6)
		map.setView(this.getLatLng(), map.getZoom() + 1);
}

function makeArrayOf(value, length) {
  var arr = [], i = length;
  while (i--) {
    arr[i] = value;
  }
  return arr;
}