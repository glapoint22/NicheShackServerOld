using DataAccess.Models;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Services.Classes
{
    public struct Column
    {
        public string Name { get; set; }
        public Background Background { get; set; }
        public Border Border { get; set; }
        public Corners Corners { get; set; }
        public Shadow Shadow { get; set; }
        public Padding Padding { get; set; }
        public float Width { get; set; }
        public int ColumnSpan { get; set; }
        public List<Breakpoint> Breakpoints { get; set; }


        [JsonConverter(typeof(WidgetJsonConverter))]
        public Widget WidgetData { get; set; }


        public async Task<HtmlNode> Create(HtmlNode row, NicheShackContext context)
        {
            // Create the column
            HtmlNode column = row.AppendChild(HtmlNode.CreateNode("<td>"));


            column.SetAttributeValue("style", "display: inline-block;width: 100%;max-width: " + Width + "px;");


            // Set the styles
            if (Background != null) await Background.SetStyle(column, context);
            if (Border != null) Border.SetStyle(column);
            if (Corners != null) Corners.SetStyle(column);
            if (Shadow != null) Shadow.SetStyle(column);
            if (Padding != null) Padding.SetStyle(column);


            column.AppendChild(new HtmlDocument().CreateComment(Table.MicrosoftIf + "<table width=\"" +
                Width + "\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td>" +
                Table.MicrosoftEndIf));

            return column;
        }
    }
}
