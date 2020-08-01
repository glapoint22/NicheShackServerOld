using DataAccess.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class Category: IItem
    {
        public int Id { get; set; }
        [ForeignKey("Media")]
        public int? ImageId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public virtual Media Media { get; set; }
        public virtual ICollection<Niche> Niches { get; set; }


        public Category()
        {
            Niches = new HashSet<Niche>();
        }
    }
}
