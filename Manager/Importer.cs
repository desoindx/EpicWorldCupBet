using System;
using System.Collections.Generic;
using System.Globalization;
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
        private const string Forces = "Forces";
        private const string Infos = "Infos";

        private const string AverageScore = "Average Score";
        private const string Tie = "Tie";

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
                        case Infos:
                            ImportInfos(sharedString, sheetData, competition);
                            break;
                        case Forces:
                            ImportForces(sharedString, sheetData, competition);
                            break;
                    }
                }
            }
        }

        private static void ImportForces(List<string> sharedString, SheetData sheetData, Competition competition)
        {
            using (var context = new Entities())
            {
                var teams = context.Teams.Where(x => x.IdCompetition == competition.Id).ToDictionary(x => x.Name);

                foreach (Row row in sheetData.Elements<Row>())
                {
                    var cells = row.Elements<Cell>();
                    var firstCell = cells == null ? null : cells.FirstOrDefault();
                    if (firstCell == null)
                    {
                        continue;
                    }

                    var team = teams[GetCellValue(sharedString, firstCell)];
                    team.Strength = double.Parse(GetCellValue(sharedString, cells.ElementAt(1)), CultureInfo.InvariantCulture);
                }
                context.SaveChanges();
            }
        }

        private static void ImportInfos(List<string> sharedString, SheetData sheetData, Competition competition)
        {
            using (var context = new Entities())
            {
                var compet = context.Competitions.First(x => x.Id == competition.Id);
                foreach (Row row in sheetData.Elements<Row>())
                {
                    var cells = row.Elements<Cell>();
                    var firstCell = cells == null ? null : cells.FirstOrDefault();
                    if (firstCell == null)
                    {
                        continue;
                    }

                    switch (GetCellValue(sharedString, firstCell))
                    {
                        case Tie:
                            compet.Tie = double.Parse(GetCellValue(sharedString, cells.ElementAt(1)), CultureInfo.InvariantCulture);
                            break;
                        case AverageScore:
                            compet.Score = double.Parse(GetCellValue(sharedString, cells.ElementAt(1)), CultureInfo.InvariantCulture);
                            break;
                    }
                }
                context.SaveChanges();
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
                    competition.EndDate = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time")).AddYears(1);
                    competition.StartDate = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time"));
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

                    var rounds = cells.Skip(2).Select(x => GetCellValue(sharedString, x)).Where(x => !string.IsNullOrEmpty(x));
                    var prizeName = GetCellValue(sharedString, firstCell);
                    var value = Int32.Parse(GetCellValue(sharedString, cells.ElementAt(1)));
                    foreach (var round in rounds)
                    {
                        var newPrize = context.CompetitionPrizes.Create();
                        newPrize.CompetitionId = competition.Id;
                        newPrize.PrizeName = prizeName;
                        newPrize.Value = value;
                        newPrize.RoundKey = round;
                        context.CompetitionPrizes.Add(newPrize);
                    }
                }
                context.SaveChanges();
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
                context.CompetitionResults.RemoveRange(
                    context.CompetitionResults.Where(x => x.CompetitionId == competition.Id));

                foreach (Row row in sheetData.Elements<Row>())
                {
                    var cells = row.Elements<Cell>();
                    var firstCell = cells == null ? null : cells.FirstOrDefault();
                    if (firstCell == null)
                    {
                        continue;
                    }

                    var results = cells.Skip(1).Select(x => GetCellValue(sharedString, x)).Where(x => !string.IsNullOrEmpty(x));
                    var roundKey = GetCellValue(sharedString, firstCell);
                    foreach (var result in results)
                    {
                        var newResult = context.CompetitionResults.Create();
                        newResult.CompetitionId = competition.Id;
                        newResult.Result = result;
                        newResult.RoundKey = roundKey;
                        context.CompetitionResults.Add(newResult);
                    }
                }
                context.SaveChanges();
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

                    var roundTeams = cells.Skip(1).Select(x => GetCellValue(sharedString, x)).Where(x => !string.IsNullOrEmpty(x));
                    var roundKey = GetCellValue(sharedString, firstCell);
                    foreach (var team in roundTeams)
                    {
                        var newGame = context.CompetitionGames.Create();
                        newGame.CompetitionId = competition.Id;
                        newGame.TeamId = GetTeamId(team, teams, competition, context);
                        newGame.RoundKey = roundKey;
                        context.CompetitionGames.Add(newGame);
                    }
                }
                context.SaveChanges();
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
            teamId.RealTeam = !team.StartsWith("*");
            context.Teams.Add(teamId);
            context.SaveChanges();

            return teamId.Id;
        }
    }
}
