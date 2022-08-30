using DataAccess.Models;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Classes
{
    public class EmailPage : PageContent
    {
        private readonly int width = 600;

        public async Task<string> CreateBody(NicheShackContext context)
        {
            // Document
            HtmlDocument doc = new HtmlDocument();
            HtmlNode node = HtmlNode.CreateNode("<html><head></head><body></body></html>");
            doc.DocumentNode.AppendChild(node);
            node.SelectSingleNode("body").SetAttributeValue("style", "margin: 0;");

            // Meta
            HtmlNode meta = HtmlNode.CreateNode("<meta>");
            meta.SetAttributeValue("name", "viewport");
            meta.SetAttributeValue("content", "width=device-width, initial-scale=1");
            node.FirstChild.AppendChild(meta);


            // Style
            HtmlNode style = HtmlNode.CreateNode("<style>");
            HtmlTextNode styleText = doc.CreateTextNode(
                "a {text-decoration: none}" +
                "body {margin: 0}" +
                "ol, ul {margin-top: 0;margin-bottom: 0;}"
                );
            style.AppendChild(styleText);
            node.FirstChild.AppendChild(style);



            // Main Table
            HtmlNode mainTable = await Table.Create(doc.DocumentNode.FirstChild.LastChild, new TableOptions
            {
                Background = new Background { Color = "#dddddd" },
                CreateRow = true
            }, context);

            // Set alignment to center
            HtmlNode td = mainTable.SelectSingleNode("tr/td");
            td.SetAttributeValue("align", "center");


            // Body
            HtmlNode body = await Table.Create(td, new TableOptions
            {
                Width = width,
                Background = Background
            }, context);


            // Rows
            if (Rows != null && Rows.Count() > 0)
            {
                await CreateRows(Rows, body, context);
            }


            return doc.DocumentNode.InnerHtml;
        }



        private async Task CreateRows(IEnumerable<Row> rows, HtmlNode parent, NicheShackContext context)
        {
            foreach (Row row in rows)
            {
                // Create the row
                HtmlNode tr = await row.Create(parent, context);

                foreach (Column column in row.Columns)
                {
                    // Create the column
                    HtmlNode td = await column.Create(tr, context);


                    // Create the widget
                    Widget widget = GetWidget(column.WidgetData.WidgetType, column.WidgetData);
                    HtmlNode widgetNode = await widget.Create(td, context);

                    if (column.WidgetData.WidgetType == WidgetType.Container)
                    {
                        ContainerWidget container = (ContainerWidget)column.WidgetData;

                        if (container.Rows != null && container.Rows.Count() > 0) await CreateRows(container.Rows, widgetNode, context);
                    }
                }
            }

        }
    }
}
