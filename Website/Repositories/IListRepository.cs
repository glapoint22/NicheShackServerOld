using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Website.Classes;
using DataAccess.Models;
using DataAccess.Repositories;
using Website.ViewModels;

namespace Website.Repositories
{
    public interface IListRepository : IRepository<List>
    {
        Task<IEnumerable<ListViewModel>> GetLists(string customerId);
        Task<IEnumerable<ListProductViewModel>> GetListProducts(IEnumerable<int> collaborators, string customerId, string sort);
    }
}
