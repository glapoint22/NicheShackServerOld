using DataAccess.Models;
using DataAccess.Repositories;
using DataAccess.ViewModels;
using Manager.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly NicheShackContext context;

        public ProductRepository(NicheShackContext context) : base(context)
        {
            this.context = context;
        }



        public async Task<IEnumerable<ProductFilterViewModel>> GetProductFilters(int productId, int filterId)
        {
            // Get filter options based on the filter id
            var filterOptions = await context.FilterOptions.Where(x => x.FilterId == filterId).Select(x => new {
                x.Id,
                x.Name
            }).ToArrayAsync();

            // Grab just the filter option ids from the filter options
            int[] filterOptionIds = filterOptions.Select(x => x.Id).ToArray();


            // These are the filter option ids that are being used in the current product
            int[] checkedFilterOptionIds = await context.ProductFilters
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

            return await context.Products.Where(x => x.Id == productId).Select(x => new ProductViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Vendor = new ItemViewModel
                {
                    Id = x.Vendor.Id,
                    Name = x.Vendor.Name
                },
                Keywords = x.Keywords.Select(y => new ItemViewModel
                {
                    Id = y.Id,
                    Name = y.Name
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
                    PriceIndices = y.Product.ProductPricePoints.Select(z => y.PriceIndices.Select(w => w.Index).Contains(z.Index))

                }),
                PricePoints = x.ProductPricePoints.Select(y => new ProductPricePointViewModel
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
                Media = x.ProductMedia.Select(y => new MediaViewModel
                {
                    Id = y.Media.Id,
                    Name = y.Media.Name,
                    Url = y.Media.Url,
                    Thumbnail = y.Media.Thumbnail,
                    Type = (MediaType)y.Media.Type
                }),
                MinPrice = x.MinPrice,
                MaxPrice = x.MaxPrice
            }).FirstOrDefaultAsync();
        }
    }
}
