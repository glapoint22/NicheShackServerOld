using DataAccess.Interfaces;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.ViewModels
{
    public class ProductMediaNotificationViewModel: GeneralNotificationViewModel, ISelect<Notification, ProductMediaNotificationViewModel>
    {
        public IEnumerable<MediaViewModel> Media { get; set; }

        public new IQueryable<ProductMediaNotificationViewModel> ViewModelSelect(IQueryable<Notification> source)
        {
            return source.Select(x => new ProductMediaNotificationViewModel
            {
                ProductId = x.Product.Id,
                ProductName = x.Product.Name,
                ProductThumbnail = x.Product.Media.Url,
                VendorId = x.Product.VendorId,
                Hoplink = x.Product.Hoplink,


                CustomerText = x.NotificationText
                .Where(z => z.NotificationId == x.Id && z.Type == 0)
                .Select(z => new NotificationTextViewModel
                {
                    TimeStamp = z.TimeStamp,
                    Thumbnail = z.Customer.image,
                    Text = z.Text
                })
                .SingleOrDefault(),

                Media = x.Product.ProductMedia.Select(m => new MediaViewModel
                {
                    Id = m.Media.Id,
                    Name = m.Media.Name,
                    Url = m.Media.Url,
                    Thumbnail = m.Media.Thumbnail,
                    Type = (MediaType)m.Media.Type
                }),


                Notes = x.NotificationText
                .Where(z => z.NotificationId == x.Id && z.Type == 1)
                .Select(z => new NotificationTextViewModel
                {
                    TimeStamp = z.TimeStamp,
                    Thumbnail = z.Customer.image,
                    Text = z.Text
                })
                .SingleOrDefault()
            });
        }
    }
}
