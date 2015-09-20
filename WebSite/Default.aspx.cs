using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Datas.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Pricer;
using SignalR;
using SignalR.SQL;
using WorldCupBetting;

public partial class _Default : Page
{
    private double? _cashToInvest;

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected IHtmlString GetOrders(int? id = null)
    {
        if (!id.HasValue)
            id = Master.GetCompetitionId();
        return
            JavaScriptSerializer.SerializeObject(Sql.GetTeamsInformation(Context.User.Identity.Name,
                Master.SelectedUniverseId, id.Value));
    }

    protected List<string> GetLastTrade(int competitionId)
    {
        var trades = Sql.GetLastTradeFor(competitionId, Master.SelectedUniverseId);
        return
            trades.Select(
                x =>
                    string.Format("At {0}, {1} {2} traded at {3}", x.Date.ToShortTimeString(), x.Quantity, Sql.GetTeamName(x.Team),
                        x.Price)).ToList();
    }

    protected List<string[]> GetMessages()
    {
        return Chats.GetChat(Master.SelectedUniverseId);
    }

    protected string GetCashToInvest()
    {
        if (_cashToInvest == null)
        {
            GetVar();
        }

        return _cashToInvest.Value.ToString("#,##0", Master.NumberFormatInfo);
    }

    protected string GetVar()
    {
        var positions = Sql.GetPosition(Context.User.Identity.Name, Master.SelectedUniverseId, Master.GetCompetitionId());
        var simulationResults = PricerHelper.GetVars(Master.GetCompetitionName(), positions, new List<double> {0, 0.1, 0.5, 0.9, 1});
        var worst10 = simulationResults.Last().Worst10;
        _cashToInvest = Master.Money + worst10;

        return worst10.ToString("#,##0", Master.NumberFormatInfo);
    }

    protected void LogInAsGuest(object sender, EventArgs e)
    {
        var random = new Random();
        var success = false;
        ApplicationUser user = null;
        while (!success)
        {
            var n = random.Next();
            user = new ApplicationUser { UserName = "Guest" + n, Email = "Guest" + n + "@Guest.com" };
            IdentityResult result = Master.UserManager.Create(user, "@Zerty123");
            success = result.Succeeded;
        }
        Master.SignInManager.SignIn(user, false, true);
        using (var context = new Entities())
        {
            context.Moneys.Add(new Money {Money1 = 10000, User = user.UserName});
            var universe = context.Universes.First(x => x.Name == "Test");
            context.UniverseAvailables.Add(new UniverseAvailable {IdUniverse = universe.Id, UserName = user.UserName});
            context.SaveChanges();
        }
        Response.Redirect("~/Default.aspx");
    }
}