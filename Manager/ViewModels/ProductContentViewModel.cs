using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.ViewModels
{
    public class ProductContentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ImageViewModel Icon { get; set; }
        public IEnumerable<bool> PriceIndices { get; set; }
    }
}
