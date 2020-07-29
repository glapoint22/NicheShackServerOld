using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class Product
    {
        public int Id { get; set; }
        [ForeignKey("Vendor")]
        public int VendorId { get; set; }
        [ForeignKey("Media")]
        public int? ImageId { get; set; }
        [ForeignKey("Niche")]
        public int NicheId { get; set; }
        [MaxLength(10)]
        [Required]
        public string UrlId { get; set; }
        [Required]
        [MaxLength(256)]
        public string UrlName { get; set; }
        [Required]
        [MaxLength(256)]
        public string Name { get; set; }
        [MaxLength(256)]
        public string Hoplink { get; set; }
        public string Description { get; set; }
        [Required]
        public double MinPrice { get; set; }
        [Required]
        public double MaxPrice { get; set; }
        public int TotalReviews { get; set; }
        public double Rating { get; set; }
        public int OneStar { get; set; }
        public int TwoStars { get; set; }
        public int ThreeStars { get; set; }
        public int FourStars { get; set; }
        public int FiveStars { get; set; }
        public virtual Niche Niche { get; set; }
        public virtual Media Media { get; set; }
        public virtual  Vendor Vendor { get; set; }
        public virtual ICollection<ProductFilter> ProductFilters { get; set; }
        public virtual ICollection<ProductMedia> ProductMedia { get; set; }
        public virtual ICollection<ProductContent> ProductContent { get; set; }
        public virtual ICollection<ProductPricePoint> ProductPricePoints { get; set; }
        public virtual ICollection<ProductReview> ProductReviews { get; set; }
        public virtual ICollection<ListProduct> ListProducts { get; set; }
        public virtual ICollection<ProductOrder> ProductOrders { get; set; }
        public virtual ICollection<ProductKeyword> Keywords { get; set; }
        public virtual ICollection<ProductEmail> ProductEmails { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }



        public Product()
        {
            ProductFilters = new HashSet<ProductFilter>();
            ProductMedia = new HashSet<ProductMedia>();
            ProductContent = new HashSet<ProductContent>();
            ProductPricePoints = new HashSet<ProductPricePoint>();
            ProductReviews = new HashSet<ProductReview>();
            ListProducts = new HashSet<ListProduct>();
            ProductOrders = new HashSet<ProductOrder>();
            Keywords = new HashSet<ProductKeyword>();
            ProductEmails = new HashSet<ProductEmail>();
            Notifications = new HashSet<Notification>();
        }
    }
}