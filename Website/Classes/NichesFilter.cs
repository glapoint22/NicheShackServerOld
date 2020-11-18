using System.Collections.Generic;

namespace Website.Classes
{
    public struct NichesFilter
    {
        public IEnumerable<NicheFilter> Visible { get; set; }
        public IEnumerable<NicheFilter> Hidden { get; set; }
        public bool ShowHidden { get; set; }
    }
}
