using System.Collections.Generic;

namespace Manager.Classes.Notifications
{
    public class NotificationReview
    {
        public int ReviewId { get; set; }
        public bool ReviewDeleted { get; set; }
        public IEnumerable<NotificationUser> Users { get; set; }
        public NotificationReviewWriter ReviewWriter { get; set; }
        public IEnumerable<NotificationEmployee> Employees { get; set; }
    }
}