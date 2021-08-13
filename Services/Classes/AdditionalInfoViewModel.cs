
namespace Services.Classes
{
    public class AdditionalInfoViewModel
    {
        public int Id { get; set; }
        public bool IsRecurring { get; set; }
        public int ShippingType { get; set; }
        public RecurringPayment RecurringPayment { get; set; }
    }
}
