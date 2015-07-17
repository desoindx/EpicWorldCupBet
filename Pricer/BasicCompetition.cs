using System;
using System.Collections.Generic;
using System.Linq;
using Datas.Entities;

namespace Pricer
{
    public class BasicCompetition
    {
        private string _name;
        private readonly Competition _competition;
        private Dictionary<int, Team> _teams;
        private Dictionary<string, List<CompetitionGame>> _games;
        private Dictionary<string, List<CompetitionResult>> _results; 
        private List<CompetitionPrize> _prizes; 

        public BasicCompetition(string name)
        {
            _name = name;
            using (var context = new Entities())
            {
                _competition = context.Competitions.First(x => x.Name == name);
                _teams = context.Teams.Where(x => x.IdCompetition == _competition.Id).ToDictionary(x => x.Id);
                _games = context.CompetitionGames.Where(x => x.CompetitionId == _competition.Id).GroupBy(x => x.RoundKey).ToDictionary(x => x.Key, x => x.ToList());
                _results = context.CompetitionResults.Where(x => x.CompetitionId == _competition.Id).GroupBy(x => x.RoundKey).ToDictionary(x => x.Key, x => x.ToList()); ;
                _prizes = context.CompetitionPrizes.Where(x => x.CompetitionId == _competition.Id).ToList();
            }
        }

        public List<Tuple<string, double>> Price()
        {
            return new List<Tuple<string, double>>();
        }
    }
}
