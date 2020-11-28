using System.Collections.Generic;

namespace DataAccess.Interfaces
{
    public interface IEnumerableSelect<T, TOut> where T : class where TOut : class
    {
        IEnumerable<TOut> Select(IEnumerable<T> source);
    }
}