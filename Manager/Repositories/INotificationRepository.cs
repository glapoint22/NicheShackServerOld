using DataAccess.Models;
using DataAccess.Repositories;
using Manager.Classes;
using Manager.Classes.Notifications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Manager.Repositories
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task<List<NotificationItem>> GetNotifications(bool isNew);
        Task<List<NotificationMessage>> GetMessageNotification(int notificationGroupId, bool isNew);
        Task<NotificationReview> GetReviewNotification(int notificationGroupId);
        Task<NotificationProduct> GetProductNotification(int notificationGroupId);
        Task GetUserImageNotification(int notificationGroupId);
    }
}