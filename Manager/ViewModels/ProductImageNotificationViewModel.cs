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
            return source.Select(x => new ProductImageNotificationViewModel
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


                Image = new ImageViewModel
                {
                    Id = x.Product.Media.Id,
                    Name = x.Product.Media.Name,
                    Url = x.Product.Media.Url
                },


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
