using DataAccess.Classes;
using DataAccess.Interfaces;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Services.Classes
{
    public class QueryBuilder : IWhere<Product>, IEnumerableOrderBy<QueryResult>, IEnumerableSelect<QueryResult, QueriedProduct>
    {
        private readonly QueryParams queryParams;
        private bool hasWhere = false;

        public QueryBuilder(QueryParams queryParams)
        {
            this.queryParams = queryParams;
        }


        public IQueryable<Product> SetWhere(IQueryable<Product> source)
        {

            // Queries
            if (queryParams.Queries != null && queryParams.Queries.Count() > 0)
            {
                ParameterExpression product = Expression.Parameter(typeof(Product));

                BinaryExpression GetQueries(IEnumerable<Query> queries)
                {
                    List<BinaryExpression> left = new List<BinaryExpression>();
                    BinaryExpression right = null;


                    foreach (Query query in queries)
                    {

                        // Auto
                        if (query.QueryType == QueryType.Auto)
                        {
                            // Browsed Products
                            if (query.IntValue == 1)
                            {
                                if (queryParams.Cookies == null || queryParams.Cookies.Count(x => x.Key == "browse") == 0) continue;

                                string browseCookie = queryParams.Cookies.Where(x => x.Key == "browse").Select(x => x.Value).SingleOrDefault();

                                if (browseCookie == null) continue;

                                List<int> productIds = browseCookie.Split(',').Select(x => Int32.Parse(x)).ToList();

                                PropertyInfo productProperty = typeof(Product).GetProperty("Id");
                                MemberExpression productId = Expression.Property(product, productProperty);
                                ConstantExpression values = Expression.Constant(productIds);
                                MethodInfo method = productIds.GetType().GetMethod("Contains");
                                MethodCallExpression call = Expression.Call(values, method, productId);
                                right = Expression.Equal(call, Expression.Constant(true));
                            }

                            // Related products
                            else if (query.IntValue == 2 && queryParams.ProductId > 0)
                            {
                                PropertyInfo nicheProperty = typeof(Product).GetProperty("NicheId");
                                PropertyInfo idProperty = typeof(Product).GetProperty("Id");
                                MemberExpression id = Expression.Property(product, idProperty);
                                ConstantExpression productId = Expression.Constant(queryParams.ProductId);
                                MemberExpression nicheId = Expression.Property(product, nicheProperty);
                                ConstantExpression value = Expression.Constant(query.IntValues[0]);

                                right = Expression.AndAlso(Expression.Equal(nicheId, value), Expression.NotEqual(id, productId));
                            }
                        }


                        // Category
                        else if (query.QueryType == QueryType.Category)
                        {
                            PropertyInfo categoryProperty1 = typeof(Product).GetProperty("Niche");
                            PropertyInfo categoryProperty2 = categoryProperty1.PropertyType.GetProperty("Category");
                            PropertyInfo categoryProperty3 = categoryProperty2.PropertyType.GetProperty("Id");
                            MemberExpression niche = Expression.Property(product, categoryProperty1);
                            MemberExpression niche_Category = Expression.Property(niche, categoryProperty2);
                            MemberExpression niche_Category_Id = Expression.Property(niche_Category, categoryProperty3);
                            ConstantExpression value = Expression.Constant(query.IntValue);
                            right = Expression.Equal(niche_Category_Id, value);
                        }


                        // Niche
                        else if (query.QueryType == QueryType.Niche)
                        {
                            if (query.IntValue > 0)
                            {
                                PropertyInfo nicheProperty = typeof(Product).GetProperty("NicheId");
                                MemberExpression nicheId = Expression.Property(product, nicheProperty);
                                ConstantExpression value = Expression.Constant(query.IntValue);
                                right = Expression.Equal(nicheId, value);
                            }
                            else
                            {
                                PropertyInfo categoryProperty1 = typeof(Product).GetProperty("Niche");
                                PropertyInfo categoryProperty2 = categoryProperty1.PropertyType.GetProperty("UrlId");
                                MemberExpression niche = Expression.Property(product, categoryProperty1);
                                MemberExpression niche_Url_Id = Expression.Property(niche, categoryProperty2);
                                ConstantExpression value = Expression.Constant(query.StringValue);
                                right = Expression.Equal(niche_Url_Id, value);
                            }
                        }


                        // Product Rating
                        else if (query.QueryType == QueryType.Rating)
                        {
                            PropertyInfo ratingProperty = typeof(Product).GetProperty("Rating");
                            MemberExpression rating = Expression.Property(product, ratingProperty);
                            ConstantExpression value = Expression.Constant(query.DoubleValue);

                            if (query.ComparisonOperator == ComparisonOperatorType.Equal) right = Expression.Equal(rating, value);
                            if (query.ComparisonOperator == ComparisonOperatorType.NotEqual) right = Expression.NotEqual(rating, value);
                            if (query.ComparisonOperator == ComparisonOperatorType.GreaterThan) right = Expression.GreaterThan(rating, value);
                            if (query.ComparisonOperator == ComparisonOperatorType.GreaterThanOrEqual) right = Expression.GreaterThanOrEqual(rating, value);
                            if (query.ComparisonOperator == ComparisonOperatorType.LessThan) right = Expression.LessThan(rating, value);
                            if (query.ComparisonOperator == ComparisonOperatorType.LessThanOrEqual) right = Expression.LessThanOrEqual(rating, value);
                        }


                        // Product Price
                        else if (query.QueryType == QueryType.Price)
                        {
                            PropertyInfo priceProperty1 = typeof(Product).GetProperty("MinPrice");
                            PropertyInfo priceProperty2 = typeof(Product).GetProperty("MaxPrice");
                            MemberExpression minPrice = Expression.Property(product, priceProperty1);
                            MemberExpression maxPrice = Expression.Property(product, priceProperty2);
                            ConstantExpression zero = Expression.Constant(0.0);
                            ConstantExpression value = Expression.Constant(query.DoubleValue);

                            if (query.ComparisonOperator == ComparisonOperatorType.Equal) right = Expression.OrElse(Expression.AndAlso(Expression.Equal(maxPrice, zero), Expression.Equal(minPrice, value)), Expression.Equal(maxPrice, value));
                            if (query.ComparisonOperator == ComparisonOperatorType.NotEqual) right = Expression.OrElse(Expression.AndAlso(Expression.Equal(maxPrice, zero), Expression.NotEqual(minPrice, value)), Expression.NotEqual(maxPrice, value));
                            if (query.ComparisonOperator == ComparisonOperatorType.GreaterThan) right = Expression.OrElse(Expression.AndAlso(Expression.Equal(maxPrice, zero), Expression.GreaterThan(minPrice, value)), Expression.GreaterThan(maxPrice, value));
                            if (query.ComparisonOperator == ComparisonOperatorType.GreaterThanOrEqual) right = Expression.OrElse(Expression.AndAlso(Expression.Equal(maxPrice, zero), Expression.GreaterThanOrEqual(minPrice, value)), Expression.GreaterThanOrEqual(maxPrice, value));
                            if (query.ComparisonOperator == ComparisonOperatorType.LessThan) right = Expression.OrElse(Expression.AndAlso(Expression.Equal(maxPrice, zero), Expression.LessThan(minPrice, value)), Expression.LessThan(maxPrice, value));
                            if (query.ComparisonOperator == ComparisonOperatorType.LessThanOrEqual) right = Expression.OrElse(Expression.AndAlso(Expression.Equal(maxPrice, zero), Expression.LessThanOrEqual(minPrice, value)), Expression.LessThanOrEqual(maxPrice, value));
                        }


                        // Product Subgroup, Product Keywords, or Featured Products
                        else if (query.QueryType == QueryType.ProductGroup || query.QueryType == QueryType.KeywordGroup)
                        {
                            PropertyInfo productProperty = typeof(Product).GetProperty("Id");
                            MemberExpression productId = Expression.Property(product, productProperty);
                            ConstantExpression values = Expression.Constant(query.IntValues);
                            MethodInfo method = query.IntValues.GetType().GetMethod("Contains");
                            MethodCallExpression call = Expression.Call(values, method, productId);
                            right = Expression.Equal(call, Expression.Constant(true));
                        }


                        // Product Creation Date
                        else if (query.QueryType == QueryType.Date)
                        {
                            PropertyInfo dateProperty = typeof(Product).GetProperty("Date");
                            MemberExpression date = Expression.Property(product, dateProperty);
                            ConstantExpression value = Expression.Constant(query.DateValue);

                            if (query.ComparisonOperator == ComparisonOperatorType.Equal) right = Expression.Equal(date, value);
                            if (query.ComparisonOperator == ComparisonOperatorType.NotEqual) right = Expression.NotEqual(date, value);
                            if (query.ComparisonOperator == ComparisonOperatorType.GreaterThan) right = Expression.GreaterThan(date, value);
                            if (query.ComparisonOperator == ComparisonOperatorType.GreaterThanOrEqual) right = Expression.GreaterThanOrEqual(date, value);
                            if (query.ComparisonOperator == ComparisonOperatorType.LessThan) right = Expression.LessThan(date, value);
                            if (query.ComparisonOperator == ComparisonOperatorType.LessThanOrEqual) right = Expression.LessThanOrEqual(date, value);
                        }


                        // Subquery
                        else if (query.QueryType == QueryType.None)
                        {
                            right = GetQueries(query.SubQueries);
                        }



                        if (left.Count == 0) left.Add(right);


                        if (left[^1] != right)
                        {

                            if (query.LogicalOperator == LogicalOperatorType.And)
                            {
                                left.Add(Expression.AndAlso(left[^1], right));
                            }
                            else
                            {
                                left.Add(Expression.OrElse(left[^1], right));
                            }
                        }
                    }

                    if (left.Count == 0) return null;

                    return left[^1];
                }



                BinaryExpression queries = GetQueries(queryParams.Queries);

                if (queries != null)
                {
                    var exp = Expression.Lambda<Func<Product, bool>>(queries, product);
                    source = source.Where(exp);
                    hasWhere = true;
                }

            }



            if (!queryParams.UsesFilters) return source;


            //Search words
            if (queryParams.Search != null && queryParams.Search != string.Empty)
            {
                string[] searchWordsArray = queryParams.Search.Split(' ').ToArray();

                source = source
                    .WhereAny(searchWordsArray.Select(w => (Expression<Func<Product, bool>>)(x =>
                        EF.Functions.Like(x.Name, "%" + w + "%")


                || queryParams.KeywordProductIds.Contains(x.Id)

                )).ToArray());
                hasWhere = true;
            }


            //Category
            if (queryParams.CategoryId != null && queryParams.CategoryId != string.Empty)
            {
                source = source.Where(x => x.Niche.Category.UrlId == queryParams.CategoryId);
                hasWhere = true;
            }


            //Niche
            if (queryParams.NicheId != null && queryParams.NicheId != string.Empty)
            {
                source = source.Where(x => x.Niche.UrlId == queryParams.NicheId);
                hasWhere = true;
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
                hasWhere = true;
            }








            // Rating filter
            if (queryParams.RatingFilter != null)
            {
                int rating = queryParams.RatingFilter.Options.Select(x => x.Id).Min();
                source = source.Where(x => x.Rating >= rating);
                hasWhere = true;
            }











            // Custom filters
            if (queryParams.FilteredProducts != null && queryParams.FilteredProducts.Count() > 0)
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
                    hasWhere = true;
                }
            }




            if (!hasWhere) source = source.Take(0);

            return source;
        }







        // ..................................................................................Select.....................................................................
        public IEnumerable<QueriedProduct> Select(IEnumerable<QueryResult> source)
        {
            return source.Select(x => new QueriedProduct
            {
                Id = x.Id,
                UrlId = x.UrlId,
                Name = x.Name,
                UrlName = x.UrlName,
                Rating = x.Rating,
                TotalReviews = x.TotalReviews,
                MinPrice = x.MinPrice,
                MaxPrice = x.MaxPrice,
                Image = x.Image,
                OneStar = x.OneStar,
                TwoStars = x.TwoStars,
                ThreeStars = x.ThreeStars,
                FourStars = x.FourStars,
                FiveStars = x.FiveStars
            });
        }





        // .............................................................................Order By.....................................................................
        public IOrderedEnumerable<QueryResult> OrderBy(IEnumerable<QueryResult> source)
        {
            IOrderedEnumerable<QueryResult> orderBy = null;



            switch (queryParams.Sort)
            {
                case "price-asc":
                    orderBy = source.OrderBy(x => x.MinPrice);
                    break;
                case "price-desc":
                    orderBy = source.OrderByDescending(x => x.MinPrice);
                    break;
                case "rating":
                    orderBy = source.OrderByDescending(x => x.Rating);
                    break;
                case "newest":
                    orderBy = source.OrderByDescending(x => x.Date);
                    break;
                default:
                    if (queryParams.Search != null && queryParams.Search != string.Empty)
                    {
                        orderBy = source.OrderBy(x => x.Name.ToLower().StartsWith(queryParams.Search.ToLower()) ? (x.Name.ToLower() == queryParams.Search.ToLower() ? 0 : 1) :
                        EF.Functions.Like(x.Name, "% " + queryParams.Search + " %")
                        ? 2 : 3)
                        .ThenByDescending(x => x.Weight);
                    }
                    else
                    {
                        orderBy = source.OrderByDescending(x => x.Weight);
                    }
                    break;
            }

            return orderBy;
        }




        // ..............................................................................Get Browse Sort Options.................................................................
        //public List<SortOption> GetBrowseSortOptions()
        //{
        //    return new List<SortOption>
        //    {
        //        new SortOption{Key = "Price: Low to High", Value ="price-asc" },
        //        new SortOption{Key = "Price: High to Low", Value = "price-desc" },
        //        new SortOption{Key = "Highest Rating", Value = "rating" },
        //        new SortOption{Key = "Newest", Value = "newest" }
        //    };
        //}







        //// ..............................................................................Get Search Sort Options.................................................................
        //public List<SortOption> GetSearchSortOptions()
        //{

        //    return new List<SortOption>
        //    {
        //        new SortOption { Key = "Best Match", Value = "best-match" },
        //        new SortOption{Key = "Price: Low to High", Value ="price-asc" },
        //        new SortOption{Key = "Price: High to Low", Value = "price-desc" },
        //        new SortOption{Key = "Highest Rating", Value = "rating" },
        //        new SortOption{Key = "Newest", Value = "newest" }
        //    };
        //}
    }
}
