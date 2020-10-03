using HtmlAgilityPack;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Services.Classes
{
    public class Widget
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public WidgetType WidgetType { get; set; }
        public string HorizontalAlignment { get; set; }


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
            }
        }


        public virtual HtmlNode Create(HtmlNode column)
        {
            // Create the table
            HtmlNode table = Table.Create(column, new TableOptions
            {
                Width = Width,
                HorizontalAlignment = HorizontalAlignment,
                CreateRow = true
            });


            HtmlNode td = table.SelectSingleNode("tr/td");
            td.SetAttributeValue("valign", "top");

            column.AppendChild(new HtmlDocument().CreateComment(Table.MicrosoftIf + "</td></tr></table>" + Table.MicrosoftEndIf));

            return table;
        }
    }
}
