namespace Manager.Classes.Notifications
{
    public class NotificationSecurity : NotificationPerson
    {
        public string NoncompliantStrikes { get; set; }
        public bool BlockNotificationSending { get; set; }
    }
}