using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Classes
{
    public class QueryResult : QueriedProduct
    {
        public int NicheId { get; set; }
        public double Weight { get; set; }
    }
}
