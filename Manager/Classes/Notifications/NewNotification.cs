using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Classes.Notifications
{
    public class NewNotification
    {
        public string UserId { get; set; }
        public int NotificationType { get; set; }
        public string UserName { get; set; }
        public string UserImage { get; set; }
        public string EmployeeNotes { get; set; }
    }
}
