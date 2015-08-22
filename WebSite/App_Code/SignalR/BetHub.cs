using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SignalR.SQL;
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
        private static readonly Dictionary<string, string> _userConnectionId = new Dictionary<string, string>();
        private static readonly Dictionary<Tuple<string, int>, DateTime> _userLastSeenTrades = new Dictionary<Tuple<string, int>, DateTime>();
        private Random _random;

        public override Task OnConnected()
        {
            _userConnectionId[User] = Context.ConnectionId;
            var userAvailableCompetitions = Sql.GetUserAvailableCompetitions(User);
            foreach (var competition in userAvailableCompetitions)
            {
                Groups.Add(Context.ConnectionId, competition.ToString(CultureInfo.InvariantCulture));
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {
            var userAvailableCompetitions = Sql.GetUserAvailableCompetitions(User);
            foreach (var competition in userAvailableCompetitions)
            {
                Groups.Remove(Context.ConnectionId, competition.ToString(CultureInfo.InvariantCulture));
            }

            return base.OnDisconnected();
        }

        public BetHub()
        {
            _random = new Random();
            ApplicationDbContext = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ApplicationDbContext));

            _betClient = new BetClient(GlobalHost.ConnectionManager.GetHubContext<BetHub>().Clients, _userConnectionId);
        }

        public void GetRanking()
        {
            _betClient.GetClassement(Context.ConnectionId);
        }

        public void SendOrder(string team, int quantity, int price, string side, int universeId, int competitionId, int universeCompetitionId)
        {
            _betClient.NewOrder(
            _random.Next(100).ToString(), team, quantity, price, side.ToUpper(), Context.ConnectionId, universeId, competitionId, universeCompetitionId);
        }

        public void CancelOrder(string side, string team, int universeId, int competitionId, int competitionUniverseId)
        {
            _betClient.CancelOrder(User, side.ToUpper(), team, Context.ConnectionId, universeId, competitionId, competitionUniverseId);
        }

        public void GetTradeHistory(int competitionId, int competitionUniverseId)
        {
            DateTime userLastSeenTrade;
            if (
                !_userLastSeenTrades.TryGetValue(new Tuple<string, int>(User, competitionUniverseId),
                    out userLastSeenTrade))
            {
                userLastSeenTrade = DateTime.MinValue;
            }
            _betClient.GetTradeHistory(userLastSeenTrade, competitionId, competitionUniverseId, Context.ConnectionId);
        }

        public void ShowTradeHistory(int competitionUniverseId)
        {
            _userLastSeenTrades[new Tuple<string, int>(User, competitionUniverseId)] = DateTime.Now;
        }

        public void SendMessage(int universeId, string message)
        {
            var lastMessage = Chats.NewMessage(universeId, User, message);
            if (lastMessage != null)
                Clients.Group(universeId.ToString(CultureInfo.InvariantCulture)).chat(lastMessage);
        }

        public void GetOrderBook(string team, int universeCompetitionId, int competitionId)
        {
            var teamsInfo = team.Split('(');
            _betClient.GetOrderBook(User, teamsInfo[0].Trim(), teamsInfo[1].Split(')')[0].Trim(), universeCompetitionId, competitionId, Context.ConnectionId);
        }

        public void ClearCache(string cache)
        {
            Sql.ClearCache(cache);
        }
    }
}