using DataAccess.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class Category: IItem, IUrlItem
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string UrlId { get; set; }


        [Required]
        [MaxLength(256)]
        public string UrlName { get; set; }


       


        [Required]
        [MaxLength(100)]
        public string Name { get; set; }


        public virtual ICollection<Niche> Niches { get; set; }


        public Category()
        {
            Niches = new HashSet<Niche>();
        }
    }
}
