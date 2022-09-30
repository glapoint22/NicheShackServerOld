using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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


        [MaxLength(50)]
        public string Image { get; set; }



        [Required]
        [MaxLength(100)]
        public string ReviewName { get; set; }

        public bool? EmailPrefNameChange { get; set; }
        public bool? EmailPrefEmailChange { get; set; }
        public bool? EmailPrefPasswordChange { get; set; }
        public bool? EmailPrefProfilePicChange { get; set; }
        public bool? EmailPrefNewCollaborator { get; set; }
        public bool? EmailPrefRemovedCollaborator { get; set; }
        public bool? EmailPrefRemovedListItem { get; set; }
        public bool? EmailPrefMovedListItem { get; set; }
        public bool? EmailPrefAddedListItem { get; set; }
        public bool? EmailPrefListNameChange { get; set; }
        public bool? EmailPrefDeletedList { get; set; }
        public bool? EmailPrefReview { get; set; }

        public int NoncompliantStrikes { get; set; }
        public bool BlockNotificationSending { get; set; }

        public bool? Active { get; set; }




        public virtual ICollection<OneTimePassword> OneTimePassword { get; set; }
        public virtual ICollection<ListCollaborator> ListCollaborators { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
        public virtual ICollection<ProductOrder> ProductOrders { get; set; }
        public virtual ICollection<ProductReview> ProductReviews { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<NotificationEmployeeNote> NotificationEmployeeNotes { get; set; }


        public Customer()
        {
            OneTimePassword = new HashSet<OneTimePassword>();
            ListCollaborators = new HashSet<ListCollaborator>();
            RefreshTokens = new HashSet<RefreshToken>();
            ProductOrders = new HashSet<ProductOrder>();
            ProductReviews = new HashSet<ProductReview>();
            Notifications = new HashSet<Notification>();
            NotificationEmployeeNotes = new HashSet<NotificationEmployeeNote>();
        }
    }
}
