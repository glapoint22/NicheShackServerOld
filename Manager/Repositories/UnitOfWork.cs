using DataAccess.Models;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly NicheShackContext context;

        // Generic Repositories
        public ISearchableRepository<Category> Categories { get; }
        public ISearchableRepository<Niche> Niches { get; }
        public ISearchableRepository<Filter> Filters { get; }
        public ISearchableRepository<FilterOption> FilterOptions { get; }
        public IRepository<LeadPage> LeadPages { get; }
        public IRepository<LeadPageEmail> LeadPageEmails { get; }
        public IRepository<ProductEmail> ProductEmails { get; }
        public ISearchableRepository<Vendor> Vendors { get; }
        public ISearchableRepository<Page> Pages { get; }
        public ISearchableRepository<Email> Emails { get; }
        public ISearchableRepository<Media> Media { get; }
        public ISearchableRepository<Keyword> Keywords { get; }
        public IRepository<Notification> Notifications { get; }
        public IRepository<ProductFilter> ProductFilters { get; }
        public IRepository<ProductPricePoint> ProductPricePoints { get; }
        public IRepository<PriceIndex> PriceIndices { get; }
        public IRepository<ProductContent> ProductContent { get; }
        public IRepository<ProductMedia> ProductMedia { get; }
        public IRepository<ProductKeyword> ProductKeywords { get; }
        public IRepository<NotificationText> NotificationText { get; }
        public ISearchableRepository<Subgroup> Subgroups { get; }
        public IRepository<SubgroupProduct> SubgroupProducts { get; }
        public IRepository<ProductReview> ProductReviews { get; }
        public IRepository<PageDisplayTypeId> PageDisplayTypeIds { get; }


        // Custom Repositories
        public IProductRepository Products { get; }
        


        public UnitOfWork(NicheShackContext context)
        {
            this.context = context;

            // Generic Repositories
            Categories = new SearchableRepository<Category>(context);
            Niches = new SearchableRepository<Niche>(context);
            Filters = new SearchableRepository<Filter>(context);
            FilterOptions = new SearchableRepository<FilterOption>(context);
            LeadPages = new Repository<LeadPage>(context);
            LeadPageEmails = new Repository<LeadPageEmail>(context);
            ProductEmails = new Repository<ProductEmail>(context);
            Vendors = new SearchableRepository<Vendor>(context);
            Pages = new SearchableRepository<Page> (context);
            Emails = new SearchableRepository<Email>(context);
            Media = new SearchableRepository<Media>(context);
            Keywords = new SearchableRepository<Keyword>(context);
            Notifications = new Repository<Notification>(context);
            ProductFilters = new Repository<ProductFilter>(context);
            ProductPricePoints = new Repository<ProductPricePoint>(context);
            PriceIndices = new Repository<PriceIndex>(context);
            ProductContent = new Repository<ProductContent>(context);
            ProductMedia = new Repository<ProductMedia>(context);
            ProductKeywords = new Repository<ProductKeyword>(context);
            NotificationText = new Repository<NotificationText>(context);
            Subgroups = new SearchableRepository<Subgroup>(context);
            SubgroupProducts = new Repository<SubgroupProduct>(context);
            ProductReviews = new Repository<ProductReview>(context);
            PageDisplayTypeIds = new Repository<PageDisplayTypeId>(context);

            // Custom Repositories
            Products = new ProductRepository(context);
            
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
