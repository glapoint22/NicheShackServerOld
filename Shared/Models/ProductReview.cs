using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class ProductReview
    {
        public int Id { get; set; }
        [ForeignKey("Product")]
        [Required]
        public int ProductId { get; set; }
        [ForeignKey("Customer")]
        [MaxLength(10)]
        [Required]
        public string CustomerId { get; set; }
        [Required]
        [MaxLength(256)]
        public string Title { get; set; }
        public double Rating { get; set; }
        public DateTime Date { get; set; }
        public bool IsVerified { get; set; }
        [Required]
        public string Text { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public virtual Product Product { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
