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

        public void GetCharts()
        {
            _betClient.GetCharts(Context.ConnectionId);
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

        public void SendMessage(string user, string message)
        {
            _betClient.SendMessage(user, message);
        }

        public void GetMessages()
        {
            _betClient.GetMessages();
        }

        public void GetLastTrades()
        {
            _betClient.GetLastTrades(Context.ConnectionId);
        }

        public void GetAllTrades(string user)
        {
            if (string.IsNullOrEmpty(user))
                return;
            _betClient.GetTrades(user, Context.ConnectionId);
        }

        public void Price(string password,
        int Brazil,
        int Croatia,
        int Mexico,
        int Cameroon,
        int Australia,
        int Chile,
        int Netherlands,
        int Spain,
        int Colombia,
        int Greece,
        int Ivory,
        int Japan,
        int Costa,
        int England,
        int Italy,
        int Uruguay,
        int Ecuador,
        int France,
        int Honduras,
        int Switzerland,
        int Argentina,
        int Bosnia,
        int Iran,
        int Nigeria,
        int Germany,
        int Ghana,
        int Portugal,
        int United,
        int Algeria,
        int Belgium,
        int Russia,
        int South)
        {
            if (password != "wc2014")
                return;

            _betClient.Price(Context.ConnectionId,
                Brazil,
        Croatia,
        Mexico,
        Cameroon,
        Australia,
        Chile,
        Netherlands,
        Spain,
        Colombia,
        Greece,
        Ivory,
        Japan,
        Costa,
        England,
        Italy,
        Uruguay,
        Ecuador,
        France,
        Honduras,
        Switzerland,
        Argentina,
        Bosnia,
        Iran,
        Nigeria,
        Germany,
        Ghana,
        Portugal,
        United,
        Algeria,
        Belgium,
        Russia,
        South);
        }
    }
}