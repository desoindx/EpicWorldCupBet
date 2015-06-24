$(function () {
    $('select#country').listbox({ 'searchbar': true });

    $('#divCountry').click(function () {
        if (currSelection != $('#country').find(':selected').text())
        {
            currSelection = $('#country').find(':selected').text();
            drawChart();
        }
    });

    function round(d) {
        return Math.round(100 * d) / 100;
    }

    currSelection = $('#country').find(':selected').text();
    drawChart = function()
    {
        var ohlcData = [];
        var volumeData = [];
        for (var i = 0; i< chartsData.length; i++)
        {
            var data = chartsData[i];
            if (data.Team == currSelection)
            {
                $('#totalVolume').html(data.TotalVolume);
                for (var j = 0; j < data.Trades.length; j++)
                {
                    var trades = data.Trades[j];
                    ohlcData.push([new Date(trades.Time), trades.High, trades.Low, trades.Open, trades.Close])
                    volumeData.push([new Date(trades.Time), trades.Volume]);
                }

                $('#jqChart').jqChart({
                    legend: { visible: false },
                    border: { lineWidth: 0, padding: 0 },
                    tooltips: {
                        type: 'shared',
                        disabled: true
                    },
                    crosshairs: {
                        enabled: true,
                        hLine: false
                    },
                    animation: { duration: 1 },
                    axes: [
                        {
                            type: 'linear',
                            location: 'left',
                            width: 30
                        }
                    ],
                    series: [
                        {
                            title: 'Price Index',
                            type: 'candlestick',
                            data: ohlcData,
                            priceUpFillStyle: 'white',
                            priceDownFillStyle: 'black',
                            strokeStyle: 'black'
                        }
                    ]
                });

                $('#jqChartVolume').jqChart({
                    legend: { visible: false },
                    border: { lineWidth: 0, padding: 0 },
                    tooltips: {
                        type: 'shared',
                        disabled: true
                    },
                    crosshairs: {
                        enabled: true,
                        hLine: false
                    },
                    animation: { duration: 1 },
                    axes: [
                        {
                            type: 'dateTime',
                            location: 'bottom'
                        },
                        {
                            type: 'linear',
                            location: 'left',
                            width: 30
                        }
                    ],
                    series: [
                        {
                            type: 'column',
                            data: volumeData,
                            fillStyle: 'black'
                        }
                    ]
                });

                var isHighlighting = false;

                $('#jqChart').bind('dataHighlighting', function (event, data) {

                    if (!data) {
                        $('#jqChartVolume').jqChart('highlightData', null);
                        return;
                    }

                    $('#open').html(data.open);
                    $('#high').html(data.high);
                    $('#low').html(data.low);
                    $('#close').html(data.close);

                    var date = data.chart.stringFormat(data.x, "mmmm d, yyyy HH") + "h";

                    $('#date').html(date);

                    if (!isHighlighting) {

                        isHighlighting = true;

                        var index = $.inArray(data.dataItem, ohlcData);
                        $('#jqChartVolume').jqChart('highlightData', [volumeData[index]]);
                    }

                    isHighlighting = false;
                });

                $('#jqChartVolume').bind('dataHighlighting', function (event, data) {

                    if (!data) {
                        $('#jqChart').jqChart('highlightData', null);
                        return;
                    }

                    $('#volume').html(data.y);

                    if (!isHighlighting) {

                        isHighlighting = true;

                        var index = $.inArray(data.dataItem, volumeData);
                        $('#jqChart').jqChart('highlightData', [ohlcData[index]]);
                    }

                    isHighlighting = false;
                });

                $('#jqChart').jqChart('highlightData', [ohlcData[0]]);
                $('#jqChartVolume').jqChart('highlightData', [volumeData[0]]);
            }
        }
    }
});