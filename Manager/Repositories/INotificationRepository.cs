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
        Task<List<UserNameNotification>> GetUserNameNotification(int notificationGroupId, bool isNew);
        Task<List<UserImageNotification>> GetUserImageNotification(int notificationGroupId, bool isNew);
        Task<List<MessageNotification>> GetMessageNotification(int notificationGroupId, bool isNew);
        Task<ReviewNotification> GetReviewNotification(int notificationGroupId);
        Task<ProductNotification> GetProductNotification(int notificationGroupId);
        Task<ErrorNotification> GetErrorNotification(int notificationGroupId);
    }
}