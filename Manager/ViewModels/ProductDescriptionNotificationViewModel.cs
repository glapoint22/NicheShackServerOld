using DataAccess.Interfaces;
using DataAccess.Models;
using Manager.Interfaces;
using System.Linq;

namespace Manager.ViewModels
{
    public class ProductDescriptionNotificationViewModel: GeneralNotificationViewModel, ISelect<Notification, ProductDescriptionNotificationViewModel>, INotification
    {
        public string ProductDescription { get; set; }

        public new IQueryable<ProductDescriptionNotificationViewModel> ViewModelSelect(IQueryable<Notification> source)
        {
            return source.Select(x => new ProductDescriptionNotificationViewModel
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

                ProductDescription = x.Product.Description,


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
