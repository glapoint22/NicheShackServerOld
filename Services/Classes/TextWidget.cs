using DataAccess.Models;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Classes
{
    public class TextWidget : Widget
    {
        public Background Background { get; set; }
        public Padding Padding { get; set; }
        public List<TextBoxData> TextBoxData { get; set; }


        public async override Task<HtmlNode> Create(HtmlNode column, NicheShackContext context)
        {
            // Call the base
            HtmlNode textWidget = await base.Create(column, context);

            // Select the td node
            HtmlNode td = textWidget.SelectSingleNode("tr/td");

            // Height
            if (Height > 0)
            {
                td.SetAttributeValue("height", Height.ToString());
                td.SetAttributeValue("style", "height: " + Height + "px;");
            }

            // Apply the styles
            td.SetAttributeValue("style", "font-family: Arial, Helvetica, sans-serif;font-size: 14px;color: #000000;");
            if (Background != null) await Background.SetStyle(td, context);
            if (Padding != null) Padding.SetStyle(td);

            foreach (TextBoxData data in TextBoxData)
            {
                GenerateHtml(data, td);
            }

            return textWidget;
        }




        private void GenerateHtml(TextBoxData textBoxData, HtmlNode parent)
        {
            HtmlNode newNode = null;

            // Div
            if (textBoxData.ElementType == ElementType.Div)
            {
                newNode = HtmlNode.CreateNode("<div>");
                parent.AppendChild(newNode);
            }

            // Span
            else if (textBoxData.ElementType == ElementType.Span)
            {
                newNode = HtmlNode.CreateNode("<span>");
                parent.AppendChild(newNode);
            }

            // Text
            else if (textBoxData.ElementType == ElementType.Text)
            {
                newNode = new HtmlDocument().CreateTextNode(textBoxData.Text);
                parent.AppendChild(newNode);
            }

            // Break
            else if (textBoxData.ElementType == ElementType.Break)
            {
                newNode = HtmlNode.CreateNode("<br>");
                parent.AppendChild(newNode);
            }

            // Unordered List
            else if (textBoxData.ElementType == ElementType.UnorderedList)
            {
                newNode = HtmlNode.CreateNode("<ul>");
                parent.AppendChild(newNode);
            }

            // Ordered List
            else if (textBoxData.ElementType == ElementType.OrderedList)
            {
                newNode = HtmlNode.CreateNode("<ol>");
                parent.AppendChild(newNode);
            }

            // List Item
            else if (textBoxData.ElementType == ElementType.ListItem)
            {
                newNode = HtmlNode.CreateNode("<li>");
                parent.AppendChild(newNode);
            }

            // Anchor
            else if (textBoxData.ElementType == ElementType.Anchor)
            {
                newNode = HtmlNode.CreateNode("<a>");
                newNode.SetAttributeValue("href", textBoxData.Link.Url);
                newNode.SetAttributeValue("target", "_blank");

                parent.AppendChild(newNode);
            }

            // Styles
            if (textBoxData.Styles != null && textBoxData.Styles.Count > 0)
            {
                string styles = "";

                foreach (StyleData style in textBoxData.Styles)
                {
                    styles += style.Name + ": " + style.Value + ";";
                }

                newNode.SetAttributeValue("style", styles);
            }

            // Indent
            if (textBoxData.Indent > 0)
            {
                string styles = newNode.GetAttributeValue("style", "");

                styles += "text-indent: " + (textBoxData.Indent * 40) + 20 + "px;";
                newNode.SetAttributeValue("style", styles);
            }


            // Children
            if (textBoxData.Children != null && textBoxData.Children.Count > 0)
            {
                foreach (TextBoxData childData in textBoxData.Children)
                {
                    GenerateHtml(childData, newNode);
                }
            }
        }



        public override void SetProperty(string property, ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            base.SetProperty(property, ref reader, options);

            switch (property)
            {
                case "background":
                    Background = (Background)JsonSerializer.Deserialize(ref reader, typeof(Background), options);
                    break;


                case "padding":
                    Padding = (Padding)JsonSerializer.Deserialize(ref reader, typeof(Padding), options);
                    break;

                case "textBoxData":
                    TextBoxData = (List<TextBoxData>)JsonSerializer.Deserialize(ref reader, typeof(List<TextBoxData>), options);
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
