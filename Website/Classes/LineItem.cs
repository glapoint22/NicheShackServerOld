namespace Website.Classes
{
    public struct LineItem
    {
        public string ItemNo { get; set; }
        public string ProductTitle { get; set; }
        public bool Shippable { get; set; }
        public bool Recurring { get; set; }
        public double AccountAmount { get; set; }
        public int Quantity { get; set; }
        public PaymentPlan PaymentPlan { get; set; }
        public string LineItemType { get; set; }
        public double ProductPrice { get; set; }
        public double ProductDiscount { get; set; }
        public double TaxAmount { get; set; }
        public double ShippingAmount { get; set; }
        public bool ShippingLiable { get; set; }
    }
}