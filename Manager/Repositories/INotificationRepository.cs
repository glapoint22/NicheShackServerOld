using DataAccess.Models;
using DataAccess.Repositories;
using Manager.Classes;
using Manager.Classes.Notifications;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Manager.Repositories
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task<IEnumerable> GetNotifications(bool isNew);
        Task<List<NotificationMessage>> GetMessageNotification(string email, int type, DateTime? archiveDate);
        Task<NotificationReview> GetReviewNotification(int productId, int type, DateTime? archiveDate);
        Task<NotificationProduct> GetProductNotification(int productId, int type, DateTime? archiveDate);
    }
}