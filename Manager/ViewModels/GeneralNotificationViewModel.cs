using DataAccess.Interfaces;
using DataAccess.Models;
using Manager.Interfaces;
using System.Linq;

namespace Manager.ViewModels
{
    public class GeneralNotificationViewModel: NotificationViewModel, ISelect<Notification, GeneralNotificationViewModel>, INotification
    {
        public string ProductThumbnail { get; set; }
        public string ProductName { get; set; }
        public int ProductId { get; set; }
        public int VendorId { get; set; }
        public string Hoplink { get; set; }

        public new IQueryable<GeneralNotificationViewModel> ViewModelSelect(IQueryable<Notification> source)
        {
            return source.Select(x => new GeneralNotificationViewModel
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
