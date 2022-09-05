using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Classes.Notifications
{
    public class NewNotificationEmployeeNote
    {
        public int MessageId { get; set; }
        public int ProductId { get; set; }
        public string Email { get; set; }
        public int NotificationType { get; set; }
        public DateTime? ArchiveDate { get; set; }
        public string Text { get; set; }
    }
}
