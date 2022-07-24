using DataAccess.Interfaces;
using DataAccess.Models;
using System.Linq;

namespace Manager.ViewModels
{
    public class ProductImageNotificationViewModel: GeneralNotificationViewModel, IQueryableSelect<Notification, ProductImageNotificationViewModel>
    {
        public ImageViewModel Image { get; set; }

        public new IQueryable<ProductImageNotificationViewModel> Select(IQueryable<Notification> source)
        {
            GeneralNotificationViewModel generalNotificationViewModel = base.Select(source).SingleOrDefault();


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
                    Src = x.Product.Media.Image
                },
                Type = x.Type
            });
        }
    }
}
