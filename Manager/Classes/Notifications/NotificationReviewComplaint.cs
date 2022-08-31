using System.Collections.Generic;

namespace Manager.Classes.Notifications
{
    public class NotificationReviewComplaint
    {
        public IEnumerable<NotificationUser> Users { get; set; }
        public NotificationEmployee Employee { get; set; }
        public NotificationReviewWriter ReviewWriter { get; set; }
    }
}