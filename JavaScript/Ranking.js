$(function () {
    drawRankingGrid = function (users, ranks) {
        var grid,
         rank = [],
         columns = [],
         options = {
             enableCellNavigation: true,
             enableColumnReorder: false
         },
        
         i, j;

        columns[0] = { id: "User", name: "User", field: "User", width: 200, sortable: true };
        columns[1] = { id: "Money", name: "Money", field: "Money", width: 200, sortable: true };

        for (i = 0; i < users.length; i++) {
            rank[i] = {};
            rank[i].User = users[i];
            rank[i].Money = ranks[i];
         }

        grid = new Slick.Grid("#RankingDiv", rank, columns, options);

        grid.setSelectionModel(new Slick.CellSelectionModel());
        grid.registerPlugin(new Slick.CellExternalCopyManager());

        grid.onSort.subscribe(function (e, args) {
            var cols = args.sortCols;
            rank.sort(function (dataRow1, dataRow2) {
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
    }
});