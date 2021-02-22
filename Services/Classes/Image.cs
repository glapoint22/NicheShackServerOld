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
        public string Url { get; set; }

        public async Task SetStyle(HtmlNode node, NicheShackContext context)
        {
            await SetData(context);


            if (Name == "Product Placeholder")
            {
                Name = "{productName}";
                Url = "{productImage}";
            }
            else if (Name == "Stars Placeholder")
            {
                Name = "Product Rating";
                Url = "{stars}";
            }

            node.SetAttributeValue("src", "{host}/images/" + Url);
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
                Url = x.Url
            })
            .SingleOrDefaultAsync();

            // If this image exists
            if (image != null)
            {
                Name = image.Name;
                Url = image.Url;
            }
        }
    }
}
