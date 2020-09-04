using System.Linq;
using DataAccess.Interfaces;
using DataAccess.Models;

namespace Website.ViewModels
{
    public class ProductDetailViewModel : ISelect<Product, ProductDetailViewModel>
    {
        public int Id { get; set; }
        public string UrlId { get; set; }
        public string Name { get; set; }
        public string UrlName { get; set; }
        public double Rating { get; set; }
        public int TotalReviews { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public string Description { get; set; }
        public string Hoplink { get; set; }
        public double OneStar { get; set; }
        public double TwoStars { get; set; }
        public double ThreeStars { get; set; }
        public double FourStars { get; set; }
        public double FiveStars { get; set; }



        // ..................................................................................Set Select.....................................................................
        public IQueryable<ProductDetailViewModel> ViewModelSelect(IQueryable<Product> source)
        {
            return source.Select(x => new ProductDetailViewModel
            {
                Id = x.Id,
                UrlId = x.UrlId,
                Name = x.Name,
                UrlName = x.UrlName,
                Rating = x.Rating,
                TotalReviews = x.TotalReviews,
                MinPrice = x.MinPrice,
                MaxPrice = x.MaxPrice,
                Hoplink = x.Hoplink,
                Description = x.Description,
                OneStar = x.OneStar,
                TwoStars = x.TwoStars,
                ThreeStars = x.ThreeStars,
                FourStars = x.FourStars,
                FiveStars = x.FiveStars
            });
        }
    }
}
