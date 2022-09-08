using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Models
{
    public class NotificationGroup
    {
        public int Id { get; set; }
        public DateTime? ArchiveDate { get; set; }


        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<NotificationEmployeeNote> NotificationEmployeeNotes { get; set; }


        public NotificationGroup()
        {
            Notifications = new HashSet<Notification>();
            NotificationEmployeeNotes = new HashSet<NotificationEmployeeNote>();
        }
    }
}