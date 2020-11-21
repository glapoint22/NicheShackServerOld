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


    public class Niches
    {
        public IEnumerable<int> CategoryIds { get; set; }
    }


    public class Query
    {
        public QueryType QueryType { get; set; }
        public ComparisonOperatorType ComparisonOperator { get; set; }
        public LogicalOperatorType LogicalOperator { get; set; }
        public int IntValue { get; set; }
        public string StringValue { get; set; }
        public double DoubleValue { get; set; }
        public DateTime DateValue { get; set; }
    }
}
