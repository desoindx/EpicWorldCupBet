using System;
using System.Collections.Generic;
using System.Linq;
using Datas.Entities;
using DocumentFormat.OpenXml.Wordprocessing;
using Pricer.Rounds;

namespace Pricer
{
    public abstract class BasicCompetition
    {
        private const int SimulationNumber = 1000000;
        private readonly string _name;

        protected readonly Dictionary<int, Team> Teams;

        private readonly Competition _competition;
        private Dictionary<string, Round> _rounds;
        private readonly Dictionary<string, List<CompetitionGame>> _games;
        private readonly Dictionary<string, List<CompetitionResult>> _results;
        private readonly List<CompetitionPrize> _prizes;

        public HashSet<SimulationResult> Simulation { get; set; }

        public List<int> Prizes { get { return _prizes.Select(x => x.Value).OrderByDescending(x => x).ToList(); } }

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

        public Dictionary<int, double> Price(bool updateSimulation)
        {
            _rounds = new Dictionary<string, Round>();
            var results = new Dictionary<int, double>();
            foreach (var team in Teams)
            {
                results[team.Value.Id] = 0;
            }

            var simulations = new HashSet<SimulationResult>();
            var existingSimulations = new Dictionary<SimulationResult, ushort>();
            for (int i = 0; i < SimulationNumber; i++)
            {
                var result = SimulateCompetition();
                if (updateSimulation)
                {
                    if (simulations.Contains(result))
                    {
                        simulations.Remove(result);
                        ushort existingSimulation;
                        if (!existingSimulations.TryGetValue(result, out existingSimulation))
                        {
                            existingSimulation = 1;
                        }
                        var simulation = (ushort) (existingSimulation + 1);
                        existingSimulations[result] = simulation;
                        result.Value = simulation;
                    }

                    simulations.Add(result);
                }
                foreach (var r in result.Result)
                {
                    results[r.Key] += r.Value;
                }
            }

            simulations.TrimExcess();
            for (int index = 0; index < results.Count; index++)
            {
                var item = results.ElementAt(index);
                results[item.Key] = item.Value / SimulationNumber;
            }

            if (updateSimulation)
            {
                Simulation = simulations;
            }

            return results;
        }

