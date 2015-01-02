$('.TeamCell').live('mouseover mouseout', function (event) {
    if (event.type == 'mouseover') {
        $.connection.Bet.server.getOrderBook(event.currentTarget.innerText);
        popup('orderBookDiv');
    } else {
        popup('orderBookDiv');
    }
});

function clickSellSide() {
    $("#SideOrderSell").addClass("SelectedSide")
    $("#SideOrderBuy").removeClass("SelectedSide");
    $("#SideOrder").val("SELL");
}

function clickBuySide() {
    $("#SideOrderBuy").addClass("SelectedSide")
    $("#SideOrderSell").removeClass("SelectedSide");
    $("#SideOrder").val("BUY");
}

function setUpOrder(order, isMine, i, shouldFlash, cellToFlash) {
    var cellNb = 0;
    if (order.LastTradedPriceEvolution > 0)
        $.bidasks[i].Team = order.Team + " (" + order.LastTradedPrice + ") " + "<span class='glyphicon glyphicon-arrow-up'style='color:green'/>";
    else if (order.LastTradedPriceEvolution < 0)
        $.bidasks[i].Team = order.Team + " (" + order.LastTradedPrice + ") " + "<span class='glyphicon glyphicon-arrow-down'style='color:red'/>";
    else
        $.bidasks[i].Team = order.Team + " (" + order.LastTradedPrice + ") " + "<span class='glyphicon glyphicon-arrow-right'style='color:blue'/>";
    $.bidasks[i].TeamName = order.Team;

    if (order.BestBid != 0) {
        if (shouldFlash) {
            var cellIndex = $.grid.getColumnIndex('BestBid');
            if ($.bidasks[i].BestBid > order.BestBid) {
                cellToFlash[cellNb] = { row: i, cell: cellIndex, className: 'priceDown' };
                cellNb++;
            }
            else if ($.bidasks[i].BestBid < order.BestBid) {
                cellToFlash[cellNb] = { row: i, cell: cellIndex, className: 'priceUp' };
                cellNb++;
            }
        }
        $.bidasks[i].BestBid = order.BestBid;
    }
    else {
        if (shouldFlash) {
            var cellIndex = $.grid.getColumnIndex('BestBid');
            if ($.bidasks[i].BestBid != '') {
                cellToFlash[cellNb] = { row: i, cell: cellIndex, className: 'priceDown' };
                cellNb++;
            }
        }
        $.bidasks[i].BestBid = '';
    }

    if (order.BestAsk != 0) {
        if (shouldFlash) {
            var cellIndex = $.grid.getColumnIndex('BestAsk');
            if ($.bidasks[i].BestAsk > order.BestAsk) {
                cellToFlash[cellNb] = { row: i, cell: cellIndex, className: 'priceDown' };
                cellNb++;
            }
            else if ($.bidasks[i].BestAsk < order.BestAsk) {
                cellToFlash[cellNb] = { row: i, cell: cellIndex, className: 'priceUp' };
                cellNb++;
            }
        }
        $.bidasks[i].BestAsk = order.BestAsk;
    }
    else {
        if (shouldFlash) {
            var cellIndex = $.grid.getColumnIndex('BestAsk');
            if ($.bidasks[i].BestAsk != '') {
                cellToFlash[cellNb] = { row: i, cell: cellIndex, className: 'priceDown' };
                cellNb++;
            }
        }
        $.bidasks[i].BestAsk = '';
    }

    $.bidasks[i].BestBidQuantity = order.BestBidQuantity;
    $.bidasks[i].BestAskQuantity = order.BestAskQuantity;

    if (!isMine) return;

    if (order.MyBid != 0) {
        if (shouldFlash) {
            var cellIndex = $.grid.getColumnIndex('MyBid');
            if ($.bidasks[i].MyBid > order.MyBid) {
                cellToFlash[cellNb] = { row: i, cell: cellIndex, className: 'priceDown' };
                cellNb++;
            }
            else if ($.bidasks[i].MyBid < order.MyBid) {
                cellToFlash[cellNb] = { row: i, cell: cellIndex, className: 'priceUp' };
                cellNb++;
            }
        }
        $.bidasks[i].MyBid = order.MyBid;
    }
    else {
        if (shouldFlash) {
            var cellIndex = $.grid.getColumnIndex('MyBid');
            if ($.bidasks[i].MyBid != '') {
                cellToFlash[cellNb] = { row: i, cell: cellIndex, className: 'priceDown' };
                cellNb++;
            }
        }
        $.bidasks[i].MyBid = '';
    }

    if (order.MyAsk != 0) {
        if (shouldFlash) {
            var cellIndex = $.grid.getColumnIndex('MyAsk');
            if ($.bidasks[i].MyAsk > order.MyAsk) {
                cellToFlash[cellNb] = { row: i, cell: cellIndex, className: 'priceDown' };
                cellNb++;
            }
            else if ($.bidasks[i].MyAsk < order.MyAsk) {
                cellToFlash[cellNb] = { row: i, cell: cellIndex, className: 'priceUp' };
                cellNb++;
            }
        }
        $.bidasks[i].MyAsk = order.MyAsk;
    }
    else {
        if (shouldFlash) {
            var cellIndex = $.grid.getColumnIndex('MyAsk');
            if ($.bidasks[i].MyAsk != '') {
                cellToFlash[cellNb] = { row: i, cell: cellIndex, className: 'priceDown' };
                cellNb++;
            }
        }
        $.bidasks[i].MyAsk = '';
    }

    $.bidasks[i].MyBidQuantity = order.MyBidQuantity;
    $.bidasks[i].MyAskQuantity = order.MyAskQuantity;
}

