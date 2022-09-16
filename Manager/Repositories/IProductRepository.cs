using DataAccess.Models;
using DataAccess.Repositories;
using Manager.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Manager.Repositories
{
    public interface IProductRepository : ISearchableRepository<Product>
    {
        Task<IEnumerable<ProductFilterViewModel>> GetProductFilters(int productId, int filterId);
        Task<ProductViewModel> GetProduct(int productId);
        Task<List<double>> GetTempProductPrices(int productId);
        Task<List<TempMedia>> GetTempProductMedia(int productId);
        Task<int> GetNicheId(int tempNicheId);
        Task<int> GetVendorId(int tempVendorId);
    }
}
