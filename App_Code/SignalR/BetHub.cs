using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR.Hubs;

namespace Microsoft.AspNet.SignalR.StockTicker
{
    [HubName("Bet")]
    public class BetHub : Hub
    {
        private BetClient _betClient;
        public BetHub()
        {
            _betClient = new BetClient(GlobalHost.ConnectionManager.GetHubContext<BetHub>().Clients);
        }

        public void GetTeam()
        {
            _betClient.GetTeam("Xavier");
        }

        public void SendOrder(string user, string team, int quantity, int price, string side)
        {
            _betClient.NewOrder(user, team, quantity, price, side);
        }
    }
}