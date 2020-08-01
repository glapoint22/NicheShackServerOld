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

        public IRepository<Category> Categories { get; }
        public IRepository<Niche> Niches { get; }
        public IRepository<Product> Products { get; }


        public UnitOfWork(NicheShackContext context)
        {
            this.context = context;

            Categories = new Repository<Category>(context);
            Niches = new Repository<Niche>(context);
            Products = new Repository<Product>(context);
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
