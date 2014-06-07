using DataAccesLayer;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.StockTicker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class BetClient
{
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

            var pos = new List<int>();
            for (int i = 0; i < users.Count; i++)
            {
                var usr = users[i];
                for (int j = 0; j < Teams.Count; j++)
                {
                    var tmpTeam = temp[Teams[j]];
                    int value;
                    if (tmpTeam.TryGetValue(usr, out value))
                        pos.Add(value);
                    else
                        pos.Add(0);
                }
            }

            Clients.Client(connectionId).newPositions(Teams, users, pos);
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

                orders.Add(order);
            }
        }
        Clients.All.newOrders(orders);
    }

    public void CancelOrder(string user, string side, string team, string connectionId)
    {
        if (!(string.IsNullOrEmpty(user) && Teams.Contains(team)))
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
        if (!(!string.IsNullOrEmpty(user) && Teams.Contains(team) && quantity > 0 && price > 0 && price < 100 && (side == "BUY" || side == "SELL")))
        {
            Clients.Client(connectionId).newMessage("Fail To add order");
            return;
        }

        lock (this)
        {
            using (var context = new Entities())
            {
                var remainingQuantity = InsertTrade(context, user, team, quantity, price, side, connectionId);
                if (remainingQuantity > 0)
                    InsertNewOrder(context, user, team, remainingQuantity, price, side, connectionId);

                context.SaveChanges();
            }
        }

        GetTeam(user, connectionId);
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
        context.SaveChanges();
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
    }
}