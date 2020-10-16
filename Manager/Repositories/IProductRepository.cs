using DataAccess.Models;
using DataAccess.Repositories;
using Manager.Classes;
using Manager.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Manager.Repositories
{
    public interface IProductRepository : ISearchableRepository<Product>
    {
        Task<IEnumerable<ProductFilterViewModel>> GetProductFilters(int productId, int filterId);
        Task<ProductViewModel> GetProduct(int productId);


        Task<IEnumerable<QueryBuilderViewModel>> GetAlita(List<Query> queries);
    }
}
