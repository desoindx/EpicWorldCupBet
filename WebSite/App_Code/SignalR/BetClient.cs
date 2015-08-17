﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Datas.Entities;
using Microsoft.AspNet.SignalR.Hubs;
using Pricer;
using SignalR.SQL;

namespace SignalR
{
    public class BetClient
    {
        private const string ErrorClass = "alert-danger";
        private const string SuccessClass = "alert-success";
        private const string InfoClass = "alert-info";

        private readonly Dictionary<string, OrderBook> _orderBooks;

        public BetClient(IHubConnectionContext clients, Dictionary<string, string> userConnectionId)
        {
            _orderBooks = new Dictionary<string, OrderBook>();
            Clients = clients;
            _userConnectionId = userConnectionId;
        }

        private IHubConnectionContext Clients
        {
            get;
            set;
        }

        private readonly Dictionary<string, string> _userConnectionId;

        public void GetClassement(string connectionId)
        {
            using (var context = new Entities())
            {
                var teamsValue = new Dictionary<int, int>();

                var userValue = context.Moneys.ToDictionary(money => money.User, money => money.Money1);

                foreach (var trade in context.Trades)
                {
                    userValue[trade.Buyer] += teamsValue[trade.Team] * trade.Quantity;
                    userValue[trade.Seller] -= teamsValue[trade.Team] * trade.Quantity;
                }

                var orderedValue = userValue.OrderByDescending(x => x.Value);
                var user = new List<string>();
                var moneys = new List<int>();

                foreach (var value in orderedValue)
                {
                    user.Add(value.Key);
                    moneys.Add(value.Value);
                }
                Clients.Client(connectionId).newRanking(user, moneys);
            }
        }

        private List<int> _teams = new List<int>();
        public void GetPositions(string user, string connectionId)
        {
            using (var context = new Entities())
            {
                var users = new List<string>();
                var temp = new Dictionary<int, Dictionary<string, int>>();
                foreach (var team in _teams)
                {
                    var teamPositions =
                        context.Trades.Where(x => (x.Buyer == user || x.Seller == user) && x.Team == team);
                    int sum = 0;
                    var userPosition = new Dictionary<string, int>();
                    temp.Add(team, userPosition);
                    foreach (var teamPosition in teamPositions)
                    {
                        if (teamPosition.Buyer == user)
                        {
                            if (!userPosition.ContainsKey(teamPosition.Seller))
                            {
                                userPosition[teamPosition.Seller] = 0;
                            }
                            sum += teamPosition.Quantity;
                            userPosition[teamPosition.Seller] += teamPosition.Quantity;
                        }
                        else if (teamPosition.Seller == user)
                        {
                            if (!userPosition.ContainsKey(teamPosition.Buyer))
                            {
                                userPosition[teamPosition.Buyer] = 0;
                            }
                            sum -= teamPosition.Quantity;
                            userPosition[teamPosition.Buyer] -= teamPosition.Quantity;
                        }
                    }
                    userPosition.Add("Sum", sum);
                }

                users.Add("Sum");
                foreach (var tmp in temp)
                {
                    foreach (var usr in tmp.Value)
                    {
                        if (!users.Contains(usr.Key))
                            users.Add(usr.Key);
                    }
                }

                var teamToRemove = new List<int>();
                foreach (var team in _teams)
                {
                    var tmpTeam = temp[team];
                    int value;
                    if (tmpTeam.TryGetValue("Sum", out value))
                    {
                        if (value == 0)
                            teamToRemove.Add(team);
                    }
                }

                var teams = new List<int>(_teams);
                foreach (var team in teamToRemove)
                {
                    temp.Remove(team);
                    teams.Remove(team);
                }

                var pos = new List<int>();
                foreach (var usr in users)
                {
                    foreach (int t in teams)
                    {
                        Dictionary<string, int> tmpTeam;
                        if (temp.TryGetValue(t, out tmpTeam))
                        {
                            int value;
                            pos.Add(tmpTeam.TryGetValue(usr, out value) ? value : 0);
                        }
                    }
                }

                Clients.Client(connectionId).newPositions(teams, users, pos);
            }
        }

