using System;
using System.Text.Json;

namespace Services.Classes
{
    #nullable enable
    public class QueryRow
    {
        public QueryType? QueryType { get; set; }
        public LogicalOperatorType? LogicalOperatorType { get; set; }
        public ComparisonOperatorType? ComparisonOperatorType { get; set; }
        public Item? Item { get; set; }
        public int? Integer { get; set; }
        public DateTime? Date { get; set; }
        public double? Price { get; set; }
        public AutoQueryType? Auto { get; set; }
        public string? StringValue { get; set; }
    }
}
