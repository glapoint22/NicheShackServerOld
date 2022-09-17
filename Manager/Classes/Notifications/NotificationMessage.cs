using Manager.Classes.Notifications;
using System;

namespace Manager.Classes
{
    public class NotificationMessage : NotificationUser
    {
        public int NotificationId { get; set; }
        public string UserName { get; set; }
        public int? EmployeeMessageId { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
        public string EmployeeImage { get; set; }
        public DateTime? EmployeeMessageDate { get; set; }
        public string EmployeeMessage { get; set; }
    }
}