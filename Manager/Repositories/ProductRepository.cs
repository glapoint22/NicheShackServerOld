using DataAccess.Classes;
using DataAccess.Models;
using DataAccess.Repositories;
using DataAccess.ViewModels;
using Manager.Classes;
using Manager.Classes.Notifications;
using Manager.ViewModels;
using Microsoft.EntityFrameworkCore;
using Services.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Manager.Classes.Utility;

namespace Manager.Repositories
{
    public class ProductRepository : SearchableRepository<Product>, IProductRepository
    {
        private readonly NicheShackContext context;

        public ProductRepository(NicheShackContext context) : base(context)
        {
            this.context = context;
        }



        public async Task<int> GetVendorId(int tempVendorId)
        {
            TempVendor tempVendor = await context.TempVendors
                .AsNoTracking()
                .Where(x => x.Id == tempVendorId)
                .SingleAsync();


            if (await context.Vendors.AnyAsync(x => x.Name == tempVendor.Name) == false)
            {
                Vendor vendor = new Vendor
                {
                    Name = tempVendor.Name,
                    PrimaryEmail = tempVendor.PrimaryEmail,
                    PrimaryFirstName = tempVendor.PrimaryFirstName,
                    PrimaryLastName = tempVendor.PrimaryLastName
                };

                context.Add(vendor);

                await context.SaveChangesAsync();

                return vendor.Id;
            }
            else
            {
                return await context.Vendors
                    .AsNoTracking()
                    .Where(x => x.Name == tempVendor.Name)
                    .Select(x => x.Id)
                    .SingleAsync();
            }
        }





        public async Task<int> GetNicheId(int tempNicheId)
        {
            Niche tempNiche = await context.TempNiches
                .AsNoTracking()
                .Where(x => x.Id == tempNicheId)
                .Select(x => new Niche
                {
                    CategoryId = x.CategoryId,
                    Name = x.Name.Trim()
                })
                .SingleOrDefaultAsync();

            string categoryName = await context.TempCategories
                .AsNoTracking()
                .Where(x => x.Id == tempNiche.CategoryId)
                .Select(x => x.Name)
                .SingleOrDefaultAsync();

            categoryName = categoryName.Trim();

            if (await context.Categories.AnyAsync(x => x.Name == categoryName) == false)
            {
                Category category = new Category
                {
                    Name = categoryName,
                    UrlName = Utility.GetUrlName(categoryName),
                    UrlId = Utility.GetUrlId()
                };

                context.Add(category);

                await context.SaveChangesAsync();

                Niche niche = new Niche
                {
                    CategoryId = category.Id,
                    Name = tempNiche.Name,
                    UrlName = Utility.GetUrlName(tempNiche.Name),
                    UrlId = Utility.GetUrlId()
                };

                context.Add(niche);

                await context.SaveChangesAsync();

                return niche.Id;
            }
            else
            {
                int categoryId = await context.Categories
                    .AsNoTracking()
                    .Where(x => x.Name == categoryName)
                    .Select(x => x.Id)
                    .SingleOrDefaultAsync();

                if (await context.Niches.AnyAsync(x => x.Name == tempNiche.Name))
                {
                    return await context.Niches
                        .AsNoTracking()
                        .Where(x => x.Name == tempNiche.Name)
                        .Select(x => x.Id)
                        .FirstOrDefaultAsync();
                }


                Niche niche = new Niche
                {
                    CategoryId = categoryId,
                    Name = tempNiche.Name,
                    UrlName = Utility.GetUrlName(tempNiche.Name),
                    UrlId = Utility.GetUrlId()
                };

                context.Add(niche);

                await context.SaveChangesAsync();

                return niche.Id;
            }
        }




        public async Task<List<TempMedia>> GetTempProductMedia(int productId)
        {
            List<int> meidiaIds = await context.TempProductMedia
                .AsNoTracking()
                .Where(x => x.ProductId == productId)
                .Select(x => x.MediaId)
                .ToListAsync();

            return await context.TempMedia
                .AsNoTracking()
                .Where(x => meidiaIds.Contains(x.Id))
                .ToListAsync();
        }




        public async Task<List<double>> GetTempProductPrices(int productId)
        {
            return await context.TempProductPricePoints
                .AsNoTracking()
                .OrderBy(x => x.Index)
                .Where(x => x.ProductId == productId)
                .Select(x => x.WholeNumber + (x.Decimal * .01))
                .ToListAsync();

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



        //private List<NotificationItem> GetNotificationItems(ProductViewModel product)
        //{






        //    return new List<NotificationItem>();
        //}











        public async Task<List<NotificationItem>> GetNotificationItems(int productId)
        {
            var allNotifications = await context.Notifications.Where(x => x.ProductId == productId).Select(x => new
            {
                Id = x.Id,
                NotificationGroupId = x.NotificationGroupId,
                ProductId = x.ProductId,
                ProductName = x.Product.Name,
                ProductImage = x.Product.Media.Thumbnail,
                NotificationType = x.Type,
                CreationDate = x.CreationDate,
                IsNew = x.NotificationGroup.ArchiveDate == null
            }).ToListAsync();


            List<NotificationItem> notifications = allNotifications
            // Group each notification by the group they belong to
            .GroupBy(x => x.NotificationGroupId, (key, n) => n
            // Then order each notification in each group by the most recent date
            .OrderByDescending(y => y.CreationDate)
            // And then return a list that consists of only the first notification of each group
            .FirstOrDefault())
            // Then take that list and order it by either the creation date (if we're compiling a New list) or the archive date (if we're compiling an Archive list)
            .OrderByDescending(x => x.CreationDate)
            .Select(x => new NotificationItem
            {
                Id = x.Id,
                NotificationGroupId = x.NotificationGroupId,
                NotificationType = x.NotificationType,
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                Image = x.ProductImage,
                IsNew = x.IsNew,
                CreationDate = x.CreationDate,
            }).ToList();

            return notifications;
        }

























        public async Task<ProductViewModel> GetProduct(int productId)
        {

            ProductViewModel product = await context.Products
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
                    //Rating = x.Rating,
                    //TotalReviews = x.TotalReviews,
                    Hoplink = x.Hoplink,
                    Description = x.Description,
                    ShippingType = x.ShippingType,
                    RecurringPayment = new RecurringPayment
                    {
                        TrialPeriod = x.TrialPeriod,
                        RecurringPrice = x.RecurringPrice,
                        RebillFrequency = x.RebillFrequency,
                        TimeFrameBetweenRebill = x.TimeFrameBetweenRebill,
                        SubscriptionDuration = x.SubscriptionDuration
                    },
                }).SingleOrDefaultAsync();


