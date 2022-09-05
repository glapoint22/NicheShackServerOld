using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Classes.Notifications
{
    public class ArchiveNotification
    {
        public int ProductId { get; set; }
        public string Email { get; set; }
        public int NotificationType { get; set; }
    }
}
