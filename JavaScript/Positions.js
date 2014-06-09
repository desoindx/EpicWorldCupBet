$(function () {
    drawPositionGrid = function (teams, users, pos) {
        var grid,
         posisitions = [],
         columns = [],
         options = {
             enableCellNavigation: true,
             enableColumnReorder: false,
             multiColumnSort: true
         },
        
         i, j;
        
        columns[0] = { id: "Team", name: "Team", field: "Team", width: 200, sortable: false };
        for (i = 0; i < users.length; i++) {
            columns[i + 1] = { id: users[i], name: users[i], field: users[i], width: 50, sortable: false };
         }

        for (i = 0; i < teams.length; i++) {
             posisitions[i] = {};
             posisitions[i].Team = teams[i];
             for (j = 0; j < users.length;j++) {
                 posisitions[i][users[j]] = pos[j*teams.length + i];
             }
         }

        grid = new Slick.Grid("#PositionsDiv", posisitions, columns, options);
    }
});