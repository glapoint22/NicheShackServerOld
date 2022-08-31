using System.Collections.Generic;

namespace Manager.Classes.Notifications
{
    public class NotificationProduct
    {
        public IEnumerable<NotificationUser> Users { get; set; }
        public NotificationEmployee Employee { get; set; }
    }
}