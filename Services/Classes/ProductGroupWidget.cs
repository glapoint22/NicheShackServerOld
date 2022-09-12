using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Classes
{
    public class ProductGroupWidget : Widget
    {
        public Caption Caption { get; set; }
        public List<QueriedProduct> Products { get; set; }
        public Query Query { get; set; }


        public override void SetProperty(string property, ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            base.SetProperty(property, ref reader, options);

            switch (property)
            {
                case "caption":
                    Caption = (Caption)JsonSerializer.Deserialize(ref reader, typeof(Caption), options);
                    break;
                case "query":
                    Query = (Query)JsonSerializer.Deserialize(ref reader, typeof(Query), options);
                    break;
            }
        }


        public async override Task SetData(NicheShackContext context, QueryParams queryParams)
        {
            ParameterExpression productParam = Expression.Parameter(typeof(Product), "x");
            //Expression expression = GetQueryExpession(Query, productParam);
            //Expression<Func<Product, bool>> exp = Expression.Lambda<Func<Product, bool>>(expression, productParam);

            ConstantExpression products = Expression.Constant(context.Products);
            ConstantExpression pricePoints = Expression.Constant(context.PricePoints);



            MemberExpression idProperty = Expression.Property(productParam, "Id");

            //MethodCallExpression selectExp = Expression.Call(
            //            typeof(Enumerable),
            //            "Select",
            //            new Type[] { typeof(Product), typeof(int) },
            //            products,
            //            Expression.Lambda<Func<Product, int>>(idProperty, productParam));


            //MethodCallExpression groupJoinExp = Expression.Call(
            //    typeof(Enumerable),
            //    "GroupJoin",
            //    new Type[] { typeof(Product), typeof(PricePoint), typeof(int),  },
            //    );


            //var trumpy = await context.Products
            //    .Where(x => context.Products
            //        .GroupJoin(context.PricePoints, a => a.Id, b => b.ProductId, (product, pricePoints) => new
            //        {
            //            product,
            //            pricePoints
            //        })
            //        .SelectMany(z => z.pricePoints.DefaultIfEmpty(), (a, b) => new
            //        {
            //            id = a.product.Id,
            //            minPrice = a.product.MinPrice,
            //            pricePointPrice = b.Price
            //        })
            //        .Where(x => x.minPrice > 10 || x.pricePointPrice > 10)
            //        .Select(x => x.id)
            //        .Contains(x.Id))
            //    .Select(x => x.Name)
            //    .ToListAsync();



            //var prods = await context.Products
            //    .GroupJoin(context.PricePoints, a => a.Id, b => b.ProductId, (products, pricePoints) => new
            //    {
            //        products,
            //        pricePoints
            //    })
            //    .SelectMany(z => z.pricePoints.DefaultIfEmpty(), (a, b) => new
            //    {
            //        id = a.products.Id,
            //        minPrice = a.products.MinPrice,
            //        pricePointPrice = b.Price
            //    })
            //    .Where(x => x.minPrice > 10 || x.pricePointPrice > 10)
            //    .Select(x => x.id)
            //    .ToListAsync();






            Products = await context.Products
                //.Where(exp)
                .Select(x => new QueriedProduct
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlId = x.UrlId,
                    UrlName = x.UrlName,
                    Rating = x.Rating,
                    TotalReviews = x.TotalReviews,
                    MinPrice = x.MinPrice,
                    MaxPrice = x.MaxPrice,
                    Image = new Image
                    {
                        Name = x.Media.Name,
                        Src = x.Media.ImageSm
                    },
                    OneStar = x.OneStar,
                    TwoStars = x.TwoStars,
                    ThreeStars = x.ThreeStars,
                    FourStars = x.FourStars,
                    FiveStars = x.FiveStars
                })
                .ToListAsync();





            //if (Queries != null)
            //{
            //    queryParams.Queries = Queries;
            //    queryParams.Limit = 24;
            //    queryParams.UsesFilters = false;
            //    QueryService queryService = new QueryService(context);
            //    Products = await queryService.GetProductGroup(queryParams);

            //    // If query type is auto and it's for related products and product id is greater than zero, set the caption
            //    if (queryParams.Queries != null && queryParams.Queries.Count(x => x.QueryType == QueryType.Auto && x.IntValue == 2) > 0 && queryParams.ProductId > 0 && Caption != null)
            //    {
            //        string nicheName = await context.Products
            //        .AsNoTracking()
            //        .Where(x => x.Id == queryParams.ProductId)
            //        .Select(x => x.Niche.Name)
            //        .SingleOrDefaultAsync();
            //        Caption.Text = "More " + nicheName + " products";
            //    }
            //}
        }


        private Expression GetQueryExpession(Query query, ParameterExpression productParam)
        {
            Expression expression = null;

            for (int i = 0; i < query.Elements.Count; i++)
            {
                QueryElement queryElement = query.Elements[i];

                if (i == 0)
                {
                    expression = queryElement.QueryElementType == QueryElementType.QueryRow ?
                        GetRowExpression(queryElement.QueryRow, productParam) : GetQueryExpession(queryElement.QueryGroup.Query, productParam);
                }
                else
                {
                    Expression rightExpression;
                    QueryElement nextQueryElement = query.Elements[i + 1];

                    // Is the query element a row or group?
                    if (nextQueryElement.QueryElementType == QueryElementType.QueryRow)
                    {
                        rightExpression = GetRowExpression(nextQueryElement.QueryRow, productParam);
                    }
                    else
                    {
                        rightExpression = GetQueryExpession(nextQueryElement.QueryGroup.Query, productParam);
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



        private Expression GetRowExpression(QueryRow queryRow, ParameterExpression productParam)
        {
            Expression expression = null;

            switch (queryRow.QueryType)
            {
                case QueryType.Category:
                    PropertyInfo categoryProperty1 = typeof(Product).GetProperty("Niche");
                    PropertyInfo categoryProperty2 = categoryProperty1.PropertyType.GetProperty("Category");
                    PropertyInfo categoryProperty3 = categoryProperty2.PropertyType.GetProperty("Id");
                    MemberExpression niche = Expression.Property(productParam, categoryProperty1);
                    MemberExpression nicheCategory = Expression.Property(niche, categoryProperty2);
                    MemberExpression nicheCategoryId = Expression.Property(nicheCategory, categoryProperty3);
                    ConstantExpression categoryId = Expression.Constant(queryRow.Item.Id);
                    expression = Expression.Equal(nicheCategoryId, categoryId);
                    break;

                case QueryType.Niche:
                    PropertyInfo nicheProperty = typeof(Product).GetProperty("NicheId");
                    MemberExpression nicheIdProperty = Expression.Property(productParam, nicheProperty);
                    ConstantExpression nicheId = Expression.Constant(queryRow.Item.Id);
                    expression = Expression.Equal(nicheIdProperty, nicheId);
                    break;

                case QueryType.ProductGroup:
                    // Product properties
                    MemberExpression SubgroupProductsProperty = Expression.Property(productParam, "SubgroupProducts");
                    MemberExpression idProperty = Expression.Property(productParam, "Id");


                    // SubgroupProduct param and its properties
                    ParameterExpression subgroupProductParameter = Expression.Parameter(typeof(SubgroupProduct), "z");
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
                        Expression.Lambda<Func<SubgroupProduct,
                        bool>>(Expression.Equal(subgroupIdProperty, subgroupId), subgroupProductParameter));



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
                    PropertyInfo priceProperty1 = typeof(Product).GetProperty("MinPrice");
                    MemberExpression minPriceProperty = Expression.Property(productParam, priceProperty1);
                    ConstantExpression price = Expression.Constant(queryRow.Price);
                    expression = GetComparisonOperatorExpression((ComparisonOperatorType)queryRow.ComparisonOperatorType, minPriceProperty, price);
                    break;

                case QueryType.Rating:
                    PropertyInfo ratingProperty = typeof(Product).GetProperty("Rating");
                    MemberExpression rating = Expression.Property(productParam, ratingProperty);
                    ConstantExpression value = Expression.Constant(queryRow.Integer);
                    expression = GetComparisonOperatorExpression((ComparisonOperatorType)queryRow.ComparisonOperatorType, rating, value);
                    break;

                case QueryType.KeywordGroup:
                    // Product properties
                    MemberExpression KeywordGroupsBelongingToProductProperty = Expression.Property(productParam, "KeywordGroups_Belonging_To_Product");
                    MemberExpression idProp = Expression.Property(productParam, "Id");


                    // KeywordGroupBelongingToProduct param and its properties
                    ParameterExpression KeywordGroupBelongingToProductParameter = Expression.Parameter(typeof(KeywordGroup_Belonging_To_Product), "z");
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
                    PropertyInfo dateProperty = typeof(Product).GetProperty("Date");
                    MemberExpression date = Expression.Property(productParam, dateProperty);
                    ConstantExpression dateValue = Expression.Constant(queryRow.Date);
                    expression = GetComparisonOperatorExpression((ComparisonOperatorType)queryRow.ComparisonOperatorType, date, dateValue);
                    break;

                case QueryType.Auto:
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

    }
}