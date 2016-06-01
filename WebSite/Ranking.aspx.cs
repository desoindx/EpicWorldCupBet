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
        var count = 10 * ranking.Count;
        double toShift = 0;
        foreach (var rank in ranking)
        {
            if (rank.Value < 0)
            {
                toShift -= rank.Value;
            }
        }

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
                var exp = Math.Exp((rank.Value + toShift) / 100000) - 1;
                perfs[rank.Key] = exp;
                sum += exp;
            }
        }

        var moneys = ranking
            .Select(x => new RankingInfo { User = x.Key, Money = x.Value, Profit = Math.Round(count * perfs[x.Key] / sum, 2) })
            .ToList();

        moneys.Sort();
        return JavaScriptSerializer.SerializeObject(moneys);
    }
}

public class RankingInfo : IComparable
{
    public string User;
    public int Money;
    public double Profit;

    public int CompareTo(object obj)
    {
        var rankingInfo = obj as RankingInfo;
        if (rankingInfo == null)
            return 0;
        return rankingInfo.Money.CompareTo(Money);
    }
}