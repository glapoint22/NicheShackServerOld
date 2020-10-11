namespace Services.Classes
{
    public class EmailMessage
    {
        public EmailType EmailType { get; set; }
        public string EmailAddress { get; set; }
        public string Subject { get; set; }
        public EmailProperties EmailProperties { get; set; }
    }
}
