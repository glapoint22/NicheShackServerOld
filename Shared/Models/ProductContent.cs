using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class ProductContent
    {
        [MaxLength(10)]
        public string Id { get; set; }
        [ForeignKey("Product")]
        [MaxLength(10)]
        public string ProductId { get; set; }
        [ForeignKey("Media")]
        public int IconId { get; set; }
        public string Name { get; set; }
        public virtual Product Product { get; set; }
        public virtual Media Media { get; set; }
        public virtual ICollection<PriceIndex> PriceIndices { get; set; }


        public ProductContent()
        {
            PriceIndices = new HashSet<PriceIndex>();
        }
    }
}
