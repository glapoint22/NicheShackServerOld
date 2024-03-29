﻿using DataAccess.Models;
using DataAccess.Repositories;
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
        public IRepository<ProductFilter> ProductFilters { get; }
        public IRepository<ProductMedia> ProductMedia { get; }
        public IRepository<ProductKeyword> ProductKeywords { get; }
        public ISearchableRepository<Subgroup> Subgroups { get; }
        public IRepository<SubgroupProduct> SubgroupProducts { get; }
        public IRepository<ProductReview> ProductReviews { get; }
        public IRepository<PageReferenceItem> PageReferenceItems { get; }
        public IRepository<PricePoint> PricePoints { get; }
        public ISearchableRepository<KeywordGroup> KeywordGroups { get; }
        public IRepository<KeywordGroup_Belonging_To_Product> KeywordGroups_Belonging_To_Product { get; }
        public IRepository<Keyword_In_KeywordGroup> Keywords_In_KeywordGroup { get; }
        public IRepository<PageKeyword> PageKeywords { get; }
        public IRepository<Subproduct> Subproducts { get; }
        public IRepository<NotificationEmployeeNote> NotificationEmployeeNotes { get; }
        public IRepository<NotificationGroup> NotificationGroups { get; }
        public IRepository<Customer> Customers { get; }
        public IRepository<BlockedNonAccountEmail> BlockedNonAccountEmails { get; }
        public IRepository<ProductPrice> ProductPrices { get; }


        // Custom Repositories
        public IProductRepository Products { get; }
        public INotificationRepository Notifications { get; }


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
            Pages = new SearchableRepository<Page>(context);
            Emails = new SearchableRepository<Email>(context);
            Media = new SearchableRepository<Media>(context);
            Keywords = new SearchableRepository<Keyword>(context);
            ProductFilters = new Repository<ProductFilter>(context);
            ProductMedia = new Repository<ProductMedia>(context);
            ProductKeywords = new Repository<ProductKeyword>(context);
            Subgroups = new SearchableRepository<Subgroup>(context);
            SubgroupProducts = new Repository<SubgroupProduct>(context);
            ProductReviews = new Repository<ProductReview>(context);
            PageReferenceItems = new Repository<PageReferenceItem>(context);
            PricePoints = new Repository<PricePoint>(context);
            KeywordGroups = new SearchableRepository<KeywordGroup>(context);
            KeywordGroups_Belonging_To_Product = new Repository<KeywordGroup_Belonging_To_Product>(context);
            Keywords_In_KeywordGroup = new Repository<Keyword_In_KeywordGroup>(context);
            PageKeywords = new Repository<PageKeyword>(context);
            Subproducts = new Repository<Subproduct>(context);
            NotificationEmployeeNotes = new Repository<NotificationEmployeeNote>(context);
            NotificationGroups = new Repository<NotificationGroup>(context);
            Customers = new Repository<Customer>(context);
            BlockedNonAccountEmails = new Repository<BlockedNonAccountEmail>(context);
            ProductPrices = new Repository<ProductPrice>(context);

            // Custom Repositories
            Products = new ProductRepository(context);
            Notifications = new NotificationRepository(context);
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
