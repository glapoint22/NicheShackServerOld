using DataAccess.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    public class Email: IItem
    { 
        public int Id { get; set; }
        public int Type { get; set; }
        [Required]
        [MaxLength(256)]
        public string Name { get; set; }
        [Required]
        public string Content { get; set; }
    }
}
