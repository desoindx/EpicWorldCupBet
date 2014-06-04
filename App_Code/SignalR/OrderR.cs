using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR.Hubs;

namespace Microsoft.AspNet.SignalR.StockTicker
{
    public class OrderR
    {
        public int Id { get; set; }
        public int Price { get; set; }
        public string User { get; set; }
        public byte[] Date { get; set; }
        public string Team { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; }
    }
}