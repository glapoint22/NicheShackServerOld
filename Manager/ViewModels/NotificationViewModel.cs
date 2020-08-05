using DataAccess.Classes;
using DataAccess.Interfaces;
using DataAccess.Models;
using Manager.Interfaces;
using System.Linq;

namespace Manager.ViewModels
{
    public class NotificationViewModel: ISelect<Notification, NotificationViewModel>, INotification
    {
        public NotificationTextViewModel CustomerText { get; set; }
        public NotificationTextViewModel Notes { get; set; }


        public IQueryable<NotificationViewModel> ViewModelSelect(IQueryable<Notification> source)
        {
            return source.Select(x => new NotificationViewModel
            {
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
