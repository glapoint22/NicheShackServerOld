using HtmlAgilityPack;
using System;
using System.Text.Json;

namespace Services.Classes
{
    public class ButtonWidget : Widget
    {
        public Background Background { get; set; }
        public Border Border { get; set; }
        public Caption Caption { get; set; }
        public Corners Corners { get; set; }
        public Shadow Shadow { get; set; }
        public Padding Padding { get; set; }
        public Link Link { get; set; }



        public override void SetProperty(Utf8JsonReader reader, JsonSerializerOptions options)
        {
            string property = reader.GetString();

            if (property == "background") BasePropertiesSet = true;

            if (!BasePropertiesSet)
            {
                base.SetProperty(reader, options);
            }
            else
            {
                switch (property)
                {
                    case "background":
                        Background = (Background)JsonSerializer.Deserialize(ref reader, typeof(Background), options);
                        break;

                    case "border":
                        Border = (Border)JsonSerializer.Deserialize(ref reader, typeof(Border), options);
                        break;

                    case "caption":
                        Caption = (Caption)JsonSerializer.Deserialize(ref reader, typeof(Caption), options);
                        break;

                    case "corners":
                        Corners = (Corners)JsonSerializer.Deserialize(ref reader, typeof(Corners), options);
                        break;

                    case "shadow":
                        Shadow = (Shadow)JsonSerializer.Deserialize(ref reader, typeof(Shadow), options);
                        break;

                    case "padding":
                        Padding = (Padding)JsonSerializer.Deserialize(ref reader, typeof(Padding), options);
                        break;

                    case "link":
                        Link = (Link)JsonSerializer.Deserialize(ref reader, typeof(Link), options);
                        break;
                }
            }
        }







        public override HtmlNode Create(HtmlNode column)
        {
            float height = Height > 0 ? Height : 40;


            // Call the base
            HtmlNode widget = base.Create(column);

            // Td
            HtmlNode td = widget.SelectSingleNode("tr/td");

            // Set the styles
            if (Background != null) Background.SetStyle(td);
            if (Border != null) Border.SetStyle(td);
            if (Corners != null) Corners.SetStyle(td);
            if (Shadow != null) Shadow.SetStyle(td);

            // Anchor
            HtmlNode anchorNode = HtmlNode.CreateNode("<a>");

            string styles = "display: block;text-align: center;" +
                (Caption.TextDecoration == null ? "text-decoration: none;" : "");



            var fontSize = int.Parse(Caption.FontSize);
            var padding = Math.Max(0, ((height - fontSize) / 2) - 1);
            var paddingTop = Padding != null && Padding.Top != null ? int.Parse(Padding.Top.Substring(0, Padding.Top.Length - 2)) : 0;
            var paddingBottom = Padding != null && Padding.Bottom != null ? int.Parse(Padding.Bottom.Substring(0, Padding.Bottom.Length - 2)) : 0;


            styles += "padding-top: " + (padding + paddingTop) + "px;";
            styles += "padding-bottom: " + (padding + paddingBottom) + "px;";

            if (Padding != null && Padding.Right != null) styles += "padding-right: " + Padding.Right + "px;";
            if (Padding != null && Padding.Left != null) styles += "padding-left: " + Padding.Left + "px;";


            anchorNode.SetAttributeValue("style", styles);

            // Caption
            Caption.SetStyle(anchorNode);

            // Link
            Link.SetStyle(anchorNode);


            td.AppendChild(new HtmlDocument().CreateComment(Table.MicrosoftIf +
                "<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" align=\"center\" style=\"padding-top: " +
                (padding + paddingTop) + "px;padding-bottom: " +
                (padding + paddingBottom) +
                "px;text-align: center;\"><tr><td>" +
                Table.MicrosoftEndIf));



            td.AppendChild(anchorNode);

            td.AppendChild(new HtmlDocument().CreateComment(Table.MicrosoftIf + "</td></tr></table>" + Table.MicrosoftEndIf));

            return anchorNode;
        }
    }
}
