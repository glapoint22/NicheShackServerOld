using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.Classes;
using Website.Interfaces;
using DataAccess.Models;
using DataAccess.Repositories;
using DataAccess.Classes;
using Website.ViewModels;
using System.Linq.Expressions;

namespace Website.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        // Set the context
        private readonly NicheShackContext context;
        public ProductRepository(NicheShackContext context) : base(context)
        {
            this.context = context;
        }





        // ..................................................................................Get Queried Products.....................................................................
        //public async Task<IEnumerable<ProductViewModel>> GetQueriedProducts(QueryParams queryParams)
        //{
        //    ProductViewModel productDTO = new ProductViewModel(queryParams, await GetFilteredProducts(queryParams));

        //    // Return products based on the query parameters
        //    return await context.Products
        //        .AsNoTracking()
        //        .SortBy(productDTO)
        //        .ThenBy(x => x.Name)
        //        .Where(productDTO)
        //        .ExtensionSelect<Product, ProductViewModel>()
        //        .ToListAsync();
        //}



        public async Task<IEnumerable<ProductViewModel>> GetProducts(string query)
        {
            List<int> keywordProductIds = new List<int>();
            List<int> searchWordsProductIds = new List<int>();
            List<int> productIds = new List<int>();

            int keywordId = await context.Keywords
                .AsNoTracking()
                .Where(x => x.Name == query)
                .Select(x => x.Id)
                .SingleOrDefaultAsync();



            if (keywordId > 0)
            {
                keywordProductIds = await context.ProductKeywords
                .AsNoTracking()
                .Where(x => x.KeywordId == keywordId)
                .Select(x => x.ProductId)
                .ToListAsync();
            }



            string[] searchWordsArray = query.Split(' ')
                //.Select(x => "%" + x + "%")
                .ToArray();




            searchWordsProductIds = await context.Products
                .AsNoTracking()
                .WhereAny(searchWordsArray.Select(w => (Expression<Func<Product, bool>>)(x =>

                EF.Functions.Like(x.Name, w + "[^a-z]%") ||
                EF.Functions.Like(x.Name, "%[^a-z]" + w + "[^a-z]%") ||
                EF.Functions.Like(x.Name, "%[^a-z]" + w)

                ||


                EF.Functions.Like(x.Description, w + "[^a-z]%") ||
                EF.Functions.Like(x.Description, "%[^a-z]" + w + "[^a-z]%") ||
                EF.Functions.Like(x.Description, "%[^a-z]" + w)




                )).ToArray())
                .Select(x => x.Id)
                .ToListAsync();



            productIds = searchWordsProductIds
                .Concat(keywordProductIds)
                .Distinct()
                .ToList();

            var productOrders = await context.ProductOrders
                .AsNoTracking()
                .Where(x => productIds.Contains(x.ProductId))
                .Where(x => x.Date >= DateTime.Now.Date.AddMonths(-1))
                .GroupBy(x => x.ProductId)
                .Select(x => new
                {
                    productId = x.Key,
                    count = x.Count()
                })

                .ToListAsync();




            var qry = productIds.GroupJoin(
          productOrders,
          prodIds => prodIds,
          prodOrders => prodOrders.productId,
          (x, y) => new { Foo = x, Bars = y })
       .SelectMany(
           x => x.Bars.DefaultIfEmpty(),
           (x, y) => new { productId = x.Foo, count = y != null ? y.count : 0 })
       .ToList();


            var foo = await context.Products
                .Where(x => productIds.Contains(x.Id))
                .Select(x => new
                {
                    productId = x.Id,
                    name = x.Name,
                    urlId = x.UrlId,
                    urlName = x.UrlName,
                    rating = x.Rating,
                    totalReviews = x.TotalReviews,
                    minPrice = x.MinPrice,
                    maxPrice = x.MaxPrice,
                    image = new ImageViewModel
                    {
                        Name = x.Media.Name,
                        Url = x.Media.Url
                    },
                    mediaCount = x.ProductMedia.Count()
                })
                .ToListAsync();

            var bar = foo.Join(qry, x => x.productId, z => z.productId, (a, b) => new
            {
                Id = a.productId,
                Name = a.name,
                UrlId = a.urlId,
                UrlName = a.urlName,
                TotalReviews = a.totalReviews,
                MinPrice = a.minPrice,
                MaxPrice = a.maxPrice,
                Image = a.image,
                salesCount = b.count,
                Rating = a.rating,
                a.mediaCount

            })
                .OrderBy(x => x.Name.ToLower().StartsWith(query.ToLower()) ? (x.Name.ToLower() == query.ToLower() ? 0 : 1) :

                //EF.Functions.Like(x.name, "%" + query + "%") 

                EF.Functions.Like(x.Name, query + " %") ||
                EF.Functions.Like(x.Name, "% " + query + " %") ||
                EF.Functions.Like(x.Name, "% " + query)

                    ? 2 : 3)
                .ThenByDescending(x => x.salesCount)
                .ThenByDescending(x => x.Rating)
                
                .ThenBy(x => x.MinPrice)
                .ThenByDescending(x => x.mediaCount)
                .Select(x => new ProductViewModel {
                    Id = x.Id,
                    Name = x.Name,
                    UrlId = x.UrlId,
                    UrlName = x.UrlName,
                    TotalReviews = x.TotalReviews,
                    MinPrice = x.MinPrice,
                    MaxPrice = x.MaxPrice,
                    Image = x.Image,
                    Rating = x.Rating,
                    
                })
                .ToList();

            return bar;
        }




        // ..................................................................................Get Filtered Products.....................................................................
        private async Task<IEnumerable<FilteredProduct>> GetFilteredProducts(QueryParams queryParams)
        {
            // Get all the filtered products based on the passed in filter options from the query params
            if (queryParams.CustomFilterOptions.Count == 0) return new List<FilteredProduct>();

            return await context.ProductFilters
                .AsNoTracking()
                .Where(x => queryParams.CustomFilterOptions
                    .Contains(x.FilterOptionId))
                .Select(x => new FilteredProduct
                {
                    ProductId = x.ProductId,
                    FilterId = x.FilterOption.FilterId
                })
                .ToListAsync();
        }






        // ..................................................................................Get Product Ids.....................................................................
        private async Task<IEnumerable<int>> GetProductIds(QueryParams queryParams)
        {
            // Return just product ids from the queried products
            return await context.Products
                .AsNoTracking()
                .Where(new ProductViewModel(queryParams, await GetFilteredProducts(queryParams)))
                .Select(x => x.Id)
                .ToListAsync();
        }








        // ..................................................................................Get Product Ratings.....................................................................
        private async Task<IEnumerable<double>> GetProductRatings(QueryParams queryParams)
        {
            // Return just product ratings from the queried products
            return await context.Products
                .AsNoTracking()
                .Where(new ProductViewModel(queryParams, await GetFilteredProducts(queryParams)))
                .Select(x => x.Rating)
                .ToListAsync();
        }






        // ..................................................................................Get Product Filters.....................................................................
        public async Task<IEnumerable<FilterData>> GetProductFilters(QueryParams queryParams, IEnumerable<ProductViewModel> products)
        {
            List<FilterData> filters = new List<FilterData>();
            List<IQueryFilterOption> options = new List<IQueryFilterOption>();

            // ******Price Filter********
            if (!queryParams.Filters.Any(x => x.Key == "Price"))
            {
                // Cross join between products and the priceRanges table to get the price range options
                var crossJoin =
                    from pr in await context.PriceRanges.ToListAsync()
                    from p in products
                    orderby pr.Id
                    where (p.MinPrice >= pr.Min && p.MinPrice < pr.Max) || (p.MaxPrice > 0 && pr.Min >= p.MinPrice && pr.Min < p.MaxPrice)
                    select (IQueryFilterOption)new PriceFilterOption
                    {
                        Min = pr.Min,
                        Max = pr.Max,
                        Label = "$" + pr.Min + " - $" + pr.Max
                    };

                options = crossJoin.Distinct().ToList();
            }


            // Create the filter data object and add it to the filters
            FilterData filterData = new FilterData
            {
                Type = "Price",
                Caption = "Price",
                Options = options
            };

            filters.Add(filterData);




            // ******Rating Filter********
            List<IQueryFilterOption> ratingOptions = new List<IQueryFilterOption>();
            IEnumerable<double> productRatings = products.Select(x => x.Rating).ToList();


            // Check to see if we have a customer rating filter selected
            if (queryParams.Filters.Any(x => x.Key == "Customer Rating"))
            {
                // Remove the customer rating filter and query without it
                // This is so we can get other rating filters that belong in the results
                queryParams.Filters.RemoveAll(x => x.Key == "Customer Rating");
                productRatings = await GetProductRatings(queryParams);
            }


            // Rating 4 and up
            if (productRatings.Count(x => x >= 4) > 0)
            {
                ratingOptions.Add(new RatingOption
                {
                    Id = "4"
                });
            }



            // Rating 3 and up
            if (productRatings.Count(x => x >= 3 && x < 4) > 0)
            {
                ratingOptions.Add(new RatingOption
                {
                    Id = "3"
                });
            }



            // Rating 2 and up
            if (productRatings.Count(x => x >= 2 && x < 3) > 0)
            {
                ratingOptions.Add(new RatingOption
                {
                    Id = "2"
                });
            }




            // Rating 1 and up
            if (productRatings.Count(x => x >= 1 && x < 2) > 0)
            {
                ratingOptions.Add(new RatingOption
                {
                    Id = "1"
                });
            }



            if (ratingOptions.Count > 0)
            {
                filterData = new FilterData
                {
                    Type = "Rating",
                    Caption = "Customer Rating",
                    Options = ratingOptions
                };

                filters.Add(filterData);
            }




            // ******Custom Filters********

            // Get all the filter option Ids that are related to the product Ids
            List<int> filterOptionIds = await context.ProductFilters
                .AsNoTracking()
                .Where(x => products.Select(y => y.Id)
                    .Contains(x.ProductId))
                .Select(x => x.FilterOptionId)
                .Distinct()
                .ToListAsync();



            // Display the other options for a selected filter
            foreach (KeyValuePair<string, string> customFilter in queryParams.CustomFilters)
            {
                List<int> optionsIds;

                // Exclude the current option from the list of options and query products
                queryParams.SetCustomFilterOptions(customFilter.Key);
                var pIds = await GetProductIds(queryParams);

                // Get a list of option ids that we can display from the current custom filter
                optionsIds = await context.ProductFilters
                    .AsNoTracking()
                    .Where(x => pIds
                        .Contains(x.ProductId) && x.FilterOption.Filter.Name == customFilter.Key)
                    .Select(x => x.FilterOptionId).Distinct()
                    .ToListAsync();

                // Add the new option ids to the list
                filterOptionIds = filterOptionIds
                    .Concat(optionsIds)
                    .ToList();
            }


            // Get the raw filter data consisting of filter name and filter option name
            var rawFilterData = await context.FilterOptions
                .AsNoTracking()
                .OrderBy(x => x.FilterId)
                .Where(x => filterOptionIds
                    .Contains(x.Id))
                .Select(x => new
                {
                    filterName = x.Filter.Name,
                    optionName = x.Name,
                    optionId = x.Id
                })
                .ToListAsync();



            // Format the raw data into a list of filter data
            List<FilterData> filterDataList = rawFilterData
                .GroupBy(x => x.filterName)
                .Select(x => new FilterData
                {
                    Type = "Custom",
                    Caption = x.Select(y => y.filterName).FirstOrDefault(),
                    Options = x.Select(y => (IQueryFilterOption)new QueryFilterOption
                    {
                        Id = y.optionId.ToString(),
                        Label = y.optionName
                    })
                    .ToList()
                })
                .ToList();

            // Add the filters
            filters.AddRange(filterDataList);


            return filters;
        }
    }
}
