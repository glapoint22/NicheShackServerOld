using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class ProductPricePoint
    {
        public int Id { get; set; }
        [ForeignKey("Product")]
        [Required]
        public int ProductId { get; set; }
        [MaxLength(50)]
        public string TextBefore { get; set; }
        public int WholeNumber { get; set; }
        public int Decimal { get; set; }
        [MaxLength(50)]
        public string TextAfter { get; set; }
        [Required]
        public int Index { get; set; }
        public virtual Product Product { get; set; }
    }
}
