using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using Datas.Entities;

namespace Pricer
{
    public abstract class BasicCompetition
    {
        private const int SimulationNumber = 200000;

        private readonly Random _teamSelector = new Random();
        private string _name;

        protected readonly Dictionary<int, Team> Teams;

        private readonly Competition _competition;
        private Dictionary<string, Round> _rounds;
        private readonly Dictionary<string, List<CompetitionGame>> _games;
        private readonly Dictionary<string, List<CompetitionResult>> _results;
        private readonly List<CompetitionPrize> _prizes;

        public Dictionary<SimulationResult, int> Simulation { get; set; }
        public string Name { get { return _name; } }

        public BasicCompetition(string name)
        {
            _name = name;
            using (var context = new Entities())
            {
                _competition = context.Competitions.First(x => x.Name == name);
                Teams = context.Teams.Where(x => x.IdCompetition == _competition.Id).ToDictionary(x => x.Id);
                _games = context.CompetitionGames.Where(x => x.CompetitionId == _competition.Id).ToList().GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.ToList());
                _results = context.CompetitionResults.Where(x => x.CompetitionId == _competition.Id).GroupBy(x => x.RoundKey).ToDictionary(x => x.Key, x => x.ToList());
                _prizes = context.CompetitionPrizes.Where(x => x.CompetitionId == _competition.Id).ToList();
            }
        }

        public Dictionary<Team, double> Price(bool updateSimulation)
        {
            _rounds = new Dictionary<string, Round>();
            var results = new Dictionary<Team, double>();
            foreach (var team in Teams)
            {
                results[team.Value] = 0;
            }

            var simulation = new Dictionary<SimulationResult, int>();
            for (int i = 0; i < SimulationNumber; i++)
            {
                var result = SimulateCompetition();
                if (updateSimulation)
                {
                    if (simulation.ContainsKey(result))
                    {
                        simulation[result] += 1;
                    }
                    else
                    {
                        simulation[result] = 1;
                    }
                }
                foreach (var r in result.Result)
                {
                    results[r.Key] += r.Value;
                }
            }

            for (int index = 0; index < results.Count; index++)
            {
                var item = results.ElementAt(index);
                results[item.Key] = item.Value / SimulationNumber;
            }

            if (updateSimulation)
            {
                Simulation = simulation;
            }

            return results;
        }

        private SimulationResult SimulateCompetition()
        {
            _rounds.Clear();
            foreach (var game in _games)
            {
                if (_rounds.ContainsKey(game.Key))
                {
                    continue;
                }

                var round = RoundFactory.GetRound("", game.Key, _competition, _results.ContainsKey(game.Key) ? _results[game.Key] : new List<CompetitionResult>(), game.Value);
                _rounds.Add(game.Key, round);
                SimulateRound(round);
            }

            var results = new Dictionary<Team, double>();

            foreach (var prize in _prizes)
            {
                Team team;
                if (!prize.RoundKey.StartsWith("*"))
                {
                    team = Teams.Values.First(x => x.Name == prize.RoundKey.Trim());
                }
                else
                {
                    var prizeInfo = prize.RoundKey.Remove(0, 1).Split('-');
                    var key = prizeInfo[0];
                    var round = _rounds[key];
                    team = round.GetTeam(int.Parse(prizeInfo[1]));
                }

                results[team] = prize.Value;
            }

            return new SimulationResult(results);
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
            for (int i = 0; i < round.Teams.Count; i++)
            {
                var team = round.Teams[i];
                var newTeam = GetTeam(round, team);
                if (newTeam == null)
                {
                    return;
                }
                round.Teams[i] = newTeam;
            }

        }

        private Team GetTeam(Round currentRound, Team team)
        {
            if (team.RealTeam.Value)
            {
                return team;
            }

            return GetTeam(currentRound, team.Name);
        }

        private Team GetTeam(Round currentRound, string teamName)
        {
            if (teamName.StartsWith("*Random"))
            {
                GenerateRandomRound(currentRound, teamName.Remove(teamName.Length - 1).Remove(0, 8));
                return null;
            }

            var teamInfos = teamName.Remove(0, 1).Split('-');
            Round targetRound;
            var key = teamInfos[0];
            if (!_rounds.TryGetValue(key, out targetRound))
            {
                targetRound = RoundFactory.GetRound("", key, _competition,
                    _results.ContainsKey(key) ? _results[key] : new List<CompetitionResult>(), _games[key]);
                _rounds[key] = targetRound;
                SimulateRound(targetRound);
            }

            var newTeam = targetRound.GetTeam(int.Parse(teamInfos[1]));
            return newTeam.RealTeam.Value ? newTeam : GetTeam(targetRound, newTeam);
        }

        private void GenerateRandomRound(Round round, string parameters)
        {
            var param = parameters.Split(',');
            var roundToGenerate = int.Parse(param[0]);
            var teamPerRound = int.Parse(param[1]);
            var originalGroups = param[2].Split('-');
            var canBeFromSameGroup = bool.Parse(param[3]);
            var canBeFromSameCountry = bool.Parse(param[4]);

            var selectionPerGroup = (roundToGenerate * teamPerRound) / originalGroups.Count();
            var teams = new List<List<Team>>();
            for (int i = 1; i <= selectionPerGroup; i++)
            {
                var hat = originalGroups.Select(originalGroup => GetTeam(round, string.Format("*{0}-{1}", originalGroup, i))).ToList();
                teams.Add(hat);
            }

            var permutations = new List<List<int>>();
            var initialList = Enumerable.Range(0, originalGroups.Count()).ToList();
            initialList.Shuffle();
            permutations.Add(initialList);
            for (int i = 1; i < selectionPerGroup; i++)
            {
                permutations.Add(initialList.Permute(canBeFromSameGroup));
            }

            int count = 0;
            int originalRoundKey = int.Parse(round.RoundKey);

            var originalTeams = round.Teams;
            originalTeams.Clear();
            while (originalTeams.Count < teamPerRound)
            {
                for (int i = 0; i < selectionPerGroup; i++)
                {
                    originalTeams.Add(teams[i][permutations[i][count]]);
                }
                count++;
            }

            for (int i = 1; i < roundToGenerate; i++)
            {
                originalRoundKey++;
                originalTeams = new List<Team>();
                while (originalTeams.Count < teamPerRound)
                {
                    for (int j = 0; j < selectionPerGroup; j++)
                    {
                        originalTeams.Add(teams[j][permutations[j][count]]);
                    }
                    count++;
                }

                var newRound = RoundFactory.GetRound("", originalRoundKey.ToString(), round.HomeAndAway, _competition,
                    new List<CompetitionResult>(), originalTeams);
                _rounds[originalRoundKey.ToString()] = newRound;
                SimulateRound(newRound);
            }
        }

        protected abstract void Simulate(Round round, Tuple<Team, Team> game);
    }
}
