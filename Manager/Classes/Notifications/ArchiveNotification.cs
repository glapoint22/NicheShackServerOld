namespace Manager.Classes.Notifications
{
    public class ArchiveNotification
    {
        public int NotificationGroupId { get; set; }
        public int NotificationId { get; set; }
        public bool ArchiveAllMessagesInGroup { get; set; }
        public bool Restore { get; set; }
        public bool RestoreAllMessagesInGroup { get; set; }
    }
}
