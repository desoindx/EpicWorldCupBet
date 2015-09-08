function createIcon(cluster) {
    var n = cluster.numberOfChilds();
	var c = ' marker-cluster-';
	if (n < 25) {
		c += 'small';
	} else if (n < 100) {
		c += 'medium';
	} else {
		c += 'large';
	}

	return new L.DivIcon({ html: '<div><span>' + addCommas(n.toString(), false) + '</span></div>', className: 'marker-cluster' + c, iconSize: new L.Point(40, 40) });
}

function createSingleIcon(val) {
    return new L.DivIcon({ html: '<div><img src="Icons/' + val + '.png"></img></div>', iconSize: new L.Point(30, 30) });
}