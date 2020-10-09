using HtmlAgilityPack;

namespace Services.Classes
{
    public class Image
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public void SetStyle(HtmlNode node)
        {
            if(Name == "Placeholder")
            {
                Name = "{productName}";
                Url = "{imageUrl}";
            }

            node.SetAttributeValue("src", "{host}/images/" + Url);
            node.SetAttributeValue("title", Name);
            node.SetAttributeValue("alt", Name);
        }
    }
}