$(function () {

    function formatter(row, cell, value, columnDef, dataContext) {
        return value;
    }

    drawOrderBook = function (team, bids, asks) {
        $("#orderBookTeamName").text(team);
        var columns = [
          { id: "Price", name: "Price", field: "Price", width: 100, sortable: true },
          { id: "Quantity", name: "Quantity", field: "Quantity", width: 100, sortable: true }
        ],
        options = {
            enableCellNavigation: true,
            enableColumnReorder: false
        };

        new Slick.Grid("#bidGrid", bids, columns, options);
        new Slick.Grid("#askGrid", asks, columns, options);
    }

    drawOrdersGrid = function (orders) {
        $.bidasks = []
         var columns = [
           { id: "Team", name: "Team", field: "Team", width: 200, sortable: true, formatter: formatter, cssClass: "TeamCell" },
           { id: "BestBid", name: "Best Bid", field: "BestBid", width: 100, sortable: true },
           { id: "BestAsk", name: "Best Ask", field: "BestAsk", width: 100, sortable: true },
           { id: "MyBid", name: "My Bid", field: "MyBid", width: 100, sortable: true },
           { id: "MyAsk", name: "My Ask", field: "MyAsk", width: 100, sortable: true }
         ],
         options = {
             enableCellNavigation: true,
             enableColumnReorder: false
         },

         numberOfItems = orders.length, isAsc = true, currentSortCol = { id: "Team" }, i;

        // Assign values to the data.
        for (i = numberOfItems; i-- > 0;) {
            order = orders[i];
            $.bidasks[i] = {};
            setUpOrder(order, true, i, false, null);
        }

        $.grid = new Slick.Grid("#BidAskDiv", $.bidasks, columns, options);

        $.grid.onSort.subscribe(function (e, args) {
            var cols = args.sortCol;
            var field = cols.field;
            var sign = args.sortAsc ? 1 : -1;
            $.bidasks.sort(function (dataRow1, dataRow2) {
                var value1 = dataRow1[field], value2 = dataRow2[field];
                if (value1 == '')
                    value1 = 0;
                if (value2 == '')
                    value2 = 0;
                var result = (value1 == value2 ? 0 : (value1 > value2 ? 1 : -1)) * sign;
                return result;
            });
            $.grid.invalidateAllRows();
            $.grid.render();
        });

        $.grid.onClick.subscribe(function (e, args) {
            var cell = $.grid.getCellFromEvent(e);
            if (!cell || !$.grid.canCellBeSelected(cell.row, cell.cell)) {
                return;
            }
            var item = args.grid.getData()[args.row];

            $("#TeamOrder").selectpicker('val', item.TeamName);

            if (cell.cell == 0) {
                return;
            }
            else if (cell.cell == 1) {
                $("#CancelOrder").hide();
                clickSellSide();
                $("#PriceOrder").val(item.BestBid);
                $("#QuantityOrder").val(item.BestBidQuantity);
            }
            else if (cell.cell == 2) {
                $("#CancelOrder").hide();
                clickBuySide();
                $("#PriceOrder").val(item.BestAsk);
                $("#QuantityOrder").val(item.BestAskQuantity);
            }
            else if (cell.cell == 3) {
                $("#CancelOrder").show();
                $("#CancelOrder").prop('value', 'Cancel Buy Order');
                clickBuySide();
                $("#PriceOrder").val(item.MyBid);
                $("#QuantityOrder").val(item.MyBidQuantity);
            }
            else if (cell.cell == 4) {
                $("#CancelOrder").show();
                $("#CancelOrder").prop('value', 'Cancel Sell Order');
                clickSellSide();
                $("#PriceOrder").val(item.MyAsk);
                $("#QuantityOrder").val(item.MyAskQuantity);
            }
            popup('newOrderDiv');
        });
    }
});