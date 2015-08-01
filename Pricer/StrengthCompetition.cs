using System;
using System.Collections.Generic;
using Datas;
using Datas.Entities;

namespace Pricer
{
    public class StrengthCompetition : BasicCompetition
    {
        private Dictionary<Team, double> _strengths;
        private Random _random;

        public StrengthCompetition(string name, Dictionary<Team, double> strengths)
            : base(name)
        {
            _strengths = strengths;
            _random = new Random();
        }

        protected override void Simulate(Round round, Tuple<Team, Team> game)
        {
            var strength1 = _strengths[game.Item1];
            var strength2 = _strengths[game.Item2];

            round.AddResult(game, _random.NextDouble(), strength1, strength2);
        }
    }
}
