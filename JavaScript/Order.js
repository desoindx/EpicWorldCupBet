$(function () {
    drawOrdersGrid = function (orders) {
        var grid,
         bidasks = [],
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
            bidasks[i] = {};
            bidasks[i].Team = order.Team;
            if (order.BestBid != 0)
                bidasks[i].BestBid = order.BestBid;
            else
                bidasks[i].BestBid = '';
            if (order.BestAsk != 0)
                bidasks[i].BestAsk = order.BestAsk;
            else
                bidasks[i].BeBestAskstBid = '';
            if (order.MyBid != 0)
                bidasks[i].MyBid = order.MyBid;
            else
                bidasks[i].MyBid = '';
            if (order.MyAsk != 0)
                bidasks[i].MyAsk = order.MyAsk;
            else
                bidasks[i].MyAsk = '';
            bidasks[i].BestBidQuantity = order.BestBidQuantity;
            bidasks[i].BestAskQuantity = order.BestAskQuantity;
            bidasks[i].MyBidQuantity = order.MyBidQuantity;
            bidasks[i].MyAskQuantity = order.MyAskQuantity;
        }

        grid = new Slick.Grid("#BidAskDiv", bidasks, columns, options);

        grid.onSort.subscribe(function (e, args) {
            var cols = args.sortCols;
            bidasks.sort(function (dataRow1, dataRow2) {
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
            if (cell.cell == 0)
                return;

            $("#TeamOrder").text(item.Team);

            if (cell.cell == 1) {
                $("#CancelOrder").hide();
                $("#SideOrder").text('SELL');
                $("#PriceOrder").val(item.BestBid);
                $("#QuantityOrder").val(item.BestBidQuantity);
            }
            else if (cell.cell == 2) {
                $("#CancelOrder").hide();
                $("#SideOrder").text('BUY');
                $("#PriceOrder").val(item.BestAsk);
                $("#QuantityOrder").val(item.BestAskQuantity);
            }
            else if (cell.cell == 3) {
                $("#CancelOrder").show();
                $("#CancelOrder").html('Cancel Buy Order');
                $("#SideOrder").text('BUY');
                $("#PriceOrder").val(item.MyBid);
                $("#QuantityOrder").val(item.MyBidQuantity);
            }
            else if (cell.cell == 4) {
                $("#CancelOrder").show();
                $("#CancelOrder").html('Cancel Sell Order');
                $("#SideOrder").text('SELL');
                $("#PriceOrder").val(item.MyAsk);
                $("#QuantityOrder").val(item.MyAskQuantity);
            }
        });
    }
});