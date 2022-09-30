using System.Collections.Generic;

namespace Manager.Classes.Notifications
{
    public class NotificationUserImage : NotificationUser
    {
        public int NotificationId { get; set; }
        public string UserImage { get; set; }
        public int EmployeeIndex { get; set; }
        public List<NotificationEmployee> EmployeeNotes { get; set; }
    }
}