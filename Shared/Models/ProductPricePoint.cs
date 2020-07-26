using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class ProductPricePoint
    {
        public int Id { get; set; }
        [ForeignKey("Product")]
        [MaxLength(10)]
        public string ProductId { get; set; }
        public string TextBefore { get; set; }
        public int WholeNumber { get; set; }
        public int Decimal { get; set; }
        public string TextAfter { get; set; }
        public virtual Product Product { get; set; }
    }
}
