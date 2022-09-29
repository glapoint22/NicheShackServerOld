using DataAccess.Classes;
using DataAccess.Interfaces;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

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




        public Expression<Func<T, bool>> BuildQuery<T>(Query query)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T));
            Expression expression = GetQueryExpression(query, parameter);

            if (expression == null) return null;

            return Expression.Lambda<Func<T, bool>>(expression, parameter);
        }


        private Expression GetQueryExpression(Query query, ParameterExpression parameter)
        {
            Expression expression = null;

            for (int i = 0; i < query.Elements.Count; i++)
            {
                QueryElement queryElement = query.Elements[i];

                if (i == 0)
                {
                    expression = queryElement.QueryElementType == QueryElementType.QueryRow ?
                        GetRowExpression(queryElement.QueryRow, parameter) : GetQueryExpression(queryElement.QueryGroup.Query, parameter);
                }
                else
                {
                    Expression rightExpression;
                    QueryElement nextQueryElement = query.Elements[i + 1];

                    // Is the query element a row or group?
                    if (nextQueryElement.QueryElementType == QueryElementType.QueryRow)
                    {
                        rightExpression = GetRowExpression(nextQueryElement.QueryRow, parameter);
                    }
                    else
                    {
                        rightExpression = GetQueryExpression(nextQueryElement.QueryGroup.Query, parameter);
                    }



                    // Is the logical operator AND or OR
                    if (queryElement.QueryRow.LogicalOperatorType == LogicalOperatorType.And)
                    {
                        expression = Expression.AndAlso(expression, rightExpression);
                    }
                    else
                    {
                        expression = Expression.OrElse(expression, rightExpression);
                    }

                    i++;
                }
            }

            return expression;
        }



        private Expression GetRowExpression(QueryRow queryRow, ParameterExpression parameter)
        {
            Expression expression;

            switch (queryRow.QueryType)
            {
                case QueryType.Category:
                    MemberExpression niche = Expression.Property(parameter, "Niche");
                    MemberExpression category = Expression.Property(niche, "Category");
                    MemberExpression categoryIdProperty = Expression.Property(category, "Id");
                    ConstantExpression categoryIdValue = Expression.Constant(queryRow.Item.Id);
                    expression = Expression.Equal(categoryIdProperty, categoryIdValue);
                    break;

                case QueryType.Niche:
                    MemberExpression nicheIdProperty = Expression.Property(parameter, "NicheId");
                    ConstantExpression nicheId = Expression.Constant(queryRow.Item.Id);
                    expression = Expression.Equal(nicheIdProperty, nicheId);
                    break;

                case QueryType.ProductGroup:
                    // Product properties
                    MemberExpression SubgroupProductsProperty = Expression.Property(parameter, "SubgroupProducts");
                    MemberExpression idProperty = Expression.Property(parameter, "Id");


                    // SubgroupProduct param and its properties
                    ParameterExpression subgroupProductParameter = Expression.Parameter(typeof(SubgroupProduct));
                    MemberExpression subgroupIdProperty = Expression.Property(subgroupProductParameter, "SubgroupId");
                    MemberExpression productIdProperty = Expression.Property(subgroupProductParameter, "ProductId");


                    // Product Group Id
                    ConstantExpression subgroupId = Expression.Constant(queryRow.Item.Id);


                    // Where
                    MethodCallExpression whereExp = Expression.Call(
                        typeof(Enumerable),
                        "Where",
                        new Type[] { typeof(SubgroupProduct) },
                        SubgroupProductsProperty,
                        Expression.Lambda<Func<SubgroupProduct, bool>>(Expression.Equal(subgroupIdProperty, subgroupId), subgroupProductParameter));



                    // Select
                    MethodCallExpression selectExp = Expression.Call(
                        typeof(Enumerable),
                        "Select",
                        new Type[] { typeof(SubgroupProduct), typeof(int) },
                        whereExp,
                        Expression.Lambda<Func<SubgroupProduct, int>>(productIdProperty, subgroupProductParameter));


                    // Contains
                    expression = Expression.Call(
                        typeof(Enumerable),
                        "Contains",
                        new[] { typeof(int) },
                        selectExp,
                        idProperty);


                    break;

                case QueryType.Price:
                    // Product properties
                    MemberExpression ProductPricesProperty = Expression.Property(parameter, "ProductPrices");
                    MemberExpression id_Property = Expression.Property(parameter, "Id");


                    // ProductPrice param and its properties
                    ParameterExpression ProductPriceParameter = Expression.Parameter(typeof(ProductPrice), "z");
                    MemberExpression PriceProperty = Expression.Property(ProductPriceParameter, "Price");
                    MemberExpression productId_Property = Expression.Property(ProductPriceParameter, "ProductId");


                    // Price
                    ConstantExpression price = Expression.Constant(queryRow.Price);


                    // Where
                    MethodCallExpression where_Exp = Expression.Call(
                        typeof(Enumerable),
                        "Where",
                        new Type[] { typeof(ProductPrice) },
                        ProductPricesProperty,
                        Expression.Lambda<Func<ProductPrice,
                        bool>>(GetComparisonOperatorExpression((ComparisonOperatorType)queryRow.ComparisonOperatorType, PriceProperty, price), ProductPriceParameter));



                    // Select
                    MethodCallExpression select_Exp = Expression.Call(
                        typeof(Enumerable),
                        "Select",
                        new Type[] { typeof(ProductPrice), typeof(int) },
                        where_Exp,
                        Expression.Lambda<Func<ProductPrice, int>>(productId_Property, ProductPriceParameter));


                    // Contains
                    expression = Expression.Call(
                        typeof(Enumerable),
                        "Contains",
                        new[] { typeof(int) },
                        select_Exp,
                        id_Property);

                    break;

                case QueryType.Rating:
                    MemberExpression rating = Expression.Property(parameter, "Rating");
                    ConstantExpression value = Expression.Constant(Convert.ToDouble(queryRow.Integer));
                    expression = GetComparisonOperatorExpression((ComparisonOperatorType)queryRow.ComparisonOperatorType, rating, value);
                    break;

                case QueryType.KeywordGroup:
                    // Product properties
                    MemberExpression KeywordGroupsBelongingToProductProperty = Expression.Property(parameter, "KeywordGroups_Belonging_To_Product");
                    MemberExpression idProp = Expression.Property(parameter, "Id");


                    // KeywordGroupBelongingToProduct param and its properties
                    ParameterExpression KeywordGroupBelongingToProductParameter = Expression.Parameter(typeof(KeywordGroup_Belonging_To_Product));
                    MemberExpression keywordGroupIdProperty = Expression.Property(KeywordGroupBelongingToProductParameter, "KeywordGroupId");
                    MemberExpression productIdProp = Expression.Property(KeywordGroupBelongingToProductParameter, "ProductId");


                    // Keyword Group Id
                    ConstantExpression keywordGroupId = Expression.Constant(queryRow.Item.Id);


                    // Where
                    MethodCallExpression whereExpression = Expression.Call(
                        typeof(Enumerable),
                        "Where",
                        new Type[] { typeof(KeywordGroup_Belonging_To_Product) },
                        KeywordGroupsBelongingToProductProperty,
                        Expression.Lambda<Func<KeywordGroup_Belonging_To_Product,
                        bool>>(Expression.Equal(keywordGroupIdProperty, keywordGroupId), KeywordGroupBelongingToProductParameter));



                    // Select
                    MethodCallExpression selectExpression = Expression.Call(
                        typeof(Enumerable),
                        "Select",
                        new Type[] { typeof(KeywordGroup_Belonging_To_Product), typeof(int) },
                        whereExpression,
                        Expression.Lambda<Func<KeywordGroup_Belonging_To_Product, int>>(productIdProp, KeywordGroupBelongingToProductParameter));


                    // Contains
                    expression = Expression.Call(
                        typeof(Enumerable),
                        "Contains",
                        new[] { typeof(int) },
                        selectExpression,
                        idProp);

                    break;

                case QueryType.Date:
                    MemberExpression date = Expression.Property(parameter, "Date");
                    ConstantExpression dateValue = Expression.Constant(queryRow.Date);
                    expression = GetComparisonOperatorExpression((ComparisonOperatorType)queryRow.ComparisonOperatorType, date, dateValue);
                    break;

                case QueryType.Auto:
                    // Select no products
                    expression = Expression.Equal(Expression.Property(parameter, "Id"), Expression.Constant(0));

                    break;

                case QueryType.Search:
                    string[] searchWordsArray = queryRow.StringValue.Split(' ').ToArray();

                    MemberExpression nameProperty = Expression.Property(parameter, "Name");

                    // EF.Functions.Like
                    MethodInfo efLikeMethod = typeof(DbFunctionsExtensions).GetMethod("Like",
                        BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                        null,
                        new[] { typeof(DbFunctions), typeof(string), typeof(string) },
                        null);

                    expression = Expression.Constant(false);

                    // Loop through each word
                    foreach (string word in searchWordsArray)
                    {
                        ConstantExpression pattern = Expression.Constant("%" + word + "%");
                        MethodCallExpression methodCall = Expression.Call(efLikeMethod,
                            Expression.Property(null, typeof(EF), nameof(EF.Functions)), nameProperty, pattern);

                        expression = Expression.OrElse(expression, methodCall);
                    }

                    break;

                default:
                    // Select no products
                    expression = Expression.Equal(Expression.Property(parameter, "Id"), Expression.Constant(0));
                    break;
            }

            return expression;
        }




        private Expression GetComparisonOperatorExpression(ComparisonOperatorType comparisonOperatorType, Expression left, Expression right)
        {
            Expression expression = null;

            switch (comparisonOperatorType)
            {
                case ComparisonOperatorType.Equal:
                    expression = Expression.Equal(left, right);
                    break;
                case ComparisonOperatorType.NotEqual:
                    expression = Expression.NotEqual(left, right);
                    break;
                case ComparisonOperatorType.GreaterThan:
                    expression = Expression.GreaterThan(left, right);
                    break;
                case ComparisonOperatorType.GreaterThanOrEqual:
                    expression = Expression.GreaterThanOrEqual(left, right);
                    break;
                case ComparisonOperatorType.LessThan:
                    expression = Expression.LessThan(left, right);
                    break;
                case ComparisonOperatorType.LessThanOrEqual:
                    expression = Expression.LessThanOrEqual(left, right);
                    break;
            }

            return expression;
        }


        public IQueryable<Product> SetWhere(IQueryable<Product> source)
        {

            //Expression<Func<Product, bool>> expression = BuildQuery<Product>(queryParams.Query);

            //if (expression != null)
            //{
            //    source = source.Where(expression);
            //    hasWhere = true;
            //}









            Regex regex = new Regex(@".+?\|(?:\d(?:,|-)?)+\|");
            var filters = HttpUtility.UrlDecode(queryParams.Filters);

            MatchCollection matches = regex.Matches(filters);

            foreach (Match match in matches)
            {
                Regex filterRegex = new Regex(@"(^[a-zA-Z\s]+)\|");

                var foo = filterRegex.Match(match.Value).Groups[1].Value;

                //Regex rgex = new Regex(@"\d+");

                //var value = match.Value;
                //var filterOptionIds = rgex.Matches(value);

                //foreach (var filterOptionId in filterOptionIds)
                //{
                //    var a = 0;
                //}
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
            //if (queryParams.PriceFilter != null || queryParams.PriceRangeFilter != null)
            //{
            //    List<Expression<Func<Product, bool>>> priceRangeQueries = new List<Expression<Func<Product, bool>>>();
            //    List<string> priceRanges = new List<string>();

            //    // Get all price ranges from the price filter
            //    if (queryParams.PriceFilter != null)
            //    {
            //        priceRanges = queryParams.PriceFilter.Options.Select(x => x.Label).ToList();
            //    }

            //    // Get the price range from the price range filter
            //    if (queryParams.PriceRangeFilter != null)
            //    {
            //        priceRanges.Add(queryParams.PriceRangeFilter.Options.Select(x => x.Label).Single());
            //    }

            //    // Loop through the price ranges and add them to the query
            //    //foreach (string priceRange in priceRanges)
            //    //{
            //    //    var priceRangeArray = priceRange.Split('-').Select(x => double.Parse(x)).OrderBy(x => x).ToArray();
            //    //    priceRangeQueries.Add(x => x.MinPrice >= priceRangeArray[0] && x.MinPrice <= priceRangeArray[1]);
            //    //}

            //    source = source.WhereAny(priceRangeQueries.ToArray());
            //    hasWhere = true;
            //}








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
