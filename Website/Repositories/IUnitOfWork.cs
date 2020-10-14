using System;
using System.Threading.Tasks;
using DataAccess.Models;
using DataAccess.Repositories;

namespace Website.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Categories { get; }
        IProductRepository Products { get; }
        IProductReviewRepository ProductReviews { get; }
        IListRepository Lists { get; }
        IProductOrderRepository ProductOrders { get; }


        // Generic repositories
        IRepository<ProductMedia> ProductMedia { get;  }
        IRepository<ProductContent> ProductContent { get; }
        IRepository<ProductPricePoint> PricePoints { get; }
        IRepository<RefreshToken> RefreshTokens { get; }
        IRepository<ListCollaborator> Collaborators { get; }
        IRepository<Customer> Customers { get; }
        IRepository<ListProduct> ListProducts { get; }
        IRepository<Media> Media { get; }
        IRepository<Notification> Notifications { get; }
        IRepository<NotificationText> NotificationText { get; }
        IRepository<Page> Pages { get; }
        IRepository<OrderProduct> OrderProducts { get; }



        Task<int> Save();
    }
}
