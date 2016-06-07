using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using SignalR.SQL;

public partial class Ranking : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected object GetRanks()
    {
        var ranking = Sql.GetRankingForAUniverse(Master.GetCompetitionUniverseId()).ToList();

        //        ranking = new List<KeyValuePair<string, int>>
        //        {
        //            new KeyValuePair<string, int>("Joueur 1", 247000),
        //            new KeyValuePair<string, int>("Joueur 2", 188000),
        //            new KeyValuePair<string, int>("Joueur 3", 165000),
        //            new KeyValuePair<string, int>("Joueur 4", 132000),
        //            new KeyValuePair<string, int>("Joueur 5", 115000),
        //            new KeyValuePair<string, int>("Joueur 6", 107000),
        //            new KeyValuePair<string, int>("Joueur 7", 113000),
        //            new KeyValuePair<string, int>("Joueur 22", 100000),
        //            new KeyValuePair<string, int>("Joueur 8", 88000),
        //            new KeyValuePair<string, int>("Joueur 9", 87500),
        //            new KeyValuePair<string, int>("Joueur 10", 87000),
        //            new KeyValuePair<string, int>("Joueur 11", 78000),
        //            new KeyValuePair<string, int>("Joueur 12", 83000),
        //            new KeyValuePair<string, int>("Joueur 13", 71000),
        //            new KeyValuePair<string, int>("Joueur 14", 59000),
        //            new KeyValuePair<string, int>("Joueur 15", 68000),
        //            new KeyValuePair<string, int>("Joueur 16", 65500),
        //            new KeyValuePair<string, int>("Joueur 17", 63000),
        //            new KeyValuePair<string, int>("Joueur 18", 60000),
        //            new KeyValuePair<string, int>("Joueur 19", 23000),
        ////            new KeyValuePair<string, int>("Joueur 20", -10000),
        ////            new KeyValuePair<string, int>("Joueur 21", -100000),
        //        };
        var count = 10 * ranking.Count;

        double sum = 0;
        var perfs = new Dictionary<string, double>();
        foreach (var rank in ranking)
        {
            if (rank.Value < 0)
            {
                perfs[rank.Key] = 0;
            }
            else
            {
                var exp = Math.Exp((double)rank.Value / 100000) - 1;
                perfs[rank.Key] = exp;
                sum += exp;
            }
        }

        var moneys = ranking
            .Select(x => new RankingInfo { User = x.Key, Money = x.Value, Profit = perfs[x.Key] })
            .ToList();

        moneys.Sort();

        sum += moneys[0].Profit;
        moneys[0].Profit += moneys[0].Profit;
        sum += moneys[1].Profit * 0.5;
        moneys[1].Profit += moneys[1].Profit * 0.5;
        sum += moneys[2].Profit * 0.25;
        moneys[2].Profit += moneys[2].Profit * 0.25;

        sum -= moneys[moneys.Count - 1].Profit / 2;
        moneys[moneys.Count - 1].Profit -= moneys[moneys.Count - 1].Profit / 2;
        sum -= moneys[moneys.Count - 2].Profit / 3;
        moneys[moneys.Count - 2].Profit -= moneys[moneys.Count - 2].Profit / 3;
        sum -= moneys[moneys.Count - 3].Profit / 4;
        moneys[moneys.Count - 3].Profit -= moneys[moneys.Count - 3].Profit / 4;
        for (int i = 0; i < moneys.Count; i++)
        {
            var money = moneys[i];
            money.Rank = i + 1;
            money.Profit = Math.Round(count * money.Profit / sum, 2);
        }
        return JavaScriptSerializer.SerializeObject(moneys);
    }
}

public class RankingInfo : IComparable
{
    public string User;
    public int Money;
    public int Rank;
    public double Profit;

    public int CompareTo(object obj)
    {
        var rankingInfo = obj as RankingInfo;
        if (rankingInfo == null)
            return 0;
        return rankingInfo.Money.CompareTo(Money);
    }
}