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
            return positions.Select(position => new TeamResult
            {
                Team = position.Key.Name,
                Position = position.Value,
                Worst = worst.GetResult(position.Key) * position.Value,
                Worst10 = worst10.GetResult(position.Key) * position.Value,
                Average = average.GetResult(position.Key) * position.Value,
                Best10 = best10.GetResult(position.Key) * position.Value,
                Best = best.GetResult(position.Key) * position.Value
            }).ToList();
        }
    }
}
