using Manager.Classes.Notifications;
using System.Collections.Generic;

namespace Manager.Classes
{
    public class NotificationMessage : NotificationUser
    {
        public int NotificationId { get; set; }
        public string UserName { get; set; }


        public NotificationEmployee EmployeeMessage { get; set; }
    }
}