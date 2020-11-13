using System.Collections.Generic;

namespace Website.Classes
{
    public struct Filters
    {
        public List<CategoryFilter> CategoryFilters { get; set; }
        public PriceFilter PriceFilter { get; set; }
        public QueryFilter RatingFilter { get; set; }
        public List<QueryFilter> CustomFilters { get; set; }

    }
}
