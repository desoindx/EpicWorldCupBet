$(function () {

    drawOrdersGrid = function (orders) {
        var grid,
         data = [],
         columns = [
           { id: "Team", name: "Team", field: "Team", width: 200, sortable: true },
           { id: "BestBid", name: "Best Bid", field: "BestBid", width: 100, sortable: true },
           { id: "BestAsk", name: "Best Ask", field: "BestAsk", width: 100, sortable: true },
           { id: "MyBid", name: "My Bid", field: "MyBid", width: 100, sortable: true },
           { id: "MyAsk", name: "My Ask", field: "MyAsk", width: 100, sortable: true }
         ],
         options = {
             enableCellNavigation: true,
             enableColumnReorder: false,
             multiColumnSort: true
         },

         numberOfItems = orders.length, isAsc = true, currentSortCol = { id: "Team" }, i;

        // Assign values to the data.
        for (i = numberOfItems; i-- > 0;) {
            team = orders[i].Team;
            data[i] = {};
            data[i].Team = team;
            data[i].BestBid = orders[i].Price;
            data[i].BestAsk = orders[i].Price;
            data[i].MyBid = orders[i].Price;
            data[i].MyAsk = orders[i].Price;
        }

        // Define function used to get the data and sort it.
        function getItem(index) {
            return isAsc ? data[orders[index].Team] : data[orders[(data.length - 1) - index].Team];
        }
        function getLength() {
            return numberOfItems;
        }

        grid = new Slick.Grid("#container", data, columns, options);

        grid.onSort.subscribe(function (e, args) {
            var cols = args.sortCols;
            data.sort(function (dataRow1, dataRow2) {
                for (var i = 0, l = cols.length; i < l; i++) {
                    var field = cols[i].sortCol.field;
                    var sign = cols[i].sortAsc ? 1 : -1;
                    var value1 = dataRow1[field], value2 = dataRow2[field];
                    var result = (value1 == value2 ? 0 : (value1 > value2 ? 1 : -1)) * sign;
                    if (result != 0) {
                        return result;
                    }
                }
                return 0;
            });
            grid.invalidateAllRows();
            grid.render();
        });
    }
});