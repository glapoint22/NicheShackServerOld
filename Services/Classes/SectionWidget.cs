using DataAccess.Models;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Classes
{
    public class SectionWidget : Widget
    {
        public int SectionType { get; set; }


        public override Task SetData(NicheShackContext context, QueryParams queryParams)
        {
            return Task.FromResult(false);
        }


        public override void SetProperty(string property, ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            base.SetProperty(property, ref reader, options);

            switch (property)
            {
                case "sectionType":
                    SectionType = (int)JsonSerializer.Deserialize(ref reader, typeof(int), options);
                    break;
            }
        }
    }
}
