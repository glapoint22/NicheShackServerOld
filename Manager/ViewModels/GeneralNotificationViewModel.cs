using DataAccess.Interfaces;
using DataAccess.Models;
using System.Linq;

namespace Manager.ViewModels
{
    public class GeneralNotificationViewModel: NotificationViewModel, IQueryableSelect<Notification, GeneralNotificationViewModel>
    {
        public string ProductThumbnail { get; set; }
        public string ProductName { get; set; }
        public int ProductId { get; set; }
        public int? VendorId { get; set; }
        public string Hoplink { get; set; }

        public new IQueryable<GeneralNotificationViewModel> Select(IQueryable<Notification> source)
        {

            NotificationViewModel notificationViewModel = base.Select(source).SingleOrDefault();

            return source.Select(x => new GeneralNotificationViewModel
            {
                Name = notificationViewModel.Name,
                CustomerText = notificationViewModel.CustomerText,
                Notes = notificationViewModel.Notes,
                ProductId = x.Product.Id,
                ProductName = x.Product.Name,
                ProductThumbnail = x.Product.Media.Url,
                VendorId = x.Product.VendorId,
                Hoplink = x.Product.Hoplink,
                Type = x.Type
            });
        }
    }
}
