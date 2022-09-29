using DataAccess.Models;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Classes
{
    public class GridWidget : Widget
    {
        //public List<Query> Queries { get; set; }
        public GridData GridData { get; set; }


        public override void SetProperty(string property, ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            base.SetProperty(property, ref reader, options);

            //switch (property)
            //{
            //    case "queries":
            //        Queries = (List<Query>)JsonSerializer.Deserialize(ref reader, typeof(List<Query>), options);
            //        break;
            //}
        }



        public async override Task SetData(NicheShackContext context, QueryParams queryParams)
        {
            //if (Queries != null) queryParams.Queries = Queries;

            //if (queryParams.Queries != null || queryParams.Search != null || queryParams.CategoryId != null || queryParams.NicheId != null)
            //{
            queryParams.Limit = 40;
            queryParams.UsesFilters = true;
            if (queryParams.Page == 0) queryParams.Page = 1;
            QueryService queryService = new QueryService(context);
            GridData = await queryService.GetGridData(queryParams);
            //}

        }
    }
}