using System;
using System.Threading.Tasks;
using DataAccess.Models;
using DataAccess.Repositories;

namespace Website.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Categories { get; }
        IProductReviewRepository ProductReviews { get; }
        IListRepository Lists { get; }
        IProductOrderRepository ProductOrders { get; }


        // Generic repositories
        IRepository<Product> Products { get; }
        IRepository<ProductMedia> ProductMedia { get;  }
        IRepository<RefreshToken> RefreshTokens { get; }
        IRepository<ListCollaborator> Collaborators { get; }
        IRepository<Customer> Customers { get; }
        IRepository<ListProduct> ListProducts { get; }
        IRepository<Media> Media { get; }
        IRepository<Notification> Notifications { get; }
        IRepository<NotificationText> NotificationText { get; }
        IRepository<Page> Pages { get; }
        IRepository<OrderProduct> OrderProducts { get; }
        IRepository<ProductKeyword> ProductKeywords { get; }
        IRepository<Keyword> Keywords { get; }
        IRepository<KeywordSearchVolume> KeywordSearchVolumes { get; }
        IRepository<ProductFilter> ProductFilters { get; }
        IRepository<Niche> Niches { get; }
        IRepository<PageReferenceItem> PageReferenceItems { get; }
        IRepository<ProductPrice> ProductPrices { get; }
        IRepository<ProductAdditionalInfo> AdditionalInfo { get; }
        IRepository<Subproduct> Subproducts { get; }


        Task<int> Save();
    }
}
