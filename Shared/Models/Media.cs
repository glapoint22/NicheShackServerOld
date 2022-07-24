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

        [MaxLength(256)]
        public string Thumbnail { get; set; }

        [MaxLength(256)]
        public string ImageAnySize { get; set; }


        [MaxLength(256)]
        public string Image200x200 { get; set; }



        [MaxLength(256)]
        public string Image500x500 { get; set; }



        [MaxLength(256)]
        public string VideoId { get; set; }


        [Required]
        public int MediaType { get; set; }
        


        public int VideoType { get; set; }


        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<ProductMedia> ProductMedia { get; set; }
        public virtual ICollection<PricePoint> ProductPrices { get; set; }
        public virtual ICollection<Category> Categtories { get; set; }
        public virtual ICollection<Niche> Niches { get; set; }
        public virtual ICollection<Subproduct> Subproducts { get; set; }


        public Media()
        {
            Products = new HashSet<Product>();
            ProductMedia = new HashSet<ProductMedia>();
            ProductPrices = new HashSet<PricePoint>();
            Categtories = new HashSet<Category>();
            Niches = new HashSet<Niche>();
            Subproducts = new HashSet<Subproduct>();
        }
    }
}