using System.Collections.Generic;
using System.Linq;

namespace SignalR.SQL
{
    public static partial class Sql
    {
        public static Dictionary<string, int> GetPosition(string user, int universeId, int competitionId)
        {
            if (string.IsNullOrEmpty(user))
                return null;

            using (var context = new Entities())
            {
                int id;
                if (!TryGetUniverseCompetitionId(universeId, competitionId, context, out id))
                    return null;

                var teams = GetTeamsForCompetition(competitionId, context);
                var positions = new Dictionary<string, int>();
                foreach (var team in teams)
                {
                    var position = GetUserPositionOnTeam(context, user, team, id);
                    positions[team.Name] = position;
                }

                return positions;
            }
        }

        public static int GetUserPositionOnTeam(string user, Team team, int id)
        {
            using (var context = new Entities())
                return GetUserPositionOnTeam(context, user, team, id);
        }

        private static int GetUserPositionOnTeam(Entities context, string user, Team team, int id)
        {
            var trades =
                context.Trades.Where(
                    x =>
                        (x.Buyer == user || x.Seller == user) && x.IdUniverseCompetition == id &&
                        x.Team == team.Id);
            int sum = 0;
            foreach (var trade in trades)
            {
                if (trade.Buyer == user)
                    sum += trade.Quantity;
                else if (trade.Seller == user)
                    sum -= trade.Quantity;
            }
            return sum;
        }

        public static List<Trade> GetAllTrades(string user, int universeId, int competitionId)
        {
            if (string.IsNullOrEmpty(user))
                return null;

            using (var context = new Entities())
            {
                int id;
                if (!TryGetUniverseCompetitionId(universeId, competitionId, context, out id))
                    return null;

                return
                    context.Trades.Where(x => x.Buyer == user || x.Seller == user)
                        .OrderByDescending(x => x.Date)
                        .ToList();
            }
        }

        public static List<OrderR> GetTeamsInformation(string user, int universeId, int competitionId)
        {
            if (string.IsNullOrEmpty(user))
                return null;

            var orders = new List<OrderR>();
            using (var context = new Entities())
            {
                if (!IsUserAuthorizedOn(user, universeId, context))
                    return null;

                int id;
                if (!TryGetUniverseCompetitionId(universeId, competitionId, context, out id))
                    return null;

                var teams = GetTeamsForCompetition(competitionId, context);
                foreach (var team in teams)
                {
                    if (team.Result.HasValue)
                        continue;

                    orders.Add(GetTeamInformation(context, user, team, id));
                }
            }
            return orders;
        }

        public static OrderR GetTeamInformation(Entities context, string user, string teamName, int universeId, int competitionId)
        {
            if (!context.UniverseAvailables.Any(x => x.UserName == user && x.IdUniverse == universeId))
                return null;

            var universeCompetition = context.UniverseCompetitions.FirstOrDefault(
                x => x.IdCompetition == competitionId && x.IdUniverse == universeId);
            if (universeCompetition == null)
                return null;

            var id = universeCompetition.Id;
            var team = context.Teams.FirstOrDefault(x => x.IdCompetition == competitionId && x.Name == teamName);

            return GetTeamInformation(context, user, team, id);
        }

        public static OrderR GetTeamInformation(Entities context, string user, Team team, int id)
        {
            var order = new OrderR { Team = team.Name };
            var allOrders =
                context.Orders.Where(x => x.IdUniverseCompetition == id && x.Team == team.Id && x.Status == 0)
                    .ToList();
            var bestAsk =
                allOrders.Where(x => x.Side.Trim() == "SELL")
                    .OrderBy(x => x.Price)
                    .FirstOrDefault();
            if (bestAsk != null)
            {
                order.BestAsk = bestAsk.Price;
                order.BestAskQuantity = bestAsk.Quantity;
            }
            var bestBid =
                allOrders.Where(x => x.Side.Trim() == "BUY")
                    .OrderByDescending(x => x.Price)
                    .FirstOrDefault();
            if (bestBid != null)
            {
                order.BestBid = bestBid.Price;
                order.BestBidQuantity = bestBid.Quantity;
            }
            var myAsk =
                allOrders.FirstOrDefault(x => x.Side.Trim() == "SELL" && x.User == user);
            if (myAsk != null)
            {
                order.MyAsk = myAsk.Price;
                order.MyAskQuantity = myAsk.Quantity;
            }
            var myBid =
                allOrders.FirstOrDefault(x => x.Side.Trim() == "BUY" && x.User == user);
            if (myBid != null)
            {
                order.MyBid = myBid.Price;
                order.MyBidQuantity = myBid.Quantity;
            }

            var lastPrices =
                context.Trades.Where(x => x.IdUniverseCompetition == id && x.Team == team.Id)
                    .OrderByDescending(x => x.Date)
                    .ToList();
            if (lastPrices.Count > 0)
            {
                order.LastTradedPrice = lastPrices[0].Price;
                if (lastPrices.Count > 1)
                    order.LastTradedPriceEvolution = order.LastTradedPrice - lastPrices[1].Price;
            }
            return order;
        }

        public static List<Team> GetTeamsForCompetition(int competitionId)
        {
            using (var context = new Entities())
            {
                return GetTeamsForCompetition(competitionId, context);
            }
        }

        private static List<Team> GetTeamsForCompetition(int competitionId, Entities context)
        {
            List<Team> teams;
            if (!CompetitionTeams.TryGetValue(competitionId, out teams))
            {
                teams = context.Teams.Where(x => x.IdCompetition == competitionId).OrderBy(x => x.Name).ToList();
                CompetitionTeams[competitionId] = teams;
            }
            return teams;
        }

        public static string GetTeamName(int teamId)
        {
            string teamName;
            if (!TeamsName.TryGetValue(teamId, out teamName))
            {
                using (var context = new Entities())
                {
                    var team = context.Teams.FirstOrDefault(x => x.Id == teamId);
                    if (team == null)
                        return null;
                    teamName = team.Name;
                    TeamsName[teamId] = team.Name;
                }
            }
            return teamName;
        }

        public static bool TryGetTeamIdForCompetition(int competitionId, string teamName, out Team team)
        {
            var key = teamName + "-" + competitionId;
            if (!TeamsValue.TryGetValue(key, out team))
            {
                using (var context = new Entities())
                {
                    team = context.Teams.FirstOrDefault(x => x.IdCompetition == competitionId && x.Name == teamName);
                    if (team == null)
                        return false;
                    TeamsValue[key] = team;
                }
            }
            return true;
        }

        public static bool TryGetUniverseCompetitionId(int universeId, int competitionId, out int id)
        {
            using (var context = new Entities())
            {
                return TryGetUniverseCompetitionId(universeId, competitionId, context, out id);
            }
        }

        public static bool TryGetUniverseCompetitionId(int universeId, int competitionId, Entities context, out int id)
        {
            var key = string.Format("{0}-{1}", universeId, competitionId);
            if (!UniverseCompetitions.TryGetValue(key, out id))
            {
                var universeCompetition = context.UniverseCompetitions.FirstOrDefault(
                   x => x.IdCompetition == competitionId && x.IdUniverse == universeId);
                if (universeCompetition == null)
                    return false;

                UniverseCompetitions[key] = universeCompetition.Id;
                id = universeCompetition.Id;
            }
            return true;
        }
    }
}