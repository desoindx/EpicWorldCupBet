﻿namespace SignalR
{
    public class OrderR
    {
        public string Team { get; set; }
        public int BestBid { get; set; }
        public int BestAsk { get; set; }
        public int MyBid { get; set; }
        public int MyAsk { get; set; }
        public int BestBidQuantity { get; set; }
        public int BestAskQuantity { get; set; }
        public int MyBidQuantity { get; set; }
        public int MyAskQuantity { get; set; }
        public int LastTradedPrice { get; set; }
        public int LastTradedPriceEvolution { get; set; }
    }
}