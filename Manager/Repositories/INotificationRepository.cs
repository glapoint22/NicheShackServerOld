using DataAccess.Models;
using DataAccess.Repositories;
using Manager.Classes.Notifications;
using Services.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Repositories
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task<IEnumerable> GetNewNotifications();
        Task<NotificationMessage> GetMessageNotification(int type, int state, string email);
        Task<NotificationReviewComplaint> GetReviewComplaintNotification(int productId, int type, int state);
        Task<NotificationProduct> GetProductNotification(int productId, int type, int state);
    }
}
