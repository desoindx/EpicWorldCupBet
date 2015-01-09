using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace SignalR
{
    [HubName("Bet")]
    public class BetHub : Hub
    {
        private readonly BetClient _betClient;
        private readonly Dictionary<string, string> _userConnectionId;

        public BetHub()
        {
            _userConnectionId = new Dictionary<string, string>();
            _betClient = new BetClient(GlobalHost.ConnectionManager.GetHubContext<BetHub>().Clients, _userConnectionId);
        }

        public void GetCharts()
        {
            _betClient.GetCharts(Context.ConnectionId);
        }

        public void GetPosition(string user)
        {
            _userConnectionId[user] = Context.ConnectionId;
            _betClient.GetPositions(user, Context.ConnectionId);
        }

        public void GetRanking()
        {
            _betClient.GetClassement(Context.ConnectionId);
        }

        public void GetTeams()
        {
            _betClient.GetTeams(Context.ConnectionId);
        }

        public void GetTeam(string user)
        {
            _userConnectionId[user] = Context.ConnectionId;
            _betClient.GetTeam(user, Context.ConnectionId);
        }

        public void SendOrder(string user, string team, int quantity, int price, string side)
        {
            _userConnectionId[user] = Context.ConnectionId;
            _betClient.NewOrder(user, team, quantity, price, side.ToUpper(), Context.ConnectionId);
        }

        public void CancelOrder(string user, string side, string team)
        {
            _userConnectionId[user] = Context.ConnectionId;
            _betClient.CancelOrder(user, side.ToUpper(), team, Context.ConnectionId);
        }

        public void GetMoney(string user)
        {
            _userConnectionId[user] = Context.ConnectionId;
            _betClient.GetMoney(user, Context.ConnectionId);
        }

        public void SendMessage(string user, string message)
        {
            _userConnectionId[user] = Context.ConnectionId;
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

        public void GetOrderBook(string team)
        {
            _betClient.GetOrderBook(team.Split('(')[0].TrimEnd(), Context.ConnectionId);
        }

        public void GetAllTrades(string user)
        {
            _userConnectionId[user] = Context.ConnectionId;
            if (string.IsNullOrEmpty(user))
                return;
            _betClient.GetTrades(user, Context.ConnectionId);
        }

        public void EliminateTeam(string password, string team, int value)
        {
            if (password == "yeah")
                _betClient.EliminateTeam(Context.ConnectionId, team, value);
        }

        public void Price(string password,
        int brazil,
        int croatia,
        int mexico,
        int cameroon,
        int australia,
        int chile,
        int netherlands,
        int spain,
        int colombia,
        int greece,
        int ivory,
        int japan,
        int costa,
        int england,
        int italy,
        int uruguay,
        int ecuador,
        int france,
        int honduras,
        int switzerland,
        int argentina,
        int bosnia,
        int iran,
        int nigeria,
        int germany,
        int ghana,
        int portugal,
        int united,
        int algeria,
        int belgium,
        int russia,
        int south)
        {
            if (password == "wc2014")
            {
                _betClient.Price(Context.ConnectionId);
            }
            else if (password == "wc2015")
            {
                _betClient.Price(Context.ConnectionId,
                  brazil,
                    croatia,
                    mexico,
                    cameroon,
                    australia,
                    chile,
                    netherlands,
                    spain,
                    colombia,
                    greece,
                    ivory,
                    japan,
                    costa,
                    england,
                    italy,
                    uruguay,
                    ecuador,
                    france,
                    honduras,
                    switzerland,
                    argentina,
                    bosnia,
                    iran,
                    nigeria,
                    germany,
                    ghana,
                    portugal,
                    united,
                    algeria,
                    belgium,
                    russia,
                    south);
            }
        }
    }
}