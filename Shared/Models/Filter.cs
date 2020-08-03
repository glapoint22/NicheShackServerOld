using DataAccess.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    public class Filter : IItem
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public virtual ICollection<FilterOption> FilterOptions { get; set; }


        public Filter()
        {
            FilterOptions = new HashSet<FilterOption>();
        }
    }
}