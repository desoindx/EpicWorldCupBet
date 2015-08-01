using System;
using System.Collections;
using System.Linq;
using Datas.Entities;

namespace Pricer
{
    using System.Collections.Generic;

    public class Round
    {
        private Dictionary<Team, int> _simulation;
        private List<Team> _teams;
        private List<CompetitionResult> _results;

        public Round(List<CompetitionResult> results, List<CompetitionGame> games)
        {
            _teams = new List<Team>();
            _simulation = new Dictionary<Team, int>();
            foreach (var game in games)
            {
                _teams.Add(game.Team);
            }

            _results = results;
        }

        public Dictionary<Team, Team> RealTeams { get; set; }

        public List<Team> Teams { get { return _teams; } }

        public List<Tuple<Team, Team>> Games()
        {
            var games = new List<Tuple<Team, Team>>();
            for (int i = 0; i < _teams.Count; i++)
            {
                var team1 = _teams[i];
                for (int j = i + 1; j < _teams.Count; j++)
                {
                    games.Add(new Tuple<Team, Team>(RealTeams[team1], RealTeams[_teams[j]]));
                }
            }

            for (int i = 0; i < _results.Count; i += 4)
            {
                var game = new Tuple<Team, Team>(RealTeams.Values.First(x => x.Name == _results[i].Result),
                    RealTeams.Values.First(x => x.Name == _results[i + 1].Result));
                AddResult(game, 0.5, Double.Parse(_results[i + 2].Result), Double.Parse(_results[i + 3].Result));
                games.Remove(game);
            }

            return games;
        }

        public void AddResult(Tuple<Team, Team> game, double random, double strength1, double strength2)
        {
            if (!_simulation.ContainsKey(game.Item1))
            {
                _simulation[game.Item1] = 0;
            }
            
            if (!_simulation.ContainsKey(game.Item2))
            {
                _simulation[game.Item2] = 0;
            }

            var r = random * (strength1 + strength2);
            if (r > strength1)
            {
                _simulation[game.Item1] += 3;
                _simulation[game.Item2] += 0;
            }
            else
            {
                _simulation[game.Item2] += 3;
                _simulation[game.Item1] += 0;
            }
        }

        public Team GetTeam(int i)
        {
            return _simulation.OrderBy(x => x.Value).ElementAt(i-1).Key;
        }
    }
}
