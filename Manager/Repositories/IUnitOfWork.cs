using DataAccess.Models;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
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
        IRepository<Notification> Notifications { get; }
        IRepository<ProductFilter> ProductFilters { get; }
        IRepository<ProductPricePoint> ProductPricePoints { get; }
        IRepository<PriceIndex> PriceIndices { get; }
        IRepository<ProductContent> ProductContent { get; }
        IRepository<ProductMedia> ProductMedia { get; }
        IRepository<ProductKeyword> ProductKeywords { get; }




        // Custom Repositories
        IProductRepository Products { get; }
        


        Task<int> Save();
    }
}
