using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class PricePoint
    {
        public int Id { get; set; }

        [ForeignKey("Product")]
        [Required]
        public int ProductId { get; set; }
        
        
        [ForeignKey("Media")]
        public int? ImageId { get; set; }


        [MaxLength(50)]
        public string Header { get; set; }


        [MaxLength(50)]
        public string Quantity { get; set; }

        public string UnitPrice { get; set; }


        [MaxLength(25)]
        public string Unit { get; set; }


        public string StrikethroughPrice { get; set; }
        //public double Price { get; set; }
        public int ShippingType { get; set; }
        public int TrialPeriod { get; set; }
        public double RecurringPrice { get; set; }
        public int RebillFrequency { get; set; }
        public int TimeFrameBetweenRebill { get; set; }
        public int SubscriptionDuration { get; set; }



        public virtual Product Product { get; set; }
        public virtual Media Media { get; set; }
    }
}
