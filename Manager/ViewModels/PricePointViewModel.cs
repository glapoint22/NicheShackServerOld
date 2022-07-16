using Services.Classes;
using System.Collections.Generic;

namespace Manager.ViewModels
{
    public struct PricePointViewModel
    {
        public int Id { get; set; }
        public Image Image { get; set; }
        public string Header { get; set; }
        public string Quantity { get; set; }
        public string UnitPrice { get; set; }
        public string Unit { get; set; }
        public string StrikethroughPrice { get; set; }
        public string Price { get; set; }
        public int ShippingType { get; set; }
        public RecurringPayment RecurringPayment { get; set; }

    }
}
