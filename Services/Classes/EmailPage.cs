﻿using HtmlAgilityPack;
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
                "a {{text-decoration: none}}" +
                "body {{margin: 0}}" +
                "ol, ul {{margin-top: 0;margin-bottom: 0;}}"
                );
            style.AppendChild(styleText);
            node.FirstChild.AppendChild(style);


















            // Main Table
            HtmlNode mainTable = Table.Create(doc.DocumentNode.FirstChild.LastChild, new TableOptions
            {
                Background = new Background { Color = "#edf0f3" },
                CreateRow = true
            });

            // Set alignment to center
            HtmlNode td = mainTable.SelectSingleNode("tr/td");
            td.SetAttributeValue("align", "center");

            // Create the body
            CreateBody(td, Width);

            return doc.DocumentNode.InnerHtml;
        }



        private void CreateBody(HtmlNode parent, float width)
        {
            HtmlNode table = Table.Create(parent, new TableOptions
            {
                Width = width,
                Background = Background
            });


            // Rows
            if (Rows != null && Rows.Count > 0)
            {
                foreach (Row row in Rows)
                {
                    // Create the row
                    HtmlNode tr = row.Create(table);

                    foreach (Column column in row.Columns)
                    {
                        // Create the column
                        HtmlNode td = column.Create(tr);


                        // Create the widget
                        Widget widget = GetWidget(column.WidgetData.WidgetType, column.WidgetData);
                        HtmlNode widgetNode = widget.Create(td);
                    }
                }
            }
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
                    widget = (TextWidget)widgetData;
                    break;
                case WidgetType.Image:
                    widget = (ImageWidget)widgetData;
                    break;
                case WidgetType.Container:
                    widget = (ContainerWidget)widgetData;
                    break;
                case WidgetType.Line:
                    widget = (LineWidget)widgetData;
                    break;
            }

            return widget;
        }




    }
}
