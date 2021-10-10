using System.Collections.Generic;
using System.Linq;

namespace Services.Classes
{
    public class KeywordInfo
    {
        public string Name { get; set; }
        public int SearchVolume { get; set; }
        public List<KeywordProduct> Products { get; set; }
        public SearchWordCategory Category { get; set; }


        public static List<SearchTerm> GetSearchTerms(List<KeywordInfo> keywordInfo, List<int> productOrderIds)
        {
            return keywordInfo.GroupBy(x => x.Name, (key, x) => new
            {
                name = key,
                searchVolume = x.Select(z => z.SearchVolume).FirstOrDefault(),
                categories = x.Select(z => new
                {
                    z.Category.Name,
                    z.Category.UrlId,
                    z.Category.UrlName,
                    productIds = z.Products.Where(w => w.CategoryId == z.Category.UrlId)
                    .Select(a => new
                    {
                        a.Id,
                        a.Rating,
                        a.MediaCount,
                        salesCount = productOrderIds.Count(c => c == a.Id)
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
                        mediaCount = z.productIds.Select(w => w.MediaCount).Sum(),
                        rating = z.productIds.Select(w => w.Rating).Sum() / z.productIds.Count()
                    })
                    .Select(z => new
                    {
                        UrlId = z.UrlId,
                        Name = z.Name,
                        UrlName = z.UrlName,
                        Weight = (z.rating * 0.8) + (z.salesCount * 0.15) + (z.mediaCount * .05)
                    }).ToList()
            })
                .Select(x => new SearchTerm
                {
                    Name = x.name,
                    SearchVolume = x.searchVolume,
                    Categories = x.categories
                        .OrderByDescending(z => z.Weight)
                        .Select(z => new SuggestionCategory
                        {
                            UrlId = z.UrlId,
                            Name = z.Name,
                            UrlName = z.UrlName
                        })
                        .ToList()
                })
                .ToList();
        }
    }
}
