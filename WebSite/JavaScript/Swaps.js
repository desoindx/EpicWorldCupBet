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
    { id: "BuyTeam", name: "Buying", field: "BuyTeam", width: 280, sortable: true, resizable: false },
    { id: "SellQuantity", name: "Quantity", field: "SellQuantity", width: 130, sortable: true, resizable: false },
    { id: "SellTeam", name: "Selling", field: "SellTeam", width: 280, sortable: true, resizable: false },
    { id: "Price", name: "Price", field: "Price", width: 130, sortable: true, resizable: false },
    { id: "Id", name: "Id", field: "Id", visible: false }
    ],
    options = {
        enableCellNavigation: true,
        enableColumnReorder: false
    };

    $.bidasks[competitionId] = orders;

    var currentGrid = new Slick.Grid("#BidAskDiv-" + competitionId, orders, columns, options);
    currentGrid.Id = competitionId;

    currentGrid.setSelectionModel(new Slick.CellSelectionModel());
    currentGrid.registerPlugin(new Slick.CellExternalCopyManager());

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

        if (item.IsMine) {
            $("#CancelOrder").show();
            $("#SendOrder").hide();
            $("#TeamBuy").selectpicker('val', item.BuyTeam);
            $("#TeamSell").selectpicker('val', item.SellTeam);
            $("#QuantityBuyOrder").val(item.BuyQuantity);
            $("#QuantitySellOrder").val(item.SellQuantity);
            $("#PriceOrder").val(item.Price);
        }
        else {
            $("#CancelOrder").hide();
            $("#SendOrder").show();
            $("#TeamBuy").selectpicker('val', item.SellTeam);
            $("#TeamSell").selectpicker('val', item.BuyTeam);
            $("#QuantityBuyOrder").val(item.SellQuantity);
            $("#QuantitySellOrder").val(item.BuyQuantity);
            $("#PriceOrder").val(-item.Price);
        }

        $("#SwapBuyTeam").html($("#TeamBuy").val());
        $("#SwapSellTeam").html($("#TeamSell").val());
        $("#SwapBuyQuantity").html($("#QuantityBuyOrder").val());
        $("#SwapSellQuantity").html($("#QuantitySellOrder").val());
        if (parseInt($("#PriceOrder").val()) > 0) {
            $("#SwapPrice").html($("#PriceOrder").val());
            $("#SwapWay").html("receive");
        } else {
            $("#SwapPrice").html(-parseInt($("#PriceOrder").val()));
            $("#SwapWay").html("pay");
        }

        selectedItem = item.Id;

        openOrderPopup();

        isNewMenu = false;

        if (item.IsMine) {
            $("#CancelOrder").show();
            $("#SendOrder").hide();
        }
        else {
            $("#CancelOrder").hide();
            $("#SendOrder").prop('value', 'Match Swap');
        }

        $('#QuantityBuyOrder').prop('readonly', true);
        $('#QuantitySellOrder').prop('readonly', true);
        $('#PriceOrder').prop('readonly', true);
        $('#TeamBuyDiv').css('pointer-events', 'none');
        $('#TeamSellDiv').css('pointer-events', 'none');
    });

    $.grids[competitionId] = currentGrid;
}