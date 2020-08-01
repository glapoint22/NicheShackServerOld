using System.Linq;

namespace DataAccess.Interfaces
{
    public interface ISort<T> where T : class
    {
        IOrderedQueryable<T> SetSortOption(IQueryable<T> source);
    }
}
