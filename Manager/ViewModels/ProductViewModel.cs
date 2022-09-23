using DataAccess.ViewModels;
using Manager.Classes.Notifications;
using Services.Classes;
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
        public IEnumerable<NotificationItem> NotificationItems { get; set; }
        public IEnumerable<ProductContentViewModel> Content { get; set; }
        public IEnumerable<PricePointViewModel> PricePoints { get; set; }
        //public ImageViewModel Image { get; set; }
        public IEnumerable<ProductMediaViewModel> Media { get; set; }
        public IEnumerable<ItemViewModel> Keywords { get; set; }
        public IEnumerable<ItemViewModel> Subgroups { get; set; }
        public IEnumerable<SubproductViewModel> Components { get; set; }
        public IEnumerable<SubproductViewModel> Bonuses { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public int ShippingType { get; set; }
        public RecurringPayment RecurringPayment { get; set; }
    }
}
