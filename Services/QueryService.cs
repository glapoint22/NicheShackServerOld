using DataAccess.Classes;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Services.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Services
{
    public class QueryService
    {
        private readonly NicheShackContext context;

        public QueryService(NicheShackContext context)
        {
            this.context = context;
        }


        // ..................................................................................Get Grid Data.....................................................................
        public async Task<GridData> GetGridData(QueryParams queryParams)
        {
            await queryParams.Init(context);

            List<QueryResult> products = await QueryProducts(queryParams);
            QueryBuilder queryBuilder = new QueryBuilder(queryParams);
            int totalProducts = products.Count();

            return new GridData
            {
                Products = products
                    .OrderBy(queryBuilder)
                    .Select(queryBuilder)
                    .Skip((int)((queryParams.Page - 1) * queryParams.Limit))
                    .Take((int)queryParams.Limit)
                    .ToList(),
                TotalProducts = totalProducts,
                PageCount = Math.Ceiling(totalProducts / queryParams.Limit),
                Filters = await GetProductFilters(products, queryParams),
                ProductCountStart = ((queryParams.Page - 1) * queryParams.Limit) + 1,
                ProductCountEnd = Math.Min(queryParams.Page * queryParams.Limit, totalProducts),
                SortOptions = queryParams.Search != null && queryParams.Search != string.Empty ? queryBuilder.GetSearchSortOptions() : queryBuilder.GetBrowseSortOptions()
            };
        }







        // ..................................................................................Get Product Group.....................................................................
        public async Task<List<QueriedProduct>> GetProductGroup(QueryParams queryParams)
        {
            await queryParams.Init(context);

            List<QueryResult> products = await QueryProducts(queryParams);
            QueryBuilder queryBuilder = new QueryBuilder(queryParams);


            return products
                    .OrderBy(queryBuilder)
                    .Select(queryBuilder)
                    .Take((int)queryParams.Limit)
                    .ToList();
        }






        // ..................................................................................Get Products.....................................................................
        private async Task<List<T>> GetProducts<T>(QueryParams queryParams, Expression<Func<Product, T>> select)
        {
            QueryBuilder queryBuilder = new QueryBuilder(queryParams);

            return await context.Products
                .AsNoTracking()
                .Where(queryBuilder)
                .Select(select)
                .ToListAsync();
        }










        // ..................................................................................Query Products.....................................................................
        private async Task<List<QueryResult>> QueryProducts(QueryParams queryParams)
        {
            // Query the products
            var products = await GetProducts(queryParams, x => new
            {
                id = x.Id,
                name = x.Name,
                urlId = x.UrlId,
                urlName = x.UrlName,
                rating = x.Rating,
                totalReviews = x.TotalReviews,
                minPrice = x.MinPrice,
                maxPrice = x.MaxPrice,
                nicheId = x.NicheId,
                date = x.Date,
                image = new Image
                {
                    Name = x.Media.Name,
                    Url = x.Media.Image
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
            return products.Join(productSalesCounts, x => x.id, y => y.productId, (product, productSalesCount) => new QueryResult
            {
                Id = product.id,
                Name = product.name,
                UrlId = product.urlId,
                UrlName = product.urlName,
                NicheId = product.nicheId,
                TotalReviews = product.totalReviews,
                MinPrice = product.minPrice,
                MaxPrice = product.maxPrice,
                Image = product.image,
                Rating = product.rating,
                Date = product.date,
                Weight = (product.rating * 0.8) + (productSalesCount.count * 0.15) + (product.mediaCount * .05)
            }).ToList();
        }





        // ..................................................................................Get Product Filters.....................................................................
        public async Task<Filters> GetProductFilters(IEnumerable<QueryResult> products, QueryParams queryParams)
        {
            Filters filters = new Filters();
            List<int> productIds = products.Select(x => x.Id).ToList();
            var avgWeight = products.Select(x => x.Weight).Sum() / products.Count();


            // Grab niche info from the products
            var niches = products
                .GroupBy(x => x.NicheId, (key, x) => new
                {
                    nicheId = key,
                    weight = x.Sum(z => z.Weight) / x.Count(),
                    productCount = x.Count()
                }).ToList();



            // Get categories based on niche ids
            var categories = await context.Niches
                    .AsNoTracking()
                    .Where(x => niches.Select(x => x.nicheId).ToList()
                        .Contains(x.Id))
                    .Select(x => new
                    {
                        nicheId = x.Id,
                        nicheName = x.Name,
                        nicheUrlid = x.UrlId,
                        nicheUrlName = x.UrlName,
                        categoryId = x.CategoryId,
                        categoryName = x.Category.Name,
                        categoryUrlId = x.Category.UrlId,
                        categoryUrlName = x.Category.UrlName
                    })
                    .ToListAsync();








            // Join the categories and niches together
            var categoryData = categories.Join(niches, x => x.nicheId, x => x.nicheId, (categories, niches) => new
            {
                categories.nicheId,
                categories.nicheName,
                categories.nicheUrlid,
                categories.nicheUrlName,
                categories.categoryId,
                categories.categoryName,
                categories.categoryUrlId,
                categories.categoryUrlName,
                niches.weight,
                niches.productCount
            })

            // Group the categories together by category id
            .GroupBy(x => x.categoryId, (key, x) => new
            {
                categoryId = key,
                categoryName = x.Select(z => z.categoryName).FirstOrDefault(),
                categoryUrlId = x.Select(z => z.categoryUrlId).FirstOrDefault(),
                categoryUrlName = x.Select(z => z.categoryUrlName).FirstOrDefault(),


                niches = x.Select(z => new
                {
                    z.nicheId,
                    z.nicheName,
                    z.nicheUrlid,
                    z.nicheUrlName,
                    z.weight,
                    z.productCount
                }).ToList()
            })

            // This will add a weight to each category and order the niches by weight
            .Select(x => new
            {
                weight = x.niches.Select(z => z.productCount * z.weight).Sum() / x.niches.Select(z => z.productCount).Sum(),
                x.categoryUrlId,
                x.categoryUrlName,
                x.categoryName,
                niches = x.niches
                    .OrderByDescending(z => z.weight)

                    // group the niches by weight greater or equal to the average weight
                    .GroupBy(z => z.weight >= avgWeight, (key, niches) => new
                    {
                        key,
                        niches = niches.Select(n => new
                        {
                            UrlId = n.nicheUrlid,
                            UrlName = n.nicheUrlName,
                            Name = n.nicheName,
                        }).ToList()
                    }).ToList()
            })

            // Order the categories by weight and group by weight greater or equal to the average weight
            .OrderByDescending(x => x.weight)
            .GroupBy(x => x.weight >= avgWeight, (key, categories) => new
            {
                key,
                categories = categories.Select(z => new
                {
                    UrlId = z.categoryUrlId,
                    UrlName = z.categoryUrlName,
                    Name = z.categoryName,
                    Niches = z.niches,
                }).ToList()
            })
            .ToList();



            // Assign the category filters
            if (categoryData.Count == 2)
            {
                filters.CategoriesFilter = new CategoriesFilter
                {
                    Visible = categoryData.Where(x => x.key).Select(x => x.categories
                        .Select(z => new CategoryFilter
                        {
                            UrlId = z.UrlId,
                            UrlName = z.UrlName,
                            Name = z.Name,
                            Niches = new NichesFilter
                            {
                                ShowHidden = false,
                                Visible = z.Niches.Where(w => w.key).Select(n => n.niches.Select(a => new NicheFilter
                                {
                                    UrlId = a.UrlId,
                                    UrlName = a.UrlName,
                                    Name = a.Name
                                }).ToList()).SingleOrDefault(),
                                Hidden = z.Niches.Where(w => !w.key).Select(n => n.niches.Select(a => new NicheFilter
                                {
                                    UrlId = a.UrlId,
                                    UrlName = a.UrlName,
                                    Name = a.Name
                                }).ToList()).SingleOrDefault()
                            }
                        })
                        .ToList())
                    .SingleOrDefault(),
                    Hidden = categoryData.Where(x => !x.key).Select(x => x.categories
                        .Select(z => new CategoryFilter
                        {
                            UrlId = z.UrlId,
                            UrlName = z.UrlName,
                            Name = z.Name
                        })
                        .ToList()).SingleOrDefault()
                };
            }
            else
            {
                filters.CategoriesFilter = new CategoriesFilter
                {
                    Visible = categoryData.Select(x => x.categories
                        .Select(z => new CategoryFilter
                        {
                            UrlId = z.UrlId,
                            UrlName = z.UrlName,
                            Name = z.Name,
                            Niches = new NichesFilter
                            {
                                ShowHidden = false,
                                Visible = z.Niches.Where(w => w.key).Select(n => n.niches.Select(a => new NicheFilter
                                {
                                    UrlId = a.UrlId,
                                    UrlName = a.UrlName,
                                    Name = a.Name
                                }).ToList()).SingleOrDefault(),
                                Hidden = z.Niches.Where(w => !w.key).Select(n => n.niches.Select(a => new NicheFilter
                                {
                                    UrlId = a.UrlId,
                                    UrlName = a.UrlName,
                                    Name = a.Name
                                }).ToList()).SingleOrDefault()
                            }
                        })
                        .ToList())
                    .SingleOrDefault(),
                };
            }







            // ******Price Filter********
            List<PriceFilterOption> priceFilterOptions = new List<PriceFilterOption>();
            var priceRanges = await context.PriceRanges.AsNoTracking().ToListAsync();

            if (queryParams.PriceFilter != null)
            {
                // Clear the price filter
                var priceFilter = queryParams.PriceFilter;
                queryParams.PriceFilter = null;

                // Query products without the price filter
                var prods = await GetProducts(queryParams, x => new
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
                productRatings = await GetProducts(queryParams, x => x.Rating);
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

                await queryParams.SetFilteredProducts(context);
                var pIds = await GetProducts(queryParams, x => x.Id);

                List<int> optionsIds;

                List<int> filterOptIds = await context.FilterOptions
                    .AsNoTracking()
                    .Where(x => x.Filter.Name == customFilter.Caption)
                    .Select(x => x.Id)
                    .ToListAsync();


                optionsIds = await context.ProductFilters
                    .AsNoTracking()
                    .Where(x => pIds
                        .Contains(x.ProductId) && filterOptIds.Contains(x.FilterOptionId))
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
