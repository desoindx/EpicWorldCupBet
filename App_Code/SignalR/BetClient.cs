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
        "Cameroon",
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
    public void GetTeam()
    {
        var random = new Random();
        var orders = new List<OrderR>();

        foreach(var team in Teams)
        {
            var order = new OrderR();
            order.Id = random.Next(32);
            order.Team = team;
            order.Quantity = random.Next(50);
            order.User = "Moi";
            order.Price = random.Next(100);
            orders.Add(order);
        }

        Clients.All.newOrders(orders);
    }
}