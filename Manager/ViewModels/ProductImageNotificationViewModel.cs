using DataAccess.Interfaces;
using DataAccess.Models;
using System.Linq;

namespace Manager.ViewModels
{
    public class ProductImageNotificationViewModel: GeneralNotificationViewModel, ISelect<Notification, ProductImageNotificationViewModel>
    {
        public ImageViewModel Image { get; set; }

        public new IQueryable<ProductImageNotificationViewModel> ViewModelSelect(IQueryable<Notification> source)
        {
            GeneralNotificationViewModel generalNotificationViewModel = base.ViewModelSelect(source).SingleOrDefault();


            return source.Select(x => new ProductImageNotificationViewModel
            {
                Name = generalNotificationViewModel.Name,
                CustomerText = generalNotificationViewModel.CustomerText,
                Notes = generalNotificationViewModel.Notes,
                ProductId = generalNotificationViewModel.ProductId,
                ProductName = generalNotificationViewModel.ProductName,
                ProductThumbnail = generalNotificationViewModel.ProductThumbnail,
                VendorId = generalNotificationViewModel.VendorId,
                Hoplink = generalNotificationViewModel.Hoplink,
                Image = new ImageViewModel
                {
                    Id = x.Product.Media.Id,
                    Name = x.Product.Media.Name,
                    Url = x.Product.Media.Url
                }
            });
        }
    }
}
