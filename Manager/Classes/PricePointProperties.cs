using Services.Classes;

namespace Manager.Classes
{
    public class PricePointProperties
    {
        public int ProductId { get; set; }
        public int Id { get; set; }
        public int? ImageId { get; set; }
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
