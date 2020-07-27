using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class OrderProduct
    {
        [MaxLength(25)]
        public string Id { get; set; } // This will be the itemNo from the instant notification
        [ForeignKey("ProductOrder")]
        [MaxLength(21)]
        public string OrderId { get; set; }
        [Required]
        [MaxLength(256)]
        public string Name { get; set; }
        public int Type { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public bool IsMain { get; set; } // lineItemType from the instant notification will determine if this field is true or false. If the value is "ORIGINAL", then it will be true
        public virtual ProductOrder ProductOrder { get; set; }
    }
}