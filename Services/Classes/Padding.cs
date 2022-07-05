using HtmlAgilityPack;
using System.Collections.Generic;

namespace Services.Classes
{
    public class Padding
    {
        public List<PaddingValue> Values { get; set; }


        public void SetStyle(HtmlNode node)
        {
            string styles = node.GetAttributeValue("style", "");

            //if (Top != null) styles += "padding-top: " + Top + ";";
            //if (Right != null) styles += "padding-right: " + Right + ";";
            //if (Bottom != null) styles += "padding-bottom: " + Bottom + ";";
            //if (Left != null) styles += "padding-left: " + Left + ";";

            node.SetAttributeValue("style", styles);
        }
    }
}