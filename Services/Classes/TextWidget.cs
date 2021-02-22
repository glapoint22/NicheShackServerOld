using DataAccess.Models;
using HtmlAgilityPack;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Classes
{
    public class TextWidget : Widget
    {
        public Background Background { get; set; }
        public Padding Padding { get; set; }
        public string HtmlContent { get; set; }



        public async override Task<HtmlNode> Create(HtmlNode column, NicheShackContext context)
        {
            // Call the base
            HtmlNode textWidget = await base.Create(column, context);

            // Select the td node
            HtmlNode td = textWidget.SelectSingleNode("tr/td");

            // Height
            if (Height > 0)
            {
                td.SetAttributeValue("height", Height.ToString());
                td.SetAttributeValue("style", "height: " + Height + "px;");
            }

            // Apply the styles
            td.SetAttributeValue("style", "font-family: Arial, Helvetica, sans-serif;font-size: 14px;color: #000000;line-height: normal;");
            if (Background != null) await Background.SetStyle(td, context);
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




        public override async Task SetData(NicheShackContext context, QueryParams queryParams)
        {
            if (Background != null && Background.Image != null)
            {
                await Background.Image.SetData(context);
            }
        }
    }
}
