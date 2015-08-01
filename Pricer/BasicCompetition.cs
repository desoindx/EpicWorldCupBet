using System;
using System.Collections.Generic;
using System.Linq;
using Datas;
using Datas.Entities;

namespace Pricer
{
    public abstract class BasicCompetition
    {
        private const int SimulationNumber = 1000000;

        private string _name;
        private readonly Competition _competition;
        private Dictionary<int, Team> _teams;
        private Dictionary<string, Round> _rounds;
        private Dictionary<string, List<CompetitionGame>> _games;
        private Dictionary<string, List<CompetitionResult>> _results;
        private List<CompetitionPrize> _prizes;
        private Dictionary<string, double> _money; 

        public BasicCompetition(string name)
        {
            _name = name;
            using (var context = new Entities())
            {
                _competition = context.Competitions.First(x => x.Name == name);
                _teams = context.Teams.Where(x => x.IdCompetition == _competition.Id).ToDictionary(x => x.Id);
                _games = context.CompetitionGames.Where(x => x.CompetitionId == _competition.Id).GroupBy(x => x.RoundKey).ToDictionary(x => x.Key, x => x.ToList());
                _results = context.CompetitionResults.Where(x => x.CompetitionId == _competition.Id).GroupBy(x => x.RoundKey).ToDictionary(x => x.Key, x => x.ToList());
                _prizes = context.CompetitionPrizes.Where(x => x.CompetitionId == _competition.Id).ToList();
            }
        }

        public Dictionary<Team, double> Price()
        {
            _rounds = new Dictionary<string, Round>();
            var results = new Dictionary<Team, double>();
            foreach (var team in _teams)
            {
                results[team.Value] = 0;
            }

            for (int i = 0; i < SimulationNumber; i++)
            {
                var result = SimulateCompetition();
                foreach (var r in result)
                {
                    results[r.Item1] += r.Item2;
                }
            }

            for (int index = 0; index < results.Count; index++)
            {
                var item = results.ElementAt(index);
                results[item.Key] = item.Value / SimulationNumber;
            }

            return results;
        }

        private List<Tuple<Team, double>> SimulateCompetition()
        {
            _rounds.Clear();
            foreach (var game in _games)
            {
                if (_rounds.ContainsKey(game.Key))
                {
                    continue;
                }

                var round = RoundFactory.GetRound("", _results.ContainsKey(game.Key) ? _results[game.Key] : new List<CompetitionResult>(), game.Value);
                _rounds.Add(game.Key, round);
                SimulateRound(round);
            }

            var results = new List<Tuple<Team, double>>();

            foreach (var prize in _prizes)
            {
                Team team;
                if (!prize.RoundKey.StartsWith("*"))
                {
                    team = _teams.Values.First(x => x.Name == prize.RoundKey.Trim());
                }
                else
                {
                    var prizeInfo = prize.RoundKey.Remove(0, 1).Split('-');
                    var key = prizeInfo[0];
                    var round = _rounds[key];
                    team = round.GetTeam(int.Parse(prizeInfo[1]));
                }

                results.Add(new Tuple<Team, double>(team, prize.Value));
            }

            return results;
        }

        private void SimulateRound(Round round)
        {
            UpdateTeams(round);
            foreach (var game in round.Games())
            {
                Simulate(round, game);
            }
        }

        private void UpdateTeams(Round round)
        {
            var realTeams = new Dictionary<Team, Team>();
            foreach (var team in round.Teams)
            {
                realTeams[team] = GetTeam(team);
            }

            round.RealTeams = realTeams;
        }

        private Team GetTeam(Team team)
        {
            if (team.RealTeam.Value)
            {
                return team;
            }

            var teamInfos = team.Name.Remove(0, 1).Split('-');
            Round round;
            var key = teamInfos[0];
            if (!_rounds.TryGetValue(key, out round))
            {
                round = RoundFactory.GetRound("", _results.ContainsKey(key) ? _results[key] : new List<CompetitionResult>(), _games[key]);
                _rounds[key] = round;
                SimulateRound(round);
            }

            var newTeam = round.GetTeam(int.Parse(teamInfos[1]));
            return newTeam.RealTeam.Value ? newTeam : GetTeam(newTeam);
        }

        protected abstract void Simulate(Round round, Tuple<Team, Team> game);
    }
}
