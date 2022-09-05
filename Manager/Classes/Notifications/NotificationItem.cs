using System;

namespace Manager.Classes.Notifications
{
    public class NotificationItem
    {
        public bool IsNew { get; set; }
        public int? ProductId { get; set; }
        public string productName { get; set; }
        public string Thumbnail { get; set; }
        public int Type { get; set; }
        public string Email { get; set; }
        public int Count { get; set; }
        public DateTime Date { get; set; }
        public DateTime? ArchiveDate { get; set; }
    }
}
