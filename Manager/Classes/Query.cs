using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Classes
{


    public enum QueryType
    {
        Category,
        Niche,
        ProductSubgroup,
        FeaturedProducts,
        CustomerRelatedProducts,
        ProductPrice,
        ProductRating,
        ProductKeywords,
        ProductCreationDate,
        ProductIds
    }


    public enum OperatorType
    {
        Equals,
        GreaterThan,
        GreaterThanOrEqualTo,
        LessThan,
        LessThanOrEqualTo,
        IsBetween
    }

    public class Niches
    {
        public IEnumerable<int> CategoryIds { get; set; }
    }

    public class Query
    {
        public QueryType QueryType { get; set; }
        public List<OperatorType> Operator { get; set; }
        public List<string> Value { get; set; }
    }
}
