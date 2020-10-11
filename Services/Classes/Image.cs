using HtmlAgilityPack;

namespace Services.Classes
{
    public class Image
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public void SetStyle(HtmlNode node)
        {
            if(Name == "Product Placeholder")
            {
                Name = "{productName}";
                Url = "{productImage}";
            }
            else if(Name == "Stars Placeholder")
            {
                Name = "Product Rating";
                Url = "{stars}";
            }

            node.SetAttributeValue("src", "{host}/images/" + Url);
            node.SetAttributeValue("title", Name);
            node.SetAttributeValue("alt", Name);
        }
    }
}
