using DataAccess.Interfaces;
using DataAccess.Models;
using System.Linq;

namespace Manager.ViewModels
{
    public class ProductDescriptionNotificationViewModel: GeneralNotificationViewModel, ISelect<Notification, ProductDescriptionNotificationViewModel>
    {
        public string ProductDescription { get; set; }

        public new IQueryable<ProductDescriptionNotificationViewModel> ViewModelSelect(IQueryable<Notification> source)
        {
            GeneralNotificationViewModel generalNotificationViewModel = base.ViewModelSelect(source).SingleOrDefault();

            return source.Select(x => new ProductDescriptionNotificationViewModel
            {
                Name = generalNotificationViewModel.Name,
                CustomerText = generalNotificationViewModel.CustomerText,
                Notes = generalNotificationViewModel.Notes,
                ProductId = generalNotificationViewModel.ProductId,
                ProductName = generalNotificationViewModel.ProductName,
                ProductThumbnail = generalNotificationViewModel.ProductThumbnail,
                VendorId = generalNotificationViewModel.VendorId,
                Hoplink = generalNotificationViewModel.Hoplink,
                ProductDescription = x.Product.Description,
                Type = x.Type
            });
        }
    }
}
