using DataAccess.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    public class Media: IItem
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(256)]
        public string Name { get; set; }
        [Required]
        [MaxLength(256)]
        public string Image { get; set; }
        [MaxLength(256)]
        public string Video { get; set; }
        [Required]
        public int Type { get; set; }


        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<ProductMedia> ProductMedia { get; set; }
        public virtual ICollection<ProductPrice> ProductPrices { get; set; }
        public virtual ICollection<Category> Categtories { get; set; }
        public virtual ICollection<Niche> Niches { get; set; }
        public virtual ICollection<Subproduct> Subproducts { get; set; }


        public Media()
        {
            Products = new HashSet<Product>();
            ProductMedia = new HashSet<ProductMedia>();
            ProductPrices = new HashSet<ProductPrice>();
            Categtories = new HashSet<Category>();
            Niches = new HashSet<Niche>();
            Subproducts = new HashSet<Subproduct>();
        }
    }
}