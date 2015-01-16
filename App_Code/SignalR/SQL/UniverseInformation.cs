using System;
using System.Collections.Generic;
using System.Linq;

namespace SignalR.SQL
{
    public static partial class Sql
    {
        public static void UpdateOrSet<K, V>(this Dictionary<K, V> dict, K key, V value, Func<V, V, V> operation)
        {
            V currentValue;
            if (dict.TryGetValue(key, out currentValue))
                dict[key] = operation(currentValue, value);
            else
                dict[key] = value;
        }

        public static Dictionary<string, int> GetRankingForAUniverse(int idUniverse)
        {
            var moneys = new Dictionary<string, int>();
            using (var context = new Entities())
            {
                var competitions = context.UniverseCompetitions.Where(x => x.IdUniverse == idUniverse).ToList();
                foreach (var competition in competitions)
                    GetRankingForACompetition(competition, moneys, context);

                var currentMoney = context.Moneys.Where(x => moneys.ContainsKey(x.User));
                foreach (var money in currentMoney)
                {
                    moneys.UpdateOrSet(money.User, money.Money1, (x, y) => x + y);
                }
            }
            return moneys;
        }

        private static void GetRankingForACompetition(UniverseCompetition competition, Dictionary<string, int> usersMoney, Entities context)
        {
            var teams = GetTeamsForCompetition(competition.IdCompetition, context);
            var teamsValue = new Dictionary<int, int>();
            foreach (var team in teams)
            {
                var value = GetTeamCurrentValue(context, team);
                teamsValue.Add(team.Id, value);
            }

            foreach (var trade in context.Trades.Where(x => x.IdUniverseCompetition == competition.Id))
            {
                usersMoney.UpdateOrSet(trade.Buyer, teamsValue[trade.Team] * trade.Quantity, (x, y) => x + y);
                usersMoney.UpdateOrSet(trade.Seller, teamsValue[trade.Team] * trade.Quantity, (x, y) => x - y);
            }
        }

        private static int GetTeamCurrentValue(Entities context, Team team)
        {
            var value = 0;
            if (team.Result.HasValue)
            {
                value = team.Result.Value;
            }
            else
            {
                var allOrders = context.Orders.Where(x => x.Team == team.Id && x.Status == 0);
                var bestAsk = allOrders.Where(x => x.Side.Trim() == "SELL").OrderBy(x => x.Price).FirstOrDefault();
                if (bestAsk != null)
                    value += bestAsk.Price;

                var bestBid = allOrders.Where(x => x.Side.Trim() == "BUY").OrderByDescending(x => x.Price).FirstOrDefault();
                if (bestBid != null)
                {
                    value += bestBid.Price;
                    if (value > bestBid.Price)
                        value /= 2;
                }

                if (value == 0)
                {
                    var lastTrade =
                        context.Trades.Where(x => x.Team == team.Id).OrderByDescending(x => x.Date).FirstOrDefault();
                    if (lastTrade != null)
                        value = lastTrade.Price;
                }
            }
            return value;
        }

        public static List<Trade> GetLastTradeForUniverse(int universeId, int numberOfTrades = 5)
        {
            using (var context = new Entities())
            {
                return context.Trades.OrderByDescending(x => x.Date).Take(numberOfTrades).ToList();
            }
        }
    }
}