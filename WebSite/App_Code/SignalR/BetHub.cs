using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Datas.Entities;
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
        private static Dictionary<Tuple<string, int>, DateTime> _userLastSeenTrades;

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
            _betClient.NewOrder(User, team, quantity, price, side.ToUpper(), Context.ConnectionId, universeId, competitionId, universeCompetitionId);
        }

        public void SendSwap(string buyTeam, int buyQuantity, string sellTeam, int sellQuantity, int price, int universeId, int competitionId, int universeCompetitionId)
        {
            _betClient.NewSwap(User, buyTeam, buyQuantity, sellTeam, sellQuantity, price, Context.ConnectionId, universeId, competitionId, universeCompetitionId);
        }

        public void CancelOrder(string side, string team, int universeId, int competitionId, int competitionUniverseId)
        {
            _betClient.CancelOrder(User, side.ToUpper(), team, Context.ConnectionId, universeId, competitionId, competitionUniverseId);
        }

        public void GetTradeHistory(int competitionId, int competitionUniverseId)
        {
            if (_userLastSeenTrades == null)
            {
                using (var context = new Entities())
                {
                    _userLastSeenTrades = context.NewTradeCheckeds.ToDictionary(x => new Tuple<string, int>(x.User, x.IdUniverseCompetition),
                        x => x.Date);
                }
            }
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
            var userLastSeenTrade = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time"));
            using (var context = new Entities())
            {
                var newTradeChecked = context.NewTradeCheckeds.FirstOrDefault(x => x.IdUniverseCompetition == competitionUniverseId && x.User == User);
                if (newTradeChecked == null)
                {
                    newTradeChecked = new NewTradeChecked
                    {
                        Date = userLastSeenTrade,
                        IdUniverseCompetition = competitionUniverseId,
                        User = User
                    };
                    context.NewTradeCheckeds.Add(newTradeChecked);
                }
                else
                {
                    newTradeChecked.Date = userLastSeenTrade;
                }
                context.SaveChanges();
            }
            _userLastSeenTrades[new Tuple<string, int>(User, competitionUniverseId)] = userLastSeenTrade;
        }

        public void GetOrderBook(string team, int universeCompetitionId, int competitionId)
        {
            var teamsInfo = team.Split('(');
            _betClient.GetOrderBook(User, teamsInfo[0].Trim(), teamsInfo[1].Split(')')[0].Trim(), universeCompetitionId, competitionId, Context.ConnectionId);
        }

        public void GetCashInfos(int universeId, int competitionId, int universeCompetitionId)
        {
            _betClient.GetCashInfos(User, universeId, competitionId, universeCompetitionId, Context.ConnectionId);
        }

        public void ClearCache(string cache)
        {
            if (Context.User.IsInRole("Admin"))
            {
                Sql.ClearCache(cache);
            }
        }
    }
}