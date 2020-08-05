using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataAccess.Models
{
    public class Email
    { 
        public int Id { get; set; }
        public int Type { get; set; }
        [Required]
        [MaxLength(256)]
        public string Subject { get; set; }
        [Required]
        public string Content { get; set; }
    }
}
