using System;
using System.Collections;
using System.Linq;
using Datas.Entities;

namespace Pricer
{
    using System.Collections.Generic;

    [Serializable]
    public class Round
    {
        [Serializable]
        private class RoundResult
        {
            public int Point;
            public int ScoreFor;
            public int ScoreAgainst;

            public void Add(int point, int scoreFor, int scoreAgainst)
            {
                Point += point;
                ScoreFor += scoreFor;
                ScoreAgainst += scoreAgainst;
            }
        }

        private List<Team> _orderedSimulation;
        private readonly Dictionary<Team, RoundResult> _simulation;
        private readonly List<Team> _teams;
        private readonly List<CompetitionResult> _results;
        private readonly Competition _competition;

        public string RoundKey { get; private set; }

        public bool HomeAndAway { get; private set; }

        public Round(string roundKey, Competition competition, List<CompetitionResult> results, List<Team> teams, bool homeAndAway)
        {
            _teams = teams;
            HomeAndAway = homeAndAway;
            _simulation = new Dictionary<Team, RoundResult>();

            RoundKey = roundKey;
            _results = results;
            _competition = competition;
        }

        public List<Team> Teams { get { return _teams; } }

        public List<Tuple<Team, Team>> Games()
        {
            var games = new List<Tuple<Team, Team>>();
            for (int i = 0; i < _teams.Count; i++)
            {
                var team1 = _teams[i];
                var start = HomeAndAway ? 0 : i + 1;
                for (int j = start; j < _teams.Count; j++)
                {
                    var team2 = _teams[j];
                    if (!Equals(team1, team2))
                    {
                        games.Add(new Tuple<Team, Team>(team1, team2));
                    }
                }
            }

            for (int i = 0; i < _results.Count; i += 4)
            {
                var game = new Tuple<Team, Team>(Teams.First(x => x.Name == _results[i].Result),
                    Teams.First(x => x.Name == _results[i + 1].Result));
                AddResult(game, int.Parse(_results[i + 2].Result), int.Parse(_results[i + 3].Result));
                games.Remove(game);
            }

            return games;
        }

        private void AddResult(Tuple<Team, Team> game, int score1, int score2)
        {
            _orderedSimulation = null;
            if (!_simulation.ContainsKey(game.Item1))
            {
                _simulation[game.Item1] = new RoundResult();
            }

            if (!_simulation.ContainsKey(game.Item2))
            {
                _simulation[game.Item2] = new RoundResult();
            }

            if (score1 > score2)
            {
                _simulation[game.Item1].Add(3, score1, score2);
                _simulation[game.Item2].Add(0, score2, score1);
            }
            else if (score2 > score1)
            {
                _simulation[game.Item1].Add(0, score1, score2);
                _simulation[game.Item2].Add(3, score2, score1);
            }
            else
            {
                _simulation[game.Item1].Add(1, score1, score2);
                _simulation[game.Item2].Add(1, score2, score1);
            }
        }

        public void AddResult(Tuple<Team, Team> game, double random, double strength1, double strength2)
        {
            _orderedSimulation = null;
            if (!_simulation.ContainsKey(game.Item1))
            {
                _simulation[game.Item1] = new RoundResult();
            }

            if (!_simulation.ContainsKey(game.Item2))
            {
                _simulation[game.Item2] = new RoundResult();
            }

            var sum = strength1 + strength2;
            var tie = _competition.Tie;
            var r = random * sum * (1 + tie);
            if (r > sum)
            {
                var score = Helper.NextPoisson(tie);
                _simulation[game.Item1].Add(1,score, score);
                _simulation[game.Item2].Add(1, score, score);
            }
            else if (r > strength1)
            {
                var score1 = Helper.NextPoisson(tie);
                var score2 = Helper.NextPoisson(tie);
                if (score2 > score1)
                {
                    _simulation[game.Item1].Add(0, score1, score2);
                    _simulation[game.Item2].Add(3, score2, score1);
                }
                else
                {
                    _simulation[game.Item1].Add(0, score2, score1);
                    _simulation[game.Item2].Add(3, score1, score2);
                }
            }
            else
            {
                var score1 = Helper.NextPoisson(tie);
                var score2 = Helper.NextPoisson(tie);
                if (score2 > score1)
                {
                    _simulation[game.Item2].Add(0, score1, score2);
                    _simulation[game.Item1].Add(3, score2, score1);
                }
                else
                {
                    _simulation[game.Item2].Add(0, score2, score1);
                    _simulation[game.Item1].Add(3, score1, score2);
                }
            }
        }

        public Team GetTeam(int i)
        {
            if (_orderedSimulation == null)
            {
                OrderSimulation();
            }

            return _orderedSimulation.ElementAt(i - 1);
        }

        private void OrderSimulation()
        {
            _orderedSimulation = _simulation.OrderByDescending(x => x.Value.Point)
                .ThenByDescending(x => x.Value.ScoreFor - x.Value.ScoreAgainst)
                .ThenByDescending(x => x.Value.ScoreFor)
                .ThenBy(x => Helper.NextDouble()).Select(x => x.Key).ToList();
        }
    }
}
