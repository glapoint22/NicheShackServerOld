using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Website.Classes;
using Website.Models;

namespace Website.Repositories
{
    public interface IListRepository : IRepository<List>
    {
        Task<IEnumerable<ListDTO>> GetLists(string customerId);
        Task<IEnumerable<ListProductDTO>> GetListProducts(IEnumerable<Guid> collaborators, string customerId, string sort, bool shared = false);
    }
}
