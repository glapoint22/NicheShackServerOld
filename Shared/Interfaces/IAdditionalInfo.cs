namespace DataAccess.Interfaces
{
    public interface IAdditionalInfo
    {
        public int Id { get; set; }
        public bool IsRecurring { get; set; }
        public int ShippingType { get; set; }
        public int TrialPeriod { get; set; }
        public double Price { get; set; }
        public int RebillFrequency { get; set; }
        public int? TimeFrameBetweenRebill { get; set; }
        public int SubscriptionDuration { get; set; }
    }
}
