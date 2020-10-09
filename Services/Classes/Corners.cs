using HtmlAgilityPack;

namespace Services.Classes
{
    public class Corners
    {
        public int TopLeft { get; set; }
        public int TopRight { get; set; }
        public int BottomRight { get; set; }
        public int BottomLeft { get; set; }



        public void SetStyle(HtmlNode node)
        {
            string styles = node.GetAttributeValue("style", "");

            if (TopLeft > 0) styles += "border-top-left-radius: " + TopLeft + "px;";
            if (TopRight > 0) styles += "border-top-right-radius: " + TopRight + "px;";
            if (BottomRight > 0) styles += "border-bottom-right-radius: " + BottomRight + "px;";
            if (BottomLeft > 0) styles += "border-bottom-left-radius: " + BottomLeft + "px;";

            node.SetAttributeValue("style", styles);
        }
    }
}
