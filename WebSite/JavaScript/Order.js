function showTeamSelectPicker() {
    var teamsPicker = $('.teamSelectPicker');
    for (var j = 0; j < teamsPicker.length; j++)
        teamsPicker[j].style.display = 'none';
    $("#TeamOrderDiv")[0].style.display = '';
    $("#TeamOrder").selectpicker('setStyle', 'teamOrderSelect', 'add');
    $("#TeamOrder").selectpicker('refresh');
}

function setUpOrder(order, isMine, i, shouldFlash, cellToFlash, competitionId) {
    var cellNb = 0;
    var currentDatas = $.bidasks[competitionId][i];
    if (order.LastTradedPriceEvolution > 0)
        currentDatas.Last = "<span class='glyphicon glyphicon-arrow-up'style='color:#4caf50'> </span>";
    else if (order.LastTradedPriceEvolution < 0)
        currentDatas.Last = "<span class='glyphicon glyphicon-arrow-down'style='color:#e51c23'> </span>";
    else
        currentDatas.Last = "<span class='glyphicon glyphicon-arrow-right'style='color:blue'> </span>";

    currentDatas.Team = order.Team + " (" + order.LastTradedPrice + ") ";
    currentDatas.TeamName = order.Team;
    currentDatas.Book = "Show All";

    if (order.BestBid != 0) {
        if (shouldFlash) {
            var cellIndex = $.grids[competitionId].getColumnIndex('BestBid');
            if (currentDatas.BestBid > order.BestBid) {
                cellToFlash[cellNb] = { row: i, cell: cellIndex, className: 'priceDown' };
                cellNb++;
            }
            else if (currentDatas.BestBid < order.BestBid) {
                cellToFlash[cellNb] = { row: i, cell: cellIndex, className: 'priceUp' };
                cellNb++;
            }
        }
        currentDatas.BestBid = order.BestBid;
    }
    else {
        if (shouldFlash) {
            var cellIndex = $.grids[competitionId].getColumnIndex('BestBid');
            if (currentDatas.BestBid != '') {
                cellToFlash[cellNb] = { row: i, cell: cellIndex, className: 'priceDown' };
                cellNb++;
            }
        }
        currentDatas.BestBid = '';
    }

    if (order.BestAsk != 0) {
        if (shouldFlash) {
            var cellIndex = $.grids[competitionId].getColumnIndex('BestAsk');
            if (currentDatas.BestAsk > order.BestAsk) {
                cellToFlash[cellNb] = { row: i, cell: cellIndex, className: 'priceDown' };
                cellNb++;
            }
            else if (currentDatas.BestAsk < order.BestAsk) {
                cellToFlash[cellNb] = { row: i, cell: cellIndex, className: 'priceUp' };
                cellNb++;
            }
        }
        currentDatas.BestAsk = order.BestAsk;
    }
    else {
        if (shouldFlash) {
            var cellIndex = $.grids[competitionId].getColumnIndex('BestAsk');
            if (currentDatas.BestAsk != '') {
                cellToFlash[cellNb] = { row: i, cell: cellIndex, className: 'priceDown' };
                cellNb++;
            }
        }
        currentDatas.BestAsk = '';
    }

    currentDatas.BestBidQuantity = order.BestBidQuantity;
    currentDatas.BestAskQuantity = order.BestAskQuantity;

    if (!isMine) return;

    if (order.MyBid != 0) {
        if (shouldFlash) {
            var cellIndex = $.grids[competitionId].getColumnIndex('MyBid');
            if (currentDatas.MyBid > order.MyBid) {
                cellToFlash[cellNb] = { row: i, cell: cellIndex, className: 'priceDown' };
                cellNb++;
            }
            else if (currentDatas.MyBid < order.MyBid) {
                cellToFlash[cellNb] = { row: i, cell: cellIndex, className: 'priceUp' };
                cellNb++;
            }
        }
        currentDatas.MyBid = order.MyBid;
    }
    else {
        if (shouldFlash) {
            var cellIndex = $.grids[competitionId].getColumnIndex('MyBid');
            if (currentDatas.MyBid != '') {
                cellToFlash[cellNb] = { row: i, cell: cellIndex, className: 'priceDown' };
                cellNb++;
            }
        }
        currentDatas.MyBid = '';
    }

    if (order.MyAsk != 0) {
        if (shouldFlash) {
            var cellIndex = $.grids[competitionId].getColumnIndex('MyAsk');
            if (currentDatas.MyAsk > order.MyAsk) {
                cellToFlash[cellNb] = { row: i, cell: cellIndex, className: 'priceDown' };
                cellNb++;
            }
            else if (currentDatas.MyAsk < order.MyAsk) {
                cellToFlash[cellNb] = { row: i, cell: cellIndex, className: 'priceUp' };
                cellNb++;
            }
        }
        currentDatas.MyAsk = order.MyAsk;
    }
    else {
        if (shouldFlash) {
            var cellIndex = $.grids[competitionId].getColumnIndex('MyAsk');
            if (currentDatas.MyAsk != '') {
                cellToFlash[cellNb] = { row: i, cell: cellIndex, className: 'priceDown' };
                cellNb++;
            }
        }
        currentDatas.MyAsk = '';
    }

    currentDatas.MyBidQuantity = order.MyBidQuantity;
    currentDatas.MyAskQuantity = order.MyAskQuantity;
}

