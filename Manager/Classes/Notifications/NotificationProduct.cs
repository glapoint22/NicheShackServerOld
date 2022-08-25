using System.Collections.Generic;

namespace Manager.Classes.Notifications
{
    public class NotificationProduct
    {
        public IEnumerable<NotificationProductUser> User { get; set; }
        public NotificationEmployee Employee { get; set; }
    }
}