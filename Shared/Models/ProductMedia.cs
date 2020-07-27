using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class ProductMedia
    {
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        [ForeignKey("Media")]
        public int? MediaId { get; set; }
        public virtual Product Product { get; set; }
        public virtual Media Media { get; set; }
    }
}