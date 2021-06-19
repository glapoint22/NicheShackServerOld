using DataAccess.Models;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Classes
{
    public class DividerWidget : Widget
    {
        public override Task SetData(NicheShackContext context, QueryParams queryParams)
        {
            return Task.FromResult(false);
        }


        public override void SetProperty(string property, ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            base.SetProperty(property, ref reader, options);
        }
    }
}
