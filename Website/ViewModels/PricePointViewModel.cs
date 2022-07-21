using Services.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.ViewModels
{
    public class PricePointViewModel
    {
        public ImageViewModel Image { get; set; }
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
