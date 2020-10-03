namespace Services.Classes
{
    public struct EmailProperties
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Host { get; set; }
        public string Link { get; set; }


        public string Set(string emailBody)
        {
            return emailBody
                .Replace("{host}", Host)
                .Replace("{firstName}", FirstName)
                .Replace("{lastName}", LastName)
                .Replace("{link}", Link);
        }
    }
}