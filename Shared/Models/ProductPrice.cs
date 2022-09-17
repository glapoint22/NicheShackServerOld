using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class ProductPrice
    {
        public int Id { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public double Price { get; set; }


        public virtual Product Product { get; set; }

        public virtual ICollection<PricePoint> PricePoints { get; set; }


        public ProductPrice()
        {
            PricePoints = new HashSet<PricePoint>();
        }
    }
}
