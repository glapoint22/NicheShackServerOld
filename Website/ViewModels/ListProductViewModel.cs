using System.Collections.Generic;
using System.Linq;
using DataAccess.Interfaces;
using DataAccess.Models;

namespace Website.ViewModels
{
    public class ListProductViewModel : IQueryableOrderBy<ListProduct>
    {
        private readonly string orderBy;

        public int Id { get; set; }
        public string UrlId { get; set; }
        public string Name { get; set; }
        public double Rating { get; set; }
        public int TotalReviews { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public string DateAdded { get; set; }
        public CollaboratorViewModel Collaborator { get; set; }
        public string Hoplink { get; set; }
        public ImageViewModel Image { get; set; }
        public string UrlName { get; set; }
        public int OneStar { get; set; }
        public int TwoStars { get; set; }
        public int ThreeStars { get; set; }
        public int FourStars { get; set; }
        public int FiveStars { get; set; }

        // Constructors
        public ListProductViewModel() { }

        public ListProductViewModel(string orderBy)
        {
            this.orderBy = orderBy;
        }


        // .............................................................................Get Sort Options.....................................................................
        public List<KeyValuePair<string, string>> GetSortOptions()
        {
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Date Added", "date"),
                new KeyValuePair<string, string>("Price: Low to High", "price-asc"),
                new KeyValuePair<string, string>("Price: High to Low", "price-desc"),
                new KeyValuePair<string, string>("Highest Rating", "rating"),
                new KeyValuePair<string, string>("Title", "title")
            };
        }



        // .............................................................................Order By.....................................................................
        public IOrderedQueryable<ListProduct> OrderBy(IQueryable<ListProduct> source)
        {
            IOrderedQueryable<ListProduct> orderResult = null;


            switch (orderBy)
            {
                case "price-asc":
                    orderResult = source.OrderBy(x => x.Product.ProductPrices.Select(x => x.Price).Min());
                    break;
                case "price-desc":
                    orderResult = source.OrderByDescending(x => x.Product.ProductPrices.Select(x => x.Price).Min());
                    break;
                case "rating":
                    orderResult = source.OrderByDescending(x => x.Product.Rating);
                    break;
                case "title":
                    orderResult = source.OrderBy(x => x.Product.Name);
                    break;
                default:
                    orderResult = source.OrderByDescending(x => x.DateAdded);
                    break;
            }

            return orderResult;
        }
    }
}
