using System.Collections.Generic;

namespace Manager.Classes.Notifications
{
    public class NotificationReviewComplaint
    {
        public IEnumerable<NotificationReviewUser> Users { get; set; }
        public NotificationEmployeeDetails Employee { get; set; }
        public NotificationReviewWriter ReviewWriter { get; set; }
    }
}