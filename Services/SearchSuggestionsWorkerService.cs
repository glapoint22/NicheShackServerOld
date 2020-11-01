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
                                .Select(x => x.ProductId)
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
                var searchWords = keywords
                .Select(x => new SearchWord
                {
                    Name = x.Name,
                    SearchVolume = x.SearchVolume,
                    Category = new SearchWordCategory
                    {
                        Name = x.category.Name,
                        UrlId = x.category.UrlId,
                        UrlName = x.category.UrlName,
                        SalesCount = productOrderIds
                            .Where(z => x.ProductIds.ToList().Contains(z))
                            .GroupBy(z => z)
                            .Select(z => z.Count())
                            .Sum()
                    }
                })
                .Distinct()
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
