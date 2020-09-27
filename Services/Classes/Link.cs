using HtmlAgilityPack;

namespace Services.Classes
{
    public struct Link
    {
        public LinkOption SelectedOption { get; set; }
        public string Url { get; set; }


        public void SetStyle(HtmlNode node)
        {
            node.SetAttributeValue("href", SelectedOption == LinkOption.WebAddress ? Url : "{0}/" + Url);
            node.SetAttributeValue("target", "_blank");
        }
    }
}
