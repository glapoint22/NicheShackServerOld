﻿using System.Collections.Generic;

namespace Manager.Classes.Notifications
{
    public class NotificationUserAccount : NotificationUser
    {
        public int NotificationId { get; set; }
        public int EmployeeIndex { get; set; }
        public List<NotificationEmployee> EmployeeNotes { get; set; }
    }
}