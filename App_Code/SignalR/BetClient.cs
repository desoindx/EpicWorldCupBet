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

    public void GetTeam()
    {
        var random = new Random();
        var orders = new List<OrderR>();

        for (int i = 0; i < 32; i++)
        {
            var order = new OrderR();
            order.Id = random.Next(32);
            order.Team = "Team" + order.Id;
            order.Quantity = random.Next(50);
            order.User = "Moi";
            order.Price = random.Next(100);
            orders.Add(order);
        }

        Clients.All.newOrders(orders);
    }
}