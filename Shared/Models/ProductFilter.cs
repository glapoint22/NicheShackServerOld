using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class ProductFilter
    {
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        [ForeignKey("FilterOption")]
        public int FilterOptionId { get; set; }
        public virtual FilterOption FilterOption { get; set; }
        public virtual Product Product { get; set; }
    }
}
