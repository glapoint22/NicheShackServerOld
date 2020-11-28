using System;

namespace Services.Classes
{
    public class QueryResult : QueriedProduct
    {
        public int NicheId { get; set; }
        public double Weight { get; set; }
        public DateTime Date { get; set; }
    }
}
