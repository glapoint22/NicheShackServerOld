using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class ProductEmail
    {
        public int Id { get; set; }
        [ForeignKey("Product")]
        [Required]
        public int ProductId { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        [MaxLength(256)]
        public string Name { get; set; }
        public virtual Product Product { get; set; }
    }
}