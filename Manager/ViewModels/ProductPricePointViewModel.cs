using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.ViewModels
{
    public struct ProductPricePointViewModel
    {
        public int Id { get; set; }
        public string TextBefore { get; set; }
        public int WholeNumber { get; set; }
        public int Decimal { get; set; }
        public string TextAfter { get; set; }
    }
}
