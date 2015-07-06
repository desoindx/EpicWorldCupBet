using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Datas.Entities;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Manager
{
    public static class Importer
    {
        private const string Games = "Games";
        private const string Results = "Results";
        private const string Prizes = "Prizes";

        public static void Import(string fileName)
        {
            var competition = GetCompetition(Path.GetFileNameWithoutExtension(fileName));
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(fileName, false))
            {
                var workbookPart = spreadsheetDocument.WorkbookPart;
                var sheets = workbookPart.Workbook.Descendants<Sheet>();
                var sharedString = CreateShareStringTable(workbookPart);
                foreach (var sheet in sheets)
                {
                    var sheetName = sheet.Name.ToString();
                    SheetData sheetData = ((WorksheetPart)workbookPart.GetPartById(sheet.Id)).Worksheet.Descendants<SheetData>().First();
                    switch (sheetName)
                    {
                        case Games:
                            ImportGames(sharedString, sheetData, competition);
                            break;
                        case Results:
                            ImportResults(sharedString, sheetData, competition);
                            break;
                        case Prizes:
                            ImportPrizes(sharedString, sheetData, competition);
                            break;
                    }
                }
            }
        }

        private static Competition GetCompetition(string competitionName)
        {
            using (var context = new Entities())
            {
                var competition = context.Competitions.FirstOrDefault(x => x.Name == competitionName);
                if (competition == null)
                {
                    competition = context.Competitions.Create();
                    competition.EndDate = DateTime.Now.AddYears(1);
                    competition.StartDate = DateTime.Now;
                    competition.Name = competitionName;
                    competition.Type = string.Empty;
                    context.Competitions.Add(competition);
                    context.SaveChanges();
                }

                return competition;
            }
        }

        private static List<string> CreateShareStringTable(WorkbookPart workbookPart)
        {
            var stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
            return stringTable == null ? new List<string>() : stringTable.SharedStringTable.Select(x => x.InnerText).ToList();
        }

        private static void ImportPrizes(List<string> sharedString, SheetData sheetData, Competition competition)
        {
            using (var context = new Entities())
            {
                context.CompetitionPrizes.RemoveRange(
                    context.CompetitionPrizes.Where(x => x.CompetitionId == competition.Id));
                foreach (Row row in sheetData.Elements<Row>())
                {
                    var cells = row.Elements<Cell>();
                    var firstCell = cells == null ? null : cells.FirstOrDefault();
                    if (firstCell == null)
                    {
                        continue;
                    }


                    var rounds = cells.Skip(1).Select(x => GetCellValue(sharedString, x));
                    var prizeName = GetCellValue(sharedString, firstCell);
                    foreach (var round in rounds)
                    {
                        var newPrize = context.CompetitionPrizes.Create();
                        newPrize.CompetitionId = competition.Id;
                        newPrize.PrizeName = prizeName;
                        newPrize.RoundKey = round;
                    }
                    context.SaveChanges();
                }
            }
        }

        private static string GetCellValue(List<string> sharedString, Cell cell)
        {
            var value = cell.InnerText;
            if (cell.DataType != null)
            {
                switch (cell.DataType.Value)
                {
                    case CellValues.SharedString:
                        return sharedString[int.Parse(value)];

                    case CellValues.Boolean:
                        if (value == "0")
                        {
                            return "FALSE";
                        }

                        return "TRUE";
                }
            }

            return value;
        }

        private static void ImportResults(List<string> sharedString, SheetData sheetData, Competition competition)
        {
            using (var context = new Entities())
            {
                context.CompetitonResults.RemoveRange(
                    context.CompetitonResults.Where(x => x.CompetitionId == competition.Id));

                foreach (Row row in sheetData.Elements<Row>())
                {
                    var cells = row.Elements<Cell>();
                    var firstCell = cells == null ? null : cells.FirstOrDefault();
                    if (firstCell == null)
                    {
                        continue;
                    }

                    var results = cells.Skip(1).Select(x => GetCellValue(sharedString, x));
                    var roundKey = GetCellValue(sharedString, firstCell);
                    foreach (var result in results)
                    {
                        var newPrize = context.CompetitonResults.Create();
                        newPrize.CompetitionId = competition.Id;
                        newPrize.Result = result;
                        newPrize.RoundKey = roundKey;
                    }
                    context.SaveChanges();
                }
            }
        }

        private static void ImportGames(List<string> sharedString, SheetData sheetData, Competition competition)
        {
            using (var context = new Entities())
            {
                context.CompetitionGames.RemoveRange(
                    context.CompetitionGames.Where(x => x.CompetitionId == competition.Id));

                var teams = context.Teams.Where(x => x.IdCompetition == competition.Id).ToList();
                foreach (Row row in sheetData.Elements<Row>())
                {
                    var cells = row.Elements<Cell>();
                    var firstCell = cells == null ? null : cells.FirstOrDefault();
                    if (firstCell == null || GetCellValue(sharedString, firstCell).StartsWith("%"))
                    {
                        continue;
                    }

                    var roundTeams = cells.Skip(1).Select(x => GetCellValue(sharedString, x));
                    var roundKey = GetCellValue(sharedString, firstCell);
                    foreach (var team in roundTeams)
                    {
                        var newPrize = context.CompetitionGames.Create();
                        newPrize.CompetitionId = competition.Id;
                        newPrize.TeamId = GetTeamId(team, teams, competition, context);
                        newPrize.RoundKey = roundKey;
                    }
                    context.SaveChanges();
                }
            }
        }

        private static int GetTeamId(string team, List<Team> teams, Competition competition, Entities context)
        {
            var teamId = teams.FirstOrDefault(x => x.Name == team);
            if (teamId != null)
            {
                return teamId.Id;
            }

            teamId = context.Teams.Create();
            teamId.IdCompetition = competition.Id;
            teamId.Name = team;
            context.SaveChanges();

            return teamId.Id;
        }
    }
}
