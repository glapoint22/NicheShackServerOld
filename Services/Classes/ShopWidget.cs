using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Classes
{
    public class ShopWidget : Widget
    {
        public ShopType ShopType { get; set; }
        public Caption Caption { get; set; }
        public string TextColor { get; set; }
        public List<ShopItem> Items { get; set; }


        public override void SetProperty(string property, ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            base.SetProperty(property, ref reader, options);

            switch (property)
            {
                case "shopType":
                    ShopType = (ShopType)JsonSerializer.Deserialize(ref reader, typeof(ShopType), options);
                    break;

                case "caption":
                    Caption = (Caption)JsonSerializer.Deserialize(ref reader, typeof(Caption), options);
                    break;

                case "textColor":
                    TextColor = (string)JsonSerializer.Deserialize(ref reader, typeof(string), options);
                    break;

                case "items":
                    Items = (List<ShopItem>)JsonSerializer.Deserialize(ref reader, typeof(List<ShopItem>), options);
                    break;
            }
        }



        public override async Task SetData(NicheShackContext context, QueryParams queryParams)
        {
            List<ItemData> itemData = null;


            if (ShopType == ShopType.Category)
            {
                itemData = await context.Categories
                .AsNoTracking()
                .Where(x => Items.Select(z => z.Id).ToList().Contains(x.Id))
                .Select(x => new ItemData
                {
                    Id = x.Id,
                    Name = x.Name,
                    ImageId = (int)x.ImageId
                })
                .ToListAsync();
            }
            else
            {
                itemData = await context.Niches
                .AsNoTracking()
                .Where(x => Items.Select(z => z.Id).ToList().Contains(x.Id))
                .Select(x => new ItemData
                {
                    Id = x.Id,
                    Name = x.Name,
                    ImageId = (int)x.ImageId
                })
                .ToListAsync();
            }


            foreach (ShopItem item in Items)
            {
                ItemData data = itemData.Where(x => x.Id == item.Id).Single();
                item.Name = data.Name;
                item.Icon = new Image();
                item.Icon.Id = data.ImageId;
                await item.Icon.SetData(context);
                //if (item.Link != null) await item.Link.SetData(context);
            }
        }
    }


    struct ItemData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ImageId { get; set; }

    }
}