using DataAccess.Models;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Classes
{
    public abstract class Widget
    {
        public string Name { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public WidgetType WidgetType { get; set; }
        public string HorizontalAlignment { get; set; }
        public List<Breakpoint> Breakpoints { get; set; }


        public virtual void SetProperty(string property, ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            switch (property)
            {
                case "width":
                    Width = (float)JsonSerializer.Deserialize(ref reader, typeof(float), options);
                    break;

                case "height":
                    Height = (float)JsonSerializer.Deserialize(ref reader, typeof(float), options);
                    break;

                case "horizontalAlignment":
                    HorizontalAlignment = (string)JsonSerializer.Deserialize(ref reader, typeof(string), options);
                    break;

                case "breakpoints":
                    Breakpoints = (List<Breakpoint>)JsonSerializer.Deserialize(ref reader, typeof(List<Breakpoint>), options);
                    break;
            }
        }


        public virtual async Task<HtmlNode> Create(HtmlNode column, NicheShackContext context)
        {
            // Create the table
            HtmlNode table = await Table.Create(column, new TableOptions
            {
                Width = Width,
                HorizontalAlignment = HorizontalAlignment,
                CreateRow = true
            }, context);


            HtmlNode td = table.SelectSingleNode("tr/td");
            td.SetAttributeValue("valign", "top");

            column.AppendChild(new HtmlDocument().CreateComment(Table.MicrosoftIf + "</td></tr></table>" + Table.MicrosoftEndIf));

            return table;
        }


        public abstract Task SetData(NicheShackContext context, QueryParams queryParams);
    }
}
