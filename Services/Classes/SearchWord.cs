using System.Collections.Generic;

namespace Services.Classes
{
    public class SearchWord
    {
        public string Name { get; set; }
        public List<SuggestionCategory> Categories { get; set; }
        public float SearchVolume { get; set; }
    }
}
