using System.Threading.Tasks;
using DataAccess.Models;
using DataAccess.Repositories;

namespace Website.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICategoryRepository Categories { get; private set; }
        public IProductRepository Products { get; private set; }
        public IProductReviewRepository ProductReviews { get; private set; }
        public IListRepository Lists { get; }
        public IProductOrderRepository ProductOrders { get; }


        // Generic repositories
        public IRepository<ProductMedia> ProductMedia { get; private set; }
        public IRepository<ProductContent> ProductContent { get; private set; }
        public IRepository<ProductPricePoint> PricePoints { get; private set; }
        public IRepository<RefreshToken> RefreshTokens { get; private set; }
        public IRepository<ListCollaborator> Collaborators { get; }
        public IRepository<Customer> Customers { get; }
        public IRepository<ListProduct> ListProducts { get; }
        public IRepository<Media> Media { get; }
        public IRepository<Notification> Notifications { get; }
        public IRepository<NotificationText> NotificationText { get; }
        public IRepository<Page> Pages { get; }


        // Declare the Nicheshack context
        private readonly NicheShackContext context;

        public UnitOfWork(NicheShackContext context)
        {
            this.context = context;

            Categories = new CategoryRepository(context);
            Products = new ProductRepository(context);
            ProductReviews = new ProductReviewRepository(context);
            Lists = new ListRepository(context);
            ProductOrders = new ProductOrderRepository(context);

            // Generic repositories
            ProductMedia = new Repository<ProductMedia>(context);
            ProductContent = new Repository<ProductContent>(context);
            PricePoints = new Repository<ProductPricePoint>(context);
            RefreshTokens = new Repository<RefreshToken>(context);
            Collaborators = new Repository<ListCollaborator>(context);
            Customers = new Repository<Customer>(context);
            ListProducts = new Repository<ListProduct>(context);
            Media = new Repository<Media>(context);
            Notifications = new Repository<Notification>(context);
            NotificationText = new Repository<NotificationText>(context);
            Pages = new Repository<Page>(context);
        }


        public void Dispose()
        {
            context.Dispose();
        }

        public async Task<int> Save()
        {
            return await context.SaveChangesAsync();
        }
    }
}
