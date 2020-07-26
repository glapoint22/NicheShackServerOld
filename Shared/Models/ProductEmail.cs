using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class ProductEmail
    {
        public int Id { get; set; }
        [ForeignKey("Product")]
        [MaxLength(10)]
        public string ProductId { get; set; }
        public string Content { get; set; }
        public string Subject { get; set; }
        public virtual Product Product { get; set; }
    }
}