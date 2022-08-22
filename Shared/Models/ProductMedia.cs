using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class ProductMedia
    {
        public int Id { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        [ForeignKey("Media")]
        public int? MediaId { get; set; }
        public virtual Product Product { get; set; }
        public int Index { get; set; }

        public virtual Media Media { get; set; }
    }
}