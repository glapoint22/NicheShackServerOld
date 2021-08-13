using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class ProductPrice
    {
        [ForeignKey("Product")]
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int Id { get; set; }
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
        public double Price { get; set; }
        public virtual ICollection<ProductPriceAdditionalInfo> ProductPriceAdditionalInfo { get; set; }
        public virtual Product Product { get; set; }
        public virtual Media Media { get; set; }




        public ProductPrice()
        {
            ProductPriceAdditionalInfo = new HashSet<ProductPriceAdditionalInfo>();
        }
    }
}
