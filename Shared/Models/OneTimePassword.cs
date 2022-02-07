using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataAccess.Models
{
    public class OneTimePassword
    {
        public int Id { get; set; }
        [Required]
        public string CustomerId { get; set; }
        [Required]
        [MaxLength(10)]
        public string Password { get; set; }
        public int Type { get; set; }
        [Required]
        public string Token { get; set; }
        public DateTime Expires { get; set; }



        public virtual Customer Customer { get; set; }
    }
}
