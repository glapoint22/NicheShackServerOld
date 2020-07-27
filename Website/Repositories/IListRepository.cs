using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Website.Classes;
using DataAccess.Models;

namespace Website.Repositories
{
    public interface IListRepository : IRepository<List>
    {
        Task<IEnumerable<ListDTO>> GetLists(string customerId);
        Task<IEnumerable<ListProductDTO>> GetListProducts(IEnumerable<int> collaborators, string customerId, string sort);
    }
}
