using DataAccess.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    public class Subgroup : IItem
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public virtual ICollection<SubgroupProduct> SubgroupProducts { get; set; }

        public Subgroup()
        {
            SubgroupProducts = new HashSet<SubgroupProduct>();
        }
    }
}