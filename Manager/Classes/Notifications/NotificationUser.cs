using System.Collections.Generic;

namespace Manager.Classes.Notifications
{
    public class NotificationUser : NotificationEmployee
    {
        public string UserId { get; set; }
        public int NoncompliantStrikes { get; set; }
        public bool BlockNotificationSending { get; set; }
    }
}