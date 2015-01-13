using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using WorldCupBetting;

namespace SignalR
{
    [HubName("Bet")]
    public class BetHub : Hub
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }

        private string User { get { return Context.User.Identity.Name; } }
        private readonly BetClient _betClient;
        private readonly Dictionary<string, string> _userConnectionId;

        public BetHub()
        {
           ApplicationDbContext = new ApplicationDbContext();
           UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ApplicationDbContext));

            _userConnectionId = new Dictionary<string, string>();
            _betClient = new BetClient(GlobalHost.ConnectionManager.GetHubContext<BetHub>().Clients, _userConnectionId);
        }

        public void GetCharts()
        {
            _betClient.GetCharts(Context.ConnectionId);
        }

        public void GetPosition()
        {
            _userConnectionId[User] = Context.ConnectionId;
            _betClient.GetPositions(User, Context.ConnectionId);
        }

        public void GetRanking()
        {
            _betClient.GetClassement(Context.ConnectionId);
        }

        public void SendOrder(string team, int quantity, int price, string side, int universeId, int competitionId)
        {
            _userConnectionId[User] = Context.ConnectionId;
            _betClient.NewOrder(User, team, quantity, price, side.ToUpper(), Context.ConnectionId, universeId, competitionId);
        }

        public void CancelOrder(string side, string team, int universeId, int competitionId)
        {
            _userConnectionId[User] = Context.ConnectionId;
            _betClient.CancelOrder(User, side.ToUpper(), team, Context.ConnectionId, universeId, competitionId);
        }

        public void SendMessage(string message)
        {
            _userConnectionId[User] = Context.ConnectionId;
            _betClient.SendMessage(User, message);
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

        public void GetAllTrades()
        {
            if (string.IsNullOrEmpty(User))
                return;
            _userConnectionId[User] = Context.ConnectionId;
            _betClient.GetTrades(User, Context.ConnectionId);
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