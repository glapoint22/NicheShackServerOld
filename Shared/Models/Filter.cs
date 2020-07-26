using System.Collections.Generic;

namespace DataAccess.Models
{
    public class Filter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<FilterOption> FilterOptions { get; set; }


        public Filter()
        {
            FilterOptions = new HashSet<FilterOption>();
        }
    }
}