using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR.Hubs;

namespace Microsoft.AspNet.SignalR.StockTicker
{
    public class TeamOrder
    {
        public string Team { get; set; }
        public List<HistoR> Bids { get; set; }
        public List<HistoR> Asks { get; set; }
    }

    public class HistoR
    {
        public DateTime Time { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
    }
}