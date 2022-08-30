using System.Collections.Generic;

namespace Manager.Classes.Notifications
{
    public class NotificationProduct
    {
        public IEnumerable<NotificationProductUser> Users { get; set; }
        public NotificationEmployeeDetails Employee { get; set; }
    }
}