            List<double> prices = await context.ProductPrices
                .AsNoTracking()
                .Where(x => x.ProductId == productId)
                .Select(x => x.Price)
                .ToListAsync();

            product.MinPrice = prices.Min();

            if (prices.Count() > 1)
            {
                product.MaxPrice = prices.Max();
            }












            var allNotifications = await context.Notifications.Where(x => x.ProductId == productId && x.Type > (int)NotificationType.Review && x.Type < (int)NotificationType.ProductReportedAsIllegal).Select(x => new
            {
                Id = x.Id,
                NotificationGroupId = x.NotificationGroupId,
                ProductId = x.ProductId,
                ProductName = x.Product.Name,
                ProductImage = x.Product.Media.Thumbnail,
                NotificationType = x.Type,
                CreationDate = x.CreationDate,
                IsNew = x.NotificationGroup.ArchiveDate == null,
                Count = x.NotificationGroup.Notifications.Count()
            }).ToListAsync();









            product.NotificationItems = allNotifications
            // Group each notification by the group they belong to
            .GroupBy(x => x.NotificationGroupId, (key, n) => n
            // Then order each notification in each group by the most recent date
            .OrderByDescending(y => y.CreationDate)
            // And then return a list that consists of only the first notification of each group
            .FirstOrDefault())
            // Then take that list and order it by either the creation date (if we're compiling a New list) or the archive date (if we're compiling an Archive list)
            .OrderByDescending(x => x.CreationDate)
            .Select(x => new NotificationItem
            {
                Id = x.Id,
                NotificationGroupId = x.NotificationGroupId,
                NotificationType = x.NotificationType,
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                Image = x.ProductImage,
                IsNew = x.IsNew,
                CreationDate = x.CreationDate,
                Count = x.Count
            }).ToList();











            // Product Price Points
            product.PricePoints = await context.PricePoints
                 .AsNoTracking()
                 .Where(x => x.ProductPrice.ProductId == productId)
                 .Select(x => new PricePointViewModel
                 {
                     Id = x.Id,
                     Image = new Image
                     {
                         Id = x.Media.Id,
                         Name = x.Media.Name,
                         Src = x.Media.ImageSm
                     },
                     Header = x.Header,
                     Quantity = x.Quantity,
                     UnitPrice = x.UnitPrice,
                     Unit = x.Unit,
                     StrikethroughPrice = x.StrikethroughPrice,
                     Price = x.ProductPrice.Price,
                     ShippingType = x.ShippingType,
                     RecurringPayment = new RecurringPayment
                     {
                         TrialPeriod = x.TrialPeriod,
                         RecurringPrice = x.RecurringPrice,
                         RebillFrequency = x.RebillFrequency,
                         TimeFrameBetweenRebill = x.TimeFrameBetweenRebill,
                         SubscriptionDuration = x.SubscriptionDuration
                     }
                 }).ToListAsync();


            // Keywords
            //product.Keywords = await context.ProductKeywords
            //    .AsNoTracking()
            //    .Where(x => x.ProductId == productId)
            //    .Select(x => new ItemViewModel
            //    {
            //        Id = x.Id,
            //        Name = x.Keyword.Name
            //    }).ToListAsync();


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
                .OrderBy(x => x.Index)
                .Where(x => x.ProductId == productId)
                .Select(y => new ProductMediaViewModel
                {
                    ProductMediaId = y.Id,
                    Id = y.Media.Id,
                    Name = y.Media.Name,
                    Thumbnail = y.Media.Thumbnail,
                    Type = y.Media.MediaType,
                    ImageMd = y.Media.ImageMd,
                    VideoId = y.Media.VideoId,
                    VideoType = y.Media.VideoType,
                    Index = y.Index
                })
                .ToListAsync();


            // Subproducts
            var subproducts = await context.Subproducts
                .AsNoTracking()
                .Where(x => x.ProductId == productId)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Description,
                    Image = new ImageViewModel
                    {
                        Id = x.Media.Id,
                        Name = x.Media.Name,
                        Src = x.Media.ImageSm
                    },
                    x.Value,
                    x.Type
                }).ToListAsync();

            if (subproducts.Count() > 0)
            {
                product.Components = subproducts
                .Where(x => x.Type == 0)
                .Select(x => new SubproductViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Image = x.Image,
                    Value = x.Value
                }).ToList();

                product.Bonuses = subproducts
                    .Where(x => x.Type == 1)
                    .Select(x => new SubproductViewModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        Image = x.Image,
                        Value = x.Value
                    }).ToList();
            }


            return product;
        }
    }
}