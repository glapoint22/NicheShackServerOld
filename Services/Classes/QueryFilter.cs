using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Classes
{
    public class QueryFilter
    {
        public string Caption { get; set; }
        public IEnumerable<QueryFilterOption> Options { get; set; }
    }
}
