using System.Collections.Generic;
using Datas.Entities;

namespace Pricer
{
    public static class RoundFactory
    {
        public static Round GetRound(string roundType, List<CompetitionResult> results, List<CompetitionGame> games, params object[] parameters)
        {
            return new Round(results, games);
        }
    }
}
