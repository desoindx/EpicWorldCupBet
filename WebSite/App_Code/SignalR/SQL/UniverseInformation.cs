using System;
using System.Collections.Generic;
using System.Linq;
using Datas.Entities;

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

        public static Dictionary<string, int> GetRankingForAUniverse(int idUniverseCompetition)
        {
            var moneys = new Dictionary<string, int>();
            using (var context = new Entities())
            {
                var competitions = context.UniverseCompetitions.Where(x => x.Id == idUniverseCompetition).ToList();
                foreach (var competition in competitions)
                    GetRankingForACompetition(competition, moneys, context);

                var currentMoney = context.Moneys.Where(x => x.IdUniverseCompetition == idUniverseCompetition && moneys.Keys.Contains<string>(x.User));
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
                var value = GetTeamCurrentValue(context, team, competition.Id);
                teamsValue.Add(team.Id, value);
            }

            foreach (var trade in context.Trades.Where(x => x.IdUniverseCompetition == competition.Id))
            {
                var value = teamsValue[trade.Team] * trade.Quantity;
                usersMoney.UpdateOrSet(trade.Buyer, value, (x, y) => x + y);
                usersMoney.UpdateOrSet(trade.Seller, value, (x, y) => x - y);
            }
        }

        public static int GetTeamCurrentValue(Team team, int id)
        {
            using (var context = new Entities())
                return GetTeamCurrentValue(context, team, id);
        }

        private static int GetTeamCurrentValue(Entities context, Team team, int id)
        {
            var value = 0;
            if (team.Result.HasValue)
            {
                var overridedValue =
                    context.ResultOverridedValues.FirstOrDefault(
                        x => x.IdUniverseCompetition == id && x.Id == team.Result.Value);
                if (overridedValue != null)
                    value = overridedValue.Value;
                else
                {
                    var defaultValue = context.Results.First(x => x.Id == team.Result.Value);
                    value = defaultValue.DefaultValue;
                }
            }
            else
            {
                var allOrders = context.Orders.Where(x => x.Team == team.Id && x.Status == 0 && x.IdUniverseCompetition == id);
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
                        context.Trades.Where(x => x.Team == team.Id && x.IdUniverseCompetition == id).OrderByDescending(x => x.Date).FirstOrDefault();
                    if (lastTrade != null)
                        value = lastTrade.Price;
                }
            }
            return value;
        }

        public static List<Trade> GetLastTradeFor(int competitionId, int universeId, int numberOfTrades = 5)
        {
            using (var context = new Entities())
            {
                int id;
                if (TryGetUniverseCompetitionId(universeId, competitionId, context, out id))
                {
                    return
                        context.Trades.Where(x => x.IdUniverseCompetition == id)
                            .OrderByDescending(x => x.Date)
                            .Take(numberOfTrades)
                            .ToList();
                }
            }
            return new List<Trade>();
        }
    }
}