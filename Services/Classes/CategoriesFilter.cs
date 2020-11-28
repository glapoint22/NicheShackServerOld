using System.Collections.Generic;

namespace Services.Classes
{
    public struct CategoriesFilter
    {
        public IEnumerable<CategoryFilter> Visible { get; set; }
        public IEnumerable<CategoryFilter> Hidden { get; set; }
    }
}