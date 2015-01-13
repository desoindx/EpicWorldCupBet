using System.Collections.Generic;
using System.Linq;
using Datas.Entities;

namespace SignalR.SQL
{
    public static partial class Sql
    {
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
                context.Orders.Where(x => x.IdUniverseCompetition == id && x.Team == team.Name && x.Status == 0)
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
                context.Trades.Where(x => x.IdUniverseCompetition == id && x.Team == team.Name)
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

        private static List<Team> GetTeamsForCompetition(int competitionId, Entities context)
        {
            List<Team> teams;
            if (!Teams.TryGetValue(competitionId, out teams))
            {
                teams = context.Teams.Where(x => x.IdCompetition == competitionId).ToList();
                Teams[competitionId] = teams;
            }
            return teams;
        }

        private static bool TryGetUniverseCompetitionId(int universeId, int competitionId, Entities context, out int id)
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

        private static bool IsUserAuthorizedOn(string user, int universeId, Entities context)
        {
            bool isAuthorized;
            var key = string.Format("user:{0}-universe:{1}", user, universeId);
            if (!UserUniverses.TryGetValue(key, out isAuthorized))
            {
                isAuthorized = context.UniverseAvailables.Any(x => x.UserName == user && x.IdUniverse == universeId);
                UserUniverses[key] = isAuthorized;
            }
            return isAuthorized;
        }
    }
}