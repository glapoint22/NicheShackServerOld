using DataAccess.Interfaces;

namespace DataAccess.Models
{
    public class ProductAdditionalInfo : IAdditionalInfo
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public bool IsRecurring { get; set; }
        public int ShippingType { get; set; }
        public int TrialPeriod { get; set; }
        public double Price { get; set; }
        public int RebillFrequency { get; set; }
        public int? TimeFrameBetweenRebill { get; set; }
        public int SubscriptionDuration { get; set; }
        public virtual Product Product { get; set; }
    }
}
