namespace Services.Classes
{
    public struct EmailProperties
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Host { get; set; }
        public string Link { get; set; }
        public string List1 { get; set; }
        public string List2 { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public string ProductLink { get; set; }
        public string CollaboratorFirstName { get; set; }
        public string CollaboratorLastName { get; set; }
        public string Email1 { get; set; }
        public string Email2 { get; set; }


        public string Set(string emailBody)
        {
            return emailBody
                .Replace("{host}", Host)
                .Replace("{firstName}", FirstName)
                .Replace("{lastName}", LastName)
                .Replace("{link}", Link)
                .Replace("{list}", List1)
                .Replace("{list2}", List2)
                .Replace("{productName}", ProductName)
                .Replace("{imageUrl}", ImageUrl)
                .Replace("{productLink}", ProductLink)
                .Replace("{collaboratorFirstName}", CollaboratorFirstName)
                .Replace("{collaboratorLastName}", CollaboratorLastName)
                .Replace("{email1}", Email1)
                .Replace("{email2}", Email2);
        }
    }
}