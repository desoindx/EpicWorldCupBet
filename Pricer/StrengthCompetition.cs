using System;
using System.Collections.Generic;
using Datas.Entities;

namespace Pricer
{
    public class StrengthCompetition : BasicCompetition
    {
        public Dictionary<Team, double> Strengths { get; set; }
        private readonly Random _random;

        public StrengthCompetition(string name)
            : base(name)
        {
            _random = new Random();
        }

        public void SetStrengthsToDefaultValues()
        {
            Strengths = new Dictionary<Team, double>();
            foreach (var team in Teams.Values)
            {
                Strengths[team] = team.Strength ?? 1;
            }
        }

        protected override void Simulate(Round round, Tuple<Team, Team> game)
        {
            var strength1 = Strengths[game.Item1];
            var strength2 = Strengths[game.Item2];

            round.AddResult(game, _random.NextDouble(), strength1, strength2);
        }
    }
}
