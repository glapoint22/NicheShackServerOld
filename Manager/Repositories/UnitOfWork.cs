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
        public IRepository<Category> Categories { get; }
        public IRepository<Niche> Niches { get; }
        public IRepository<Filter> Filters { get; }
        public IRepository<FilterOption> FilterOptions { get; }
        public IRepository<LeadPage> LeadPages { get; }
        public IRepository<LeadPageEmail> LeadPageEmails { get; }
        public IRepository<ProductEmail> ProductEmails { get; }
        public IRepository<Vendor> Vendors { get; }
        public IRepository<Page> Pages { get; }
        public IRepository<Email> Emails { get; }
        public IRepository<Media> Media { get; }
        public IRepository<Notification> Notifications { get; }


        // Custom Repositories
        public IProductRepository Products { get; }


        public UnitOfWork(NicheShackContext context)
        {
            this.context = context;

            // Generic Repositories
            Categories = new Repository<Category>(context);
            Niches = new Repository<Niche>(context);
            Filters = new Repository<Filter>(context);
            FilterOptions = new Repository<FilterOption>(context);
            LeadPages = new Repository<LeadPage>(context);
            LeadPageEmails = new Repository<LeadPageEmail>(context);
            ProductEmails = new Repository<ProductEmail>(context);
            Vendors = new Repository<Vendor>(context);
            Pages = new Repository<Page> (context);
            Emails = new Repository<Email>(context);
            Media = new Repository<Media>(context);
            Notifications = new Repository<Notification>(context);

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
