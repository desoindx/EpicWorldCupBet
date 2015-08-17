using System;
using System.Collections.Generic;
using System.Linq;
using Datas.Entities;

namespace Pricer
{
    public static class PricerHelper
    {
        private static readonly Dictionary<string, BasicCompetition> Competitions = new Dictionary<string, BasicCompetition>();

        public static Dictionary<Team, double> Price(string competitionName, Dictionary<Team, double> strengths)
        {
            var competition = GetCompetion(competitionName);
            ((StrengthCompetition)competition).Strengths = strengths;
            return competition.Price(true);
        }

        private static BasicCompetition GetCompetion(string competitionName)
        {
            BasicCompetition competition;
            if (!Competitions.TryGetValue(competitionName, out competition))
            {
                competition = new StrengthCompetition(competitionName);
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
                competition.Price(true);
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
                competition.Price(true);
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
            var average = orderedResults[(int)(vars[2]* count)].Item2;
            var best10 = orderedResults[(int)(vars[3] * count)].Item2;
            var best = orderedResults[(int)(vars[4] * count)].Item2;
            
            var teamResults = new List<TeamResult>();
            var total = new TeamResult {Team = "Total", Position = 0};
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
    }
}
