namespace Services.Classes
{
    public struct EmailProperties
    {
        public string Host { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }


        public string Set(string emailBody)
        {
            return emailBody
                .Replace("{host}", Host)
                .Replace("{firstName}", FirstName)
                .Replace("{lastName}", LastName);
        }
    }
}