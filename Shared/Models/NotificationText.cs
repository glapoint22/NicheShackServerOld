using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class NotificationText
    {
        [ForeignKey("Customer")]
        [Required]
        public string CustomerId { get; set; }
        [ForeignKey("Notification")]
        public int NotificationId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Text { get; set; }
        public int Type { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Notification Notification { get; set; }
    }
}
