using System;
using System.Collections.Generic;

namespace Website.Classes
{
    public struct OrderNotification
    {
        public string TransactionTime { get; set; }
        public string Receipt { get; set; }
        public string TransactionType { get; set; }
        public string Vendor { get; set; }
        public string Affiliate { get; set; }
        public string Role { get; set; }
        public double TotalAccountAmount { get; set; }
        public string PaymentMethod { get; set; }
        public double TotalOrderAmount { get; set; }
        public IEnumerable<string> TrackingCodes { get; set; }
        public IEnumerable<LineItem> LineItems { get; set; }
        public CustomerShipping Customer { get; set; }
        public Upsell Upsell { get; set; }
        public float Version { get; set; }
        public int AttemptCount { get; set; }
    }
}
