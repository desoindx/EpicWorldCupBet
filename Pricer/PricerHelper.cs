using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using Datas.Entities;

namespace Pricer
{
    public static class PricerHelper
    {
        private static readonly Dictionary<string, BasicCompetition> Competitions = new Dictionary<string, BasicCompetition>();
        private const bool CacheFile = true;

        public static void Clear()
        {
            Competitions.Clear();
            if (CacheFile && HttpContext.Current != null)
            {
                var pricingResult = HttpContext.Current.Server.MapPath("~/Pricer/");
                foreach (var file in Directory.GetFiles(pricingResult))
                {
                    File.Delete(file);
                }
            }
        }

        public static Dictionary<Team, double> Price(string competitionName, Dictionary<Team, double> strengths)
        {
            var competition = GetCompetion(competitionName);
            ((StrengthCompetition)competition).Strengths = strengths;
            return competition.Price();
        }

        private static BasicCompetition GetCompetion(string competitionName)
        {
            BasicCompetition competition = null;
            if (!Competitions.TryGetValue(competitionName, out competition))
            {
                competition = new StrengthCompetition(competitionName);
                if (CacheFile && HttpContext.Current != null)
                {
                    var pricingResult = HttpContext.Current.Server.MapPath("~/Pricer/");
                    if (File.Exists(pricingResult + competitionName + ".xml"))
                    {
                        try
                        {
                            XmlSerializer xs = new XmlSerializer(typeof(PricerResult));
                            using (StreamReader rd = new StreamReader(pricingResult + competitionName + ".xml"))
                            {
                                var pricerResult = (PricerResult)(xs.Deserialize(rd));
                                var prizes = pricerResult.Prizes.Split('-').Select(Convert.ToDouble).ToList();
                                competition.Simulation = pricerResult.Scenarios.ToDictionary(x =>
                                {
                                    var results = new Dictionary<Team, double>();
                                    var teamsId = x.Teams.Split('-');
                                    for (int i = 0; i < teamsId.Length; i++)
                                    {
                                        var teamId = teamsId[i];
                                        results[new Team { Id = Convert.ToInt32(teamId) }] = prizes[i];
                                    }
                                    var simulationResult = new SimulationResult(results);
                                    return simulationResult;
                                }, x => x.Value);
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                Competitions[competitionName] = competition;
            }
            return competition;
        }

        public static double GetWorstScenario(string competitionName, Dictionary<Team, int> positions, double var)
        {
            var competition = GetCompetion(competitionName);
            if (competition.Simulation == null)
            {
                ((StrengthCompetition)competition).SetStrengthsToDefaultValues();
                competition.Price();
            }

            var results = new List<Tuple<double, SimulationResult>>();
            foreach (var simulation in competition.Simulation)
            {
                double result = simulation.Key.GetResult(positions);
                for (int i = 0; i < simulation.Value; i++)
                {
                    results.Add(new Tuple<double, SimulationResult>(result, simulation.Key));
                }
            }

            var orderedResults = results.OrderBy(x => x.Item1).ToList();
            var count = orderedResults.Count - 1;
            return orderedResults[(int)(var * count)].Item1;
        }

        public static List<TeamResult> GetVars(string competitionName, Dictionary<Team, int> positions, List<double> vars)
        {
            var competition = GetCompetion(competitionName);
            if (competition.Simulation == null)
            {
                ((StrengthCompetition)competition).SetStrengthsToDefaultValues();
                competition.Price();
            }

            var results = new List<Tuple<double, SimulationResult>>();
            foreach (var simulation in competition.Simulation)
            {
                double result = simulation.Key.GetResult(positions);
                for (int i = 0; i < simulation.Value; i++)
                {
                    results.Add(new Tuple<double, SimulationResult>(result, simulation.Key));
                }
            }

            var orderedResults = results.OrderBy(x => x.Item1).ToList();
            var count = orderedResults.Count - 1;
            var worst = orderedResults[(int)(vars[0] * count)].Item2;
            var worst10 = orderedResults[(int)(vars[1] * count)].Item2;
            var average = orderedResults[(int)(vars[2] * count)].Item2;
            var best10 = orderedResults[(int)(vars[3] * count)].Item2;
            var best = orderedResults[(int)(vars[4] * count)].Item2;

            var teamResults = new List<TeamResult>();
            var total = new TeamResult { Team = "Total", Position = 0 };
            foreach (var position in positions)
            {
                var team = position.Key;
                var quantity = position.Value;
                var worstValue = worst.GetResult(team) * quantity;
                var worst10Value = worst10.GetResult(team) * quantity;
                var averageValue = average.GetResult(team) * quantity;
                var best10Value = best10.GetResult(team) * quantity;
                var bestValue = best.GetResult(team) * quantity;
                teamResults.Add(new TeamResult
                {
                    Team = team.Name,
                    Position = quantity,
                    Worst = worstValue,
                    Worst10 = worst10Value,
                    Average = averageValue,
                    Best10 = best10Value,
                    Best = bestValue
                });
                total.Worst += worstValue;
                total.Worst10 += worst10Value;
                total.Average += averageValue;
                total.Best10 += best10Value;
                total.Best += bestValue;
            }

            teamResults.Add(total);
            return teamResults;
        }

        public static Dictionary<Team, double> Price(this BasicCompetition competition)
        {
            var result = competition.Price(true);
            if (!CacheFile)
            {
                return result;
            }

            string pricingResult;
            if (HttpContext.Current != null)
            {
                pricingResult = HttpContext.Current.Server.MapPath("~/Pricer/");
            }
            else
            {
                pricingResult = "C:/Users/xavier/Documents/Pricer/";
            }
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(PricerResult));
                using (StreamWriter wr = new StreamWriter(pricingResult + competition.Name + ".xml"))
                {
                    var prizes = competition.Prizes;
                    var pricerResult = new PricerResult
                    {
                        Prizes = prizes.Skip(1).Aggregate(prizes.First().ToString(), (current, prize) => current + ("-" + prize)),
                        Scenarios = competition.Simulation.Select(
                            x =>
                            {
                                var allTeams = x.Key.Result.OrderByDescending(t => t.Value);
                                return new Scenario
                                {
                                    Teams =
                                        allTeams.Skip(1)
                                            .Aggregate(allTeams.First().Key.Id.ToString(),
                                                (current, t) => current + ("-" + t.Key.Id)),
                                    Value = x.Value
                                };
                            }).ToArray()
                    };
                    xs.Serialize(wr, pricerResult);
                }
            }
            catch (Exception)
            {
            }

            return result;
        }

        public class PricerResult
        {
            public string Prizes;
            public Scenario[] Scenarios;
        }

        public class Scenario
        {
            public string Teams;
            public int Value;
        }
    }
}
