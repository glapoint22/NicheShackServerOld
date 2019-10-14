using System.Collections.Generic;

namespace Website.Classes
{
    public struct ProductGroup
    {
        public string Caption { get; set; }
        public IEnumerable<ProductDTO> Products { get; set; }
    }
}
