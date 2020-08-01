using System.Linq;

namespace DataAccess.Interfaces
{
    public interface IWhere<T> where T: class
    {
        IQueryable<T> SetWhere(IQueryable<T> source);
    }
}
