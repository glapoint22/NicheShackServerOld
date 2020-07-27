using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class ProductContent
    {
        public int Id { get; set; }
        [ForeignKey("Product")]
        [Required]
        public int ProductId { get; set; }
        [ForeignKey("Media")]
        public int? IconId { get; set; }
        [Required]
        [MaxLength(256)]
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
