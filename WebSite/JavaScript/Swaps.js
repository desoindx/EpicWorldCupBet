function showTeamSelectPicker() {
    var teamsPicker = $('.teamSelectPicker');
    for (var j = 0; j < teamsPicker.length; j++)
        teamsPicker[j].style.display = 'none';
    $("#TeamBuyDiv")[0].style.display = '';
    $("#TeamSellDiv")[0].style.display = '';
    $("#TeamBuy").selectpicker('setStyle', 'teamOrderSelect', 'add');
    $("#TeamBuy").selectpicker('refresh');
    $("#TeamSell").selectpicker('setStyle', 'teamOrderSelect', 'add');
    $("#TeamSell").selectpicker('refresh');
}

function formatter(row, cell, value, columnDef, dataContext) {
    return value;
}

function buttonsFormatter(row, cell, value, columnDef, dataContext) {
    return "<input type='button' class='btn btn-info btn-xs center-block' value='" + value + "' id='btnForm' value2='" + row + "' value3='" + cell + "'/>";
}

function checkBoxFormatter(row, cell, value, columnDef, dataContext) {
    if (value == true)
        return "<input type='checkbox' checked  onclick='return false'></input>";

    return "<input type='checkbox' onclick='return false'></input>";
}

function onOrder(objBtn) {
    alert("row::[" + objBtn.value2 + "]\n\ncell::[" + objBtn.value3 + "]");
    openBookPopup($.bidasks[competitionId][objBtn.value2].TeamName);
}

function drawSwapsGrid(orders, competitionId) {
    if (!$.bidasks)
        $.bidasks = [];
    if (!$.grids)
        $.grids = [];

    var columns = [
      { id: "IsMine", name: "Mine", field: "IsMine", width: 40, sortable: true, formatter: checkBoxFormatter, resizable: false },
      { id: "BuyQuantity", name: "Quantity", field: "BuyQuantity", width: 130, sortable: true, resizable: false },
      { id: "BuyTeam", name: "Buyed Team", field: "BuyTeam", width: 280, sortable: true, resizable: false },
      { id: "SellQuantity", name: "Quantity", field: "SellQuantity", width: 130, sortable: true, resizable: false },
      { id: "SellTeam", name: "Selled Team", field: "SellTeam", width: 280, sortable: true, resizable: false },
      { id: "Price", name: "Price", field: "Price", width: 130, sortable: true, resizable: false }
    ],
    options = {
        enableCellNavigation: true,
        enableColumnReorder: false
    };

    $.bidasks[competitionId] = orders;

    var currentGrid = new Slick.Grid("#BidAskDiv-" + competitionId, orders, columns, options);
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