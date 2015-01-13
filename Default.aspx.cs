using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datas.Entities;
using SignalR;
using SignalR.SQL;

public partial class _Default : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    private List<Competition> _universeCompetitions;
    protected bool UniverseHasMultipleCompetition()
    {
        _universeCompetitions = Sql.GetUniverseCompetitions(Master.SelectedUniverse);
        return _universeCompetitions.Count > 1;
    }

    protected string GetUniverseCompetition()
    {
        if (_universeCompetitions != null && _universeCompetitions.Count == 1)
            return _universeCompetitions[0].Name;

        return string.Empty;
    }

    protected int GetCompetitionId()
    {
        if (_universeCompetitions != null && _universeCompetitions.Count == 1)
            return _universeCompetitions[0].Id;

        return -1;
    }

    protected IHtmlString GetOrders()
    {
        return
            JavaScriptSerializer.SerializeObject(Sql.GetTeamsInformation(Context.User.Identity.Name,
                Master.SelectedUniverseId,
                _universeCompetitions[0].Id));
    }
}