using DataAccess.ViewModels;
using System.Collections.Generic;

namespace Manager.ViewModels
{
    public struct ProductViewModel
    {
        public int Id { get; set; }
        public ItemViewModel Vendor { get; set; }
        public string Hoplink { get; set; }
        public string Name { get; set; }
        public double Rating { get; set; }
        public int TotalReviews { get; set; }
        public string Description { get; set; }
        public IEnumerable<ProductContentViewModel> Content { get; set; }
        public IEnumerable<ProductPricePointViewModel> PricePoints { get; set; }
        public ImageViewModel Image { get; set; }
        public IEnumerable<ProductMediaViewModel> Media { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public IEnumerable<ItemViewModel> Keywords { get; set; }
    }
}
