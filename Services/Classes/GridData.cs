using System.Collections.Generic;

namespace Services.Classes
{
    public struct GridData
    {
        public List<QueriedProduct> Products { get; set; }
        public int TotalProducts { get; set; }
        public double PageCount { get; set; }
        public Filters Filters { get; set; }
        public double ProductCountStart { get; set; }
        public double ProductCountEnd { get; set; }
    }
}