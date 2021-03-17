using HtmlAgilityPack;

namespace Services.Classes
{
    public class Padding
    {
        public string Top { get; set; }
        public string Right { get; set; }
        public string Bottom { get; set; }
        public string Left { get; set; }
        public bool Constrain { get; set; }


        public void SetStyle(HtmlNode node)
        {
            string styles = node.GetAttributeValue("style", "");

            if (Top != null) styles += "padding-top: " + Top + ";";
            if (Right != null) styles += "padding-right: " + Right + ";";
            if (Bottom != null) styles += "padding-bottom: " + Bottom + ";";
            if (Left != null) styles += "padding-left: " + Left + ";";

            node.SetAttributeValue("style", styles);
        }
    }
}