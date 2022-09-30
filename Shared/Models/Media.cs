using DataAccess.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    public class Media : IItem
    {
        public int Id { get; set; }

        [MaxLength(256)]
        public string Name { get; set; }

        [MaxLength(256)]
        public string Thumbnail { get; set; }
        public int ThumbnailWidth { get; set; }

        public int ThumbnailHeight { get; set; }


        [MaxLength(256)]
        public string ImageSm { get; set; }

        public int ImageSmWidth { get; set; }

        public int ImageSmHeight { get; set; }





        [MaxLength(256)]
        public string ImageMd { get; set; }

        public int ImageMdWidth { get; set; }

        public int ImageMdHeight { get; set; }


        [MaxLength(256)]
        public string ImageLg { get; set; }


        public int ImageLgWidth { get; set; }

        public int ImageLgHeight { get; set; }


        [MaxLength(256)]
        public string ImageAnySize { get; set; }

        public int ImageAnySizeWidth { get; set; }

        public int ImageAnySizeHeight { get; set; }




        [MaxLength(256)]
        public string VideoId { get; set; }


        [Required]
        public int MediaType { get; set; }



        public int VideoType { get; set; }






        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<ProductMedia> ProductMedia { get; set; }
        public virtual ICollection<PricePoint> ProductPrices { get; set; }
        public virtual ICollection<Subproduct> Subproducts { get; set; }


        public Media()
        {
            Products = new HashSet<Product>();
            ProductMedia = new HashSet<ProductMedia>();
            ProductPrices = new HashSet<PricePoint>();
            Subproducts = new HashSet<Subproduct>();
        }
    }
}