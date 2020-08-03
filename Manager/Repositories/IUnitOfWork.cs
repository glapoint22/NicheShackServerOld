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



        // Custom Repositories
        IProductRepository Products { get; }


        Task<int> Save();
    }
}
