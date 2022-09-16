using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    public class NotificationEmployeeMessage
    {
        public int Id { get; set; }


        [ForeignKey("Customer")]
        [Required]
        public string EmployeeId { get; set; }

        public string Message { get; set; }

        public DateTime CreationDate { get; set; }





        //[ForeignKey("Notification")]
        //[Required]
        //public int NotificationId { get; set; }

        //public virtual Notification Notification { get; set; }











        public virtual Customer Customer { get; set; }
    }
}