using System;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Pricer
{
    public static class Pricer
    {
        private static readonly Dictionary<string, BasicCompetition> Competitions = new Dictionary<string, BasicCompetition>();

        public static List<Tuple<string, double>> Price(string competitionName)
        {
            BasicCompetition competition;
            if (!Competitions.TryGetValue(competitionName, out competition))
            {
                competition = new BasicCompetition(competitionName);
            }

            return competition.Price();
        }
    }
}
