using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Classes
{
    public class Shadow
    {
        public bool Enable { get; set; }
        public int X { get; set; } = 5;
        public int Y { get; set; } = 5;
        public int Blur { get; set; } = 5;
        public int Size { get; set; } = 5;
        public string Color { get; set; } = "#000000bf";


        public void SetStyle(HtmlNode node)
        {
            if (Enable)
            {
                string styles = node.GetAttributeValue("style", "");

                styles += "box-shadow: " + X + "px " + Y + "px " + Blur + "px " + Size + "px " + Color + ";";

                node.SetAttributeValue("style", styles);
            }
        }
    }
}
