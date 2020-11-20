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
    }

    public class Niches
    {
        public IEnumerable<int> CategoryIds { get; set; }
    }

    public class Query
    {
        public QueryType QueryType { get; set; }
        public List<OperatorType> Operator { get; set; }
        public List<int> IntValue { get; set; }
        public List<string> StringValue { get; set; }
        public List<double> DoubleValue { get; set; }
        public List<DateTime> DateValue { get; set; }
    }



    public class TempClass
    {
        public QueryType QueryType { get; set; }
        public int LogicalOperator { get; set; }
        public int IntValue { get; set; }
    }
}
