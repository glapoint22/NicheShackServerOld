using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Classes
{
    public class ProductPriceProperties
    {
        public int ProductId { get; set; }
        public int Id { get; set; }
        public int? ImageId { get; set; }
        public string Header { get; set; }
        public string Quantity { get; set; }
        public string UnitPrice { get; set; }
        public string Unit { get; set; }
        public string StrikethroughPrice { get; set; }
        public double Price { get; set; }
        public int Shipping { get; set; }
        public double? ShippingPrice { get; set; }
    }
}
