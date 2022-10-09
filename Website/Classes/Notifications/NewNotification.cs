namespace Website.Classes.Notifications
{
    public class NewNotification
    {
        public int? ProductId { get; set; }
        public int? ReviewId { get; set; }
        public int Type { get; set; }
        public string UserName { get; set; }
        public string UserImage { get; set; }
        public string Text { get; set; }
        public string Email { get; set; }
        public string NonAccountName { get; set; }
        public string NonAccountEmail { get; set; }
    }
}