using System.Collections.Generic;

namespace DataAccess.Models
{
    public class Media
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Thumbnail { get; set; }
        public int Type { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<ProductMedia> ProductMedia { get; set; }
        public virtual ICollection<ProductContent> ProductContent { get; set; }
        public virtual ICollection<Category> Categtories { get; set; }


        public Media()
        {
            Products = new HashSet<Product>();
            ProductMedia = new HashSet<ProductMedia>();
            ProductContent = new HashSet<ProductContent>();
            Categtories = new HashSet<Category>();
        }
    }
}