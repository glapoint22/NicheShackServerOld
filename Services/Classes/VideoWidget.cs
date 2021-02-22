using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Classes
{
    public class VideoWidget : Widget
    {
        public Border Border { get; set; }
        public Corners Corners { get; set; }
        public Shadow Shadow { get; set; }
        public Video Video { get; set; }




        public override void SetProperty(string property, ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            base.SetProperty(property, ref reader, options);


            switch (property)
            {
                case "border":
                    Border = (Border)JsonSerializer.Deserialize(ref reader, typeof(Border), options);
                    break;

                case "corners":
                    Corners = (Corners)JsonSerializer.Deserialize(ref reader, typeof(Corners), options);
                    break;

                case "shadow":
                    Shadow = (Shadow)JsonSerializer.Deserialize(ref reader, typeof(Shadow), options);
                    break;

                case "video":
                    Video = (Video)JsonSerializer.Deserialize(ref reader, typeof(Video), options);
                    break;
            }
        }





        public override async Task SetData(NicheShackContext context, QueryParams queryParams)
        {
            await Video.SetData(context);
        }
    }
}
