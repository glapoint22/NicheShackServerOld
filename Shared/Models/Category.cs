using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class Category
    {
        public int Id { get; set; }
        [ForeignKey("Media")]
        public int ImageId { get; set; }
        public string Name { get; set; }
        public virtual Media Media { get; set; }
        public virtual ICollection<Niche> Niches { get; set; }


        public Category()
        {
            Niches = new HashSet<Niche>();
        }
    }
}
