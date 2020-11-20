using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class SearchSuggestionsWorkerService : BackgroundService
    {

        private readonly IServiceScope scope;
        private readonly NicheShackContext context;
        private readonly SearchSuggestionsService searchSuggestionsService;


        public SearchSuggestionsWorkerService(IServiceScopeFactory serviceScopeFactory, SearchSuggestionsService searchSuggestionsService)
        {
            scope = serviceScopeFactory.CreateScope();
            context = scope.ServiceProvider.GetRequiredService<NicheShackContext>();
            this.searchSuggestionsService = searchSuggestionsService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Get all product order ids from the last month
                List<int> productOrderIds = await context.ProductOrders
                .AsNoTracking()
                .Where(x => x.Date >= DateTime.Now.Date.AddMonths(-1))
                .Select(x => x.ProductId).ToListAsync();


                // This will get all keywords info
                var keywords = await context.ProductKeywords
                    .AsNoTracking()
                    .Select(x => new
                    {
                        x.Keyword.Name,
                        SearchVolume = x.Keyword.KeywordSearchVolumes.Where(z => z.Date >= DateTime.Now.Date.AddMonths(-1) && z.KeywordId == x.KeywordId).Count(),
                        ProductIds = context.ProductKeywords
                                .Where(z => z.KeywordId == x.KeywordId && z.Product.Niche.CategoryId == x.Product.Niche.Category.Id)
                                .Select(x => new
                                {
                                    productId = x.ProductId,
                                    categoryId = x.Product.Niche.Category.UrlId,
                                    x.Product.Rating,
                                    mediaCount = x.Product.ProductMedia.Count()
                                })
                                .ToList(),
                        category = new
                        {
                            x.Product.Niche.Category.UrlId,
                            x.Product.Niche.Category.Name,
                            x.Product.Niche.Category.UrlName,
                        }
                    })
                    .ToListAsync();


                // Project the keywords into search words
                var searchWords = keywords.GroupBy(x => x.Name, (key, x) => new
                {
                    name = key,
                    searchVolume = x.Select(z => z.SearchVolume).FirstOrDefault(),
                    categories = x.Select(z => new
                    {
                        z.category.Name,
                        z.category.UrlId,
                        z.category.UrlName,
                        productIds = z.ProductIds.Where(w => w.categoryId == z.category.UrlId)
                        .Select(a => new
                        {
                            a.productId,
                            a.Rating,
                            a.mediaCount,
                            salesCount = productOrderIds.Count(c => c == a.productId)
                        })
                        .ToList()
                    })
                    .GroupBy(x => x.UrlId, (key, a) => new
                    {
                        UrlId = key,
                        Name = a.Select(w => w.Name).FirstOrDefault(),
                        UrlName = a.Select(w => w.UrlName).FirstOrDefault(),
                        productIds = a.Select(w => w.productIds).FirstOrDefault()
                    })
                    .Select(z => new
                    {
                        z.UrlId,
                        z.Name,
                        z.UrlName,
                        salesCount = z.productIds.Select(w => w.salesCount).Sum(),
                        mediaCount = z.productIds.Select(w => w.mediaCount).Sum(),
                        rating = z.productIds.Select(w => w.Rating).Sum() / z.productIds.Count()
                    })
                    .Select(z => new SearchWordCategory
                    {
                        UrlId = z.UrlId,
                        Name = z.Name,
                        UrlName = z.UrlName,
                        Weight = (z.rating * 0.8) + (z.salesCount * 0.15) + (z.mediaCount * .05)
                    }).ToList()
                })
                .Select(x => new SearchWord
                {
                    Name = x.name,
                    SearchVolume = x.searchVolume,
                    Categories = x.categories
                })
                .ToList();


                

                // Clear the root node
                searchSuggestionsService.root = new Node();


                // Insert the search words
                foreach (SearchWord searchWord in searchWords)
                {
                    searchSuggestionsService.Insert(searchWord);
                }


                // 1 hour
                await Task.Delay(1000 * 60 * 60);
            }
        }



        public override Task StopAsync(CancellationToken cancellationToken)
        {
            scope.Dispose();

            return base.StopAsync(cancellationToken);
        }
    }
}
