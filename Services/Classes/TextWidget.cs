using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Services.Classes
{
    public class TextWidget : Widget
    {
        public Background Background { get; set; }
        public Padding Padding { get; set; }
        public string HtmlContent { get; set; }



        public override HtmlNode Create(HtmlNode column)
        {
            // Call the base
            HtmlNode textWidget = base.Create(column);

            HtmlNode td = textWidget.SelectSingleNode("tr/td");

            if(Height > 0)
            {
                td.SetAttributeValue("height", Height.ToString());
                td.SetAttributeValue("style", "height: " + Height + "px;");
            }
            

            Background.SetStyle(td);
            Padding.SetStyle(td);

            td.InnerHtml = HtmlContent;

            //HtmlDocument doc = new HtmlDocument();
            //doc.LoadHtml(HtmlContent);



            //// Td
            //HtmlNode td = textWidget.SelectSingleNode("tr/td");

            //td.AppendChild(doc.DocumentNode);

            return textWidget;
        }

        public override void SetProperty(Utf8JsonReader reader, JsonSerializerOptions options)
        {
            string property = reader.GetString();

            if (property == "background") BasePropertiesSet = true;

            if (!BasePropertiesSet)
            {
                base.SetProperty(reader, options);
            }
            else
            {
                switch (property)
                {
                    case "background":
                        Background = (Background)JsonSerializer.Deserialize(ref reader, typeof(Background), options);
                        break;
                    

                    case "padding":
                        Padding = (Padding)JsonSerializer.Deserialize(ref reader, typeof(Padding), options);
                        break;

                    case "htmlContent":
                        HtmlContent = (string)JsonSerializer.Deserialize(ref reader, typeof(string), options);
                        break;
                }
            }
        }
    }
}
