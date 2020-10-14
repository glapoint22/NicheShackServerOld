using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    public class Customer : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }
        [Required]
        [MaxLength(100)]
        public string ReviewName { get; set; }
        [MaxLength(50)]
        public string Image { get; set; }
        public virtual ICollection<ListCollaborator> ListCollaborators { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
        public virtual ICollection<ProductOrder> ProductOrders { get; set; }
        public virtual ICollection<ProductReview> ProductReviews { get; set; }
        public virtual ICollection<NotificationText> NotificationText { get; set; }


        public Customer()
        {
            ListCollaborators = new HashSet<ListCollaborator>();
            RefreshTokens = new HashSet<RefreshToken>();
            ProductOrders = new HashSet<ProductOrder>();
            ProductReviews = new HashSet<ProductReview>();
            NotificationText = new HashSet<NotificationText>();
        }
    }
}
