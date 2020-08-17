using DataAccess.Classes;
using DataAccess.Interfaces;
using DataAccess.Models;
using Manager.Classes;
using System.Linq;

namespace Manager.ViewModels
{
    public class NotificationViewModel: ISelect<Notification, NotificationViewModel>
    {
        public string Name { get; set; }
        public NotificationTextViewModel CustomerText { get; set; }
        public NotificationTextViewModel Notes { get; set; }
        public int Type { get; set; }


        public IQueryable<NotificationViewModel> ViewModelSelect(IQueryable<Notification> source)
        {
            return source.Select(x => new NotificationViewModel
            {
                Name = Utility.GetNotificationName(x.Type),
                CustomerText = x.NotificationText
                .Where(z => z.NotificationId == x.Id && z.Type == 0)
                .Select(z => new NotificationTextViewModel
                {
                    TimeStamp = z.TimeStamp.ToString("MMMM dd, yyyy hh:mm tt"),
                    Thumbnail = z.Customer.image,
                    Text = z.Text
                })
                .SingleOrDefault(),




                Notes = x.NotificationText
                .Where(z => z.NotificationId == x.Id && z.Type == 1)
                .Select(z => new NotificationTextViewModel
                {
                    TimeStamp = z.TimeStamp.ToString("MMMM dd, yyyy hh:mm tt"),
                    Thumbnail = z.Customer.image,
                    Text = z.Text
                })
                .SingleOrDefault(),
                Type = x.Type
            });
        }
    }
}
