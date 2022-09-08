using System.Collections.Generic;

namespace Manager.Classes.Notifications
{
    public class NotificationReview
    {
        public IEnumerable<NotificationUser> Users { get; set; }
        public NotificationReviewWriter ReviewWriter { get; set; }
        public IEnumerable<NotificationProfile> Employees { get; set; }
    }
}