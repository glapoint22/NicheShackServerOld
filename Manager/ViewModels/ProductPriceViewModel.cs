﻿using Services.Classes;
using System.Collections.Generic;

namespace Manager.ViewModels
{
    public struct ProductPriceViewModel
    {
        public int Id { get; set; }
        public ImageViewModel Image { get; set; }
        public string Header { get; set; }
        public string Quantity { get; set; }
        public string UnitPrice { get; set; }
        public string Unit { get; set; }
        public string StrikethroughPrice { get; set; }
        public double Price { get; set; }
        public IEnumerable<AdditionalInfoViewModel> AdditionalInfo { get; set; }

    }
}
