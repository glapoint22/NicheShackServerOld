using System.Linq;

namespace DataAccess.Interfaces
{
    public interface IQueryableOrderBy<T> where T : class
    {
        IOrderedQueryable<T> OrderBy(IQueryable<T> source);
    }
}
