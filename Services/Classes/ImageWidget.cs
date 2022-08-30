using DataAccess.Models;
using HtmlAgilityPack;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Classes
{
    public class ImageWidget : Widget
    {
        public Image Image { get; set; }
        public Border Border { get; set; }
        public Corners Corners { get; set; }
        public Shadow Shadow { get; set; }
        public Link Link { get; set; }


        public async override Task<HtmlNode> Create(HtmlNode column, NicheShackContext context)
        {
            // Call the base
            HtmlNode widget = await base.Create(column, context);

            // Td
            HtmlNode td = widget.SelectSingleNode("tr/td");


            // Image
            HtmlNode img = HtmlNode.CreateNode("<img>");

            if (Width > 0) img.SetAttributeValue("style", "width: " + Width + "px;");


            // Set the styles
            if (Border != null) Border.SetStyle(img);
            if (Corners != null) Corners.SetStyle(img);
            if (Shadow != null) Shadow.SetStyle(img);


            await Image.SetStyle(img, context);

            if (Link != null && Link.Url != null)
            {
                // Anchor
                HtmlNode anchorNode = HtmlNode.CreateNode("<a>");
                await Link.SetStyle(anchorNode, context);

                anchorNode.AppendChild(img);

                td.AppendChild(anchorNode);
                return anchorNode;
            }
            else
            {
                td.AppendChild(img);
                return img;
            }
        }



        public override void SetProperty(string property, ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            base.SetProperty(property, ref reader, options);

            switch (property)
            {
                case "image":
                    Image = (Image)JsonSerializer.Deserialize(ref reader, typeof(Image), options);
                    break;

                case "border":
                    Border = (Border)JsonSerializer.Deserialize(ref reader, typeof(Border), options);
                    break;

                case "corners":
                    Corners = (Corners)JsonSerializer.Deserialize(ref reader, typeof(Corners), options);
                    break;

                case "shadow":
                    Shadow = (Shadow)JsonSerializer.Deserialize(ref reader, typeof(Shadow), options);
                    break;

                case "link":
                    Link = (Link)JsonSerializer.Deserialize(ref reader, typeof(Link), options);
                    break;
            }
        }





        public override async Task SetData(NicheShackContext context, QueryParams queryParams)
        {
            await Image.SetData(context);
            //if (Link != null) await Link.SetData(context);
        }
    }
}
