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
        double count = ranking.Count;

        double sum = 0;
        var perfs = new Dictionary<string, double>();
        for (int i = 0; i < count; i++)
        {
            var rank = ranking[i];
            perfs[rank.Key] = rank.Value;
            double f = count / (i+1) - 1.0;
            sum += f;
        }

        var moneys = ranking
            .Select(x => new RankingInfo { User = x.Key, Money = x.Value, Profit = perfs[x.Key] })
            .ToList();

        moneys.Sort();
        for (int i = 0; i < moneys.Count; i++)
        {
            var money = moneys[i];
            money.Rank = i + 1;
            double f = count / (i + 1) - 1.0;
            money.Profit = Math.Round(10* count * f / sum, 1);
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