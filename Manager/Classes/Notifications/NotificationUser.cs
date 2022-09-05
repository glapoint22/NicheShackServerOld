namespace Manager.Classes.Notifications
{
    public class NotificationUser : NotificationProfile
    {
        public int NoncompliantStrikes { get; set; }
        public bool BlockNotificationSending { get; set; }
    }
}