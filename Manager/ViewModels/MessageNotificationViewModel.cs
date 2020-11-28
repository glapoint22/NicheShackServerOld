using DataAccess.Interfaces;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.ViewModels
{
    public class MessageNotificationViewModel : NotificationViewModel, IQueryableSelect<Notification, MessageNotificationViewModel>
    {

        public string MessengerName { get; set; }
        public string Email { get; set; }


        public new IQueryable<MessageNotificationViewModel> Select(IQueryable<Notification> source)
        {

            NotificationViewModel notificationViewModel = base.Select(source).SingleOrDefault();

            return source.Select(x => new MessageNotificationViewModel
            {
                Type = notificationViewModel.Type,
                Name = notificationViewModel.Name,
                CustomerText = notificationViewModel.CustomerText,
                Notes = notificationViewModel.Notes,
                MessengerName = x.NotificationText
                .Where(z => z.NotificationId == x.Id && z.Type == 0)
                .Select(z => z.Name).SingleOrDefault(),
                Email = x.NotificationText
                .Where(z => z.NotificationId == x.Id && z.Type == 0)
                .Select(z => z.Email).SingleOrDefault()
            });
        }
    }
}
