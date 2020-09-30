using HtmlAgilityPack;
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

            // Select the td node
            HtmlNode td = textWidget.SelectSingleNode("tr/td");

            // Height
            if (Height > 0)
            {
                td.SetAttributeValue("height", Height.ToString());
                td.SetAttributeValue("style", "height: " + Height + "px;");
            }

            // Apply the styles
            if (Background != null) Background.SetStyle(td);
            if (Padding != null) Padding.SetStyle(td);

            // Assign the content
            td.InnerHtml = HtmlContent;

            return textWidget;
        }

        public override void SetProperty(string property, ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            base.SetProperty(property, ref reader, options);

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
