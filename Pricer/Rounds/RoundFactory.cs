using System.Collections.Generic;
using System.Linq;
using Datas.Entities;

namespace Pricer
{
    public static class RoundFactory
    {
        public static Round GetRound(string roundType, string roundKey, Competition competition, List<CompetitionResult> results, List<CompetitionGame> games, params object[] parameters)
        {
            return GetRound(roundType, roundKey, games[0].HomeAndAway, competition, results, games.Select(x => x.Team).ToList());
        }
        
        public static Round GetRound(string roundType, string roundKey, bool homeAndAway, Competition competition, List<CompetitionResult> results, List<Team> teams, params object[] parameters)
        {
            return new Round(roundKey, competition, results, teams, homeAndAway);
        }
    }
}
