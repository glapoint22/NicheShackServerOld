using System.Collections.Generic;

namespace Services.Classes
{
    public struct Filters
    {
        public CategoriesFilter NichesFilter { get; set; }
        public PriceFilter PriceFilter { get; set; }
        public QueryFilter RatingFilter { get; set; }
        public List<QueryFilter> CustomFilters { get; set; }

    }
}
