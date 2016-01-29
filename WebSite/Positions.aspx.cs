using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using SignalR.SQL;
using Pricer;
using WebGrease.Css.Extensions;

public partial class Positions : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected object GetPositions(int competitionId)
    {
        var positions = Sql.GetPosition(Context.User.Identity.Name, Master.SelectedUniverseId, competitionId).Where(x => x.Value != 0).ToDictionary(x => x.Key, x => x.Value);
        var simulationResults = PricerHelper.GetVars(Master.GetCompetitionName(), positions, new List<double> {0, 0.1, 0.5, 0.9, 1});
        return
            JavaScriptSerializer.SerializeObject(simulationResults);
    }
}