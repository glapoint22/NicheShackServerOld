using DataAccess.Models;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Classes
{
    public class Link
    {
        public int Id { get; set; }
        public LinkType LinkType { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }


        public async Task SetStyle(HtmlNode node, NicheShackContext context)
        {
            await SetData(context);

            node.SetAttributeValue("href", LinkType == LinkType.WebAddress ? Url : "{host}/" + Url);
            node.SetAttributeValue("target", "_blank");
        }



        public async Task SetData(NicheShackContext context)
        {
            if (LinkType != LinkType.Page && LinkType != LinkType.Product) return;

            if (LinkType == LinkType.Page)
            {
                var page = await context.Pages
                    .AsNoTracking()
                    .Where(x => x.Id == Id)
                    .Select(x => new
                    {
                        x.Name,
                        x.PageType,
                        x.UrlName,
                        x.UrlId
                    })
                    .SingleOrDefaultAsync();

                if (page != null)
                {
                    Name = page.Name;
                    Url = GetPageDisplay((PageType)page.PageType) + page.UrlName + "/" + page.UrlId;
                }
            }
            else if (LinkType == LinkType.Product)
            {
                var product = await context.Products
                    .AsNoTracking()
                    .Where(x => x.Id == Id)
                    .Select(x => new
                    {
                        x.Name,
                        x.UrlName,
                        x.UrlId
                    })
                    .SingleOrDefaultAsync();

                if (product != null)
                {
                    Name = product.Name;
                    Url = product.UrlName + "/" + product.UrlId;
                }
            }
        }



        private string GetPageDisplay(PageType pageDisplayType)
        {
            string value = "";

            switch (pageDisplayType)
            {
                case PageType.Custom:
                    value = "cp/";
                    break;
                case PageType.Browse:
                    value = "browse/";
                    break;
            }

            return value;
        }
    }
}