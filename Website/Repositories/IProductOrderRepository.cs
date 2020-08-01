using System.Collections.Generic;
using System.Threading.Tasks;
using Website.Classes;
using DataAccess.Models;
using DataAccess.Repositories;
using Website.ViewModels;

namespace Website.Repositories
{
    public interface IProductOrderRepository : IRepository<ProductOrder>
    {
        Task<IEnumerable<ProductOrderViewModel>> GetOrders(string customerId, string filter, string searchWords = "");
        Task<IEnumerable<OrderProductQueryResultViewModel>> GetOrderProducts(string customerId, string searchWords);
        Task<List<KeyValuePair<string, string>>> GetOrderFilters(string customerId);
    }
}
