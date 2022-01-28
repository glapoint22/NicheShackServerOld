namespace Services.Classes
{
    public class EmailProperties
    {
        public Recipient Recipient { get; set; }
        public string Host { get; set; }
        public string Link { get; set; }
        public Person Person { get; set; }
        public string Var1 { get; set; }
        public string Var2 { get; set; }
        public string Stars { get; set; }
        public ProductData Product { get; set; }

        public string Set(string emailBody)
        {
            return emailBody
                .Replace("{host}", Host)
                .Replace("{recipientFirstName}", Recipient.FirstName)
                .Replace("{recipientLastName}", Recipient.LastName)
                .Replace("{link}", Link)
                .Replace("{var1}", Var1)
                .Replace("{var2}", Var2)
                .Replace("{productName}", Product != null ? Product.Name : null)
                .Replace("{productImage}", Product != null ? Product.Image : null)
                .Replace("{productUrl}", Product != null ? Product.Url : null)
                .Replace("{personFirstName}", Person != null ? Person.FirstName : null)
                .Replace("{personLastName}", Person != null ? Person.LastName : null)
                .Replace("{stars}", Stars);
        }
    }
}