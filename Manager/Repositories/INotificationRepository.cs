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
    public interface INotificationRepository : IRepository<DataAccess.Models.Notification>
    {
        Task<IEnumerable> GetNewNotifications();
        Task<IEnumerable> GetMessageNotification(int type, int state, string email);
        Task<NotificationReviewComplaint> GetReviewComplaintNotification(int productId, int type, int state);
        Task<IEnumerable> GetProductNotification(int productId, int type, int state);
    }
}
