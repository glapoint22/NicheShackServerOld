using DataAccess.Interfaces;
using System.Linq;

namespace DataAccess.ViewModels
{


    public class ItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }



    public class ItemViewModel<T>: ItemViewModel, IQueryableSelect<T, ItemViewModel<T>> where T: class, IItem
    {
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