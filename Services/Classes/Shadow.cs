using HtmlAgilityPack;

namespace Services.Classes
{
    public class Shadow
    {
        public int X { get; set; } = 5;
        public int Y { get; set; } = 5;
        public int Blur { get; set; } = 5;
        public int Size { get; set; } = 5;
        public string Color { get; set; } = "#000000bf";
        public bool Enabled { get; set; }


        public void SetStyle(HtmlNode node)
        {
            string styles = node.GetAttributeValue("style", "");

            styles += "box-shadow: " + X + "px " + Y + "px " + Blur + "px " + Size + "px " + Color + ";";
            node.SetAttributeValue("style", styles);
        }
    }
}