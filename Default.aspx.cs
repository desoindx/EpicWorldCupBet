using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using SignalR;
using SignalR.SQL;
using WorldCupBetting;

public partial class _Default : Page
{
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

    protected List<string> GetTeamFor(int competitionId)
    {
        return Sql.GetTeamsForCompetition(competitionId).Select(x => x.Name).ToList();
    }
}