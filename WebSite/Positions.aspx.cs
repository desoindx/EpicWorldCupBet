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
    protected object GetTrades(int competitionId)
    {
        var user = User.Identity.Name;
        var trades = Sql.GetAllTrades(user, Master.SelectedUniverseId, competitionId);
        var result = trades.Select(
            trade =>
                string.Format("{6} {2}, You traded {5}{0} {1} at {3} with {4}", trade.Quantity, Sql.GetTeamName(trade.Team),
                    trade.Date.ToLongTimeString(), trade.Price, trade.Seller == user ? trade.Buyer : trade.Seller,
                    trade.Seller == user ? "-" : "", trade.Date.ToLongDateString())).ToList();
        return JavaScriptSerializer.SerializeObject(result);
    }

    protected object GetPositions(int competitionId)
    {
        var positions = Sql.GetPosition(Context.User.Identity.Name, Master.SelectedUniverseId, competitionId);
        var simulationResults = PricerHelper.GetVars(Master.GetCompetitionName(), positions, new List<double> {0, 0.1, 0.5, 0.9, 1});
        return
            JavaScriptSerializer.SerializeObject(simulationResults);
    }
}