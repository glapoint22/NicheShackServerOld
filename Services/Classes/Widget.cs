using HtmlAgilityPack;
using System.Text.Json;

namespace Services.Classes
{
    public class Widget
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public WidgetType WidgetType { get; set; }
        public string HorizontalAlignment { get; set; }


        public virtual void SetProperty(Utf8JsonReader reader, JsonSerializerOptions options)
        {
            string property = reader.GetString();

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
            // Create the widget
            HtmlNode table = Table.Create(column, Width, Height);
            Table.CreateRow(table);

            if(HorizontalAlignment != null)
            {
                string styles = table.GetAttributeValue("style", "");
                styles += "margin: " + HorizontalAlignment + ";";
                table.SetAttributeValue("style", styles);
            }

            return table;
        }
    }
}