        public void CancelOrder(string user, string side, string team, string connectionId, int universeId, int competitionId, int competitionUniverseId)
        {
            if (string.IsNullOrEmpty(user))
            {
                Clients.Client(connectionId).newMessage("Fail To cancel order, User not found", ErrorClass);
                return;
            }

            Team teamValue;
            if (!Sql.TryGetTeamIdForCompetition(competitionId, team, out teamValue))
            {
                Clients.Client(connectionId).newMessage("Fail To cancel order, Team not found", ErrorClass);
                return;
            }

            lock (this)
            {
                using (var context = new Entities())
                {
                    var previousOrders = context.Orders.Where(x => x.User == user && x.Status == 0 && x.Team == teamValue.Id && x.Side.Trim() == side && x.IdUniverseCompetition == competitionUniverseId);
                    if (!previousOrders.Any())
                    {
                        Clients.Client(connectionId).newMessage("No order found to cancel", ErrorClass);
                        return;
                    }
                    foreach (var previousOrder in previousOrders)
                    {
                        previousOrder.Status = 2;
                    }
                    context.SaveChanges();
                    OnNewOrder(context, team, new HashSet<string> { user }, universeId, competitionId, competitionUniverseId);
                }
            }

            Clients.Client(connectionId).newMessage("Successfuly canceled order", SuccessClass);
        }

        private bool CheckOrder(string user, string team, int quantity, int price, string side, string connectionId, int universeId, int competitionId, int universeCompetitionId)
        {
            if (string.IsNullOrEmpty(user))
            {
                Clients.Client(connectionId).newMessage("Fail To add order because you are not logged in", ErrorClass);
                return false;
            }

            if (!Sql.IsUserAuthorizedOn(user, universeId))
            {
                Clients.Client(connectionId).newMessage("Fail To add order because you are not allowed on this universe", ErrorClass);
                return false;
            }

            if (quantity < 1)
            {
                Clients.Client(connectionId).newMessage("Fail To add order because the quantity specified must be stricly positive", ErrorClass);
                return false;
            }

            if (price < 1 || price > 999)
            {
                Clients.Client(connectionId).newMessage("Fail To add order because the price specified must be between 1 and 999", ErrorClass);
                return false;
            }

            if (side != "BUY" && side != "SELL")
            {
                Clients.Client(connectionId).newMessage("Fail To add order because the way specified is incorect", ErrorClass);
                return false;
            }

            return true;
        }

        public void NewOrder(string user, string team, int quantity, int price, string side, string connectionId, int universeId, int competitionId, int universeCompetitionId)
        {
            if (!CheckOrder(user, team, quantity, price, side, connectionId, universeId, competitionId, universeCompetitionId))
                return;

            var usersToNotify = new HashSet<string> { user };
            lock (this)
            {
                using (var context = new Entities())
                {
                    Team choosedTeam;
                    if (!Sql.TryGetTeamIdForCompetition(competitionId, team, out choosedTeam))
                    {
                        Clients.Client(connectionId).newMessage("Fail To add order because the team does not exist", ErrorClass);
                        return;
                    }

                    if (choosedTeam.Result.HasValue)
                    {
                        Clients.Client(connectionId).newMessage("Cannot place order on this team anymore", ErrorClass);
                        return;
                    }

                    var otherOrder =
                        context.Orders.FirstOrDefault(
                            x =>
                                x.User == user && x.Status == 0 && x.Team == choosedTeam.Id && x.Side != side &&
                                x.IdUniverseCompetition == universeCompetitionId);
                    if (otherOrder != null)
                    {
                        if ((side == "BUY" && otherOrder.Price <= price) || (side == "SELL" && otherOrder.Price > price))
                        {
                            Clients.Client(connectionId).newMessage("Please check your price !!!", ErrorClass);
                            return;
                        }
                    }

                    if (UserHasEnough(context, user, choosedTeam, quantity, price, side, universeId, competitionId, universeCompetitionId))
                    {
                        var remainingQuantity = InsertTrade(context, user, choosedTeam.Id, choosedTeam.Name, quantity, price, side, universeCompetitionId, connectionId, usersToNotify);
                        if (remainingQuantity > 0)
                            InsertNewOrder(context, user, choosedTeam.Id, remainingQuantity, price, side, universeCompetitionId, connectionId);

                        context.SaveChanges();
                        OnNewOrder(context, team, usersToNotify, universeId, competitionId, universeCompetitionId);
                    }
                    else
                    {
                        Clients.Client(connectionId).newMessage("You already have too much exposure", ErrorClass);
                        return;
                    }
                }
            }
        }

        private bool UserHasEnough(Entities context, string user, Team team, int quantity, int price, string side, int universeId, int competitionId, int universeCompetitionId)
        {
            var positions = Sql.GetPosition(user, universeId, competitionId);
            var signedQuantity = (side == "SELL" ? -1 : 1) * quantity;
            positions[team] += signedQuantity;
            var worstScenario = PricerHelper.GetWorstScenario(context.Competitions.First(x => x.Id == competitionId).Name, positions, 0.1);
            var userMoney = context.Moneys.First(x => x.User == user && x.IdUniverseCompetition == universeCompetitionId);

            return  userMoney.Money1 - signedQuantity * price + worstScenario >= 0;
        }

