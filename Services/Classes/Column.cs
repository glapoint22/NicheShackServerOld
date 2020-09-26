using HtmlAgilityPack;
using System.Text.Json.Serialization;

namespace Services.Classes
{
    public struct Column
    {
        public Background Background { get; set; }
        public Border Border { get; set; }
        public Corners Corners { get; set; }
        public Shadow Shadow { get; set; }
        public Padding Padding { get; set; }
        public float ColumnSpan { get; set; }

        [JsonConverter(typeof(WidgetJsonConverter))]
        public Widget WidgetData { get; set; }


        public HtmlNode Create(HtmlNode row, float columnWidth)
        {
            // Create the column
            HtmlNode column = row.AppendChild(HtmlNode.CreateNode("<td>"));


            column.SetAttributeValue("style", "display: inline-block;width: 100%;max-width: " + columnWidth + "px;");


            // Set the styles
            if (Background != null) Background.SetStyle(column);
            if (Border != null) Border.SetStyle(column);
            if (Corners != null) Corners.SetStyle(column);
            if (Shadow != null) Shadow.SetStyle(column);
            if (Padding != null) Padding.SetStyle(column);

            return column;
        }
    }
}
