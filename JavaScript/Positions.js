$(function () {
    drawPositionGrid = function (teams, users, pos) {
        var grid,
         posisitions = [],
         columns = [],
         options = {
             enableCellNavigation: true,
             enableColumnReorder: false
         },
        
         i, j;
        
        columns[0] = { id: "Team", name: "Team", field: "Team", width: 200, sortable: true };
        for (i = 0; i < users.length; i++) {
            columns[i + 1] = { id: users[i], name: users[i], field: users[i], width: 150, sortable: true };
         }

        for (i = 0; i < teams.length; i++) {
             posisitions[i] = {};
             posisitions[i].Team = teams[i];
             for (j = 0; j < users.length;j++) {
                 posisitions[i][users[j]] = pos[j*teams.length + i];
             }
         }

        grid = new Slick.Grid("#PositionsDiv", posisitions, columns, options);

        grid.setSelectionModel(new Slick.CellSelectionModel());
        grid.registerPlugin(new Slick.CellExternalCopyManager());

        grid.onSort.subscribe(function (e, args) {
            var cols = args.sortCol;
            var field = cols.field;
            var sign = args.sortAsc ? 1 : -1;
            posisitions.sort(function (dataRow1, dataRow2) {
                var value1 = dataRow1[field], value2 = dataRow2[field];
                if (value1 == '')
                    value1 = 0;
                if (value2 == '')
                    value2 = 0;
                var result = (value1 == value2 ? 0 : (value1 > value2 ? 1 : -1)) * sign;
                return result;
            });
            grid.invalidateAllRows();
            grid.render();
        });
    }
});