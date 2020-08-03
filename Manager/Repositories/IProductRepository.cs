using DataAccess.Models;
using DataAccess.Repositories;
using Manager.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Manager.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<ProductFilterViewModel>> GetProductFilters(int productId, int filterId);
        Task<ProductViewModel> GetProduct(int productId);
    }
}
