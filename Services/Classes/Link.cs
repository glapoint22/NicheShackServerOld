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
        public LinkOption SelectedOption { get; set; }
        public string Url { get; set; }
        public string OptionValue { get; set; }


        public void SetStyle(HtmlNode node)
        {
            node.SetAttributeValue("href", SelectedOption == LinkOption.WebAddress ? Url : "{host}/" + Url);
            node.SetAttributeValue("target", "_blank");
        }



        public async Task SetData(NicheShackContext context)
        {
            if (SelectedOption != LinkOption.Page && SelectedOption != LinkOption.Product) return;

            if (SelectedOption == LinkOption.Page)
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
                    OptionValue = page.Name;
                    Url = GetPageDisplay((PageType)page.PageType) + page.UrlName + "/" + page.UrlId;
                }
            }
            else if (SelectedOption == LinkOption.Product)
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
                    OptionValue = product.Name;
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