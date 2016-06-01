function createIcon(cluster, id) {
    var n = cluster.numberOfChilds();
    return new L.DivIcon({ html: '<canvas id="myChart' + id + '" width="40" height="40"></canvas><div class="innerDonut">' + addCommas(n.toString(), false) + '</div>', iconSize: new L.Point(40, 40) });
}

function createSingleIcon(val) {
    return new L.DivIcon({ html: "<div><img style='width:100%;height:100%' src='Icons/" + val + ".png'></img></div>", iconSize: new L.Point(70, 70) });
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
            color: "#ED1C24"
        },
        {
            value: values[2],
            color: "#39B54A"
        },
        {
            value: values[3],
            color: "#FCEE21"
        },
        {
            value: values[4],
            color: "#3FA9F5"
        }
    ];
    new Chart(ctx).Doughnut(data, {
        animation: false,
        showTooltips:false
    });
}