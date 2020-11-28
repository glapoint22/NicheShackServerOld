using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Services.Classes
{
    public class QueryParams
    {
        public string Search { get; set; }
        public string Sort { get; set; }
        public string CategoryId { get; set; }
        public string NicheId { get; set; }
        public List<QueryFilter> CustomFilters = new List<QueryFilter>();
        public QueryFilter PriceFilter { get; set; }
        public QueryFilter PriceRangeFilter { get; set; }
        public QueryFilter RatingFilter { get; set; }
        public IEnumerable<Query> Queries { get; set; }
        public double Page { get; set; }
        public double Limit { get; set; }
        public string Filters { get; set; }
        public List<FilteredProduct> FilteredProducts { get; set; }
        public List<int> KeywordProductIds = new List<int>();
        public NicheShackContext Context { get; set; }

        


        // ..................................................................................Init....................................................................
        public async Task Init(NicheShackContext context)
        {
            Context = context;

            SetFilters();
            await SetFilteredProducts();
            await SetKeywords();
            await UpdateQueries(Queries);
        }





        private async Task SetKeywords()
        {
            if (Search == null || Search == string.Empty) return;

            int keywordId = await Context.Keywords
                .AsNoTracking()
                .Where(x => x.Name == Search)
                .Select(x => x.Id)
                .SingleOrDefaultAsync();

            if (keywordId > 0)
            {
                KeywordProductIds = await Context.ProductKeywords
                    .AsNoTracking()
                    .Where(x => x.KeywordId == keywordId)
                    .Select(x => x.ProductId)
                    .ToListAsync();

            }
        }
        




        //..................................................................................Set Filters....................................................................
        private void SetFilters()
        {
            if (Filters == null || Filters == string.Empty) return;

            Filters = HttpUtility.UrlDecode(Filters);

            string[] filterStringArray = Filters.Split('|');

            List<QueryFilter> filters = new List<QueryFilter>();

            for (int i = 0; i < filterStringArray.Length - 1; i++)
            {
                var optionsArray = filterStringArray[i + 1].Split(',');

                QueryFilter queryFilter = new QueryFilter
                {
                    Caption = filterStringArray[i],
                    Options = optionsArray.Select(x => new QueryFilterOption
                    {
                        Id = filterStringArray[i] != "Price" && filterStringArray[i] != "Price Range" ? int.Parse(x) : 0,
                        Label = filterStringArray[i] == "Price" || filterStringArray[i] == "Price Range" ? x : null
                    }).ToList()
                };

                filters.Add(queryFilter);

                i++;
            }

            // Custom Filters
            CustomFilters = filters
                .Where(x => x.Caption != "Price" && x.Caption != "Customer Rating" && x.Caption != "Price Range")
                .ToList();

            // Price Filter
            PriceFilter = filters
                .Where(x => x.Caption == "Price")
                .SingleOrDefault();


            // Price Range Filter
            PriceRangeFilter = filters
                .Where(x => x.Caption == "Price Range")
                .SingleOrDefault();


            // Rating Filter
            RatingFilter = filters
                .Where(x => x.Caption == "Customer Rating")
                .SingleOrDefault();
        }





        // ..................................................................................Set Filtered Products....................................................................
        public async Task SetFilteredProducts()
        {
            FilteredProducts = new List<FilteredProduct>();

            if (CustomFilters.Count > 0)
            {
                List<int> filterOptionIds = CustomFilters
                .SelectMany(x => x.Options.Select(z => z.Id).ToList())
                .ToList();

                if (filterOptionIds.Count > 0)
                {
                    FilteredProducts = await Context.ProductFilters
                        .AsNoTracking()
                        .Where(x => filterOptionIds.Contains(x.FilterOptionId))
                        .Select(x => new FilteredProduct
                        {
                            ProductId = x.ProductId,
                            FilterId = x.FilterOption.FilterId
                        })
                        .ToListAsync();
                }
            }
        }




        // ..................................................................................Update Queries....................................................................
        async Task UpdateQueries(IEnumerable<Query> queries)
        {
            if(queries != null && queries.Count() > 0)
            {
                foreach (Query query in queries)
                {
                    if (query.QueryType == QueryType.ProductSubgroup)
                    {
                        query.IntValues = await Context.SubgroupProducts.Where(x => x.SubgroupId == query.IntValue).Select(x => x.ProductId).ToListAsync();
                    }


                    if (query.QueryType == QueryType.ProductKeywords)
                    {
                        List<int> keywordIds = await Context.ProductKeywords.Where(x => query.StringValues.Contains(x.Keyword.Name)).Select(x => x.Keyword.Id).Distinct().ToListAsync();
                        query.IntValues = await Context.ProductKeywords.Where(x => keywordIds.Contains(x.KeywordId)).Select(x => x.ProductId).ToListAsync();
                    }


                    if (query.QueryType == QueryType.SubQuery)
                    {
                        await UpdateQueries(query.SubQueries);
                    }
                }
            }
        }
    }
}
