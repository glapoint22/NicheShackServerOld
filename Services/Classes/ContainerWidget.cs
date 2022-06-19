using DataAccess.Models;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Classes
{
    public class ContainerWidget : Widget
    {
        public Background Background { get; set; }
        public Border Border { get; set; }
        public Corners Corners { get; set; }
        public Shadow Shadow { get; set; }
        public IEnumerable<Row> Rows { get; set; }


        public ContainerWidget()
        {
            Height = 250;
        }


        public async override Task<HtmlNode> Create(HtmlNode column, NicheShackContext context)
        {
            // Call the base
            HtmlNode widget = await base.Create(column, context);

            // Td
            HtmlNode td = widget.SelectSingleNode("tr/td");
            td.SetAttributeValue("height", Height.ToString());
            td.SetAttributeValue("style", "height: " + Height + "px;");

            // Set the styles
            if (Background != null) await Background.SetStyle(td, context);
            if (Border != null) Border.SetStyle(td);
            if (Corners != null) Corners.SetStyle(td);
            if (Shadow != null) Shadow.SetStyle(td);


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


                case "rows":
                    Rows = (IEnumerable<Row>)JsonSerializer.Deserialize(ref reader, typeof(IEnumerable<Row>), options);
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
