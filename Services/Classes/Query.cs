using System;
using System.Collections.Generic;

namespace Services.Classes
{


    public enum QueryType
    {
        None,
        Category,
        Niche,
        ProductGroup,
        Price,
        Rating,
        KeywordGroup,
        Date,
        Auto,
        Search
    }



    public enum ComparisonOperatorType
    {
        Equal,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
    }


    public enum LogicalOperatorType
    {
        And,
        Or
    }



    public enum AutoQueryType
    {
        Browsed,
        Related,
        RelatedBrowsed,
        RelatedBought,
        RelatedWishlist
    }



    public enum QueryElementType
    {
        QueryRow,
        QueryGroup
    }



    public class Query
    {
        public List<QueryElement> Elements { get; set; }
    }
}
