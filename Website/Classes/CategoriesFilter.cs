using System.Collections.Generic;

namespace Website.Classes
{
    public struct CategoriesFilter
    {
        public IEnumerable<CategoryFilter> Visible { get; set; }
        public IEnumerable<CategoryFilter> Hidden { get; set; }
    }
}