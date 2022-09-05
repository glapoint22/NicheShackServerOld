using Manager.Classes.Notifications;
using System;

namespace Manager.Classes
{
    public class NotificationMessage : NotificationUser
    {
        public int MessageId { get; set; }
        public string SenderName { get; set; }
        public int? EmployeeReplyId { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
        public string EmployeeImage { get; set; }
        public DateTime ReplyDate { get; set; }
        public string Reply { get; set; }
    }
}