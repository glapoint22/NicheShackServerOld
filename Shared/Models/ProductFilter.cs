using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class ProductFilter
    {
        [ForeignKey("Product")]
        [MaxLength(10)]
        public string ProductId { get; set; }
        [ForeignKey("FilterOption")]
        public int FilterOptionId { get; set; }
        public virtual FilterOption FilterOption { get; set; }
        public virtual Product Product { get; set; }
    }
}
