using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using SignalR;
using SignalR.SQL;

public partial class _Default : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    public List<Competition> UniverseCompetitions;
    protected bool UniverseHasMultipleCompetition()
    {
        UniverseCompetitions = Sql.GetUniverseCompetitions(Master.SelectedUniverse);
        return UniverseCompetitions.Count > 1;
    }

    protected string GetUniverseCompetition()
    {
        if (UniverseCompetitions != null && UniverseCompetitions.Count == 1)
            return UniverseCompetitions[0].Name;

        return string.Empty;
    }

    protected int GetCompetitionId()
    {
        if (UniverseCompetitions != null && UniverseCompetitions.Count == 1)
            return UniverseCompetitions[0].Id;

        return -1;
    }

    protected IHtmlString GetOrders(int? id = null)
    {
        if (!id.HasValue)
            id = GetCompetitionId();
        return
            JavaScriptSerializer.SerializeObject(Sql.GetTeamsInformation(Context.User.Identity.Name,
                Master.SelectedUniverseId, id.Value));
    }

    protected List<string> GetLastTrade()
    {
        var trades = Sql.GetLastTradeForUniverse(Master.SelectedUniverseId);
        return
            trades.Select(
                x =>
                    string.Format("At {0}, {1} {2} traded at {3}", x.Date.ToShortTimeString(), x.Quantity, x.Team,
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