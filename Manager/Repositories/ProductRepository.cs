using DataAccess.Classes;
using DataAccess.Models;
using DataAccess.Repositories;
using DataAccess.ViewModels;
using Manager.Classes;
using Manager.ViewModels;
using Microsoft.EntityFrameworkCore;
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

            var product = await context.Products
                .AsNoTracking()
                .Where(x => x.Id == productId)
                .Select(x => new ProductViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Vendor = new ItemViewModel
                    {
                        Id = x.Vendor.Id,
                        Name = x.Vendor.Name
                    },
                    Rating = x.Rating,
                    TotalReviews = x.TotalReviews,
                    Hoplink = x.Hoplink,
                    Description = x.Description,
                    Image = new ImageViewModel
                    {
                        Id = x.Media.Id,
                        Name = x.Media.Name,
                        Url = x.Media.Url
                    },
                    MinPrice = x.MinPrice,
                    MaxPrice = x.MaxPrice
                }).SingleOrDefaultAsync();


            // Keywords
            product.Keywords = await context.ProductKeywords
                .AsNoTracking()
                .Where(x => x.ProductId == productId)
                .Select(x => new ItemViewModel
                {
                    Id = x.Id,
                    Name = x.Keyword.Name
                }).ToListAsync();


            // Subgroups
            product.Subgroups = await context.SubgroupProducts
                .AsNoTracking()
                .Where(x => x.ProductId == productId)
                .Select(x => new ItemViewModel
                {
                    Id = x.Id,
                    Name = x.Subgroup.Name
                }).ToListAsync();


            // Content
            product.Content = await context.ProductContent
                .AsNoTracking()
                .Where(x => x.ProductId == productId)

                .Select(y => new ProductContentViewModel
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

                }).ToListAsync();



            // Price points
            product.PricePoints = await context.ProductPricePoints
                .AsNoTracking()
                .Where(x => x.ProductId == productId)
                .OrderBy(y => y.Index)
                .Select(y => new ProductPricePointViewModel
                {
                    Id = y.Id,
                    TextBefore = y.TextBefore,
                    WholeNumber = y.WholeNumber,
                    Decimal = y.Decimal,
                    TextAfter = y.TextAfter
                })
                .ToListAsync();


            // Media
            product.Media = await context.ProductMedia
                .AsNoTracking()
                .Where(x => x.ProductId == productId)
                .Select(y => new ProductMediaViewModel
                {
                    ItemId = y.Id,
                    Id = y.Media.Id,
                    Name = y.Media.Name,
                    Url = y.Media.Url,
                    Thumbnail = y.Media.Thumbnail,
                    Type = y.Media.Type
                })
                .ToListAsync();

            return product;
        }
    }
}