using DataAccess.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    public class Page : IItem
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(256)]
        public string Name { get; set; }


        [Required]
        [MaxLength(10)]
        public string UrlId { get; set; }
        [Required]
        [MaxLength(256)]
        public string UrlName { get; set; }


        [Required]
        public string Content { get; set; }
    }
}
