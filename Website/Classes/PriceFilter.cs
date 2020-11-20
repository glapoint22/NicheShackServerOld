using System.Collections.Generic;

namespace Website.Classes
{
    public class PriceFilter
    {
        public string Caption { get; set; }
        public IEnumerable<PriceFilterOption> Options { get; set; }
    }
}
