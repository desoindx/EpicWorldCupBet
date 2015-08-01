using System;
using System.Collections.Generic;
using Datas.Entities;

namespace Pricer
{
    public class QuoteCompetition : BasicCompetition
    {
        public QuoteCompetition(string name)
            : base(name)
        {
        }

        protected override void Simulate(Round round, Tuple<Team, Team> game)
        {
        }
    }
}
