using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Classes
{
    public struct EmailPage
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public Background Background { get; set; }
        public List<Row> Rows { get; set; }



        public string BuildEmail()
        {
            HtmlDocument doc = new HtmlDocument();
            HtmlNode node = HtmlNode.CreateNode("<html><head></head><body></body></html>");
            doc.DocumentNode.AppendChild(node);

            HtmlNode meta = HtmlNode.CreateNode("<meta>");

            meta.SetAttributeValue("name", "viewport");
            meta.SetAttributeValue("content", "width=device-width, initial-scale=1");

            node.FirstChild.AppendChild(meta);

            // Create the main table
            CreateTable(doc.DocumentNode.FirstChild.LastChild, Width);

            return doc.DocumentNode.InnerHtml;
        }



        private void CreateTable(HtmlNode parent, float width)
        {
            HtmlNode table = Table.Create(parent, width);
            Background.SetStyle(table);

            // Rows
            if (Rows != null && Rows.Count > 0)
            {
                foreach (Row row in Rows)
                {
                    // Create the row
                    HtmlNode tr = row.Create(table);

                    foreach (Column column in row.Columns)
                    {
                        float columnWidth = GetColumnWidth(width, row.Columns.Count, column.ColumnSpan);


                        tr.AppendChild(new HtmlDocument().CreateComment("<!--[if (gte mso 9)|(IE)]><table width=\"" + columnWidth + "\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td>[endif]-->"));

                        // Create the column
                        HtmlNode td = column.Create(tr, columnWidth);



                        Widget widget = GetWidget(column.WidgetData.WidgetType, column.WidgetData);

                        HtmlNode widgetNode = widget.Create(td);

                        tr.AppendChild(new HtmlDocument().CreateComment("<!--[if (gte mso 9)|(IE)]></td></tr></table>[endif]-->"));
                    }
                }
            }
        }




        private float GetColumnWidth(float tableWidth, int columnCount, float columnSpan)
        {
            float totalColumns = columnCount == 5 ? 10 : 12;

            float percentage = columnSpan / totalColumns;

            return percentage * tableWidth;
        }





        private Widget GetWidget(WidgetType widgetType, Widget widgetData)
        {
            Widget widget = null;

            switch (widgetType)
            {
                case WidgetType.Button:
                    widget = (ButtonWidget)widgetData;
                    break;
                case WidgetType.Text:
                    widget = new TextWidget();
                    break;
                case WidgetType.Image:
                    widget = new ImageWidget();
                    break;
                case WidgetType.Container:
                    widget = new ContainerWidget();
                    break;
                case WidgetType.Line:
                    widget = new LineWidget();
                    break;
            }

            return widget;
        }


        

    }
}
