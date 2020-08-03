using System.Linq;

namespace DataAccess.Interfaces
{
    public interface ISelect<T, TOut> where T : class where TOut : class
    {
        IQueryable<TOut> ViewModelSelect(IQueryable<T> source);
    }
}
