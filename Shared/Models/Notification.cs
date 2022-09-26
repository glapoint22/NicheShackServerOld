using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class Notification
    {
        public int Id { get; set; }




        [ForeignKey("NotificationGroup")]
        public int NotificationGroupId { get; set; }


        public int Type { get; set; }





        [ForeignKey("Product")]
        public int? ProductId { get; set; }



        [ForeignKey("Customer")]
        public string CustomerId { get; set; }



        public string UserComment { get; set; }




        [MaxLength(256)]
        public string NonAccountUserName { get; set; }




        [MaxLength(256)]
        public string NonAccountUserEmail { get; set; }




        public bool MessageArchived { get; set; }


        [ForeignKey("Media")]
        public int? UserImageId { get; set; }


        [ForeignKey("NotificationEmployeeMessage")]
        public int? EmployeeMessageId { get; set; }



        [ForeignKey("ProductReview")]
        public int? ReviewId { get; set; }



        public DateTime CreationDate { get; set; }










        public virtual NotificationGroup NotificationGroup { get; set; }
        public virtual Product Product { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Media Media { get; set; }
        public virtual NotificationEmployeeMessage NotificationEmployeeMessage { get; set; }
        public virtual ProductReview ProductReview { get; set; }
    }
}