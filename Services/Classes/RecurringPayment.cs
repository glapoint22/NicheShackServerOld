namespace Services.Classes
{
    public class RecurringPayment
    {
        public int TrialPeriod { get; set; }
        public double RecurringPrice { get; set; }
        public int RebillFrequency { get; set; }
        public int TimeFrameBetweenRebill { get; set; }
        public int SubscriptionDuration { get; set; }
    }
}
