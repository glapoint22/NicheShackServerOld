using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Classes.Notifications
{
    public class NewNotificationEmployeeNote
    {
        public int NotificationGroupId { get; set; }
        public int? NotificationId { get; set; }
        public string Note { get; set; }
    }
}
