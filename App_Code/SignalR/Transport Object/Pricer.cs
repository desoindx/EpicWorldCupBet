﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SignalR
{
    /// <summary>
    /// Summary description for Pricer
    /// </summary>
    public class Pricer
    {
        private const int mc = 1000000;
        private Random r = new Random();
        private List<string> Teams = new List<string> {"Brazil",
            "Croatia",
            "Mexico",
            "Cameroon",
            "Australia",
            "Chile",
            "Netherlands",
            "Spain",
            "Colombia",
            "Greece",
            "Ivory Coast",
            "Japan",
            "Costa Rica",
            "England",
            "Italy",
            "Uruguay",
            "Ecuador",
            "France",
            "Honduras",
            "Switzerland",
            "Argentina",
            "Bosnia And Herzgovina",
            "Iran",
            "Nigeria",
            "Germany",
            "Ghana",
            "Portugal",
            "United States",
            "Algeria",
            "Belgium",
            "Russia",
            "South Korea"
        };

        public Dictionary<string, double> TeamsValue = new Dictionary<string, double> { {"Brazil", 100},
            {"Croatia",51},
            {"Mexico", 21.9},
            {"Cameroon",15},
            {"Australia",15},
            {"Chile",23.5},
            {"Netherlands",42.5},
            {"Spain",137},
            {"Colombia",35.1},
            {"Greece",26},
            {"Ivory Coast",42},
            {"Japan",50},
            {"Costa Rica",12.2},
            {"England",97},
            {"Italy",102},
            {"Uruguay",26.6},
            {"Ecuador",42},
            {"France",33},
            {"Honduras",1},
            {"Switzerland",16.6},
            {"Argentina",58.7},
            {"Bosnia And Herzgovina",42},
            {"Iran",3},
            {"Nigeria",16.3},
            {"Germany",61.3},
            {"Ghana",52},
            {"Portugal",115},
            {"United States",17.5},
            {"Algeria",9.2},
            {"Belgium",46.1},
            {"Russia",59},
            {"South Korea",18}
        };

        public Dictionary<string, double> Price(List<int> values)
        {
            var price = new Dictionary<string, double>();
            int i = 0;
            foreach (var team in Teams)
            {
                TeamsValue[team] = values[i];
                price.Add(team, 0);
                i++;
            }
            return SimulateWC(price);
        }

        private Dictionary<string, double> SimulateWC(Dictionary<string, double> price)
        {
            for (int i = 0; i < mc; i++)
            {
                var huitieme = Poule();
                if (_count != _playedGame.Count)
                    return null;
                _count = 0;
                huitieme["0-1"] = "Brazil";
                huitieme["0-2"] = "Mexico";
                huitieme["1-1"] = "Netherlands";
                huitieme["1-2"] = "Brazil";
                huitieme["2-1"] = "Colombia";
                huitieme["2-2"] = "Greece";
                huitieme["3-1"] = "Costa Rica";
                huitieme["3-2"] = "Colombia";
                huitieme["4-1"] = "France";
                huitieme["4-2"] = "Switzerland";
                huitieme["5-1"] = "Argentina";
                huitieme["5-2"] = "Nigeria";
                huitieme["6-1"] = "Germany";
                huitieme["6-2"] = "United States";
                huitieme["7-1"] = "Belgium";
                huitieme["7-2"] = "Algeria";
                var quart = Huitiemes(huitieme, price);
                var demies = Quarts(quart, price);
                var winner1 = PlayGame(demies[57], demies[58]);
                var winner2 = PlayGame(demies[59], demies[60]);
                string finalist1;
                string finalist2;
                string looser1;
                string looser2;

                if (winner1 == 1)
                {
                    looser1 = demies[58];
                    finalist1 = demies[57];
                }
                else
                {
                    looser1 = demies[57];
                    finalist1 = demies[58];
                }

                if (winner2 == 1)
                {
                    looser2 = demies[60];
                    finalist2 = demies[59];
                }
                else
                {
                    looser2 = demies[59];
                    finalist2 = demies[60];
                }

                var thirdWinner = PlayGame(looser1, looser2);
                if (thirdWinner == 1)
                {
                    price[looser1] += 50;
                    price[looser2] += 45;
                }
                else
                {
                    price[looser2] += 50;
                    price[looser1] += 45;
                }

                var topWinner = PlayGame(finalist1, finalist1);
                if (topWinner == 1)
                {
                    price[finalist1] += 100;
                    price[finalist2] += 75;
                }
                else
                {
                    price[finalist2] += 100;
                    price[finalist1] += 75;
                }
            }

            var results = new Dictionary<string, double>();
            foreach (var p in price)
            {
                results.Add(p.Key, p.Value / mc);
            }
            return results;
        }

        private void PlayQuartGame(int team1, int team2, int matchNumber, Dictionary<int, string> games, Dictionary<string, double> price, Dictionary<int, string> demies)
        {
            var winner = PlayGame(games[team1], games[team2]);
            if (winner == 1)
            {
                demies.Add(matchNumber, games[team1]);
                price[games[team2]] += 25;
            }
            else
            {
                demies.Add(matchNumber, games[team2]);
                price[games[team1]] += 25;
            }
        }

        private Dictionary<int, string> Quarts(Dictionary<int, string> games, Dictionary<string, double> price)
        {
            var demies = new Dictionary<int, string>();
            PlayQuartGame(49, 50, 57, games, price, demies);
            PlayQuartGame(51, 52, 58, games, price, demies);
            PlayQuartGame(53, 54, 59, games, price, demies);
            PlayQuartGame(55, 56, 60, games, price, demies);

            return demies;
        }

        private Dictionary<int, string> Huitiemes(Dictionary<string, string> games, Dictionary<string, double> price)
        {
            var quarts = new Dictionary<int, string>();
            PlayHuitiemGame("0-1", "1-2", 49, games, price, quarts);
            PlayHuitiemGame("2-1", "3-2", 50, games, price, quarts);
            PlayHuitiemGame("4-1", "5-2", 51, games, price, quarts);
            PlayHuitiemGame("6-1", "7-2", 52, games, price, quarts);
            PlayHuitiemGame("1-1", "0-2", 53, games, price, quarts);
            PlayHuitiemGame("3-1", "2-2", 54, games, price, quarts);
            PlayHuitiemGame("5-1", "4-2", 55, games, price, quarts);
            PlayHuitiemGame("7-1", "6-2", 56, games, price, quarts);

            return quarts;
        }

        private void PlayHuitiemGame(string team1, string team2, int matchNumber, Dictionary<string, string> games, Dictionary<string, double> price, Dictionary<int, string> quarts)
        {

            var winner = PlayGame(games[team1], games[team2]);
            if (winner == 1)
            {
                quarts.Add(matchNumber, games[team1]);
                price[games[team2]] += 10;
            }
            else
            {
                quarts.Add(matchNumber, games[team2]);
                price[games[team1]] += 10;
            }
        }

        private Dictionary<string, string> Poule()
        {
            var finals = new Dictionary<string, string>();
            for (int i = 0; i < 8; i++)
            {
                var rank = new Dictionary<string, int>();
                rank.Add(Teams[i * 4 + 0], 0);
                rank.Add(Teams[i * 4 + 1], 0);
                rank.Add(Teams[i * 4 + 2], 0);
                rank.Add(Teams[i * 4 + 3], 0);
                PlayPouleGame(Teams[i * 4 + 0], Teams[i * 4 + 1], rank);
                PlayPouleGame(Teams[i * 4 + 0], Teams[i * 4 + 2], rank);
                PlayPouleGame(Teams[i * 4 + 0], Teams[i * 4 + 3], rank);
                PlayPouleGame(Teams[i * 4 + 1], Teams[i * 4 + 2], rank);
                PlayPouleGame(Teams[i * 4 + 1], Teams[i * 4 + 3], rank);
                PlayPouleGame(Teams[i * 4 + 2], Teams[i * 4 + 3], rank);

                var finalRanking = rank.OrderByDescending(x => x.Value).ThenByDescending(x => r.NextDouble() * TeamsValue[x.Key]).Select(x => x.Key).ToList();
                finals.Add(i + "-1", finalRanking[0]);
                finals.Add(i + "-2", finalRanking[1]);

            }
            return finals;
        }

        private int PlayGame(string team1, string team2)
        {
            double team1Value = TeamsValue[team1];
            double team2Value = TeamsValue[team2];
            var result = r.NextDouble() * (team1Value + team2Value);
            return result < team1Value ? 1 : 2;
        }

        private int _count;
        private void PlayPouleGame(string team1, string team2, Dictionary<string, int> rank)
        {
            if (AlreadyPlayedGame(team1, team2, rank))
            {
                _count++;
                return;
            }

            double team1Value = TeamsValue[team1];
            double team2Value = TeamsValue[team2];
            double draw = (team1Value + team2Value) / 5;

            var result = r.NextDouble() * (team1Value + team2Value + draw);
            if (result < draw)
            {
                rank[team1] += 1;
                rank[team2] += 1;
            }
            else if (result < draw + team1Value)
            {
                rank[team1] += 3;
            }
            else
            {
                rank[team2] += 3;
            }
        }

        private readonly Dictionary<string, int> _playedGame = new Dictionary<string, int> { 
            { "Brazil-Croatia", 1 },
            { "Mexico-Cameroon", 1 },
            { "Netherlands-Spain", 1 },
            { "Colombia-Greece", 1 },
            { "Ivory Coast-Japan", 1 },
            { "Uruguay-Costa Rica", 3 },
            { "England-Italy", 3 },
            { "Switzerland-Ecuador", 1 },
            { "France-Honduras", 1 },
            { "Argentina-Bosnia And Herzgovina", 1 },
            { "Germany-Portugal", 1 },
            { "Ghana-United States", 3 },
            { "Iran-Nigeria", 2 },
            { "Belgium-Algeria", 1 },
            { "Brazil-Mexico", 2 },
            { "Russia-South Korea", 2 },
            { "Australia-Netherlands", 3 },
            { "Spain-Chile", 3 },
            { "Colombia-Ivory Coast", 1 },
            { "Cameroon-Croatia", 3 },
            { "Uruguay-England", 1 },
            { "Japan-Greece", 2 },
            { "Italy-Costa Rica", 3 },
            { "Switzerland-France", 3 },
            { "Honduras-Ecuador", 3 },
            { "Argentina-Iran", 1 },
            { "Germany-Ghana", 2 },
            { "Portugal-United States", 2 },
            { "Nigeria-Bosnia And Herzgovina", 1 },
            { "Belgium-Russia", 1 },
            { "South Korea-Algeria", 3 },
            { "Netherlands-Chile", 1 },
            { "Australia-Spain", 3 },
            { "Brazil-Cameroon", 1 },
            { "Mexico-Croatia", 1 },
            { "Chile-Australia", 1 } };

        private bool AlreadyPlayedGame(string team1, string team2, Dictionary<string, int> rank)
        {
            int result;
            if (_playedGame.TryGetValue(team1 + "-" + team2, out result))
            {
                switch (result)
                {
                    case 1:
                        rank[team1] += 3;
                        break;
                    case 2:
                        rank[team1] += 1;
                        rank[team2] += 1;
                        break;
                    case 3:
                        rank[team2] += 3;
                        break;
                }
                return true;
            }
            if (_playedGame.TryGetValue(team2 + "-" + team1, out result))
            {
                switch (result)
                {
                    case 1:
                        rank[team2] += 3;
                        break;
                    case 2:
                        rank[team1] += 1;
                        rank[team2] += 1;
                        break;
                    case 3:
                        rank[team1] += 3;
                        break;
                }
                return true;
            }

            return false;
        }

        internal Dictionary<string, double> Price()
        {
            var price = new Dictionary<string, double>();
            int i = 0;
            foreach (var team in Teams)
            {
                price.Add(team, 0);
                i++;
            }

            return SimulateWC(price);
        }
    }
}