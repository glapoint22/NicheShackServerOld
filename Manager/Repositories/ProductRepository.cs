using DataAccess.Classes;
using DataAccess.Models;
using DataAccess.Repositories;
using DataAccess.ViewModels;
using Manager.Classes;
using Manager.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Repositories
{
    public class ProductRepository : SearchableRepository<Product>, IProductRepository
    {
        private readonly NicheShackContext context;

        public ProductRepository(NicheShackContext context) : base(context)
        {
            this.context = context;
        }



        public async Task<IEnumerable<ProductFilterViewModel>> GetProductFilters(int productId, int filterId)
        {
            // Get filter options based on the filter id
            var filterOptions = await context.FilterOptions.AsNoTracking().Where(x => x.FilterId == filterId).Select(x => new
            {
                x.Id,
                x.Name
            }).ToArrayAsync();

            // Grab just the filter option ids from the filter options
            int[] filterOptionIds = filterOptions.Select(x => x.Id).ToArray();


            // These are the filter option ids that are being used in the current product
            int[] checkedFilterOptionIds = await context.ProductFilters
                .AsNoTracking()
                .Where(x => filterOptionIds.Contains(x.FilterOptionId) && x.ProductId == productId)
                .Select(x => x.FilterOptionId)
                .ToArrayAsync();


            // Return the product filters
            IEnumerable<ProductFilterViewModel> productFilters = filterOptions.Select(x => new ProductFilterViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Checked = checkedFilterOptionIds.Contains(x.Id)
            }).ToArray();

            return productFilters;
        }




        public async Task<ProductViewModel> GetProduct(int productId)
        {

            return await context.Products
                .AsNoTracking()
                .Where(x => x.Id == productId).Select(x => new ProductViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Vendor = new ItemViewModel
                    {
                        Id = x.Vendor.Id,
                        Name = x.Vendor.Name
                    },
                    Keywords = x.ProductKeywords.Select(y => new ItemViewModel
                    {
                        Id = y.Id,
                        Name = y.Keyword.Name
                    }),
                    Rating = x.Rating,
                    TotalReviews = x.TotalReviews,
                    Hoplink = x.Hoplink,
                    Description = x.Description,
                    Content = x.ProductContent.Select(y => new ProductContentViewModel
                    {
                        Id = y.Id,
                        Name = y.Name,
                        Icon = new ImageViewModel
                        {
                            Id = y.Media.Id,
                            Name = y.Media.Name,
                            Url = y.Media.Url
                        },
                        PriceIndices = y.Product.ProductPricePoints
                        .OrderBy(z => z.Index)
                        .Select(z => y.PriceIndices.Select(w => w.Index).Contains(z.Index))

                    }),
                    PricePoints = x.ProductPricePoints
                .OrderBy(y => y.Index)
                .Select(y => new ProductPricePointViewModel
                {
                    Id = y.Id,
                    TextBefore = y.TextBefore,
                    WholeNumber = y.WholeNumber,
                    Decimal = y.Decimal,
                    TextAfter = y.TextAfter
                }),
                    Image = new ImageViewModel
                    {
                        Id = x.Media.Id,
                        Name = x.Media.Name,
                        Url = x.Media.Url
                    },
                    Media = x.ProductMedia.Select(y => new ProductMediaViewModel
                    {
                        ItemId = y.Id,
                        Id = y.Media.Id,
                        Name = y.Media.Name,
                        Url = y.Media.Url,
                        Thumbnail = y.Media.Thumbnail,
                        Type = y.Media.Type
                    }),
                    MinPrice = x.MinPrice,
                    MaxPrice = x.MaxPrice
                }).SingleOrDefaultAsync();







        }






        public async Task<IEnumerable<QueryBuilderViewModel>> GetAlita(List<Query> queries)
        {
            int categoryQueryIndex = queries.FindIndex(x => x.QueryType == QueryType.Category);
            int nicheQueryIndex = queries.FindIndex(x => x.QueryType == QueryType.Niche);
            int subgroupQueryIndex = queries.FindIndex(x => x.QueryType == QueryType.ProductSubgroup);
            int productIdsQueryIndex = -1;


            //queryBuilderData.ProductIds = await context.ProductKeywords.Where(x => queryBuilderData.Keywords.Contains(x.Name)).Select(x => x.ProductId).ToListAsync();



            ////If a category query exists
            //if (categoryQueryIndex != -1)
            //{
            //    List<int> categoryIds = queries[categoryQueryIndex].Value.ConvertAll(int.Parse);
            //    List<int> NicheIds = await context.Niches.Where(x => categoryIds.Contains(x.CategoryId)).Select(x => x.Id).ToListAsync();


            //    //If a niche query does NOT exist
            //    if (nicheQueryIndex == -1)
            //    {
            //        //Create one
            //        queries.Add(new Query { QueryType = QueryType.Niche, Operator = new List<OperatorType> { OperatorType.Equals }, Value = new List<string>() });
            //        nicheQueryIndex = queries.Count - 1;
            //    }

            //    //Loop through each niche id
            //    foreach (var nicheId in NicheIds)
            //    {
            //        //Add each niche id to the niche query
            //        queries[nicheQueryIndex].Value.Add(nicheId.ToString());
            //    }
            //}

            //If a subgroup query exists
            if (subgroupQueryIndex != -1)
            {
                productIdsQueryIndex = queries.FindIndex(x => x.QueryType == QueryType.ProductIds);
                List<int> productIds = await context.SubgroupProducts.Where(x => queries[subgroupQueryIndex].Value.ConvertAll(int.Parse).Contains(x.SubgroupId)).Select(x => x.ProductId).ToListAsync();

                //If a products id query does NOT exist
                if (productIdsQueryIndex == -1)
                {
                    //Create one
                    queries.Add(new Query { QueryType = QueryType.ProductIds, Operator = new List<OperatorType> { OperatorType.Equals }, Value = new List<string>() });
                    productIdsQueryIndex = queries.Count - 1;
                }


                //Loop through each product id
                foreach (var productId in productIds)
                {
                    //Add each product id to the productid query
                    queries[productIdsQueryIndex].Value.Add(productId.ToString());
                }
            }



            return await context.Products
            .AsNoTracking()
            .Where(new QueryBuilderViewModel(queries))
            //.AsQueryable()
            .Select(x => new QueryBuilderViewModel
            {
                Name = x.Name,
                Rating = x.Rating,
                TotalReviews = x.TotalReviews,
                MinPrice = x.MinPrice,
                MaxPrice = x.MaxPrice,
                ImageName = x.Media.Name,
                ImageUrl = x.Media.Url

            }).ToListAsync();
        }
    }
}
