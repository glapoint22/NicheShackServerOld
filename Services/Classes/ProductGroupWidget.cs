using DataAccess.Models;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Classes
{
    public class ProductGroupWidget : Widget
    {
        public Caption Caption { get; set; }
        public List<QueriedProduct> Products { get; set; }
        public List<Query> Queries { get; set; }


        public override void SetProperty(string property, ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            base.SetProperty(property, ref reader, options);

            switch (property)
            {
                case "caption":
                    Caption = (Caption)JsonSerializer.Deserialize(ref reader, typeof(Caption), options);
                    break;
                case "queries":
                    Queries = (List<Query>)JsonSerializer.Deserialize(ref reader, typeof(List<Query>), options);
                    break;
            }
        }


        public async override Task SetData(NicheShackContext context, QueryParams queryParams)
        {
            if (Queries != null)
            {
                queryParams.Queries = Queries;
                queryParams.Limit = 24;
                queryParams.UsesFilters = false;
                QueryService queryService = new QueryService(context);
                Products = await queryService.GetProductGroup(queryParams);
            }
        }
    }
}