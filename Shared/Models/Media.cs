using DataAccess.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    public class Media : IItem
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; }

        [MaxLength(256)]
        public string Thumbnail { get; set; }


        [MaxLength(256)]
        public string ImageSm { get; set; }

        public double ImageSmWidth { get; set; }

        public double ImageSmHeight { get; set; }





        [MaxLength(256)]
        public string ImageMd { get; set; }

        public double ImageMdWidth { get; set; }

        public double ImageMdHeight { get; set; }


        [MaxLength(256)]
        public string ImageLg { get; set; }


        public double ImageLgWidth { get; set; }

        public double ImageLgHeight { get; set; }


        [MaxLength(256)]
        public string ImageAnySize { get; set; }

        public double ImageAnySizeWidth { get; set; }

        public double ImageAnySizeHeight { get; set; }




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
        public virtual ICollection<ImageReference> ImageReferences { get; set; }


        public Media()
        {
            Products = new HashSet<Product>();
            ProductMedia = new HashSet<ProductMedia>();
            ProductPrices = new HashSet<PricePoint>();
            Categtories = new HashSet<Category>();
            Niches = new HashSet<Niche>();
            Subproducts = new HashSet<Subproduct>();
            ImageReferences = new HashSet<ImageReference>();
        }
    }
}