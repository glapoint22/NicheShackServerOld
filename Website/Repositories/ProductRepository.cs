using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.Classes;
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




        // ..................................................................................Query Products.....................................................................
        private async Task<List<T>> QueryProducts<T>(QueryParams queryParams, Expression<Func<Product, T>> select)
        {
            ProductViewModel productViewModel = new ProductViewModel(queryParams);

            return await context.Products
                .AsNoTracking()
                .Where(productViewModel)
                .Select(select)
                .ToListAsync();
        }










        // ..................................................................................Get Products.....................................................................
        public async Task<IEnumerable<ProductViewModel>> GetProducts(QueryParams queryParams)
        {
            // Query the products
            var products = await QueryProducts(queryParams, x => new
            {
                id = x.Id,
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
            });



            // Extract the product ids
            var productIds = products.Select(x => x.id).ToList();





            // Get the sales count for each product from product orders
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




            // Join the productIds and the product orders together to form product sales counts
            var productSalesCounts = productIds
               .GroupJoin(productOrders, prodIds => prodIds, prodOrders => prodOrders.productId, (x, y) => new { productId = x, productOrders = y })
               .SelectMany(x => x.productOrders.DefaultIfEmpty(), (x, y) => new
               {
                   x.productId,
                   count = y != null ? y.count : 0
               })
               .ToList();




            // Join the products and product sales counts to get the final result
            return products.Join(productSalesCounts, x => x.id, y => y.productId, (product, productSalesCount) => new
            {
                Id = product.id,
                Name = product.name,
                UrlId = product.urlId,
                UrlName = product.urlName,
                TotalReviews = product.totalReviews,
                MinPrice = product.minPrice,
                MaxPrice = product.maxPrice,
                Image = product.image,
                salesCount = productSalesCount.count,
                Rating = product.rating,
                product.mediaCount
            })
            .OrderBy(x => x.Name.ToLower().StartsWith(queryParams.Query.ToLower()) ? (x.Name.ToLower() == queryParams.Query.ToLower() ? 0 : 1) :
                EF.Functions.Like(x.Name, queryParams.Query + " %") ||
                EF.Functions.Like(x.Name, "% " + queryParams.Query + " %") ||
                EF.Functions.Like(x.Name, "% " + queryParams.Query)
                ? 2 : 3)
                .ThenByDescending(x => x.salesCount)
                .ThenByDescending(x => x.Rating)
                .ThenBy(x => x.MinPrice)
                .ThenByDescending(x => x.mediaCount)
                .Select(x => new ProductViewModel
                {
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
        }




        






        // ..................................................................................Get Product Filters.....................................................................
        public async Task<Filters> GetProductFilters(IEnumerable<ProductViewModel> products, QueryParams queryParams)
        {
            Filters filters = new Filters();

            List<int> productIds = products.Select(x => x.Id).ToList();



            // ******Categories********
            var nicheIds = await context.Products
                .AsNoTracking()
                .Where(y => productIds
                    .Contains(y.Id))
                .Select(y => y.NicheId)
                .Distinct()
                .ToListAsync();


            var categoryIds = await context.Niches
                    .AsNoTracking()
                    .Where(x => nicheIds
                        .Contains(x.Id))
                    .Select(x => x.CategoryId)
                    .Distinct()
                    .ToListAsync();



            filters.CategoryFilters = await context.Categories
                 .AsNoTracking()
                 .Where(x => categoryIds
                     .Contains(x.Id))
                 .Select(x => new CategoryFilter
                 {
                     UrlId = x.UrlId,
                     UrlName = x.UrlName,
                     Name = x.Name,
                     Niches = x.Niches
                         .Where(y => nicheIds
                             .Contains(y.Id))
                         .Select(y => new NicheFilter
                         {
                             UrlId = y.UrlId,
                             UrlName = y.UrlName,
                             Name = y.Name
                         })
                         .ToList()
                 })
                 .ToListAsync();







            // ******Price Filter********
            List<PriceFilterOption> priceFilterOptions = new List<PriceFilterOption>();
            var priceRanges = await context.PriceRanges.AsNoTracking().ToListAsync();

            if (queryParams.PriceFilter != null)
            {
                // Clear the price filter
                var priceFilter = queryParams.PriceFilter;
                queryParams.PriceFilter = null;

                // Query products without the price filter
                var prods = await QueryProducts(queryParams, x => new
                {
                    x.MinPrice,
                    x.MaxPrice
                });

                // Get the selected price filter options
                var selectedPriceFilterOptions = priceFilter.Options
                    .Select(x => x.Label.Split('-')
                        .Select(z => int.Parse(z))
                        .ToArray())
                        .Select(z => new
                        {
                            min = z[0],
                            max = z[1]
                        })
                    .ToList();


                // Get the price filter options based on the queried products
                priceFilterOptions =
                (from pr in priceRanges
                 from p in prods
                 orderby pr.Id
                 where ((p.MinPrice >= pr.Min && p.MinPrice < pr.Max) || selectedPriceFilterOptions.Contains(new
                 {
                     min = pr.Min,
                     max = pr.Max
                 }))
                 select new PriceFilterOption
                 {
                     Label = pr.Label,
                     Min = pr.Min,
                     Max = pr.Max
                 }).ToList();


                queryParams.PriceFilter = priceFilter;
            }
            else
            {
                priceFilterOptions =
                (from pr in priceRanges
                 from p in products
                 orderby pr.Id
                 where (p.MinPrice >= pr.Min && p.MinPrice < pr.Max)
                 select new PriceFilterOption
                 {
                     Label = pr.Label,
                     Min = pr.Min,
                     Max = pr.Max
                 }).ToList();
            }



            List<PriceFilterOption> options = priceFilterOptions.Distinct().ToList();

            filters.PriceFilter = new PriceFilter
            {
                Caption = "Price",
                Options = options
            };
            








            // ******Rating Filter********
            IEnumerable<double> productRatings;
            List<QueryFilterOption> ratingOptions = new List<QueryFilterOption>();

            // Check to see if we have a customer rating filter selected
            if (queryParams.RatingFilter != null)
            {
                // Remove the customer rating filter and query without it
                // This is so we can get other rating filters that belong in the results
                var ratingFilter = queryParams.RatingFilter;
                queryParams.RatingFilter = null;
                productRatings = await QueryProducts(queryParams, x => x.Rating);
                queryParams.RatingFilter = ratingFilter;

                List<double> selectedRatingOptions = ratingFilter.Options.Select(x => Convert.ToDouble(x.Id)).ToList();
                productRatings = productRatings.Concat(selectedRatingOptions).ToList();
            }
            else
            {
                productRatings = products.Select(x => x.Rating).ToList();
            }


            // Rating 4 and up
            if (productRatings.Count(x => x >= 4) > 0)
            {
                ratingOptions.Add(new QueryFilterOption
                {
                    Id = 4
                });
            }



            // Rating 3 and up
            if (productRatings.Count(x => x >= 3 && x < 4) > 0)
            {
                ratingOptions.Add(new QueryFilterOption
                {
                    Id = 3
                });
            }



            // Rating 2 and up
            if (productRatings.Count(x => x >= 2 && x < 3) > 0)
            {
                ratingOptions.Add(new QueryFilterOption
                {
                    Id = 2
                });
            }




            // Rating 1 and up
            if (productRatings.Count(x => x >= 1 && x < 2) > 0)
            {
                ratingOptions.Add(new QueryFilterOption
                {
                    Id = 1
                });
            }


            filters.RatingFilter = new QueryFilter
            {
                Caption = "Customer Rating",
                Options = ratingOptions
            };






            // ******Custom Filters********
            // Get all the filter option Ids that are related to the product Ids
            List<int> filterOptionIds = await context.ProductFilters
                .AsNoTracking()
                .Where(x => productIds
                    .Contains(x.ProductId))
                .Select(x => x.FilterOptionId)
                .Distinct()
                .ToListAsync();


            var customFilters = queryParams.CustomFilters;



            foreach (var customFilter in customFilters)
            {
                queryParams.CustomFilters = customFilters.Where(x => x.Caption != customFilter.Caption).ToList();

                await queryParams.SetFilteredProducts();
                var pIds = await QueryProducts(queryParams, x => x.Id);

                List<int> optionsIds;

                optionsIds = await context.ProductFilters
                    .AsNoTracking()
                    .Where(x => pIds
                        .Contains(x.ProductId) && x.FilterOption.Filter.Name == customFilter.Caption)
                    .Select(x => x.FilterOptionId).Distinct()
                    .ToListAsync();

                // Add the new option ids to the list
                filterOptionIds = filterOptionIds
                    .Concat(optionsIds)
                    .Concat(customFilter.Options.Select(x => x.Id).ToList())
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
            filters.CustomFilters = rawFilterData
                .GroupBy(x => x.filterName)
                .Select(x => new QueryFilter
                {
                    Caption = x.Select(y => y.filterName).FirstOrDefault(),
                    Options = x.Select(y => new QueryFilterOption
                    {
                        Id = y.optionId,
                        Label = y.optionName
                    })
                    .ToList()
                })
                .ToList();

            return filters;
        }
    }
}
