using DataAccess.Models;
using DataAccess.Repositories;
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

    }
}
