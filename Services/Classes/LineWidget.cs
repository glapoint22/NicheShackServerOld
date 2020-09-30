using HtmlAgilityPack;
using System.Text.Json;

namespace Services.Classes
{
    public class LineWidget : Widget
    {
        public Border Border { get; set; }
        public Shadow Shadow { get; set; }


        public override HtmlNode Create(HtmlNode column)
        {
            // Call the base
            HtmlNode widget = base.Create(column);

            // Td
            HtmlNode td = widget.SelectSingleNode("tr/td");


            td.SetAttributeValue("style", "border-bottom: " + Border.Width + "px " + Border.Style + " " + Border.Color + ";");
            if (Shadow != null) Shadow.SetStyle(td);


            return widget;
        }




        public override void SetProperty(string property, ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            base.SetProperty(property, ref reader, options);

            switch (property)
            {
                case "border":
                    Border = (Border)JsonSerializer.Deserialize(ref reader, typeof(Border), options);
                    break;

                case "shadow":
                    Shadow = (Shadow)JsonSerializer.Deserialize(ref reader, typeof(Shadow), options);
                    break;
            }
        }
    }
}
