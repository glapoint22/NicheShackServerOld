using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class Notification
    {
        public int Id { get; set; }
        [ForeignKey("Product")]
        public int? ProductId { get; set; }
        public int Type { get; set; }
        public DateTime? ArchiveDate { get; set; }
        public virtual Product Product { get; set; }
        public virtual ICollection<NotificationDetails> NotificationDetails { get; set; }


        public Notification()
        {
            NotificationDetails = new HashSet<NotificationDetails>();
        }

    }
}