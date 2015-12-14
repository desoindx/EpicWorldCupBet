function drawPositionGrid(competitionId, positions) {
    var grid,
        columns = [],
        options = {
            enableCellNavigation: true,
            enableColumnReorder: false
        };


    columns[0] = { id: "Team", name: "Team", field: "Team", width: 200, sortable: true, resizable: false };
    columns[1] = { id: "Position", name: "Position", field: "Position", width: 75, sortable: true, resizable: false };
    columns[2] = { id: "Best", name: "Best", field: "Best", width: 75, sortable: true, resizable: false };
    columns[3] = { id: "Best10", name: "Best 10%", field: "Best10", width: 75, sortable: true, resizable: false };
    columns[4] = { id: "Average", name: "Average", field: "Average", width: 75, sortable: true, resizable: false };
    columns[5] = { id: "Worst10", name: "Worst 10%", field: "Worst10", width: 75, sortable: true, resizable: false };
    columns[6] = { id: "Worst", name: "Worst", field: "Worst", width: 75, sortable: true, resizable: false };

    grid = new Slick.Grid("#PositionsDiv-" + competitionId, positions, columns, options);

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
}