        private int InsertTrade(Entities context, string user, int team, string teamName, int quantity, int price, string side, int universeCompetitionId, string connectionId, HashSet<string> usersToNotify)
        {
            IQueryable<Order> matchingOrders = side == "BUY"
                ? context.Orders.Where(
                    x => x.User != user && x.Status == 0 && x.Team == team && x.Side.Trim() == "SELL" && x.Price <= price && x.IdUniverseCompetition == universeCompetitionId)
                    .OrderBy(x => x.Price)
                    .ThenBy(x => x.Date)
                : context.Orders.Where(
                    x => x.User != user && x.Status == 0 && x.Team == team && x.Side.Trim() == "BUY" && x.Price >= price && x.IdUniverseCompetition == universeCompetitionId)
                    .OrderByDescending(x => x.Price)
                    .ThenBy(x => x.Date);

            foreach (var matchingOrder in matchingOrders)
            {
                usersToNotify.Add(matchingOrder.User);
                InsertNewTrade(context, side == "BUY" ? user : matchingOrder.User, side == "SELL" ? user : matchingOrder.User, team, Math.Min(quantity, matchingOrder.Quantity), matchingOrder.Price, universeCompetitionId);
                Clients.Client(connectionId).newMessage(string.Format("You traded {0} {1} at {2} with {3}", Math.Min(quantity, matchingOrder.Quantity), teamName, matchingOrder.Price, matchingOrder.User), InfoClass);
                if (quantity < matchingOrder.Quantity)
                {
                    matchingOrder.Quantity -= quantity;
                    return 0;
                }

                matchingOrder.Status = 1;
                quantity -= matchingOrder.Quantity;

                if (quantity == 0)
                    return 0;
            }
            return quantity;
        }

        private void InsertNewTrade(Entities context, string buyer, string seller, int team, int quantity, int price, int universeCompetitionId)
        {
            var trade = new Trade
            {
                Date = DateTime.Now,
                Quantity = quantity,
                Team = team,
                Buyer = buyer,
                Price = price,
                Seller = seller,
                IdUniverseCompetition = universeCompetitionId
            };
            context.Trades.Add(trade);

            var buyerMoney = context.Moneys.FirstOrDefault(x => x.User == buyer && x.IdUniverseCompetition == universeCompetitionId);
            var sellerMoney = context.Moneys.FirstOrDefault(x => x.User == seller && x.IdUniverseCompetition == universeCompetitionId);

            buyerMoney.Money1 -= price * quantity;
            sellerMoney.Money1 += price * quantity;
        }

        private void InsertNewOrder(Entities context, string user, int team, int quantity, int price, string side, int idUniverseCompetition, string connectionId)
        {
            var previousOrders =
                context.Orders.Where(
                    x =>
                        x.User == user && x.Status == 0 && x.Team == team && x.Side.Trim() == side && x.Status != 2 &&
                        x.IdUniverseCompetition == idUniverseCompetition);
            foreach (var previousOrder in previousOrders)
            {
                previousOrder.Status = 2;
            }

            var order = new Order
            {
                Date = DateTime.Now,
                Quantity = quantity,
                Status = 0,
                Team = team,
                User = user,
                Price = price,
                Side = side,
                IdUniverseCompetition = idUniverseCompetition
            };
            context.Orders.Add(order);

            Clients.Client(connectionId).newMessage("Succefuly add order", SuccessClass);
        }

        private void OnNewOrder(Entities context, string team, HashSet<string> users, int universeId, int competitionId, int id)
        {
            _orderBooks.Remove(team + "-" + id);
            var connectionIdToIgnore = new string[users.Count];
            OrderR order = null;
            int i = 0;
            foreach (var user in users)
            {
                order = Sql.GetTeamInformation(context, user, team, universeId, competitionId);
                string connectionId;
                if (_userConnectionId.TryGetValue(user, out connectionId))
                {
                    Clients.Client(connectionId).newPrice(order, true, competitionId);
                    connectionIdToIgnore[i] = connectionId;
                    i++;
                }
            }

            if (order != null)
                Clients.Group(competitionId.ToString(CultureInfo.InvariantCulture), connectionIdToIgnore)
                    .newPrice(order, false, competitionId);
        }

