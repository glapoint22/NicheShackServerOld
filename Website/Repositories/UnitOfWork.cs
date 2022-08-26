using System.Threading.Tasks;
using DataAccess.Models;
using DataAccess.Repositories;

namespace Website.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICategoryRepository Categories { get; private set; }
        public IProductReviewRepository ProductReviews { get; private set; }
        public IListRepository Lists { get; }
        public IProductOrderRepository ProductOrders { get; }


        // Generic repositories
        public IRepository<Product> Products { get; private set; }
        public IRepository<ProductMedia> ProductMedia { get; private set; }
        public IRepository<RefreshToken> RefreshTokens { get; private set; }
        public IRepository<ListCollaborator> Collaborators { get; }
        public IRepository<Customer> Customers { get; }
        public IRepository<ListProduct> ListProducts { get; }
        public IRepository<Media> Media { get; }
        public IRepository<Notification> Notifications { get; }
        public IRepository<NotificationDetails> NotificationText { get; }
        public IRepository<Page> Pages { get; }
        public IRepository<OrderProduct> OrderProducts { get; }
        public IRepository<ProductKeyword> ProductKeywords { get; }
        public IRepository<Keyword> Keywords { get; }
        public IRepository<KeywordSearchVolume> KeywordSearchVolumes { get; }
        public IRepository<ProductFilter> ProductFilters { get; }
        public IRepository<Niche> Niches { get; }
        public IRepository<PageReferenceItem> PageReferenceItems { get; }
        public IRepository<PricePoint> ProductPrices { get; }
        //public IRepository<ProductAdditionalInfo> AdditionalInfo { get; }
        public IRepository<Subproduct> Subproducts { get; }
        public IRepository<OneTimePassword> OneTimePasswords { get; }


        // Declare the Nicheshack context
        private readonly NicheShackContext context;

        public UnitOfWork(NicheShackContext context)
        {
            this.context = context;

            Categories = new CategoryRepository(context);
            ProductReviews = new ProductReviewRepository(context);
            Lists = new ListRepository(context);
            ProductOrders = new ProductOrderRepository(context);

            // Generic repositories
            Products = new Repository<Product>(context);
            ProductMedia = new Repository<ProductMedia>(context);
            RefreshTokens = new Repository<RefreshToken>(context);
            Collaborators = new Repository<ListCollaborator>(context);
            Customers = new Repository<Customer>(context);
            ListProducts = new Repository<ListProduct>(context);
            Media = new Repository<Media>(context);
            Notifications = new Repository<Notification>(context);
            NotificationText = new Repository<NotificationDetails>(context);
            Pages = new Repository<Page>(context);
            OrderProducts = new Repository<OrderProduct>(context);
            ProductKeywords = new Repository<ProductKeyword>(context);
            Keywords = new Repository<Keyword>(context);
            KeywordSearchVolumes = new Repository<KeywordSearchVolume>(context);
            ProductFilters = new Repository<ProductFilter>(context);
            Niches = new Repository<Niche>(context);
            PageReferenceItems = new Repository<PageReferenceItem>(context);
            ProductPrices = new Repository<PricePoint>(context);
            //AdditionalInfo = new Repository<ProductAdditionalInfo>(context);
            Subproducts = new Repository<Subproduct>(context);
            OneTimePasswords = new Repository<OneTimePassword>(context);
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
