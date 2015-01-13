using System;
using System.Collections.Generic;

namespace SignalR
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