        private SimulationResult SimulateCompetition()
        {
            _rounds.Clear();
            _bestThirds = null;
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

            var results = new Dictionary<int, int>(_prizes.Count);

            foreach (var prize in _prizes)
            {
                Team team;
                if (!prize.RoundKey.StartsWith("*"))
                {
                    var teamName = prize.RoundKey.Trim();
                    team = Teams.Values.First(x => x.Name == teamName);
                }
                else
                {
                    team = GetTeam(null, prize.RoundKey);
                }

                results[team.Id] = prize.Value;
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

            if (teamName.StartsWith("*BestThird"))
            {
                var index = Convert.ToInt32(teamName.Remove(teamName.Length - 1).Remove(0, 11)) - 1;
                GenerateBestThirdRound(currentRound);
                return _bestThirds[index];
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

        private List<Team> _bestThirds;
        private void GenerateBestThirdRound(Round round)
        {
            if (_bestThirds != null)
            {
                return;
            }

            var thirds = new List<Tuple<int, RoundResult>>();
            var thirdsTeams = new List<Team>();
            for (int i = 1; i < 7; i++)
            {
                var team = GetTeam(round, string.Format("*{0}-3", i));
                thirdsTeams.Add(team);
                thirds.Add(new Tuple<int, RoundResult>(i, _rounds[i.ToString()].GetTeamResult(team)));
            }

            var allThirds = thirds.OrderByDescending(x => x.Item2.Point)
                .ThenByDescending(x => x.Item2.ScoreFor - x.Item2.ScoreAgainst)
                .ThenByDescending(x => x.Item2.ScoreFor)
                .Select(x => x.Item1).ToList();
            var bestThirds = allThirds.Take(4).ToList();

            _bestThirds = new List<Team>();
            if (bestThirds.Contains(1))
            {
                if (bestThirds.Contains(2))
                {
                    if (bestThirds.Contains(3))
                    {
                        if (bestThirds.Contains(4))
                        {
                            // A B C D
                            _bestThirds.Add(thirdsTeams[2]);
                            _bestThirds.Add(thirdsTeams[3]);
                            _bestThirds.Add(thirdsTeams[0]);
                            _bestThirds.Add(thirdsTeams[1]);
                        }
                        else if (bestThirds.Contains(5))
                        {
                            // A B C E
                            _bestThirds.Add(thirdsTeams[2]);
                            _bestThirds.Add(thirdsTeams[0]);
                            _bestThirds.Add(thirdsTeams[1]);
                            _bestThirds.Add(thirdsTeams[4]);
                        }
                        else
                        {
                            // A B C F
                            _bestThirds.Add(thirdsTeams[2]);
                            _bestThirds.Add(thirdsTeams[0]);
                            _bestThirds.Add(thirdsTeams[1]);
                            _bestThirds.Add(thirdsTeams[5]);
                        }
                    }
                    else
                    {
                        if (bestThirds.Contains(4))
                        {
                            if (bestThirds.Contains(5))
                            {
                                // A B D E
                                _bestThirds.Add(thirdsTeams[3]);
                                _bestThirds.Add(thirdsTeams[0]);
                                _bestThirds.Add(thirdsTeams[1]);
                                _bestThirds.Add(thirdsTeams[4]);
                            }
                            else
                            {
                                // A B D F
                                _bestThirds.Add(thirdsTeams[3]);
                                _bestThirds.Add(thirdsTeams[0]);
                                _bestThirds.Add(thirdsTeams[1]);
                                _bestThirds.Add(thirdsTeams[5]);
                            }
                        }
                        else
                        {
                            // A B E F
                            _bestThirds.Add(thirdsTeams[4]);
                            _bestThirds.Add(thirdsTeams[0]);
                            _bestThirds.Add(thirdsTeams[1]);
                            _bestThirds.Add(thirdsTeams[5]);
                        }
                    }
                }
                else if (bestThirds.Contains(3))
                {
                    if (bestThirds.Contains(4))
                    {
                        if (bestThirds.Contains(5))
                        {
                            // A C D E
                            _bestThirds.Add(thirdsTeams[2]);
                            _bestThirds.Add(thirdsTeams[3]);
                            _bestThirds.Add(thirdsTeams[0]);
                            _bestThirds.Add(thirdsTeams[4]);
                        }
                        else
                        {
                            // A C D F
                            _bestThirds.Add(thirdsTeams[2]);
                            _bestThirds.Add(thirdsTeams[3]);
                            _bestThirds.Add(thirdsTeams[0]);
                            _bestThirds.Add(thirdsTeams[5]);
                        }
                    }
                    else
                    {
                        // A C E F
                        _bestThirds.Add(thirdsTeams[2]);
                        _bestThirds.Add(thirdsTeams[0]);
                        _bestThirds.Add(thirdsTeams[5]);
                        _bestThirds.Add(thirdsTeams[4]);
                    }
                }
                else
                {
                    // A D E F
                    _bestThirds.Add(thirdsTeams[3]);
                    _bestThirds.Add(thirdsTeams[0]);
                    _bestThirds.Add(thirdsTeams[5]);
                    _bestThirds.Add(thirdsTeams[4]);
                }
            }
            else if (bestThirds.Contains(2))
            {
                if (bestThirds.Contains(3))
                {
                    if (bestThirds.Contains(4))
                    {
                        if (bestThirds.Contains(5))
                        {
                            // B C D E
                            _bestThirds.Add(thirdsTeams[2]);
                            _bestThirds.Add(thirdsTeams[3]);
                            _bestThirds.Add(thirdsTeams[1]);
                            _bestThirds.Add(thirdsTeams[4]);
                        }
                        else
                        {
                            // B C D F
                            _bestThirds.Add(thirdsTeams[2]);
                            _bestThirds.Add(thirdsTeams[3]);
                            _bestThirds.Add(thirdsTeams[1]);
                            _bestThirds.Add(thirdsTeams[5]);
                        }
                    }
                    else
                    {
                        // B C E F
                        _bestThirds.Add(thirdsTeams[4]);
                        _bestThirds.Add(thirdsTeams[2]);
                        _bestThirds.Add(thirdsTeams[1]);
                        _bestThirds.Add(thirdsTeams[5]);
                    }
                }
                else
                {
                    // B D E F
                    _bestThirds.Add(thirdsTeams[4]);
                    _bestThirds.Add(thirdsTeams[3]);
                    _bestThirds.Add(thirdsTeams[1]);
                    _bestThirds.Add(thirdsTeams[5]);
                }
            }
            else
            {
                // C D E F
                _bestThirds.Add(thirdsTeams[2]);
                _bestThirds.Add(thirdsTeams[3]);
                _bestThirds.Add(thirdsTeams[5]);
                _bestThirds.Add(thirdsTeams[4]);
            }
            _bestThirds.Add(thirdsTeams[allThirds[4] - 1]);
            _bestThirds.Add(thirdsTeams[allThirds[5] - 1]);
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
