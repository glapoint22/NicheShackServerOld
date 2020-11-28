using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Interfaces
{
    public interface IEnumerableOrderBy<T> where T : class
    {
        IOrderedEnumerable<T> OrderBy(IEnumerable<T> source);
    }
}