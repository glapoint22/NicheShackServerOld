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
        //Task<IEnumerable<ProductViewModel>> GetQueriedProducts(QueryParams queryParams);
        Task<IEnumerable<FilterData>> GetProductFilters(QueryParams queryParams, IEnumerable<ProductViewModel> products);
        Task<IEnumerable<ProductViewModel>> GetProducts(string query);
    }
}
