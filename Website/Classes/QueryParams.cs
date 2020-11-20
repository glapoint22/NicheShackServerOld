using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Website.Repositories;

namespace Website.Classes
{
    public class QueryParams
    {
        public string Query { get; private set; }
        public string Sort { get; private set; }
        public string CategoryId { get; private set; }
        public string NicheId { get; private set; }
        public List<QueryFilter> CustomFilters = new List<QueryFilter>();
        public QueryFilter PriceFilter { get; set; }
        public QueryFilter PriceRangeFilter { get; set; }
        public QueryFilter RatingFilter { get; set; }

        public List<FilteredProduct> FilteredProducts { get; set; }

        public List<int> KeywordProductIds = new List<int>();

        private IUnitOfWork UnitOfWork;

        public QueryParams(string query, string filters, string categoryId, string nicheId, string sort)
        {
            Query = query;
            Sort = sort;
            CategoryId = categoryId;
            NicheId = nicheId;
            SetFilters(filters);
        }



        // ..................................................................................Set Filters....................................................................
        private void SetFilters(string filterString)
        {
            if (filterString == string.Empty) return;

            filterString = HttpUtility.UrlDecode(filterString);

            string[] filterStringArray = filterString.Split('|');

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







        // ..................................................................................Init....................................................................
        public async Task Init(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
            await SetFilteredProducts();


            int keywordId = await unitOfWork.Keywords.Get(x => x.Name == Query, x => x.Id);

            if (keywordId > 0)
            {
                KeywordProductIds = (List<int>)await unitOfWork.ProductKeywords.GetCollection(x => x.KeywordId == keywordId, x => x.ProductId);
            }
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
                    FilteredProducts = (List<FilteredProduct>)await UnitOfWork.ProductFilters.GetCollection(x => filterOptionIds.Contains(x.FilterOptionId), x => new FilteredProduct
                    {
                        ProductId = x.ProductId,
                        FilterId = x.FilterOption.FilterId
                    });
                }
            }
        }
    }
}