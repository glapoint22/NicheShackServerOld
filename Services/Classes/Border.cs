using HtmlAgilityPack;

namespace Services.Classes
{
    public class Border
    {
        public int Width { get; set; } = 1;
        public string Style { get; set; } = "solid";
        public string Color { get; set; } = "#bebebe";
        public bool Enabled { get; set; }


        public void SetStyle(HtmlNode node)
        {
            string styles = node.GetAttributeValue("style", "");

            styles += "border: " + Width + "px " + Style + " " + Color + ";";
            node.SetAttributeValue("style", styles);
        }
    }
}
