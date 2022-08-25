using System.Collections.Generic;

namespace Manager.Classes.Notifications
{
    public class NotificationReviewComplaint
    {
        public IEnumerable<NotificationReviewUser> User { get; set; }
        public NotificationEmployee Employee { get; set; }
        public NotificationReviewWriter ReviewWriter { get; set; }
    }
}