using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class Notification
    {
        public int Id { get; set; }
        [ForeignKey("Product")]
        [MaxLength(10)]
        public string ProductId { get; set; }
        [ForeignKey("Customer")]
        [MaxLength(10)]
        public string CustomerId { get; set; }
        [ForeignKey("Customer")]
        [MaxLength(10)]
        public string EmployeeId { get; set; }
        public int Type { get; set; }
        public DateTime CustomerTimeStamp { get; set; }
        public string  CustomerText { get; set; }
        public DateTime EmployeeTimeStamp { get; set; }
        public string EmployeeText { get; set; }
        public int State { get; set; }
        public virtual Product Product { get; set; }
        public virtual Customer Customer { get; set; }
    }
}