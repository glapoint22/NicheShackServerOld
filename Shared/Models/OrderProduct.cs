using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class OrderProduct
    {
        public int Id { get; set; }
        [ForeignKey("ProductOrder")]
        [MaxLength(21)]
        [Required]
        public string OrderId { get; set; }
        [Required]
        [MaxLength(256)]
        public string Name { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        [MaxLength(8)]
        public string LineItemType { get; set; }
        public string RebillFrequency { get; set; }
        public double RebillAmount { get; set; }
        public virtual ProductOrder ProductOrder { get; set; }
    }
}