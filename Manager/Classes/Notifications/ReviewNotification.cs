using System.Collections.Generic;

namespace Manager.Classes.Notifications
{
    public class ReviewNotification
    {
        public int ReviewId { get; set; }
        public bool ReviewDeleted { get; set; }
        public IEnumerable<NotificationUser> Users { get; set; }
        public NotificationReviewWriter ReviewWriter { get; set; }
        public IEnumerable<NotificationEmployee> EmployeeNotes { get; set; }
    }
}