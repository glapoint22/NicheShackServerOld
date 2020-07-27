using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class FilterOption
    {
        public int Id { get; set; }
        [ForeignKey("Filter")]
        public int FilterId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public virtual Filter Filter { get; set; }
        public virtual ICollection<ProductFilter> ProductFilters { get; set; }

        public FilterOption()
        {
            ProductFilters = new HashSet<ProductFilter>();
        }
    }
}