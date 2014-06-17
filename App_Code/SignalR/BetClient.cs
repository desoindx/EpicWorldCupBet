using DataAccesLayer;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.StockTicker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class BetClient
{
    private const int MaxExposure = 10000;

    public BetClient(IHubConnectionContext clients)
    {
        Clients = clients;
        Teams.Sort();
    }

    private IHubConnectionContext Clients
    {
        get;
        set;
    }

    private List<string> Teams = new List<string> {"Brazil",
        "Croatia",
        "Mexico",
        "Australia",
        "Chile",
        "Netherlands",
        "Spain",
        "Colombia",
        "Greece",
        "Ivory Coast",
        "Japan",
        "Costa Rica",
        "England",
        "Italy",
        "Uruguay",
        "Ecuador",
        "Cameroon",
        "France",
        "Honduras",
        "Switzerland",
        "Argentina",
        "Bosnia And Herzgovina",
        "Iran",
        "Nigeria",
        "Germany",
        "Ghana",
        "Portugal",
        "United States",
        "Algeria",
        "Belgium",
        "Russia",
        "South Korea"
    };

    public void GetTrades(string user, string connectionId)
    {
        var tradesMessages = new List<string>();
        using (var context = new Entities())
        {
            var trades = context.Trades.Where(x => x.Buyer == user  || x.Seller == user).OrderByDescending(x => x.Date);
            foreach (var trade in trades)
            {
                tradesMessages.Add(string.Format("{6} {2}, You traded {5}{0} {1} at {3} with {4}", trade.Quantity, trade.Team, trade.Date.ToLongTimeString(), trade.Price, trade.Seller == user ? trade.Buyer : trade.Seller, trade.Seller == user ? "-" : "", trade.Date.ToLongDateString()));
            }
        }
        Clients.Client(connectionId).histoTrades(tradesMessages);
    }

    public void GetClassement(string connectionId)
    {
        using (var context = new Entities())
        {
            var teamsValue = new Dictionary<string, int>();
            foreach (var team in Teams)
            {
                var result = context.Results.Where(x => x.Team == team).FirstOrDefault();
                if (result != null)
                {
                    teamsValue.Add(team, result.Value);
                }
                else
                {
                    var value = 0;
                    var bestAsk = context.Orders.Where(x => x.Team == team && x.Status == 0 && x.Side == "SELL").OrderByDescending(x => x.Price).FirstOrDefault();
                    if (bestAsk != null)
                    {
                        value += bestAsk.Price;
                    }

                    var bestBid = context.Orders.Where(x => x.Team == team && x.Status == 0 && x.Side == "BUY").OrderBy(x => x.Price).FirstOrDefault();
                    if (bestBid != null)
                    {
                        value += bestBid.Price;
                        if (value > bestBid.Price)
                            value /= 2;
                    }
                    
                    if (value == 0)
                    {
                        var lastTrade = context.Trades.Where(x => x.Team == team).OrderByDescending(x => x.Date).FirstOrDefault();
                        value = lastTrade.Price;
                    }
                    teamsValue.Add(team, value);
                }
            }

            var userValue = new Dictionary<string, int>();
            foreach (var money in context.Moneys)
            {
                userValue.Add(money.User, money.Money1);
            }

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

    public void GetPositions(string user, string connectionId)
    {
        using (var context = new Entities())
        {
            List<string> users = new List<string>();
            var temp = new Dictionary<string, Dictionary<string, int>>();
            foreach (var team in Teams)
            {
                var teamPositions = context.Trades.Where(x => (x.Buyer == user || x.Seller == user) && x.Team == team);
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
            foreach (var team in Teams)
            {
                var tmpTeam = temp[team];
                int value;
                if (tmpTeam.TryGetValue("Sum", out value))
                {
                    if (value == 0)
                        teamToRemove.Add(team);
                }
            }

            var stuntedTeams = new List<string>(Teams);
            foreach (var team in teamToRemove)
            {
                temp.Remove(team);
                stuntedTeams.Remove(team);
            }

            var pos = new List<int>();
            for (int i = 0; i < users.Count; i++)
            {
                var usr = users[i];
                for (int j = 0; j < stuntedTeams.Count; j++)
                {
                    Dictionary<string, int> tmpTeam;
                    if (temp.TryGetValue(stuntedTeams[j], out tmpTeam))
                    {
                        int value;
                        if (tmpTeam.TryGetValue(usr, out value))
                            pos.Add(value);
                        else
                            pos.Add(0);
                    }
                }
            }

            Clients.Client(connectionId).newPositions(stuntedTeams, users, pos);
        }
    }

    public void GetTeam(string user, string connectionId)
    {
        var random = new Random();
        var orders = new List<OrderR>();

        using (var context = new Entities())
        {
            foreach (var team in Teams)
            {
                var order = new OrderR();
                order.Team = team;
                var bestAsk = context.Orders.Where(x => x.Team == team && x.Status == 0 && x.Side == "SELL").OrderBy(x => x.Price).FirstOrDefault();
                if (bestAsk != null)
                {
                    order.BestAsk = bestAsk.Price;
                    order.BestAskQuantity = bestAsk.Quantity;
                }
                var bestBid = context.Orders.Where(x => x.Team == team && x.Status == 0 && x.Side == "BUY").OrderByDescending(x => x.Price).FirstOrDefault();
                if (bestBid != null)
                {
                    order.BestBid = bestBid.Price;
                    order.BestBidQuantity = bestBid.Quantity;
                }
                var myAsk = context.Orders.Where(x => x.Team == team && x.Status == 0 && x.Side == "SELL" && x.User == user).OrderBy(x => x.Price).FirstOrDefault();
                if (myAsk != null)
                {
                    order.MyAsk = myAsk.Price;
                    order.MyAskQuantity = myAsk.Quantity;
                }
                var myBid = context.Orders.Where(x => x.Team == team && x.Status == 0 && x.Side == "BUY" && x.User == user).OrderByDescending(x => x.Price).FirstOrDefault();
                if (myBid != null)
                {
                    order.MyBid = myBid.Price;
                    order.MyBidQuantity = myBid.Quantity;
                }
                var lastPrice = context.Trades.Where(x => x.Team == team).OrderByDescending(x => x.Date).FirstOrDefault();
                if (lastPrice != null)
                {
                    order.LastTradedPrice = lastPrice.Price;
                }
                orders.Add(order);
            }
        }
        Clients.Client(connectionId).newOrders(orders);
    }

    public void CancelOrder(string user, string side, string team, string connectionId)
    {
        if (!(!string.IsNullOrEmpty(user) && Teams.Contains(team)))
        {
            Clients.Client(connectionId).newMessage("Fail To cancel order");
            return;
        }

        lock (this)
        {
            using (var context = new Entities())
            {
                var previousOrders = context.Orders.Where(x => x.User == user && x.Status == 0 && x.Team == team && x.Side == side);
                if (previousOrders.Count() == 0)
                {
                    Clients.Client(connectionId).newMessage("No order found to cancel");
                    return;
                }
                foreach (var previousOrder in previousOrders)
                {
                    previousOrder.Status = 2;
                }
                context.SaveChanges();
            }
        }

        Clients.Client(connectionId).newMessage("Successfuly canceled order");
        GetTeam(user, connectionId);
    }

    public void NewOrder(string user, string team, int quantity, int price, string side, string connectionId)
    {
        if (!(!string.IsNullOrEmpty(user) && Teams.Contains(team) && quantity > 0 && price > 0 && price < 1000 && (side == "BUY" || side == "SELL")))
        {
            Clients.Client(connectionId).newMessage("Fail To add order");
            return;
        }

        lock (this)
        {
            using (var context = new Entities())
            {
                if (context.Results.Where(x => x.Team == team).FirstOrDefault() != null)
                {
                    Clients.Client(connectionId).newMessage("Cannot place order on this team anymore");
                    return;
                }

                var otherOrder = context.Orders.Where(x => x.User == user && x.Status == 0 && x.Team == team && x.Side != side).FirstOrDefault();
                if (otherOrder != null)
                {
                    if ((side == "BUY" && otherOrder.Price <= price) || (side == "SELL" && otherOrder.Price > price))
                    {
                        Clients.Client(connectionId).newMessage("please check your price !!!");
                        return;
                    }
                }

                if (UserHasEnough(context, user, quantity, price, side))
                {
                    var remainingQuantity = InsertTrade(context, user, team, quantity, price, side, connectionId);
                    if (remainingQuantity > 0)
                        InsertNewOrder(context, user, team, remainingQuantity, price, side, connectionId);

                    context.SaveChanges();
                }
                else
                {
                    Clients.Client(connectionId).newMessage("You already have too much exposure");
                    return;
                }
            }
        }

        GetMoney(user, connectionId);
        GetTeam(user, connectionId);
        GetLastTrades();
    }

    private bool UserHasEnough(Entities context, string user, int quantity, int price, string side)
    {
        if (side == "SELL")
            return true;

        var userMoney = context.Moneys.Where(x => x.User == user).FirstOrDefault();
        if (userMoney == null)
        {
            userMoney = new Money { User = user, Money1 = MaxExposure };
            context.Moneys.Add(userMoney);
        }

        var previousOrders = context.Orders.Where(x => x.User == user && x.Status == 0 && x.Side == "BUY");
        int exposure = price * quantity;
        foreach (var previousOrder in previousOrders)
        {
            exposure += previousOrder.Price * previousOrder.Quantity;
        }

        return exposure <= userMoney.Money1;
    }

    private int InsertTrade(Entities context, string user, string team, int quantity, int price, string side, string connectionId)
    {
        IQueryable<Order> matchingOrders;
        if (side == "BUY")
            matchingOrders = context.Orders.Where(x => x.User != user && x.Status == 0 && x.Team == team && x.Side == "SELL" && x.Price <= price).OrderBy(x => x.Price).ThenBy(x => x.Date);
        else
            matchingOrders = context.Orders.Where(x => x.User != user && x.Status == 0 && x.Team == team && x.Side == "BUY" && x.Price >= price).OrderByDescending(x => x.Price).ThenBy(x => x.Date);

        foreach (var matchingOrder in matchingOrders)
        {
            InsertNewTrade(context, side == "BUY" ? user : matchingOrder.User, side == "SELL" ? user : matchingOrder.User, team, Math.Min(quantity, matchingOrder.Quantity), matchingOrder.Price);
            Clients.Client(connectionId).newMessage(string.Format("you traded {0} {1} at {2} with {3}", Math.Min(quantity, matchingOrder.Quantity), team, matchingOrder.Price, matchingOrder.User));
            if (quantity < matchingOrder.Quantity)
            {
                matchingOrder.Quantity -= quantity;
                return 0;
            }
            else
            {
                matchingOrder.Status = 1;
                quantity -= matchingOrder.Quantity;
            }

            if (quantity == 0)
                return 0;
        }
        return quantity;
    }

    private void InsertNewTrade(Entities context, string buyer, string seller, string team, int quantity, int price)
    {
        var trade = new Trade();
        trade.Date = DateTime.Now;
        trade.Quantity = quantity;
        trade.Team = team;
        trade.Buyer = buyer;
        trade.Price = price;
        trade.Seller = seller;
        context.Trades.Add(trade);

        var buyerMoney = context.Moneys.Where(x => x.User == buyer).FirstOrDefault();
        if (buyerMoney == null)
        {
            buyerMoney = new Money { User = buyer, Money1 = MaxExposure };
            context.Moneys.Add(buyerMoney);
        }
        var sellerMoney = context.Moneys.Where(x => x.User == seller).FirstOrDefault();
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
        var previousOrders = context.Orders.Where(x => x.User == user && x.Status == 0 && x.Team == team && x.Side == side);
        foreach (var previousOrder in previousOrders)
        {
            previousOrder.Status = 2;
        }

        var order = new Order();
        order.Date = DateTime.Now;
        order.Quantity = quantity;
        order.Status = 0;
        order.Team = team;
        order.User = user;
        order.Price = price;
        order.Side = side;
        context.Orders.Add(order);

        Clients.Client(connectionId).newMessage("Succefuly add order");
    }

    public void GetMoney(string user, string connectionId)
    {
        if (string.IsNullOrEmpty(user))
            return;

        using (var context = new Entities())
        {
            var money = context.Moneys.Where(x => x.User == user).FirstOrDefault();
            if (money == null)
            {
                money = new Money { User = user, Money1 = MaxExposure };
                context.Moneys.Add(money);
                context.SaveChanges();
            }
            Clients.Client(connectionId).newMoney(money.Money1);
        }
    }

    public void GetCharts(string connectionId)
    {
        const int hour = 4;
        using (var context = new Entities())
        {
            var teamsTrades = new Dictionary<string, TeamTrades>();
            DateTime currDate = new DateTime(2014, 6, 11, 7, 00, 00);
            TradeR currentTradeHisto = null;
            int lastPrice = 0;
            foreach (var trade in context.Trades.OrderBy(x => x.Team).ThenBy(x => x.Date))
            {
                TeamTrades teamTrade;
                lastPrice = trade.Price;
                if (!teamsTrades.TryGetValue(trade.Team, out teamTrade))
                {
                    currDate = new DateTime(2014, 6, 11, 7, 00, 00);
                    teamTrade = new TeamTrades { Team = trade.Team, Trades = new List<TradeR>() };
                    teamsTrades.Add(trade.Team, teamTrade);
                    while (currDate.AddHours(hour) < trade.Date)
                    {
                        currDate = currDate.AddHours(hour);
                    }
                    currentTradeHisto = new TradeR();
                    currentTradeHisto.Time = currDate;
                    currentTradeHisto.Open = trade.Price;
                    currentTradeHisto.Close = trade.Price;
                    currentTradeHisto.High = trade.Price;
                    currentTradeHisto.Low = trade.Price;
                    currentTradeHisto.Volume = trade.Quantity;
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
                            currentTradeHisto = new TradeR();
                            currentTradeHisto.Time = currDate;
                            currentTradeHisto.Open = lastPrice;
                            currentTradeHisto.Close = lastPrice;
                            currentTradeHisto.High = lastPrice;
                            currentTradeHisto.Low = lastPrice;
                            teamTrade.Trades.Add(currentTradeHisto);
                        }

                        currentTradeHisto = new TradeR();
                        currentTradeHisto.Time = currDate;
                        currentTradeHisto.Open = trade.Price;
                        currentTradeHisto.Close = trade.Price;
                        currentTradeHisto.High = trade.Price;
                        currentTradeHisto.Low = trade.Price;
                        currentTradeHisto.Volume = trade.Quantity;
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

    public void Price(string connectionId,
        int Brazil,
        int Croatia,
        int Mexico,
        int Cameroon,
        int Australia,
        int Chile,
        int Netherlands,
        int Spain,
        int Colombia,
        int Greece,
        int Ivory,
        int Japan,
        int Costa,
        int England,
        int Italy,
        int Uruguay,
        int Ecuador,
        int France,
        int Honduras,
        int Switzerland,
        int Argentina,
        int Bosnia,
        int Iran,
        int Nigeria,
        int Germany,
        int Ghana,
        int Portugal,
        int United,
        int Algeria,
        int Belgium,
        int Russia,
        int South)
    {
        var pricer = new Pricer();
        var pricingResult = pricer.Price(new List<int> {Brazil,
        Croatia,
        Mexico,
        Cameroon,
        Australia,
        Chile,
        Netherlands,
        Spain,
        Colombia,
        Greece,
        Ivory,
        Japan,
        Costa,
        England,
        Italy,
        Uruguay,
        Ecuador,
        France,
        Honduras,
        Switzerland,
        Argentina,
        Bosnia,
        Iran,
        Nigeria,
        Germany,
        Ghana,
        Portugal,
        United,
        Algeria,
        Belgium,
        Russia,
        South});

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

    private static List<string> chat = new List<string>();

    public void SendMessage(string user, string message)
    {
        if (string.IsNullOrEmpty(user))
            return;

        if (chat.Count() > 9)
        {
            chat.RemoveAt(0);
        }

        var messages = new List<string>();
        var nbMessage = Math.Min(9, chat.Count);
        for (int i = 0; i < nbMessage; i++)
        {
            messages.Add(chat[i]);
        }
        messages.Add(user + " - " + message);

        chat = messages;
        GetMessages();
    }

    public void GetMessages()
    {
        Clients.All.chat(chat);
    }
}