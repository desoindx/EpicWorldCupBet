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
             enableCellNavigation: false,
             enableColumnReorder: false
         },
         numberOfItems = orders.length, isAsc = true, currentSortCol = { id: "Team" }, i;


        // Assign values to the data.
        for (i = numberOfItems; i-- > 0;) {
            team = orders[i].Team;
            data[team] = {};
            data[team].Team = team;
            data[team].BestBid = orders[i].Price;
            data[team].BestAsk = orders[i].Price;
            data[team].MyBid = orders[i].Price;
            data[team].MyAsk = orders[i].Price;
        }

        // Define function used to get the data and sort it.
        function getItem(index) {
            alert(index);
            return isAsc ? data[orders[index].Team] : data[orders[(data.length - 1) - index].Team];
        }
        function getLength() {
            return numberOfItems;
        }

        grid = new Slick.Grid("#container", { getLength: getLength, getItem: getItem }, columns, options);
        grid.onSort.subscribe(function (e, args) {
            currentSortCol = args.sortCol;
            isAsc = args.sortAsc;
            grid.invalidateAllRows();
            grid.render();
        });
    }
});