﻿using System.Collections.Generic;

namespace Services.Classes
{
    public struct CategoryFilter
    {
        public string UrlId { get; set; }
        public string Name { get; set; }
        public string UrlName { get; set; }
        public NichesFilter Subniches { get; set; }
    }
}
