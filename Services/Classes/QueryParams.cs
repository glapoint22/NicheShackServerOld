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
        public Query Query { get; set; }
        public double Page { get; set; }
        public double Limit { get; set; }
        public string Filters { get; set; }
        public List<FilteredProduct> FilteredProducts { get; set; }
        public List<int> KeywordProductIds = new List<int>();
        public List<KeyValuePair<string, string>> Cookies { get; set; }
        public string Id { get; set; }
        public bool UsesFilters { get; set; }
        public int ProductId { get; set; }




        // ..................................................................................Init....................................................................
        public async Task Init(NicheShackContext context)
        {
            SetFilters();
            await SetFilteredProducts(context);
            await SetKeywords(context);
            //await UpdateQueries(Queries, context);
        }





        private async Task SetKeywords(NicheShackContext context)
        {
            if (Search == null || Search == string.Empty) return;

            int keywordId = await context.Keywords
                .AsNoTracking()
                .Where(x => x.Name == Search)
                .Select(x => x.Id)
                .SingleOrDefaultAsync();

            if (keywordId > 0)
            {
                KeywordProductIds = await context.ProductKeywords
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
        public async Task SetFilteredProducts(NicheShackContext context)
        {
            FilteredProducts = new List<FilteredProduct>();

            if (CustomFilters.Count > 0)
            {
                List<int> filterOptionIds = CustomFilters
                .SelectMany(x => x.Options.Select(z => z.Id).ToList())
                .ToList();

                if (filterOptionIds.Count > 0)
                {
                    FilteredProducts = await context.ProductFilters
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
        //async Task UpdateQueries(IEnumerable<Query> queries, NicheShackContext context)
        //{
        //    if (queries != null && queries.Count() > 0)
        //    {
        //        foreach (Query query in queries)
        //        {

        //            // Auto
        //            if (query.QueryType == QueryType.Auto && query.IntValue == 2)
        //            {
        //                query.IntValues = await context.Products
        //                    .AsNoTracking()
        //                    .Where(x => x.Id == ProductId)
        //                    .Select(x => x.NicheId).ToListAsync();
        //            }

        //            // Product Subgroup
        //            else if (query.QueryType == QueryType.ProductGroup)
        //            {
        //                query.IntValues = await context.SubgroupProducts
        //                    .AsNoTracking()
        //                    .Where(x => x.SubgroupId == query.IntValue)
        //                    .Select(x => x.ProductId).ToListAsync();
        //            }

        //            // Product Keywords
        //            else if (query.QueryType == QueryType.KeywordGroup)
        //            {
        //                List<int> keywordIds = await context.ProductKeywords
        //                    .AsNoTracking()
        //                    .Where(x => query.StringValues.Contains(x.Keyword.Name))
        //                    .Select(x => x.Keyword.Id).Distinct()
        //                    .ToListAsync();


        //                query.IntValues = await context.ProductKeywords
        //                    .Where(x => keywordIds.Contains(x.KeywordId))
        //                    .Select(x => x.ProductId)
        //                    .ToListAsync();
        //            }

        //            // Subquery
        //            else if (query.QueryType == QueryType.None)
        //            {
        //                await UpdateQueries(query.SubQueries, context);
        //            }
        //        }
        //    }
        //}
    }
}
