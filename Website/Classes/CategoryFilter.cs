using System.Collections.Generic;

namespace Website.Classes
{
    public struct CategoryFilter
    {
        public string UrlId { get; set; }
        public string Name { get; set; }
        public string UrlName { get; set; }
        public IEnumerable<NicheFilter> Niches { get; set; }
    }
}
