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
        IRepository<Category> Categories { get; }
        IRepository<Niche> Niches { get; }
        IRepository<Filter> Filters { get; }
        IRepository<FilterOption> FilterOptions { get; }
        IRepository<LeadPage> LeadPages { get; }
        IRepository<LeadPageEmail> LeadPageEmails { get; }
        IRepository<ProductEmail> ProductEmails { get; }
        IRepository<Vendor> Vendors { get; }
        IRepository<Page> Pages { get; }
        IRepository<Email> Emails { get; }
        IRepository<Media> Media { get; }
        IRepository<Notification> Notifications { get; }



        // Custom Repositories
        IProductRepository Products { get; }
        


        Task<int> Save();
    }
}
