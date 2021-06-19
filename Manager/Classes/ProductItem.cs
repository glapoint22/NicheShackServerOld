using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Classes
{
    public class ProductItem
    {
        public int ProductId { get; set; }
        public int ItemId { get; set; }

        public bool IsMultiPrice { get; set; }
    }
}
