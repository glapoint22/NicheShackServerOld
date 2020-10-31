using DataAccess.Interfaces;
using System.Linq;

namespace Website.ViewModels
{
    public class UrlItemViewModel
    {
        public string UrlId { get; set; }
        public string Name { get; set; }
        public string UrlName { get; set; }
    }


    public class UrlItemViewModel<T> : UrlItemViewModel, ISelect<T, UrlItemViewModel<T>> where T : class, IUrlItem
    {
        public IQueryable<UrlItemViewModel<T>> ViewModelSelect(IQueryable<T> source)
        {
            return source.Select(x => new UrlItemViewModel<T>
            {
                UrlId = x.UrlId,
                Name = x.Name,
                UrlName = x.UrlName
            });
        }
    }
}
