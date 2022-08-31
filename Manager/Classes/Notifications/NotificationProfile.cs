using System;

namespace Manager.Classes.Notifications
{
    public class NotificationProfile
    {
        public DateTime Date { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Image { get; set; }
        public string Email { get; set; }
        public string Text { get; set; }
    }
}