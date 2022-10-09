using DataAccess.Models;
using DataAccess.Repositories;
using System.Threading.Tasks;
using Website.Classes.Notifications;

namespace Website.Repositories
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task CreateNotification(NewNotification newNotification, string userId);
    }
}