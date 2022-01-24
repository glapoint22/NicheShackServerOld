namespace Website.ViewModels
{
    public struct OrderProductInfoViewModel
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public ImageViewModel Image { get; set; }
        public string RebillFrequency { get; set; }
        public double RebillAmount { get; set; }
        public int PaymentsRemaining { get; set; }
        public string UrlId { get; set; }
        public string UrlName { get; set; }
    }
}
