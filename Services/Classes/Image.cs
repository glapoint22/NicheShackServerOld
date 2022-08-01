using DataAccess.Models;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Classes
{
    public class Image
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Src { get; set; }

        public async Task SetStyle(HtmlNode node, NicheShackContext context)
        {
            await SetData(context);


            if (Name == "Product Placeholder")
            {
                Name = "{productName}";
                Src = "{productImage}";
            }
            else if (Name == "Stars Placeholder")
            {
                Name = "Product Rating";
                Src = "{stars}";
            }

            node.SetAttributeValue("src", "{host}/images/" + Src);
            node.SetAttributeValue("title", Name);
            node.SetAttributeValue("alt", Name);
        }


        public async Task SetData(NicheShackContext context)
        {
            Image image = await context.Media
            .AsNoTracking()
            .Where(x => x.Id == Id)
            .Select(x => new Image
            {
                Name = x.Name,
                Src = x.ImageAnySize
            })
            .SingleOrDefaultAsync();

            // If this image exists
            if (image != null)
            {
                Name = image.Name;
                Src = image.Src;
            }
        }
    }
}
