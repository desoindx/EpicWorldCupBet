﻿function drawPricingGrid(ranks) {
    var grid,
     columns = [],
     options = {
         enableCellNavigation: true,
         enableColumnReorder: false
     };

    columns[0] = { id: "Team", name: "Team", field: "Team", width: 200, sortable: true };
    columns[1] = { id: "Price", name: "Price", field: "Price", width: 200, sortable: true };

    grid = new Slick.Grid("#PricingDiv", ranks, columns, options);

    grid.setSelectionModel(new Slick.CellSelectionModel());
    grid.registerPlugin(new Slick.CellExternalCopyManager());

    grid.onSort.subscribe(function (e, args) {
        var cols = args.sortCol;
        var field = cols.field;
        var sign = args.sortAsc ? 1 : -1;
        this.getData().sort(function (dataRow1, dataRow2) {
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
    grid.setSortColumn("Team", false);
}