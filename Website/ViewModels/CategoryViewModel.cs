using DataAccess.Models;
using DataAccess.ViewModels;
using System.Collections.Generic;

namespace Website.ViewModels
{
    public class CategoryViewModel: ItemViewModel<Category>
    {
        public IEnumerable<ItemViewModel<Niche>> Niches { get; set; }
    }
}
