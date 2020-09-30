using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Services.Classes
{
    public class ContainerWidget : Widget
    {
        public Background Background { get; set; }
        public Border Border { get; set; }
        public Corners Corners { get; set; }
        public Shadow Shadow { get; set; }
        public Padding Padding { get; set; }
        public IEnumerable<Row> Rows { get; set; }


        public ContainerWidget()
        {
            Height = 250;
        }


        public override HtmlNode Create(HtmlNode column)
        {
            // Call the base
            HtmlNode widget = base.Create(column);

            // Td
            HtmlNode td = widget.SelectSingleNode("tr/td");
            td.SetAttributeValue("height", Height.ToString());
            td.SetAttributeValue("style", "height: " + Height + "px;");

            // Set the styles
            if (Background != null) Background.SetStyle(td);
            if (Border != null) Border.SetStyle(td);
            if (Corners != null) Corners.SetStyle(td);
            if (Shadow != null) Shadow.SetStyle(td);
            if (Padding != null) Padding.SetStyle(td);


            HtmlNode container = Table.Create(td);


            return container;
        }

        public override void SetProperty(string property, ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            base.SetProperty(property, ref reader, options);


            switch (property)
            {
                case "background":
                    Background = (Background)JsonSerializer.Deserialize(ref reader, typeof(Background), options);
                    break;

                case "border":
                    Border = (Border)JsonSerializer.Deserialize(ref reader, typeof(Border), options);
                    break;

                case "corners":
                    Corners = (Corners)JsonSerializer.Deserialize(ref reader, typeof(Corners), options);
                    break;

                case "shadow":
                    Shadow = (Shadow)JsonSerializer.Deserialize(ref reader, typeof(Shadow), options);
                    break;

                case "padding":
                    Padding = (Padding)JsonSerializer.Deserialize(ref reader, typeof(Padding), options);
                    break;

                case "rows":
                    Rows = (IEnumerable<Row>)JsonSerializer.Deserialize(ref reader, typeof(IEnumerable<Row>), options);
                    break;
            }
        }

    }
}
