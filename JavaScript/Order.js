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
            order = orders[i];
            data[i] = {};
            data[i].Team = order.Team;
            data[i].BestBid = order.BestBid;
            data[i].BestAsk = order.BestAsk;
            data[i].MyBid = order.MyBid;
            data[i].MyAsk = order.MyAsk;
        }

        // Define function used to get the data and sort it.
        function getItem(index) {
            return isAsc ? data[orders[index].Team] : data[orders[(data.length - 1) - index].Team];
        }
        function getLength() {
            return numberOfItems;
        }

        grid = new Slick.Grid("#BidAskDiv", data, columns, options);

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

        grid.onClick.subscribe(function (e, args) {
            var cell = grid.getCellFromEvent(e);
            if (!cell || !grid.canCellBeSelected(cell.row, cell.cell)) {
                return;
            }
            var item = args.grid.getData()[args.row];

            $("#TeamOrder").text(item.Team);

            if (cell.cell == 1)
            {
                $("#SideOrder").text('BUY');
                $("#PriceOrder").val(item.BestBid);
                $("#QuantityOrder").val('5');
            }
            else if (cell.cell == 2) {
                $("#SideOrder").text('Sell');
                $("#PriceOrder").val(item.BestAsk);
                $("#QuantityOrder").val('5');
            }
            else if (cell.cell == 3) {
                $("#SideOrder").text('BUY');
                $("#PriceOrder").val(item.MyBid);
                $("#QuantityOrder").val('5');
            }
            else if (cell.cell == 4) {
                $("#SideOrder").text('Sell');
                $("#PriceOrder").val(item.MyAsk);
                $("#QuantityOrder").val('5');
            }
        });
    }
});