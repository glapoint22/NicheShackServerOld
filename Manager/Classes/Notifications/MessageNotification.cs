using Manager.Classes.Notifications;
using System.Collections.Generic;

namespace Manager.Classes
{
    public class MessageNotification : NotificationUser
    {
        public int NotificationId { get; set; }
        public string NonAccountName { get; set; }


        public NotificationEmployee EmployeeMessage { get; set; }
    }
}