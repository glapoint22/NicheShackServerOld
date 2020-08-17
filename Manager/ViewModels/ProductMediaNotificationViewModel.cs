using DataAccess.Interfaces;
using DataAccess.Models;
using System.Collections.Generic;
using System.Linq;
using static Manager.Classes.Utility;

namespace Manager.ViewModels
{
    public class ProductMediaNotificationViewModel: GeneralNotificationViewModel, ISelect<Notification, ProductMediaNotificationViewModel>
    {
        public IEnumerable<MediaViewModel> Media { get; set; }

        public new IQueryable<ProductMediaNotificationViewModel> ViewModelSelect(IQueryable<Notification> source)
        {

            GeneralNotificationViewModel generalNotificationViewModel = base.ViewModelSelect(source).SingleOrDefault();

            return source.Select(x => new ProductMediaNotificationViewModel
            {
                Name = generalNotificationViewModel.Name,
                CustomerText = generalNotificationViewModel.CustomerText,
                Notes = generalNotificationViewModel.Notes,
                ProductId = generalNotificationViewModel.ProductId,
                ProductName = generalNotificationViewModel.ProductName,
                ProductThumbnail = generalNotificationViewModel.ProductThumbnail,
                VendorId = generalNotificationViewModel.VendorId,
                Hoplink = generalNotificationViewModel.Hoplink,
                Media = x.Product.ProductMedia.Select(m => new MediaViewModel
                {
                    Id = m.Media.Id,
                    Name = m.Media.Name,
                    Url = m.Media.Url,
                    Thumbnail = m.Media.Thumbnail,
                    Type = m.Media.Type
                }),
                Type = x.Type
            });
        }
    }
}
