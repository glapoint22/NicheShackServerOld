using HtmlAgilityPack;

namespace Services.Classes
{
    public class Caption
    {
        public string Text { get; set; }
        public string FontWeight { get; set; }
        public string FontStyle { get; set; }
        public string TextDecoration { get; set; }
        public string Color { get; set; }
        public KeyValue Font { get; set; }
        public KeyValue FontSize { get; set; }




        public void SetStyle(HtmlNode node)
        {
            string styles = node.GetAttributeValue("style", "");

            // Text
            HtmlTextNode textNode = new HtmlDocument().CreateTextNode(Text);
            node.AppendChild(textNode);



            // Font Weight
            if (FontWeight != null)
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
            styles += "font-family: " + (Font.Value != null ? Font.Value : "Arial, Helvetica, sans-serif") + ";";


            // Font Size
            styles += "font-size: " + (FontSize.Key != null ? FontSize.Key : "14") + "px;";


            // Text Break
            styles += "word-break: break-word;overflow-wrap: break-word;";


            node.SetAttributeValue("style", styles);
        }
    }
}
