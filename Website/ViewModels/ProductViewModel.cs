using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DataAccess.Classes;
using DataAccess.Interfaces;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Website.Classes;

namespace Website.ViewModels
{
    // ISort<Product>, ISelect<Product, ProductViewModel>, 
    public class ProductViewModel : IWhere<Product>
    {
        private readonly QueryParams queryParams;

        public int Id { get; set; }
        public string Name { get; set; }
        public string UrlName { get; set; }
        public string UrlId { get; set; }
        public double Rating { get; set; }
        public int TotalReviews { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public ImageViewModel Image { get; set; }



        // Constructors
        public ProductViewModel() { }

        public ProductViewModel(QueryParams queryParams)
        {
            this.queryParams = queryParams;
        }




        // ..............................................................................Get Num Products Per Page Options.................................................................
        public List<KeyValuePair<string, string>> GetNumProductsPerPageOptions()
        {
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("24", "24"),
                new KeyValuePair<string, string>("48", "48"),
                new KeyValuePair<string, string>("72", "72"),
                new KeyValuePair<string, string>("96", "96")
            };
        }







        // ..............................................................................Get Browse Sort Options.................................................................
        public List<KeyValuePair<string, string>> GetBrowseSortOptions()
        {
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Price: Low to High", "price-asc"),
                new KeyValuePair<string, string>("Price: High to Low", "price-desc"),
                new KeyValuePair<string, string>("Highest Rating", "rating")
            };
        }







        // ..............................................................................Get Search Sort Options.................................................................
        public List<KeyValuePair<string, string>> GetSearchSortOptions()
        {
            List<KeyValuePair<string, string>> options = new List<KeyValuePair<string, string>>();
            options.Add(new KeyValuePair<string, string>("Best Match", "best-match"));
            options.AddRange(GetBrowseSortOptions());

            return options;
        }






        // ..................................................................................Set Select.....................................................................
        public IEnumerable<ProductViewModel> Select(IEnumerable<QueriedProduct> source)
        {
            return source.Select(x => new ProductViewModel
            {
                Id = x.Id,
                UrlId = x.UrlId,
                Name = x.Name,
                UrlName = x.UrlName,
                Rating = x.Rating,
                TotalReviews = x.TotalReviews,
                MinPrice = x.MinPrice,
                MaxPrice = x.MaxPrice,
                Image = x.Image
            });
        }





        // .............................................................................Set Sort Option.....................................................................
        public IOrderedEnumerable<QueriedProduct> OrderBy(IEnumerable<QueriedProduct> source)
        {
            IOrderedEnumerable<QueriedProduct> orderBy = null;


            orderBy = source.OrderBy(x => x.Name.ToLower().StartsWith(queryParams.Query.ToLower()) ? (x.Name.ToLower() == queryParams.Query.ToLower() ? 0 : 1) :
                EF.Functions.Like(x.Name, queryParams.Query + " %") ||
                EF.Functions.Like(x.Name, "% " + queryParams.Query + " %") ||
                EF.Functions.Like(x.Name, "% " + queryParams.Query)
                ? 2 : 3)
                .ThenByDescending(x => x.Weight);



            switch (queryParams.Sort)
            {
                case "price-asc":
                    orderBy = orderBy.ThenBy(x => x.MinPrice);
                    break;
                case "price-desc":
                    orderBy = orderBy.ThenByDescending(x => x.MinPrice);
                    break;
                case "rating":
                    orderBy = orderBy.ThenByDescending(x => x.Rating);
                    break;
            }

            return orderBy;
        }






        // ..................................................................................Set Where.....................................................................
        public IQueryable<Product> SetWhere(IQueryable<Product> source)
        {
            //Search words
            if (queryParams.Query != string.Empty)
            {
                string[] searchWordsArray = queryParams.Query.Split(' ').ToArray();

                source = source.WhereAny(searchWordsArray.Select(w => (Expression<Func<Product, bool>>)(x =>

                EF.Functions.Like(x.Name, w + "[^a-z]%") ||
                EF.Functions.Like(x.Name, "%[^a-z]" + w + "[^a-z]%") ||
                EF.Functions.Like(x.Name, "%[^a-z]" + w)

                ||


                EF.Functions.Like(x.Description, w + "[^a-z]%") ||
                EF.Functions.Like(x.Description, "%[^a-z]" + w + "[^a-z]%") ||
                EF.Functions.Like(x.Description, "%[^a-z]" + w)


                || queryParams.KeywordProductIds.Contains(x.Id)

                )).ToArray());

            }


            //Category
            if (queryParams.CategoryId != string.Empty)
            {
                source = source.Where(x => x.Niche.Category.UrlId == queryParams.CategoryId);
            }


            //Niche
            if (queryParams.NicheId != string.Empty)
            {
                source = source.Where(x => x.Niche.UrlId == queryParams.NicheId);
            }


            


            //Price Filter
            if (queryParams.PriceFilter != null || queryParams.PriceRangeFilter != null)
            {
                List<Expression<Func<Product, bool>>> priceRangeQueries = new List<Expression<Func<Product, bool>>>();
                List<string> priceRanges = new List<string>();

                // Get all price ranges from the price filter
                if (queryParams.PriceFilter != null)
                {
                    priceRanges = queryParams.PriceFilter.Options.Select(x => x.Label).ToList();
                }

                // Get the price range from the price range filter
                if (queryParams.PriceRangeFilter != null)
                {
                    priceRanges.Add(queryParams.PriceRangeFilter.Options.Select(x => x.Label).Single());
                }

                // Loop through the price ranges and add them to the query
                foreach (string priceRange in priceRanges)
                {
                    var priceRangeArray = priceRange.Split('-').Select(x => double.Parse(x)).OrderBy(x => x).ToArray();
                    priceRangeQueries.Add(x => x.MinPrice >= priceRangeArray[0] && x.MinPrice <= priceRangeArray[1]);
                }

                source = source.WhereAny(priceRangeQueries.ToArray());
            }








            // Rating filter
            if (queryParams.RatingFilter != null)
            {
                int rating = queryParams.RatingFilter.Options.Select(x => x.Id).Min();
                source = source.Where(x => x.Rating >= rating);
            }











            // Custom filters
            if (queryParams.FilteredProducts.Count() > 0)
            {
                // Group the filtered products into their respective filters outputting only product ids
                var productIds = queryParams.FilteredProducts
                .GroupBy(x => x.FilterId)
                .Select(x => x.Select(z => z.ProductId).ToList())
                .ToList();



                // Set the where clause for each group of product ids
                for (int i = 0; i < productIds.Count; i++)
                {
                    var ids = productIds[i];
                    source = source.Where(x => ids.Contains(x.Id));
                }
            }
            return source;

        }
    }
}
