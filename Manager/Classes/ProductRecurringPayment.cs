using Services.Classes;

namespace Manager.Classes
{
    public class ProductRecurringPayment
    {
        public int Id { get; set; }
        public RecurringPayment RecurringPayment { get; set; }
    }
}
