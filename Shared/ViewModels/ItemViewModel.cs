using DataAccess.Interfaces;
using System.Linq;

namespace DataAccess.ViewModels
{
    public class ItemViewModel<T>: ISelect<T, ItemViewModel<T>> where T: class, IItem
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public IQueryable<ItemViewModel<T>> Select(IQueryable<T> source)
        {
            return source.Select(x => new ItemViewModel<T>
            {
                Id = x.Id,
                Name = x.Name
            });
        }
    }
}