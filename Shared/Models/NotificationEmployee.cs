using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class NotificationEmployee
    {
        public int Id { get; set; }
        [ForeignKey("Customer")]
        [Required]
        public string EmployeeId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Note { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<NotificationDetails> NotificationDetails { get; set; }


        public NotificationEmployee()
        {
            NotificationDetails = new HashSet<NotificationDetails>();
        }
    }
}
