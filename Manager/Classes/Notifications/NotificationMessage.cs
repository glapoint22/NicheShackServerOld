using System.Collections.Generic;

namespace Manager.Classes.Notifications
{
    public class NotificationMessage
    {
        public IEnumerable<NotificationMessageUser> Users { get; set; }
        public IEnumerable<NotificationMessageEmployee> Employee { get; set; }
    }
}