using DataAccess.ViewModels;
using Manager.Classes;
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
        public IEnumerable<ProductPriceViewModel> Price { get; set; }
        public bool IsMultiPrice { get; set; }
        public ImageViewModel Image { get; set; }
        public IEnumerable<ProductMediaViewModel> Media { get; set; }
        public IEnumerable<ItemViewModel> Keywords { get; set; }
        public IEnumerable<ItemViewModel> Subgroups { get; set; }
    }
}
