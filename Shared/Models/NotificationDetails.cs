using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class NotificationDetails
    {
        public int Id { get; set; }
        [ForeignKey("Customer")]
        public string CustomerId { get; set; }
        [ForeignKey("Notification")]
        public int NotificationId { get; set; }
        [ForeignKey("NotificationEmployeeNotes")]
        public int? NotificationEmployeeNoteId { get; set; }
        [ForeignKey("ProductReview")]
        public int? ReviewId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Text { get; set; }
        [MaxLength(256)]
        public string Name { get; set; }
        [MaxLength(256)]
        public string Email { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Notification Notification { get; set; }
        public virtual ProductReview ProductReview { get; set; }
        public virtual NotificationEmployeeNote NotificationEmployeeNotes { get; set; }
    }
}