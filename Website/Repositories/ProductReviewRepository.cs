using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;
using DataAccess.Repositories;
using DataAccess.Classes;
using Website.ViewModels;
using System;

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
        public async Task<IEnumerable<ProductReviewViewModel>> GetReviews(string productId, string sortBy, int page, string filterBy)
        {
            ProductReviewViewModel productReview = new ProductReviewViewModel(sortBy, filterBy);

            return await context.ProductReviews
                .AsNoTracking()
                .OrderBy(productReview)
                .ThenByDescending(x => x.Date)
                .Where(x => x.Product.UrlId == productId && !x.Deleted)
                .Where(productReview)
                .Select<ProductReview, ProductReviewViewModel>()
                .Skip((page - 1) * productReview.GetReviewsPerPage())
                .Take(productReview.GetReviewsPerPage())
                .ToListAsync();
        }




        // ..................................................................................Get Page Count.....................................................................
        public async Task<double> GetTotalReviews(string productId, string filterBy)
        {
            double totalReviews = 0;
            ProductReviewViewModel productReview = new ProductReviewViewModel(null, filterBy);

            if (filterBy != null)
            {
                totalReviews = await context.ProductReviews
                .AsNoTracking()
                .Where(x => x.Product.UrlId == productId && !x.Deleted)
                .Where(productReview)
                .CountAsync();
            }
            else
            {
                totalReviews = await context.ProductReviews
                    .AsNoTracking()
                    .CountAsync(x => x.Product.UrlId == productId);
            }

            return totalReviews;
        }




        // .............................................................................Get Negative Review................................................................
        public async Task<ProductReviewViewModel> GetNegativeReview(string productId)
        {
            return await context.ProductReviews
                .AsNoTracking()
                .OrderBy(x => x.Rating)
                .ThenByDescending(x => x.Likes)
                .ThenByDescending(x => x.Date)
                .Where(x => x.Product.UrlId == productId && x.Likes > 0 && !x.Deleted)
                .Select<ProductReview, ProductReviewViewModel>()
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
                .Where(x => x.Product.UrlId == productId && x.Likes > 0 && !x.Deleted)
                .Select<ProductReview, ProductReviewViewModel>()
                .FirstOrDefaultAsync();
        }
    }
}
