using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class Niche
    {
        public int Id { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<LeadPage> LeadPages { get; set; }

        public Niche()
        {
            Products = new HashSet<Product>();
            LeadPages = new HashSet<LeadPage>();
        }
    }
}