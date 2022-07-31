using DataAccess.Models;
using DataAccess.Repositories;
using System;
using System.Threading.Tasks;

namespace Manager.Repositories
{
    public interface IUnitOfWork : IDisposable
    {


        // Generic Repositories
        ISearchableRepository<Category> Categories { get; }
        ISearchableRepository<Niche> Niches { get; }
        ISearchableRepository<Filter> Filters { get; }
        ISearchableRepository<FilterOption> FilterOptions { get; }
        IRepository<LeadPage> LeadPages { get; }
        IRepository<LeadPageEmail> LeadPageEmails { get; }
        IRepository<ProductEmail> ProductEmails { get; }
        ISearchableRepository<Vendor> Vendors { get; }
        ISearchableRepository<Page> Pages { get; }
        ISearchableRepository<Email> Emails { get; }
        ISearchableRepository<Media> Media { get; }
        ISearchableRepository<Keyword> Keywords { get; }
        IRepository<Notification> Notifications { get; }
        IRepository<ProductFilter> ProductFilters { get; }
        IRepository<ProductMedia> ProductMedia { get; }
        IRepository<ProductKeyword> ProductKeywords { get; }
        IRepository<NotificationText> NotificationText { get; }
        ISearchableRepository<Subgroup> Subgroups { get; }
        IRepository<SubgroupProduct> SubgroupProducts { get; }
        IRepository<ProductReview> ProductReviews { get; }
        IRepository<PageReferenceItem> PageReferenceItems { get; }
        IRepository<PricePoint> PricePoints { get; }
        ISearchableRepository<KeywordGroup> KeywordGroups { get; }
        IRepository<KeywordGroup_Belonging_To_Product> KeywordGroups_Belonging_To_Product { get; }
        IRepository<Keyword_In_KeywordGroup> Keywords_In_KeywordGroup { get; }
        IRepository<PageKeyword> PageKeywords { get; }
        IRepository<Subproduct> Subproducts { get; }
        IRepository<ImageReference> ImageReferences { get; }



        // Custom Repositories
        IProductRepository Products { get; }



        Task<int> Save();
    }
}
