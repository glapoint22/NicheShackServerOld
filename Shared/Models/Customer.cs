using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DataAccess.Models
{
    public class Customer : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ReviewName { get; set; }
        public string image { get; set; }
        public virtual ICollection<ListCollaborator> ListCollaborators { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
        public virtual ICollection<ProductOrder> ProductOrders { get; set; }
        public virtual ICollection<ProductReview> ProductReviews { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }


        public Customer()
        {
            ListCollaborators = new HashSet<ListCollaborator>();
            RefreshTokens = new HashSet<RefreshToken>();
            ProductOrders = new HashSet<ProductOrder>();
            ProductReviews = new HashSet<ProductReview>();
            Notifications = new HashSet<Notification>();
        }
    }
}
