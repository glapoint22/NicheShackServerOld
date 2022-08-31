using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class NotificationEmployeeDetails
    {
        public int Id { get; set; }
        [ForeignKey("Customer")]
        [Required]
        public string EmployeeId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Text { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<NotificationDetails> NotificationDetails { get; set; }


        public NotificationEmployeeDetails()
        {
            NotificationDetails = new HashSet<NotificationDetails>();
        }
    }
}
