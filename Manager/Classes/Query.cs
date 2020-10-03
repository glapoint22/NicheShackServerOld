using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Classes
{


    public enum WhereType
    {
        Category,
        Niche,
        ProductSubgroup,
        FeaturedProducts,
        CustomerRelatedProducts,
        ProductPrice,
        ProductRating,
        ProductKeywords,
        ProductCreationDate
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


    public class Query
    {
        public WhereType Where { get; set; }
        public OperatorType Operator { get; set; }
        public string Value { get; set; }
    }
}
