using System;
using System.Collections.Generic;

namespace Manager.Classes.Notifications
{
    public class ErrorNotification
    {
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public List<NotificationEmployee> EmployeeNotes { get; set; }
    }
}