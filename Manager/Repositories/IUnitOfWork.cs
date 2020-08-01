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
        IRepository<Category> Categories { get; }
        IRepository<Niche> Niches { get; }
        IRepository<Product> Products { get; }


        Task<int> Save();
    }
}
