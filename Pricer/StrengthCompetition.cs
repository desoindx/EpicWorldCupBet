using System;
using System.Collections.Generic;
using Datas.Entities;
using System.IO;
using System.Web;

namespace Pricer
{
    public class StrengthCompetition : BasicCompetition
    {
        public Dictionary<string, Tuple<double, double, double>> Odds;
        public Dictionary<Team, double> Strengths { get; set; }
        private readonly Random _random;

        public StrengthCompetition(string name)
            : base(name)
        {
            _random = new Random();
            Odds = new Dictionary<string, Tuple<double, double, double>>();
            LoadOdds();
        }

        private void LoadOdds()
        {
            string file = @"C:\Temp\auto_odds_worldcup2018.csv";
            if (HttpContext.Current != null)
            {
                var pricingResult = HttpContext.Current.Server.MapPath("~/Pricer/");
                file = pricingResult + "csv\\auto_odds_worldcup2018.csv";
            }

            using (var reader = new StreamReader(file))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');
                    Odds[values[0] + "-" + values[1]] 
                        = new Tuple<double, double, double>(Double.Parse(values[2].Replace('.',',')), Double.Parse(values[3].Replace('.', ',')), Double.Parse(values[4].Replace('.', ',')));
                }
            }
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
            var key = game.Item1.Name+"-"+ game.Item2.Name;
            var key2 = game.Item2.Name + "-" + game.Item1.Name;
            Tuple<double, double, double> odds;
            if (Odds.TryGetValue(key, out odds)){
                round.AddResult(game, _random.NextDouble(), odds.Item1, odds.Item2, odds.Item3);
            } else if (Odds.TryGetValue(key2, out odds))
            {
                round.AddResult(game, _random.NextDouble(), odds.Item3, odds.Item2, odds.Item1);
            }
            else
            {
                var strength1 = Strengths[game.Item1];
                var strength2 = Strengths[game.Item2];

                round.AddResult(game, _random.NextDouble(), strength1, strength2);
            }
        }
    }
}
