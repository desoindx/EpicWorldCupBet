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

        public void GetPosition(string user)
        {
            _betClient.GetPositions(user, Context.ConnectionId);
        }

        public void GetRanking()
        {
            _betClient.GetClassement(Context.ConnectionId);
        }

        public void GetTeam(string user)
        {
            _betClient.GetTeam(user, Context.ConnectionId);
        }

        public void SendOrder(string user, string team, int quantity, int price, string side)
        {
            _betClient.NewOrder(user, team, quantity, price, side.ToUpper(), Context.ConnectionId);
        }

        public void CancelOrder(string user, string side, string team)
        {
            _betClient.CancelOrder(user, side.ToUpper(), team, Context.ConnectionId);
        }

        public void GetMoney(string user)
        {
            _betClient.GetMoney(user, Context.ConnectionId);
        }
    }
}