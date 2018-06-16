using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using SignalR.SQL;
using Pricer;
using Datas.Entities;

public partial class PricerDeOuf : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected object GetPricing()
    {
        Dictionary<int, double> prices = null;
        Dictionary<int, string> names = new Dictionary<int, string>();
        using (var context = new Entities())
        {
            var id = Master.GetCompetitionId();
            var competition = context.Competitions.First(x => id == x.Id);
            var strength = new Dictionary<Team, double>();
            foreach (var team in context.Teams.Where(x => x.IdCompetition == competition.Id && x.RealTeam.HasValue && x.RealTeam.Value))
            {
                strength[team] = team.Strength ?? 1;
                names[team.Id] = team.Name;
            }
            prices = PricerHelper.Price(competition.Name, strength);
        }
        return JavaScriptSerializer.SerializeObject(prices.Where(x => x.Value > 0).Select(x => new PricingInfo { Team = names[x.Key], Price = x.Value }).ToList());
    }
}

public class PricingInfo
{
    public string Team;
    public double Price;
}