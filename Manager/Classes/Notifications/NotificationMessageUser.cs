using Manager.Classes.Notifications;

namespace Manager.Classes
{
    public class NotificationMessageUser : NotificationIdentity
    {
        public string Name { get; set; }
        public string Message { get; set; }
    }
}