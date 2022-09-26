using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class NotificationEmployeeNote
    {
        public int Id { get; set; }

        [ForeignKey("NotificationGroup")]
        public int NotificationGroupId { get; set; }

        public int? NotificationId { get; set; }

        [ForeignKey("Customer")]
        [Required]
        public string EmployeeId { get; set; }
       
        public string Note { get; set; }

        public DateTime CreationDate { get; set; }




        public virtual NotificationGroup NotificationGroup { get; set; }
        public virtual Customer Customer { get; set; }
    }
}