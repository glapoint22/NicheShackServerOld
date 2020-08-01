using System.Collections.Generic;
using Website.ViewModels;

namespace Website.Classes
{
    public struct ProductGroup
    {
        public string Caption { get; set; }
        public IEnumerable<ProductViewModel> Products { get; set; }
    }
}
