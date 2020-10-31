using System.Collections.Generic;
using System.Threading.Tasks;
using Website.Classes;
using DataAccess.Models;
using DataAccess.Repositories;
using Website.ViewModels;

namespace Website.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        //Task<IEnumerable<CategoryViewModel>> GetQueriedCategories(QueryParams queryParams, IEnumerable<ProductViewModel> products);
    }
}
