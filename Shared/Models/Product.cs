using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class Product
    {
        [MaxLength(10)]
        public string Id { get; set; }
        [ForeignKey("Vendor")]
        public int VendorId { get; set; }
        [ForeignKey("Media")]
        public int ImageId { get; set; }
        [ForeignKey("Niche")]
        public int NicheId { get; set; }
        public string Name { get; set; }
        public string UrlName { get; set; }
        public string Hoplink { get; set; }
        public int TotalReviews { get; set; }
        public double Rating { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public double OneStar { get; set; }
        public double TwoStars { get; set; }
        public double ThreeStars { get; set; }
        public double FourStars { get; set; }
        public double FiveStars { get; set; }
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
        public virtual ICollection<Keyword> Keywords { get; set; }
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
            Keywords = new HashSet<Keyword>();
            ProductEmails = new HashSet<ProductEmail>();
            Notifications = new HashSet<Notification>();
        }
    }
}