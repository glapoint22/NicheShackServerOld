using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess.Models;
using DataAccess.Repositories;
using Website.ViewModels;

namespace Website.Repositories
{
    public interface IProductReviewRepository : IRepository<ProductReview>
    {
        Task<IEnumerable<ProductReviewViewModel>> GetReviews(string productId, string sortBy, int page, string filterBy);
        Task<ProductReviewViewModel> GetPositiveReview(string productId);
        Task<ProductReviewViewModel> GetNegativeReview(string productId);
        Task<double> GetTotalReviews(string productId, string filterBy);
    }
}
