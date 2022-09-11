namespace Manager.Classes.Notifications
{
    public class NotificationUser : NotificationProfile
    {
        public string UserId { get; set; }
        public int NoncompliantStrikes { get; set; }
        public bool BlockNotificationSending { get; set; }
    }
}