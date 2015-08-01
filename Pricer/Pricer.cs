using System.Collections.Generic;
using Datas.Entities;

namespace Pricer
{
    public static class Pricer
    {
        private static readonly Dictionary<string, BasicCompetition> Competitions = new Dictionary<string, BasicCompetition>();

        public static Dictionary<Team, double> Price(string competitionName, Dictionary<Team, double> strengths)
        {
            BasicCompetition competition;
            if (!Competitions.TryGetValue(competitionName, out competition))
            {
                competition = new StrengthCompetition(competitionName, strengths);
                Competitions[competitionName] = competition;
            }

            return competition.Price();
        }
    }
}
