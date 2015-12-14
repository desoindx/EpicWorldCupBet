function sorterDateIso(a, b) {
    var regex_a = new RegExp("^((19[1-9][1-9])|([2][01][0-9]))\\d-([0]\\d|[1][0-2])-([0-2]\\d|[3][0-1])(\\s([0]\\d|[1][0-2])(\\:[0-5]\\d){1,2}(\\:[0-5]\\d){1,2})?$", "gi");
    var regex_b = new RegExp("^((19[1-9][1-9])|([2][01][0-9]))\\d-([0]\\d|[1][0-2])-([0-2]\\d|[3][0-1])(\\s([0]\\d|[1][0-2])(\\:[0-5]\\d){1,2}(\\:[0-5]\\d){1,2})?$", "gi");

    if (regex_a.test(a[sortcol]) && regex_b.test(b[sortcol])) {
        var date_a = new Date(a[sortcol]);
        var date_b = new Date(b[sortcol]);
        var diff = date_a.getTime() - date_b.getTime();
        return sortdir * (diff === 0 ? 0 : (date_a > date_b ? 1 : -1));
    }
    else {
        var x = a[sortcol], y = b[sortcol];
        return sortdir * (x === y ? 0 : (x > y ? 1 : -1));
    }
}

function drawHistoryGrid(competitionId, trades) {
    var grid,
        columns = [],
        options = {
            enableCellNavigation: true,
            enableColumnReorder: false
        };


    columns[0] = { id: "Date", name: "Date", field: "Date", width: 150, sortable: true, sorter: sorterDateIso, resizable: false };
    columns[1] = { id: "Quantity", name: "Quantity", field: "Quantity", width: 75, sortable: true, resizable: false };
    columns[2] = { id: "Team", name: "Team", field: "Team", width: 200, sortable: true, resizable: false };
    columns[3] = { id: "Price", name: "Price", field: "Price", width: 65, sortable: true, resizable: false };
    columns[4] = { id: "Buyer", name: "Buyer", field: "Buyer", width: 150, sortable: true, resizable: false };
    columns[5] = { id: "Seller", name: "Seller", field: "Seller", width: 150, sortable: true, resizable: false };

    grid = new Slick.Grid("#TradesDiv-" + competitionId, trades, columns, options);

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
