using System;
using System.Collections.Generic;
using System.Linq;
using Datas.Entities;
using Microsoft.AspNet.SignalR.Hubs;
using SignalR.SQL;

namespace SignalR
{
    public class BetClient
    {
        private const string ErrorClass = "alert-danger";
        private const string SuccessClass = "alert-success";
        private const string InfoClass = "alert-info";

        private const int MaxExposure = 10000;
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

        public void GetTrades(string user, string connectionId)
        {
            var tradesMessages = new List<string>();
            using (var context = new Entities())
            {
                var trades = context.Trades.Where(x => x.Buyer == user || x.Seller == user).OrderByDescending(x => x.Date);
                tradesMessages.AddRange(
                    trades.Select(
                        trade =>
                            string.Format("{6} {2}, You traded {5}{0} {1} at {3} with {4}", trade.Quantity, trade.Team,
                                trade.Date.ToLongTimeString(), trade.Price,
                                trade.Seller == user ? trade.Buyer : trade.Seller, trade.Seller == user ? "-" : "",
                                trade.Date.ToLongDateString())));
            }
            Clients.Client(connectionId).histoTrades(tradesMessages);
        }

        public void GetClassement(string connectionId)
        {
            using (var context = new Entities())
            {
                var teamsValue = new Dictionary<string, int>();
               
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

        private List<string> _teams = new List<string>(); 
        public void GetPositions(string user, string connectionId)
        {
            using (var context = new Entities())
            {
                var users = new List<string>();
                var temp = new Dictionary<string, Dictionary<string, int>>();
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

                var teamToRemove = new List<string>();
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

                var teams = new List<string>(_teams);
                foreach (var team in teamToRemove)
                {
                    temp.Remove(team);
                    teams.Remove(team);
                }

                var pos = new List<int>();
                foreach (var usr in users)
                {
                    foreach (string t in teams)
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

        public void CancelOrder(string user, string side, string team, string connectionId, int universeId, int competitionId)
        {
            if (string.IsNullOrEmpty(user))
            {
                Clients.Client(connectionId).newMessage("Fail To cancel order", ErrorClass);
                return;
            }

            lock (this)
            {
                using (var context = new Entities())
                {
                    var previousOrders = context.Orders.Where(x => x.User == user && x.Status == 0 && x.Team == team && x.Side.Trim() == side);
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
                    OnNewOrder(context, team, new HashSet<string> {user}, universeId, competitionId);
                }
            }

            Clients.Client(connectionId).newMessage("Successfuly canceled order", SuccessClass);
        }

        private bool CheckOrder(string user, string team, int quantity, int price, string side, string connectionId)
        {
            if (string.IsNullOrEmpty(user))
            {
                Clients.Client(connectionId).newMessage("Fail To add order because you are not logged in", ErrorClass);
                return false;
            }

            if (!_teams.Contains(team))
            {
                Clients.Client(connectionId).newMessage("Fail To add order because the team does not exist", ErrorClass);
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

        public void NewOrder(string user, string team, int quantity, int price, string side, string connectionId, int universeId, int competitionId)
        {
            if (!CheckOrder(user, team, quantity, price, side, connectionId))
                return;

            var usersToNotify = new HashSet<string> { user };
            lock (this)
            {
                using (var context = new Entities())
                {
                    if (context.Results.FirstOrDefault(x => x.Team == team) != null)
                    {
                        Clients.Client(connectionId).newMessage("Cannot place order on this team anymore", ErrorClass);
                        return;
                    }

                    var otherOrder = context.Orders.FirstOrDefault(x => x.User == user && x.Status == 0 && x.Team == team && x.Side != side);
                    if (otherOrder != null)
                    {
                        if ((side == "BUY" && otherOrder.Price <= price) || (side == "SELL" && otherOrder.Price > price))
                        {
                            Clients.Client(connectionId).newMessage("please check your price !!!", ErrorClass);
                            return;
                        }
                    }

                    if (UserHasEnough(context, user, team, quantity, price, side))
                    {
                        var remainingQuantity = InsertTrade(context, user, team, quantity, price, side, connectionId, usersToNotify);
                        if (remainingQuantity > 0)
                            InsertNewOrder(context, user, team, remainingQuantity, price, side, connectionId);

                        context.SaveChanges();
                        OnNewOrder(context, team, usersToNotify, universeId, competitionId);
                    }
                    else
                    {
                        Clients.Client(connectionId).newMessage("You already have too much exposure", InfoClass);
                        return;
                    }
                }
            }

            // GetMoney(user, connectionId);
            GetLastTrades();
        }

        private bool UserHasEnough(Entities context, string user, string team, int quantity, int price, string side)
        {
            var positions = context.Trades.Where(x => (x.Seller == user || x.Buyer == user) && x.Team == team);
            var total = 0;
            foreach (var position in positions)
            {
                if (position.Buyer == user)
                    total += position.Quantity;
                else
                    total -= position.Quantity;
            }

            if (side == "SELL" && total < 0)
            {
                total -= quantity;
                if (total < -250)
                    return false;
            }
            else if (side == "BUY" && total > 0)
            {
                total += quantity;
                if (total > 250)
                    return false;
            }

            if (side == "SELL")
                return true;

            var userMoney = context.Moneys.FirstOrDefault(x => x.User == user);
            if (userMoney == null)
            {
                userMoney = new Money { User = user, Money1 = MaxExposure };
                context.Moneys.Add(userMoney);
            }

            var previousOrders = context.Orders.Where(x => x.User == user && x.Status == 0 && x.Side.Trim() == "BUY");
            int exposure = price * quantity;
            foreach (var previousOrder in previousOrders)
            {
                exposure += previousOrder.Price * previousOrder.Quantity;
            }

            return exposure <= userMoney.Money1;
        }

        private int InsertTrade(Entities context, string user, string team, int quantity, int price, string side, string connectionId, HashSet<string> usersToNotify)
        {
            IQueryable<Order> matchingOrders = side == "BUY"
                ? context.Orders.Where(
                    x => x.User != user && x.Status == 0 && x.Team == team && x.Side.Trim() == "SELL" && x.Price <= price)
                    .OrderBy(x => x.Price)
                    .ThenBy(x => x.Date)
                : context.Orders.Where(
                    x => x.User != user && x.Status == 0 && x.Team == team && x.Side.Trim() == "BUY" && x.Price >= price)
                    .OrderByDescending(x => x.Price)
                    .ThenBy(x => x.Date);

            foreach (var matchingOrder in matchingOrders)
            {
                usersToNotify.Add(matchingOrder.User);
                InsertNewTrade(context, side == "BUY" ? user : matchingOrder.User, side == "SELL" ? user : matchingOrder.User, team, Math.Min(quantity, matchingOrder.Quantity), matchingOrder.Price);
                Clients.Client(connectionId).newMessage(string.Format("you traded {0} {1} at {2} with {3}", Math.Min(quantity, matchingOrder.Quantity), team, matchingOrder.Price, matchingOrder.User), InfoClass);
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

        private void InsertNewTrade(Entities context, string buyer, string seller, string team, int quantity, int price)
        {
            var trade = new Trade
            {
                Date = DateTime.Now,
                Quantity = quantity,
                Team = team,
                Buyer = buyer,
                Price = price,
                Seller = seller
            };
            context.Trades.Add(trade);

            var buyerMoney = context.Moneys.FirstOrDefault(x => x.User == buyer);
            if (buyerMoney == null)
            {
                buyerMoney = new Money { User = buyer, Money1 = MaxExposure };
                context.Moneys.Add(buyerMoney);
            }
            var sellerMoney = context.Moneys.FirstOrDefault(x => x.User == seller);
            if (sellerMoney == null)
            {
                sellerMoney = new Money { User = seller, Money1 = MaxExposure };
                context.Moneys.Add(sellerMoney);
            }

            buyerMoney.Money1 -= price * quantity;
            sellerMoney.Money1 += price * quantity;
        }

        private void InsertNewOrder(Entities context, string user, string team, int quantity, int price, string side, string connectionId)
        {
            var previousOrders = context.Orders.Where(x => x.User == user && x.Status == 0 && x.Team == team && x.Side.Trim() == side && x.Status != 2);
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
                Side = side
            };
            context.Orders.Add(order);

            Clients.Client(connectionId).newMessage("Succefuly add order", SuccessClass);
        }

        private void OnNewOrder(Entities context, string team, HashSet<string> users, int universeId, int competitionId)
        {
            _orderBooks.Remove(team);
            var connectionIdToIgnore = new string[users.Count];
            OrderR order = null;
            int i = 0;
            foreach (var user in users)
            {
                order = Sql.GetTeamInformation(context, user, team, universeId, competitionId);
                string connectionId;
                if (_userConnectionId.TryGetValue(user, out connectionId))
                {
                    Clients.Client(connectionId).newPrice(order, true);
                    connectionIdToIgnore[i] = connectionId;
                    i++;
                }
            }

            if (order != null)
                Clients.AllExcept(connectionIdToIgnore).newPrice(order, false);
        }

        public void GetCharts(string connectionId)
        {
            const int hour = 4;
            using (var context = new Entities())
            {
                var teamsTrades = new Dictionary<string, TeamTrades>();
                var currDate = new DateTime(2014, 6, 11, 7, 00, 00);
                TradeR currentTradeHisto = null;
                foreach (var trade in context.Trades.OrderBy(x => x.Team).ThenBy(x => x.Date))
                {
                    TeamTrades teamTrade;
                    int lastPrice = trade.Price;
                    if (!teamsTrades.TryGetValue(trade.Team, out teamTrade))
                    {
                        currDate = new DateTime(2014, 6, 11, 7, 00, 00);
                        teamTrade = new TeamTrades { Team = trade.Team, Trades = new List<TradeR>() };
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

        public void EliminateTeam(string connectionId, string team, int value)
        {
            using (var context = new Entities())
            {
                context.Results.Add(new Result { Team = team, Value = value });
                context.SaveChanges();
            }
            Clients.Client(connectionId).newMessage("Team Eliminated");
        }

        public void Price(string connectionId,
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
            var pricer = new Pricer();
            var pricingResult = pricer.Price(new List<int> {brazil,
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
                south});

            var teams = new List<string>();
            var price = new List<double>();

            foreach (var value in pricingResult)
            {
                teams.Add(value.Key.Replace(" ", ""));
                price.Add(value.Value);
            }
            Clients.Client(connectionId).pricingFinished(teams, price);
        }

        public void GetLastTrades(string connectionId = null)
        {
            using (var context = new Entities())
            {
                var tradeList = new List<string>();
                var trades = context.Trades.OrderByDescending(x => x.Date);
                var enumerator = trades.GetEnumerator();
                int i = 0;
                while (enumerator.MoveNext() && i < 5)
                {
                    i++;
                    tradeList.Add(string.Format("At {0}, {1} {2} traded at {3}", enumerator.Current.Date.ToShortTimeString(), enumerator.Current.Quantity, enumerator.Current.Team, enumerator.Current.Price));
                }

                if (connectionId != null)
                    Clients.Client(connectionId).lastTrades(tradeList);
                else
                    Clients.All.lastTrades(tradeList);
            }
        }

        private static List<string> _chat = new List<string>();
        private static List<string> _chatName = new List<string>();

        public void SendMessage(string user, string message)
        {
            if (string.IsNullOrEmpty(user))
                return;

            if (_chat.Count() > 9)
            {
                _chat.RemoveAt(0);
                _chatName.RemoveAt(0);
            }

            var messages = new List<string>();
            var messagesName = new List<string>();
            var nbMessage = Math.Min(9, _chat.Count);
            for (int i = 0; i < nbMessage; i++)
            {
                messages.Add(_chat[i]);
                messagesName.Add(_chatName[i]);
            }
            messages.Add(message);
            messagesName.Add(string.Format("{0} ({1}) -", user, DateTime.Now.ToShortTimeString()));

            _chat = messages;
            _chatName = messagesName;
            GetMessages();
        }

        public void GetMessages()
        {
            Clients.All.chat(_chatName, _chat);
        }

        internal void Price(string connectionId)
        {
            var pricer = new Pricer();
            var pricingResult = pricer.Price();

            var teams = new List<string>();
            var price = new List<double>();

            foreach (var value in pricingResult)
            {
                teams.Add(value.Key.Replace(" ", ""));
                price.Add(value.Value);
            }
            Clients.Client(connectionId).pricingFinished(teams, price);
        }

        public void GetOrderBook(string team, string connectionId)
        {
            OrderBook orderBook;
            if (_orderBooks.TryGetValue(team, out orderBook))
            {
                Clients.Client(connectionId).showOrderBook(team, orderBook.Bids, orderBook.Asks);
            }
            else
                using (var context = new Entities())
                {
                    var asks = context.Orders.Where(x => x.Team == team && x.Status == 0 && x.Side.Trim() == "SELL").GroupBy(x => x.Price).Select(x => new OrderBookInfo { Price = x.Key, Quantity = x.Sum(o => o.Quantity) }).OrderBy(x => x.Price).ToList();
                    var bids = context.Orders.Where(x => x.Team == team && x.Status == 0 && x.Side.Trim() == "BUY").GroupBy(x => x.Price).Select(x => new OrderBookInfo { Price = x.Key, Quantity = x.Sum(o => o.Quantity) }).OrderByDescending(x => x.Price).ToList();

                    _orderBooks[team] = new OrderBook { Asks = asks, Bids = bids };
                    Clients.Client(connectionId).showOrderBook(team, bids, asks);
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