using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR.Hubs;

namespace Microsoft.AspNet.SignalR.StockTicker
{
    public class TeamTrades
    {
        public string Team { get; set; }
        public int TotalVolume { get; set; }
        public List<TradeR> Trades { get; set; }
    }

    public class TradeR
    {
        public DateTime Time { get; set; }
        public int Open { get; set; }
        public int Close { get; set; }
        public int High { get; set; }
        public int Low { get; set; }
        public int Volume { get; set; }
    }
}