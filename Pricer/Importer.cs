namespace Datas
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using DocumentFormat.OpenXml.Packaging;
    using DocumentFormat.OpenXml.Spreadsheet;

    public static class Importer
    {
        private const string Games = "Games";
        private const string Results = "Results";
        private const string Prizes = "Prizes";

        public static BasicCompetition Import(string fileName)
        {
            var competition = new BasicCompetition(Path.GetFileNameWithoutExtension(fileName));
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

            return competition;
        }

        private static List<string> CreateShareStringTable(WorkbookPart workbookPart)
        {
            var stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
            return stringTable == null ? new List<string>() : stringTable.SharedStringTable.Select(x => x.InnerText).ToList();
        }

        private static void ImportPrizes(List<string> sharedString, SheetData sheetData, BasicCompetition competition)
        {
            foreach (Row row in sheetData.Elements<Row>())
            {
                var cells = row.Elements<Cell>();
                var firstCell = cells == null ? null : cells.FirstOrDefault();
                if (firstCell == null)
                {
                    continue;
                }

                competition.AddPrize(GetCellValue(sharedString, firstCell), cells.Skip(1).Select(x => GetCellValue(sharedString, x)));
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

        private static void ImportResults(List<string> sharedString, SheetData sheetData, BasicCompetition competition)
        {
            foreach (Row row in sheetData.Elements<Row>())
            {
                var cells = row.Elements<Cell>();
                var firstCell = cells == null ? null : cells.FirstOrDefault();
                if (firstCell == null)
                {
                    continue;
                }

                competition.AddResultForRound(int.Parse(GetCellValue(sharedString, firstCell)), cells.Skip(1).Select(x => GetCellValue(sharedString, x)));
            }
        }

        private static void ImportGames(List<string> sharedString, SheetData sheetData, BasicCompetition competition)
        {
            foreach (Row row in sheetData.Elements<Row>())
            {
                var cells = row.Elements<Cell>();
                var firstCell = cells == null ? null : cells.FirstOrDefault();
                if (firstCell == null || GetCellValue(sharedString, firstCell).StartsWith("%"))
                {
                    continue;
                }

                var index = int.Parse(row.RowIndex);
                competition.AddRound(index, cells.Select(x => GetCellValue(sharedString, x)));
            }
        }
    }
}
