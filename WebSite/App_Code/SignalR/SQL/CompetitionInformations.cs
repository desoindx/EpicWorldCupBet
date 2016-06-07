using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Datas.Entities;

namespace SignalR.SQL
{
    public static partial class Sql
    {
        public static Dictionary<Team, int> GetPosition(string user, int universeId, int competitionId)
        {
            if (string.IsNullOrEmpty(user))
                return null;

            using (var context = new Entities())
            {
                int id;
                if (!TryGetUniverseCompetitionId(universeId, competitionId, context, out id))
                    return null;

                var teams = GetTeamsForCompetition(competitionId, context);
                var positions = new Dictionary<Team, int>();
                foreach (var team in teams)
                {
                    var position = GetUserPositionOnTeam(context, user, team, id);
                    positions[team] = position;
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
                    context.Trades.Where(x => x.IdUniverseCompetition == id && (x.Buyer == user || x.Seller == user))
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
                foreach (var team in teams.Where(x => !x.Result.HasValue))
                {
                    if (team.Result.HasValue)
                        continue;

                    orders.Add(GetTeamInformation(context, user, team, id));
                }
            }
            return orders;
        }

        public static List<SwapR> GetSwaps(string user, int universeId, int competitionId)
        {
            if (string.IsNullOrEmpty(user))
                return null;

            var orders = new List<SwapR>();
            using (var context = new Entities())
            {
                if (!IsUserAuthorizedOn(user, universeId, context))
                    return null;

                int id;
                if (!TryGetUniverseCompetitionId(universeId, competitionId, context, out id))
                    return null;

                var swaps = context.SwapOrders.Where(x => x.IdUniverseCompetition == id && x.Status == 0);
                foreach (var swap in swaps)
                {
                    var buyTeam = GetTeam(swap.BuyTeam);
                    var sellTeam = GetTeam(swap.SellTeam);
                    if (buyTeam.Result.HasValue || sellTeam.Result.HasValue)
                    {
                        continue;
                    }
                    orders.Add(new SwapR
                    {
                        BuyQuantity = swap.BuyQuantity,
                        BuyTeam = buyTeam.Name,
                        IsMine = swap.User == user,
                        Price = swap.Price,
                        SellQuantity = swap.SellQuantity,
                        SellTeam = sellTeam.Name,
                        Id = swap.Id
                    });
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
                context.Trades.Where(x => x.IdUniverseCompetition == id && x.Team == team.Id && x.Price != 0)
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
                teams = context.Teams.Where(x => x.IdCompetition == competitionId && (!x.RealTeam.HasValue || x.RealTeam.Value)).OrderBy(x => x.Name).ToList();
                CompetitionTeams[competitionId] = teams;
            }
            return teams;
        }

        public static Team GetTeam(int teamId)
        {
            Team team;
            if (!Teams.TryGetValue(teamId, out team))
            {
                using (var context = new Entities())
                {
                    team = context.Teams.FirstOrDefault(x => x.Id == teamId);
                    if (team == null)
                        return null;

                    Teams[teamId] = team;
                }
            }
            return team;
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

        private static readonly ConcurrentDictionary<Tuple<string, int>, Tuple<Competition, int>> UserDefaultCompetition =
            new ConcurrentDictionary<Tuple<string, int>, Tuple<Competition, int>>();

        public static void UpdateDefaultCompetition(string userName, int competitionId)
        {
            var universe = GetUserSelectedUniverse(userName);
            if (universe == null)
            {
                return;
            }

            var competitions = GetUniverseCompetitions(universe.Name);
            var competition = competitions.FirstOrDefault(x => x.Id == competitionId);
            if (competition == null)
            {
                return;
            }

            int id;
            TryGetUniverseCompetitionId(universe.Id, competition.Id, out id);
            UserDefaultCompetition[new Tuple<string, int>(userName, universe.Id)] = new Tuple<Competition, int>(competition, id);
        }

        public static Competition GetUserSelectedCompetition(Universe universe, string name, out int id)
        {
            id = -1;
            if (universe == null)
            {
                return null;
            }

            Tuple<Competition, int> compet;
            if (UserDefaultCompetition.TryGetValue(new Tuple<string, int>(name, universe.Id), out compet))
            {
                id = compet.Item2;
                return compet.Item1;
            }

            var competitions = GetUniverseCompetitions(universe.Name);
            if (competitions.Count > 0)
            {
                var competition = competitions[0];
                TryGetUniverseCompetitionId(universe.Id, competition.Id, out id);
                return competition;
            }

            return null;
        }
    }
}