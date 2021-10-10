using System.Collections.Generic;

namespace Services.Classes
{
    public class SplitSearchTerm
    {
        public string searchTerm { get; set; }
        public List<SuggestionCategory> Categories { get; set; }
        public float SearchVolume { get; set; }
        public List<SearchTerm> Parents { get; set; }
    }
}