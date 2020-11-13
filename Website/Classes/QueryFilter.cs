using System.Collections.Generic;

namespace Website.Classes
{
    public class QueryFilter
    {
        public string Caption { get; set; }
        public IEnumerable<QueryFilterOption> Options { get; set; }
    }
}
