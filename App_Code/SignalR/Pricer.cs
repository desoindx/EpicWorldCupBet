using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Pricer
/// </summary>
public class Pricer
{
    private const int mc = 100000;
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

    public Dictionary<string, int> TeamsValue = new Dictionary<string, int> { {"Brazil", 100},
         {"Croatia",20},
         {"Mexico", 20},
         {"Cameroon",18},
         {"Australia",5},
         {"Chile",10},
         {"Netherlands",40},
         {"Spain",70},
         {"Colombia",25},
         {"Greece",28},
         {"Ivory Coast",19},
         {"Japan",10},
         {"Costa Rica",10},
         {"England",30},
         {"Italy",40},
         {"Uruguay",40},
         {"Ecuador",30},
         {"France",60},
         {"Honduras",5},
         {"Switzerland",15},
         {"Argentina",80},
         {"Bosnia And Herzgovina",5},
         {"Iran",5},
         {"Nigeria",10},
         {"Germany",95},
         {"Ghana",39},
         {"Portugal",55},
         {"United States",30},
         {"Algeria",20},
         {"Belgium",29},
         {"Russia",17},
         {"South Korea",9}
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

        for (i = 0; i < mc; i++)
        {
            var huitieme = Poule();
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

            var finalRanking = rank.OrderByDescending(x => x.Value).ThenByDescending(x => r.NextDouble() *TeamsValue[x.Key]).Select(x => x.Key).ToList();
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
        if (result < team1Value)
        {
            return 1;
        }
        else
        {
            return 2;
        }
    }

    private void PlayPouleGame(string team1, string team2, Dictionary<string, int> rank)
    {
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
}