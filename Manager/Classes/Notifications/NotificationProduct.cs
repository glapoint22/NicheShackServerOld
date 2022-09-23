using System.Collections.Generic;

namespace Manager.Classes.Notifications
{
    public class NotificationProduct
    {
        public string ProductHoplink { get; set; }
        public bool ProductDisabled { get; set; }
        public IEnumerable<NotificationUser> Users { get; set; }
        public IEnumerable<NotificationProfile> Employees { get; set; }
    }
}