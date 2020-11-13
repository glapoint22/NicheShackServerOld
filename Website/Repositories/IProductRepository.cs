using System.Collections.Generic;
using System.Threading.Tasks;
using Website.Classes;
using DataAccess.Models;
using DataAccess.Repositories;
using Website.ViewModels;

namespace Website.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Filters> GetProductFilters(IEnumerable<ProductViewModel> products, QueryParams queryParams);
        Task<IEnumerable<ProductViewModel>> GetProducts(QueryParams queryParams);
    }
}
