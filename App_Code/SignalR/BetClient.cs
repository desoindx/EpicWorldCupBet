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

    public void GetTeam(string user)
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
                    order.BestAsk = bestAsk.Price;
                var bestBid = context.Orders.Where(x => x.Team == team && x.Status == 0 && x.Side == "BUY").OrderByDescending(x => x.Price).FirstOrDefault();
                if (bestBid != null)
                    order.BestBid = bestBid.Price;
                var myAsk = context.Orders.Where(x => x.Team == team && x.Status == 0 && x.Side == "SELL" && x.User == user).OrderBy(x => x.Price).FirstOrDefault();
                if (myAsk != null)
                    order.MyAsk = myAsk.Price;
                var myBid = context.Orders.Where(x => x.Team == team && x.Status == 0 && x.Side == "BUY" && x.User == user).OrderByDescending(x => x.Price).FirstOrDefault();
                if (myBid != null)
                    order.MyBid = myBid.Price;

                orders.Add(order);
            }
        }
        Clients.All.newOrders(orders);
    }

    public void NewOrder(string user, string team, int quantity, int price, string side, string connectionId)
    {
        if (!(string.IsNullOrEmpty(user) && Teams.Contains(team) && quantity > 0 && price > 0 && price < 100 && (side == "BUY" || side == "SELL")))
        {
            Clients.Client(connectionId).newMessage("Fail To add order");
            return; 
        }

        lock (this)
        {
            using (var context = new Entities())
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
                context.SaveChanges();
            }
        }

        Clients.Client(connectionId).newMessage("Successfuly added order");
        GetTeam(user);
    }
}