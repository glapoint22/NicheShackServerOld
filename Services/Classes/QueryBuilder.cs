﻿using DataAccess.Classes;
using DataAccess.Interfaces;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Services.Classes
{
    public class QueryBuilder : IWhere<Product>, IEnumerableOrderBy<QueryResult>, IEnumerableSelect<QueryResult, QueriedProduct>
    {
        private readonly QueryParams queryParams;

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
                        if (query.QueryType == QueryType.Category)
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


                        if (query.QueryType == QueryType.Niche)
                        {
                            PropertyInfo nicheProperty = typeof(Product).GetProperty("NicheId");
                            MemberExpression nicheId = Expression.Property(product, nicheProperty);
                            ConstantExpression value = Expression.Constant(query.IntValue);
                            right = Expression.Equal(nicheId, value);
                        }



                        if (query.QueryType == QueryType.ProductRating)
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



                        if (query.QueryType == QueryType.ProductPrice)
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



                        if (query.QueryType == QueryType.ProductSubgroup || query.QueryType == QueryType.ProductKeywords || query.QueryType == QueryType.FeaturedProducts)
                        {
                            PropertyInfo productProperty = typeof(Product).GetProperty("Id");
                            MemberExpression productId = Expression.Property(product, productProperty);
                            ConstantExpression values = Expression.Constant(query.IntValues);
                            MethodInfo method = query.IntValues.GetType().GetMethod("Contains");
                            MethodCallExpression call = Expression.Call(values, method, productId);
                            right = Expression.Equal(call, Expression.Constant(true));
                        }



                        if (query.QueryType == QueryType.ProductCreationDate)
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


                        if (query.QueryType == QueryType.SubQuery)
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

                    return left[^1];
                }




                var exp = Expression.Lambda<Func<Product, bool>>(GetQueries(queryParams.Queries), product);
                source = source.Where(exp);
            }




            //Search words
            if (queryParams.Search != null && queryParams.Search != string.Empty)
            {
                string[] searchWordsArray = queryParams.Search.Split(' ').ToArray();

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
            if (queryParams.CategoryId !=null && queryParams.CategoryId != string.Empty)
            {
                source = source.Where(x => x.Niche.Category.UrlId == queryParams.CategoryId);
            }


            //Niche
            if (queryParams.NicheId != null && queryParams.NicheId != string.Empty)
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
                Image = x.Image
            });
        }





        // .............................................................................Order By.....................................................................
        public IOrderedEnumerable<QueryResult> OrderBy(IEnumerable<QueryResult> source)
        {
            IOrderedEnumerable<QueryResult> orderBy = null;

            if(queryParams.Search != null && queryParams.Search != string.Empty)
            {
                orderBy = source.OrderBy(x => x.Name.ToLower().StartsWith(queryParams.Search.ToLower()) ? (x.Name.ToLower() == queryParams.Search.ToLower() ? 0 : 1) :
                EF.Functions.Like(x.Name, queryParams.Search + " %") ||
                EF.Functions.Like(x.Name, "% " + queryParams.Search + " %") ||
                EF.Functions.Like(x.Name, "% " + queryParams.Search)
                ? 2 : 3)
                .ThenByDescending(x => x.Weight);
            } else
            {
                orderBy = source.OrderByDescending(x => x.Weight);
            }
            



            switch (queryParams.Sort)
            {
                case "price-asc":
                    orderBy = orderBy.OrderBy(x => x.MinPrice);
                    break;
                case "price-desc":
                    orderBy = orderBy.OrderByDescending(x => x.MinPrice);
                    break;
                case "rating":
                    orderBy = orderBy.OrderByDescending(x => x.Rating);
                    break;
            }

            return orderBy;
        }




        // ..............................................................................Get Browse Sort Options.................................................................
        public List<SortOption> GetBrowseSortOptions()
        {
            return new List<SortOption>
            {
                new SortOption{Key = "Price: Low to High", Value ="price-asc" },
                new SortOption{Key = "Price: High to Low", Value = "price-desc" },
                new SortOption{Key = "Highest Rating", Value = "rating" }
            };
        }







        // ..............................................................................Get Search Sort Options.................................................................
        public List<SortOption> GetSearchSortOptions()
        {
            List<SortOption> options = new List<SortOption>();
            options.Add(new SortOption { Key = "Best Match", Value = "best-match" });
            options.AddRange(GetBrowseSortOptions());

            return options;
        }
    }
}