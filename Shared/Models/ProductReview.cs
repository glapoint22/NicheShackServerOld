using System;
using System.Collections.Generic;
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
        [Required]
        public string CustomerId { get; set; }
        [Required]
        [MaxLength(256)]
        public string Title { get; set; }
        public double Rating { get; set; }
        public DateTime Date { get; set; }
        [Required]
        public string Text { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public bool Deleted { get; set; }

        public virtual Product Product { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual ICollection<NotificationText> NotificationText { get; set; }


        public ProductReview()
        {
            NotificationText = new HashSet<NotificationText>();
        }
    }
}
