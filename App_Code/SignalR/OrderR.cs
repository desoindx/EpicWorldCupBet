using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR.Hubs;

namespace Microsoft.AspNet.SignalR.StockTicker
{
    public class OrderR
    {
        public string Team { get; set; }
        public int BestBid { get; set; }
        public int BestAsk { get; set; }
        public int MyBid { get; set; }
        public int MyAsk { get; set; }
    }
}