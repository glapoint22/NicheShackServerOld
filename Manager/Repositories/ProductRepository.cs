using DataAccess.Classes;
using DataAccess.Models;
using DataAccess.Repositories;
using DataAccess.ViewModels;
using Manager.ViewModels;
using Microsoft.EntityFrameworkCore;
using Services.Classes;
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
                    MinPrice = x.MinPrice,
                    MaxPrice = x.MaxPrice,
                    ShippingType = x.ShippingType,
                    RecurringPayment = new RecurringPayment
                    {
                        TrialPeriod = x.TrialPeriod,
                        RecurringPrice = x.RecurringPrice,
                        RebillFrequency = x.RebillFrequency,
                        TimeFrameBetweenRebill = x.TimeFrameBetweenRebill,
                        SubscriptionDuration = x.SubscriptionDuration
                    },
                    Image = new ImageViewModel
                    {
                        Id = x.Media.Id,
                        Name = x.Media.Name,
                        Image = x.Media.Image
                    }
                }).SingleOrDefaultAsync();


            // Product Price Points
            product.PricePoints = await context.PricePoints
                 .AsNoTracking()
                 .Where(x => x.ProductId == productId)
                 .Select(x => new PricePointViewModel
                 {
                     Id = x.Id,
                     Image = new Image
                     {
                         Id = x.Media.Id,
                         Name = x.Media.Name,
                         Src = x.Media.Image
                     },
                     Header = x.Header,
                     Quantity = x.Quantity,
                     UnitPrice = x.UnitPrice,
                     Unit = x.Unit,
                     StrikethroughPrice = x.StrikethroughPrice,
                     Price = x.Price,
                     ShippingType = x.ShippingType,
                     TrialPeriod = x.TrialPeriod,
                     RecurringPrice = x.RecurringPrice,
                     RebillFrequency = x.RebillFrequency,
                     TimeFrameBetweenRebill = x.TimeFrameBetweenRebill,
                     SubscriptionDuration = x.SubscriptionDuration
                 }).ToListAsync();


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





            // Media
            product.Media = await context.ProductMedia
                .AsNoTracking()
                .Where(x => x.ProductId == productId)
                .Select(y => new ProductMediaViewModel
                {
                    ItemId = y.Id,
                    Id = y.Media.Id,
                    Name = y.Media.Name,
                    Image = y.Media.Image,
                    Thumbnail = y.Media.VideoId,
                    Type = y.Media.MediaType
                })
                .ToListAsync();


            // Shipping
            //product.ShippingType = await context.ProductAdditionalInfo
            //    .AsNoTracking()
            //    .Where(x => x.ProductId == productId && !x.IsRecurring)
            //    .Select(x => x.ShippingType)
            //    .SingleOrDefaultAsync();

            // RecurringPayment
            //product.RecurringPayment = await context.ProductAdditionalInfo
            //    .AsNoTracking()
            //    .Where(x => x.ProductId == productId && x.IsRecurring)
            //    .Select(x => new RecurringPayment
            //    {
            //        TrialPeriod = x.TrialPeriod,
            //        Price = x.Price,
            //        RebillFrequency = x.RebillFrequency,
            //        TimeFrameBetweenRebill = x.TimeFrameBetweenRebill,
            //        SubscriptionDuration = x.SubscriptionDuration
            //    })
            //    .SingleOrDefaultAsync();

            // Additional Info
            //product.AdditionalInfo = await context.ProductAdditionalInfo
            //    .AsNoTracking()
            //    .Where(x => x.ProductId == productId)
            //    .Select(x => new AdditionalInfoViewModel
            //    {
            //        Id = x.Id,
            //        IsRecurring = x.IsRecurring,
            //        ShippingType = x.ShippingType,
            //        RecurringPayment = new RecurringPayment
            //        {
            //            TrialPeriod = x.TrialPeriod,
            //            Price = x.Price,
            //            RebillFrequency = x.RebillFrequency,
            //            TimeFrameBetweenRebill = x.TimeFrameBetweenRebill,
            //            SubscriptionDuration = x.SubscriptionDuration
            //        }
            //    })
            //    .ToListAsync();


            return product;
        }
    }
}