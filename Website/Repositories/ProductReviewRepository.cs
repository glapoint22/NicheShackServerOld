using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;
using DataAccess.Repositories;
using DataAccess.Classes;
using Website.ViewModels;

namespace Website.Repositories
{
    public class ProductReviewRepository : Repository<ProductReview>, IProductReviewRepository
    {
        // Set the context
        private readonly NicheShackContext context;

        public ProductReviewRepository(NicheShackContext context) : base(context)
        {
            this.context = context;
        }



        // ..................................................................................Get Reviews.....................................................................
        public async Task<IEnumerable<ProductReviewViewModel>> GetReviews(string productId, string sortBy, int page)
        {
            ProductReviewViewModel productReviewDTO = new ProductReviewViewModel(sortBy);

            return await context.ProductReviews
                .AsNoTracking()
                .SortBy(productReviewDTO)
                .ThenByDescending(x => x.Date)
                .Where(x => x.Product.UrlId == productId)
                .ExtensionSelect<ProductReview, ProductReviewViewModel>()
                .Skip((page - 1) * productReviewDTO.GetReviewsPerPage())
                .Take(productReviewDTO.GetReviewsPerPage())
                .ToListAsync();
        }





        // .............................................................................Get Negative Review................................................................
        public async Task<ProductReviewViewModel> GetNegativeReview(string productId)
        {
            return await context.ProductReviews
                .AsNoTracking()
                .OrderBy(x => x.Rating)
                .ThenByDescending(x => x.Likes)
                .ThenByDescending(x => x.Date)
                .Where(x => x.Product.UrlId == productId && x.Likes > 0)
                .ExtensionSelect<ProductReview, ProductReviewViewModel>()
                .FirstOrDefaultAsync();
        }





        // .............................................................................Get Positive Review................................................................
        public async Task<ProductReviewViewModel> GetPositiveReview(string productId)
        {
            return await context.ProductReviews
                .AsNoTracking()
                .OrderByDescending(x => x.Rating)
                .ThenByDescending(x => x.Likes)
                .ThenByDescending(x => x.Date)
                .Where(x => x.Product.UrlId == productId && x.Likes > 0)
                .ExtensionSelect<ProductReview, ProductReviewViewModel>()
                .FirstOrDefaultAsync();
        }
    }
}
