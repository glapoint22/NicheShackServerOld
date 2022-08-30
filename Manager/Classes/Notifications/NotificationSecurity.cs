namespace Manager.Classes.Notifications
{
    public class NotificationSecurity : NotificationProfile
    {
        public int NoncompliantStrikes { get; set; }
        public bool BlockNotificationSending { get; set; }
    }
}