        public void GetCharts(string connectionId)
        {
            const int hour = 4;
            using (var context = new Entities())
            {
                var teamsTrades = new Dictionary<int, TeamTrades>();
                var currDate = new DateTime(2014, 6, 11, 7, 00, 00);
                TradeR currentTradeHisto = null;
                foreach (var trade in context.Trades.OrderBy(x => x.Team).ThenBy(x => x.Date))
                {
                    TeamTrades teamTrade;
                    int lastPrice = trade.Price;
                    if (!teamsTrades.TryGetValue(trade.Team, out teamTrade))
                    {
                        currDate = new DateTime(2014, 6, 11, 7, 00, 00);
                        teamTrade = new TeamTrades { Team = trade.Team.ToString(), Trades = new List<TradeR>() };
                        teamsTrades.Add(trade.Team, teamTrade);
                        while (currDate.AddHours(hour) < trade.Date)
                        {
                            currDate = currDate.AddHours(hour);
                        }
                        currentTradeHisto = new TradeR
                        {
                            Time = currDate,
                            Open = trade.Price,
                            Close = trade.Price,
                            High = trade.Price,
                            Low = trade.Price,
                            Volume = trade.Quantity
                        };
                        teamTrade.TotalVolume = trade.Quantity;
                        teamTrade.Trades.Add(currentTradeHisto);
                    }
                    else
                    {
                        if (currDate.AddHours(hour) < trade.Date)
                        {
                            while (currDate.AddHours(hour) < trade.Date)
                            {
                                currDate = currDate.AddHours(hour);
                                currentTradeHisto = new TradeR
                                {
                                    Time = currDate,
                                    Open = lastPrice,
                                    Close = lastPrice,
                                    High = lastPrice,
                                    Low = lastPrice
                                };
                                teamTrade.Trades.Add(currentTradeHisto);
                            }

                            currentTradeHisto = new TradeR
                            {
                                Time = currDate,
                                Open = trade.Price,
                                Close = trade.Price,
                                High = trade.Price,
                                Low = trade.Price,
                                Volume = trade.Quantity
                            };
                            teamTrade.TotalVolume += trade.Quantity;
                            teamTrade.Trades.Add(currentTradeHisto);
                        }
                        else
                        {
                            teamTrade.TotalVolume += trade.Quantity;
                            currentTradeHisto.Volume += trade.Quantity;
                            currentTradeHisto.Close = trade.Price;
                            if (trade.Price > currentTradeHisto.High)
                                currentTradeHisto.High = trade.Price;
                            if (trade.Price < currentTradeHisto.Low)
                                currentTradeHisto.Low = trade.Price;
                        }
                    }
                }
                Clients.Client(connectionId).newCharts(teamsTrades.Values.ToList());
            }
        }

        public void GetOrderBook(string user, string team, string lastTradedPrice, int universeCompetitionId, int competitionId, string connectionId)
        {
            int lastTradedPriceValue;
            if (!Int32.TryParse(lastTradedPrice, out lastTradedPriceValue))
                lastTradedPriceValue = 0;

            Team choosedTeam;
            if (!Sql.TryGetTeamIdForCompetition(competitionId, team, out choosedTeam))
                return;

            OrderBook orderBook;
            var key = team + "-" + universeCompetitionId;

            var midPrice = Sql.GetTeamCurrentValue(choosedTeam, universeCompetitionId);
            var position = Sql.GetUserPositionOnTeam(user, choosedTeam, universeCompetitionId);
            if (_orderBooks.TryGetValue(key, out orderBook))
            {
                Clients.Client(connectionId).showOrderBook(team, orderBook.Bids, orderBook.Asks, lastTradedPriceValue, midPrice, position);
            }
            else
            {
                using (var context = new Entities())
                {
                    var orders = context.Orders.Where(x => x.Team == choosedTeam.Id && x.Status == 0 && x.IdUniverseCompetition == universeCompetitionId);
                    var asks = orders.Where(x => x.Side.Trim() == "SELL").GroupBy(x => x.Price).Select(x => new OrderBookInfo { Price = x.Key, Quantity = x.Sum(o => o.Quantity) }).OrderBy(x => x.Price).ToList();
                    var bids = orders.Where(x => x.Side.Trim() == "BUY").GroupBy(x => x.Price).Select(x => new OrderBookInfo { Price = x.Key, Quantity = x.Sum(o => o.Quantity) }).OrderByDescending(x => x.Price).ToList();

                    _orderBooks[key] = new OrderBook { Asks = asks, Bids = bids };
                    Clients.Client(connectionId).showOrderBook(team, bids, asks, lastTradedPriceValue, midPrice, position);
                }
            }
        }

        private class OrderBook
        {
            public List<OrderBookInfo> Asks;
            public List<OrderBookInfo> Bids;
        }

        private class OrderBookInfo
        {
            public int Price;
            public int Quantity;
        }
    }
}