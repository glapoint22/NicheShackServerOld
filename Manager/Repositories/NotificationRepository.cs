using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Repositories
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        private readonly NicheShackContext context;

        public NotificationRepository(NicheShackContext context) : base(context)
        {
            this.context = context;
        }



        public async Task<IEnumerable> GetNewNotifications()
        {

            var messageNotifications = await context.Notifications.Where(x => x.State == 0 && x.Type == 0 && x.NotificationText.Select(z => z.Email).FirstOrDefault() != null).Select(x => new
            {
                Thumbnail = x.Product.Media.Thumbnail,
                Type = 0,
                Email = x.NotificationText.Select(y => y.Email).FirstOrDefault(),
                count = context.NotificationText.Where(y => y.Notification.State == x.State && y.Notification.Type == x.Type && y.Email == x.NotificationText.Select(z => z.Email).FirstOrDefault()).Count()
            }).ToListAsync();


            var notifications = await context.Notifications.Where(x => x.State == 0 && x.Type != 0).Select(x => new
            {
                Thumbnail = x.Product.Media.Thumbnail,
                Type = x.Type,
                Email = x.NotificationText.Select(y => y.Email).FirstOrDefault(),
                count = context.Notifications.Where(y => y.State == x.State && y.ProductId == x.ProductId && y.Type == x.Type).Count()
            }).ToListAsync();


            var newNotifications = notifications.Concat(messageNotifications).Distinct();

            return newNotifications;
        }













    }
}
