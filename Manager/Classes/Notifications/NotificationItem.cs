using System;

namespace Manager.Classes.Notifications
{
    public class NotificationItem
    {
        public int NotificationGroupId { get; set; }
        public int NotificationType { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public bool IsNew { get; set; }
        public int Count { get; set; }
    }
}
