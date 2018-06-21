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
        var id = Master.GetCompetitionUniverseId();
        var ranking = Sql.GetRankingForAUniverse(id).ToList();
        double ranks = ranking.Count;
        double count = Sql.CountUser(id);

        double sum = 0;
        var perfs = new Dictionary<string, double>();
        for (int i = 0; i < 20; i++)
        {
            var rank = ranking[i];
            perfs[rank.Key] = rank.Value;
            double f = count / (i+1) - 1.0;
            sum += f;
        }

        for (var i = 20; i < ranks; i++)
        {
            var rank = ranking[i];
            perfs[rank.Key] = 0;
        }

        var moneys = ranking
            .Select(x => new RankingInfo(x.Value) { User = x.Key, Profit = 0 })
            .ToList();

        moneys.Sort();
        for (int i = 0; i < ranks; i++)
        {
            var money = moneys[i];
            money.Rank = i + 1;
            double f = count / (i + 1) - 1.0;
            if (i < 20)
            money.Profit = Math.Round(10* count * f / sum, 1);
        }

        return JavaScriptSerializer.SerializeObject(moneys);
    }
}

public class RankingInfo : IComparable
{
    public string User;
    private int Money { get; set; }
    public string MoneyString;
    public int Rank;
    public double Profit;

    public RankingInfo(int value)
    {
        Money = value;
        MoneyString = (Money / 1000).ToString() + "K";
    }

    public int CompareTo(object obj)
    {
        var rankingInfo = obj as RankingInfo;
        if (rankingInfo == null)
            return 0;
        return rankingInfo.Money.CompareTo(Money);
    }
}