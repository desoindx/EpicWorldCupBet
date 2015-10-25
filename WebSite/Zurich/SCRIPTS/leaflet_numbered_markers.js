function createIcon(cluster, id) {
    var n = cluster.numberOfChilds();
    return new L.DivIcon({ html: '<canvas id="myChart' + id + '" width="40" height="40"></canvas><div class="innerDonut">' + addCommas(n.toString(), false) + '</div>', iconSize: new L.Point(40, 40) });
}

function createSingleIcon(val) {
    return new L.DivIcon({ html: '<div><img src="Icons/' + val + '.png"></img></div>', iconSize: new L.Point(30, 30) });
}

function animatePieChart(cluster, id) {
    var ctx = document.getElementById("myChart" + id).getContext("2d");
    var values = cluster.getValues();
    var data = [
        {
            value: values[0],
            color: "#0000FF"
        },
        {
            value: values[1],
            color: "#FF0000"
        },
        {
            value: values[2],
            color: "#00FF33"
        },
        {
            value: values[3],
            color: "#F2FF00"
        }
    ];
    new Chart(ctx).Doughnut(data, {
        animation: false,
        showTooltips:false
    });
}