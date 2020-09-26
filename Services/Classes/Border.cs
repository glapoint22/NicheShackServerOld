using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Classes
{
    public class Border
    {
        public bool Enable { get; set; }
        public int Width { get; set; } = 1;
        public string Style { get; set; } = "solid";
        public string Color { get; set; } = "#bebebe";

        public void SetStyle(HtmlNode node)
        {
            if(Enable)
            {
                string styles = node.GetAttributeValue("style", "");

                styles += "border: " + Width + "px " + Style + " " + Color + ";";

                node.SetAttributeValue("style", styles);
            }
        }
    }
}
