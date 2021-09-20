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
                List<KeywordInfo> keywords = await context.ProductKeywords
                    .AsNoTracking()
                    .Select(x => new KeywordInfo
                    {
                        Name = x.Keyword.Name,
                        SearchVolume = x.Keyword.KeywordSearchVolumes.Where(z => z.Date >= DateTime.Now.Date.AddMonths(-1) && z.KeywordId == x.KeywordId).Count(),
                        Products = context.ProductKeywords
                                .Where(z => z.KeywordId == x.KeywordId && z.Product.Niche.CategoryId == x.Product.Niche.Category.Id)
                                .Select(x => new KeywordProduct
                                {
                                    Id = x.ProductId,
                                    CategoryId = x.Product.Niche.Category.UrlId,
                                    Rating = x.Product.Rating,
                                    MediaCount = x.Product.ProductMedia.Count()
                                })
                                .ToList(),
                        Category = new SearchWordCategory
                        {
                            UrlId = x.Product.Niche.Category.UrlId,
                            Name = x.Product.Niche.Category.Name,
                            UrlName = x.Product.Niche.Category.UrlName,
                        }
                    })
                    .ToListAsync();





                // CategoryIds used for the nodes
                List<string> categoryIds = await context.Categories
                    .AsNoTracking()
                    .Select(x => x.UrlId)
                    .ToListAsync();

                



                searchSuggestionsService.Init(productOrderIds, keywords, categoryIds);




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
