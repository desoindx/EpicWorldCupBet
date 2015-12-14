using System;
using System.Linq;
using System.Web.UI;
using SignalR.SQL;

public partial class Positions : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected object GetTrades(int competitionId)
    {
        var user = User.Identity.Name;
        var trades = Sql.GetAllTrades(user, Master.SelectedUniverseId, competitionId);
        return
            JavaScriptSerializer.SerializeObject(
                trades.Select(
                    x =>
                        new
                        {
                            Date = x.Date.ToShortDateString() + " " + x.Date.ToShortTimeString(),
                            Quantity = x.Quantity,
                            Team = Sql.GetTeamName(x.Team),
                            Price = x.Price,
                            Buyer = x.Buyer,
                            Seller = x.Seller
                        }));
    }
}