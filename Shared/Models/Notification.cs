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





        [ForeignKey("Customer")]
        public string UserId { get; set; }




        [ForeignKey("Product")]
        public int? ProductId { get; set; }




        [ForeignKey("ProductReview")]
        public int? ReviewId { get; set; }





        public int Type { get; set; }






        [MaxLength(200)]
        public string UserName { get; set; }






        [MaxLength(50)]
        public string UserImage { get; set; }



        public string UserComment { get; set; }




        [MaxLength(256)]
        public string NonAccountUserName { get; set; }




        [MaxLength(256)]
        public string NonAccountUserEmail { get; set; }




        public bool IsArchived { get; set; }

        public DateTime CreationDate { get; set; }










        public virtual NotificationGroup NotificationGroup { get; set; }
        public virtual Product Product { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual ProductReview ProductReview { get; set; }
    }
}