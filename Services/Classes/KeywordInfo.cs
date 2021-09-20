using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Classes
{
    public class KeywordInfo
    {
        public string Name { get; set; }
        public int SearchVolume { get; set; }
        public List<KeywordProduct> Products { get; set; }
        public SearchWordCategory Category { get; set; }

    }
}