function formatter(row, cell, value, columnDef, dataContext) {
    return value;
}

function buttonsFormatter(row, cell, value, columnDef, dataContext) {
    return "<input type='button' class='btn btn-info btn-xs center-block' value='" + value + "' id='btnForm' value2='" + row + "' value3='" + cell + "'/>";
}

function onOrder(objBtn) {
    alert("row::[" + objBtn.value2 + "]\n\ncell::[" + objBtn.value3 + "]");
    openBookPopup($.bidasks[competitionId][objBtn.value2].TeamName);
}

function drawOrderBook(team, bids, asks) {
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

function drawOrdersGrid(orders, competitionId) {
    if (!$.bidasks)
        $.bidasks = [];
    if (!$.grids)
        $.grids = [];

    var currentDatas = [];

    var columns = [
      { id: "Last", name: "", field: "Last", width: 20, sortable: true, formatter: formatter, resizable:false },
      { id: "Team", name: "Team", field: "Team", width: 280, sortable: true, resizable: false },
      { id: "BestBid", name: "Best Bid", field: "BestBid", width: 130, sortable: true, resizable: false },
      { id: "BestAsk", name: "Best Ask", field: "BestAsk", width: 130, sortable: true, resizable: false },
      { id: "MyBid", name: "My Bid", field: "MyBid", width: 130, sortable: true, resizable: false },
      { id: "MyAsk", name: "My Ask", field: "MyAsk", width: 130, sortable: true, resizable: false },
      { id: "Book", name: "Order Book", field: "Book", width: 90, sortable: false, formatter: buttonsFormatter, resizable: false }
    ],
    options = {
        enableCellNavigation: true,
        enableColumnReorder: false
    },

    numberOfItems = orders.length, i;

    $.bidasks[competitionId] = currentDatas;
    // Assign values to the data.
    for (i = numberOfItems; i-- > 0;) {
        var order = orders[i];
        currentDatas[i] = {};
        setUpOrder(order, true, i, false, null, competitionId);
    }

    var currentGrid = new Slick.Grid("#BidAskDiv-" + competitionId, currentDatas, columns, options);
    currentGrid.Id = competitionId;

    currentGrid.onSort.subscribe(function (e, args) {
        var cols = args.sortCol;
        var field = cols.field;
        var sign = args.sortAsc ? 1 : -1;
        $.bidasks[this.Id].sort(function (dataRow1, dataRow2) {
            var value1 = dataRow1[field], value2 = dataRow2[field];
            if (value1 == '')
                value1 = 0;
            if (value2 == '')
                value2 = 0;
            var result = (value1 == value2 ? 0 : (value1 > value2 ? 1 : -1)) * sign;
            return result;
        });
        this.invalidateAllRows();
        this.render();
    });

    currentGrid.onClick.subscribe(function (e, args) {
        var cell = this.getCellFromEvent(e);
        if (!cell || !this.canCellBeSelected(cell.row, cell.cell)) {
            return;
        }
        var item = args.grid.getData()[args.row];
        showTeamSelectPicker();
        $("#TeamOrder").selectpicker('val', item.TeamName);

        if (cell.cell < 2) {
            return;
        }
        else if (cell.cell == 2) {
            $("#CancelOrder").hide();
            $("#SellSide").prop( "checked", true );
            if (item.BestBid != 0)
                $("#PriceOrder").val(item.BestBid);
            if (item.BesBestBidQuantitytBid != 0)
                $("#QuantityOrder").val(item.BestBidQuantity);
        }
        else if (cell.cell == 3) {
            $("#CancelOrder").hide();
            $("#BuySide").prop("checked", true);
            if (item.BestAsk != 0)
                $("#PriceOrder").val(item.BestAsk);
            if (item.BestAskQuantity != 0)
                $("#QuantityOrder").val(item.BestAskQuantity);
        }
        else if (cell.cell == 4) {
            $("#CancelOrder").show();
            $("#CancelOrder").prop('value', 'Cancel Existing Order');
            $("#BuySide").prop("checked", true);
            if (item.MyBid != 0)
                $("#PriceOrder").val(item.MyBid);
            if (item.MyBidQuantity != 0)
                $("#QuantityOrder").val(item.MyBidQuantity);
        }
        else if (cell.cell == 5) {
            $("#CancelOrder").show();
            $("#CancelOrder").prop('value', 'Cancel Existing Order');
            $("#SellSide").prop("checked", true);
            if (item.MyAsk != 0)
                $("#PriceOrder").val(item.MyAsk);
            if (item.MyAskQuantity != 0)
                $("#QuantityOrder").val(item.MyAskQuantity);
        }
        else if (cell.cell == 6) {
            openBookPopup(item.Team);
            return;
        }
        openOrderPopup();
    });

    $.grids[competitionId] = currentGrid;
}