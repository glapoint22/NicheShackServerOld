using System.Collections.Generic;
using System.Linq;
using DataAccess.Interfaces;
using DataAccess.Models;

namespace Website.ViewModels
{
    public class ProductReviewViewModel : IQueryableSelect<ProductReview, ProductReviewViewModel>, IQueryableOrderBy<ProductReview>, IWhere<ProductReview>
    {
        private readonly string orderBy;
        private readonly string filterBy;

        public int Id { get; set; }
        public string Title { get; set; }
        public double Rating { get; set; }
        public string UserName { get; set; }
        public ImageViewModel ProfileImage { get; set; }
        public string Date { get; set; }
        public bool IsVerified { get; set; }
        public string Text { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int ProductId { get; set; }



        // Constructors
        public ProductReviewViewModel() { }

        public ProductReviewViewModel(string orderBy, string filterBy)
        {
            this.orderBy = orderBy;
            this.filterBy = filterBy;
        }


        // ..................................................................................Get Reviews Per Page.....................................................................
        public int GetReviewsPerPage()
        {
            return 10;
        }





        // ..................................................................................Get Sort Options.....................................................................
        public List<KeyValuePair<string, string>> GetSortOptions()
        {
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("High to Low Rating", "high-low-rating"),
                new KeyValuePair<string, string>("Low to High Rating", "low-high-rating"),
                new KeyValuePair<string, string>("Newest to Oldest", "new-old"),
                new KeyValuePair<string, string>("Oldest to Newest", "old-new"),
                new KeyValuePair<string, string>("Most helpful", "most-helpful")
            };
        }




        // ..................................................................................Set Select.....................................................................
        public IQueryable<ProductReviewViewModel> Select(IQueryable<ProductReview> source)
        {
            return source.Select(x => new ProductReviewViewModel
            {
                Id = x.Id,
                Title = x.Title,
                ProductId = x.ProductId,
                Rating = x.Rating,
                UserName = x.Customer.FirstName + " " + x.Customer.LastName,
                ProfileImage = new ImageViewModel
                {
                    Name = x.Customer.FirstName + " " + x.Customer.LastName,
                    Url = x.Customer.Image
                },
                Date = x.Date.ToString("MMMM dd, yyyy"),
                IsVerified = x.Product.ProductOrders.Count(z => z.CustomerId == x.CustomerId && z.ProductId == x.ProductId) > 0,
                Text = x.Text,
                Likes = x.Likes,
                Dislikes = x.Dislikes
            });
        }




        // .............................................................................Order By.....................................................................
        public IOrderedQueryable<ProductReview> OrderBy(IQueryable<ProductReview> source)
        {
            IOrderedQueryable<ProductReview> orderResult = null;


            switch (orderBy)
            {
                case "low-to-high":
                    orderResult = source.OrderBy(x => x.Rating);
                    break;

                case "newest-to-oldest":
                    orderResult = source.OrderByDescending(x => x.Date);
                    break;

                case "oldest-to-newest":
                    orderResult = source.OrderBy(x => x.Date);
                    break;

                case "most-helpful":
                    orderResult = source.OrderByDescending(x => x.Likes);
                    break;

                default:
                    // High to low rating
                    orderResult = source.OrderByDescending(x => x.Rating);
                    break;
            }

            return orderResult;
        }




        // ..................................................................................Set Where.....................................................................
        public IQueryable<ProductReview> SetWhere(IQueryable<ProductReview> source)
        {
            switch (filterBy)
            {
                case "five-stars":
                    source = source.Where(x => x.Rating == 5);
                    break;
                case "four-stars":
                    source = source.Where(x => x.Rating == 4);
                    break;

                case "three-stars":
                    source = source.Where(x => x.Rating == 3);
                    break;


                case "two-stars":
                    source = source.Where(x => x.Rating == 2);
                    break;


                case "one-star":
                    source = source.Where(x => x.Rating == 1);
                    break;

                default:
                    source = source.Where(x => x.Rating > 0);
                    break;
            }

            return source;
        }
    }
}
