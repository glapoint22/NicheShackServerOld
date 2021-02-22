using DataAccess.Models;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Classes
{
    public class CarouselWidget : Widget
    {
        public List<LinkableImage> Banners { get; set; }



        public override void SetProperty(string property, ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            base.SetProperty(property, ref reader, options);

            switch (property)
            {
                case "banners":
                    Banners = (List<LinkableImage>)JsonSerializer.Deserialize(ref reader, typeof(List<LinkableImage>), options);
                    break;
            }
        }


        public async override Task SetData(NicheShackContext context, QueryParams queryParams)
        {
            if (Banners != null)
            {
                foreach (LinkableImage banner in Banners)
                {
                    await banner.SetData(context);
                }
            }

        }
    }
}