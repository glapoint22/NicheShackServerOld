using static Services.EmailService;

namespace Services.Classes
{
    public struct EmailProperties
    {
        public EmailType EmailType { get; set; }
        public string Host { get; set; }
        public string CustomerName { get; set; }
    }
}
