using DataAccess.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class Niche: IItem, IUrlItem
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(10)]
        public string UrlId { get; set; }
        [Required]
        [MaxLength(256)]
        public string UrlName { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        [ForeignKey("Media")]
        public int? ImageId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public virtual Category Category { get; set; }
        public virtual Media Media { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<LeadPage> LeadPages { get; set; }
        public virtual ICollection<PageReferenceItem> PageReferenceItems { get; set; }

        public Niche()
        {
            Products = new HashSet<Product>();
            LeadPages = new HashSet<LeadPage>();
            PageReferenceItems = new HashSet<PageReferenceItem>();
        }
    }
}