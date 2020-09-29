using HtmlAgilityPack;

namespace Services.Classes
{
    public class Caption
    {
        public string Text { get; set; }
        public string FontWeight { get; set; }
        public string FontStyle { get; set; }
        public string TextDecoration { get; set; }
        public string Color { get; set; } = "#c8c8c8";
        public string Font { get; set; } = "Arial, Helvetica, sans-serif";
        public string FontSize { get; set; } = "14";




        public void SetStyle(HtmlNode node)
        {
            string styles = node.GetAttributeValue("style", "");

            // Text
            HtmlTextNode textNode = new HtmlDocument().CreateTextNode(Text);
            node.AppendChild(textNode);



            // Font Weight
            if(FontWeight != null)
            {
                styles += "font-weight: " + FontWeight + ";";
            }


            // Font Style
            if (FontStyle != null)
            {
                styles += "font-style: " + FontStyle + ";";
            }



            // Text Decoration
            if (TextDecoration != null)
            {
                styles += "text-decoration: " + TextDecoration + ";";
            }



            // Color
            styles += "color: " + Color + ";";


            // Font
            styles += "font-family: " + Font + ";";


            // Font Size
            styles += "font-size: " + FontSize + "px;";


            // Text Break
            styles += "word-break: break-word;overflow-wrap: break-word;";


            node.SetAttributeValue("style", styles);
        }
    }
}
