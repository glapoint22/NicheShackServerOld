using System.Collections.Generic;

namespace Manager.Classes.Notifications
{
    public class NotificationMessage
    {
        public IEnumerable<NotificationMessageUser> User { get; set; }
        public NotificationMessageEmployee Employee { get; set; }
    }
}