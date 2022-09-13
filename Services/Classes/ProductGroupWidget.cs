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
            if (Query != null)
            {
                queryParams.Query = Query;
                queryParams.Limit = 24;
                queryParams.UsesFilters = false;
                QueryService queryService = new QueryService(context);
                Products = await queryService.GetProductGroup(queryParams);
            }




            ////ParameterExpression productParam = Expression.Parameter(typeof(Product), "x");
            ////Expression expression = GetQueryExpression(Query, productParam);

            //if (expression != null)
            //{
            //    //Expression<Func<Product, bool>> exp = Expression.Lambda<Func<Product, bool>>(expression, productParam);

            //    Products = await context.Products
            //    .Where(exp)
            //    .Select(x => new QueriedProduct
            //    {
            //        Id = x.Id,
            //        Name = x.Name,
            //        UrlId = x.UrlId,
            //        UrlName = x.UrlName,
            //        Rating = x.Rating,
            //        TotalReviews = x.TotalReviews,
            //        MinPrice = x.MinPrice,
            //        MaxPrice = x.MaxPrice,
            //        Image = new Image
            //        {
            //            Name = x.Media.Name,
            //            Src = x.Media.ImageSm
            //        },
            //        OneStar = x.OneStar,
            //        TwoStars = x.TwoStars,
            //        ThreeStars = x.ThreeStars,
            //        FourStars = x.FourStars,
            //        FiveStars = x.FiveStars
            //    })
            //    .ToListAsync();
            //}




            //ConstantExpression products = Expression.Constant(foo, typeof(List<Product>));

            //ParameterExpression PricePointParam = Expression.Parameter(typeof(PricePoint), "b");
            //ParameterExpression pricePointListParam = Expression.Parameter(typeof(IEnumerable<PricePoint>), "pricePoints");

            //NewExpression newGroupJoinDestinationExp = Expression.New(typeof(GroupJoinDestination));
            //MemberInfo productMember = typeof(GroupJoinDestination).GetMember("Product")[0];
            //MemberInfo pricePointsMember = typeof(GroupJoinDestination).GetMember("PricePoints")[0];

            //MemberAssignment productMemberBinding = Expression.Bind(productMember, productParam);
            //MemberAssignment PricePointsMemberBinding = Expression.Bind(pricePointsMember, pricePointListParam);


            //MemberInitExpression groupJoinDestinationMemberInitExp = Expression.MemberInit(newGroupJoinDestinationExp, productMemberBinding, PricePointsMemberBinding);

            //Expression<Func<Product, int>> outerKeySelector = Expression.Lambda<Func<Product, int>>(Expression.Property(productParam, "Id"), productParam);

            //Expression<Func<PricePoint, int>> innerKeySelector = Expression.Lambda<Func<PricePoint, int>>(Expression.Property(PricePointParam, "ProductId"), PricePointParam);

            //Expression<Func<Product, IEnumerable<PricePoint>, GroupJoinDestination>> groupJoinResultSelector = Expression.Lambda<Func<Product, IEnumerable<PricePoint>, GroupJoinDestination>>(groupJoinDestinationMemberInitExp, productParam, pricePointListParam);

            //MethodCallExpression groupJoinExp = Expression.Call(
            //    typeof(Enumerable),
            //    "GroupJoin",
            //    new Type[] { typeof(Product), typeof(PricePoint), typeof(int), typeof(GroupJoinDestination) },
            //    products,
            //    Expression.Property(productParam, "PricePoints"),
            //    outerKeySelector,
            //    innerKeySelector,
            //    groupJoinResultSelector);





            //ParameterExpression groupJoinDestinationParam = Expression.Parameter(typeof(GroupJoinDestination), "a");



            //MethodCallExpression defaultIfEmptyExp = Expression.Call(
            //    typeof(Enumerable),
            //    "DefaultIfEmpty",
            //    new Type[] { typeof(PricePoint) },
            //    Expression.Property(groupJoinDestinationParam, "PricePoints"));



            //Expression<Func<GroupJoinDestination, IEnumerable<PricePoint>>> collectionSelector = 
            //    Expression.Lambda<Func<GroupJoinDestination, IEnumerable<PricePoint>>>(defaultIfEmptyExp, groupJoinDestinationParam);





            //NewExpression newSelectManyDestinationExp = Expression.New(typeof(SelectManyDestination));
            //MemberInfo idMember = typeof(SelectManyDestination).GetMember("Id")[0];
            //MemberInfo minPriceMember = typeof(SelectManyDestination).GetMember("MinPrice")[0];
            //MemberInfo priceMember = typeof(SelectManyDestination).GetMember("Price")[0];


            //MemberExpression productProperty = Expression.Property(groupJoinDestinationParam, typeof(GroupJoinDestination).GetProperty("Product"));
            //MemberExpression idProperty = Expression.Property(productProperty, typeof(Product).GetProperty("Id"));
            //MemberExpression minPriceProperty = Expression.Property(productProperty, typeof(Product).GetProperty("MinPrice"));
            //MemberExpression priceProperty = Expression.Property(PricePointParam, typeof(PricePoint).GetProperty("Price"));


            //MemberAssignment idMemberBinding = Expression.Bind(idMember, idProperty);
            //MemberAssignment minPriceMemberBinding = Expression.Bind(minPriceMember, minPriceProperty);
            //MemberAssignment priceMemberBinding = Expression.Bind(priceMember, priceProperty);

            //MemberInitExpression selectManyDestinationMemberInitExp = Expression.MemberInit(newSelectManyDestinationExp, idMemberBinding, minPriceMemberBinding, priceMemberBinding);



            //Expression<Func<GroupJoinDestination, PricePoint, SelectManyDestination>> selectManyResultSelector =
            //    Expression.Lambda<Func<GroupJoinDestination, PricePoint, SelectManyDestination>>(selectManyDestinationMemberInitExp, groupJoinDestinationParam, PricePointParam);


            //MethodCallExpression selectManyExp = Expression.Call(
            //    typeof(Enumerable),
            //    "SelectMany",
            //    new Type[] { typeof(GroupJoinDestination), typeof(PricePoint), typeof(SelectManyDestination) },
            //    groupJoinExp,
            //    collectionSelector,
            //    selectManyResultSelector
            //    );


            //ParameterExpression selectManyDestinationParam = Expression.Parameter(typeof(SelectManyDestination), "x");
            //MemberExpression selectManyMinPriceProperty = Expression.Property(selectManyDestinationParam, "MinPrice");
            //MemberExpression selectManyPriceProperty = Expression.Property(selectManyDestinationParam, "Price");
            //MemberExpression selectManyIdProperty = Expression.Property(selectManyDestinationParam, "Id");
            //ConstantExpression value = Expression.Constant(10d);


            //var or = Expression.OrElse(Expression.GreaterThan(selectManyMinPriceProperty, value), Expression.GreaterThan(selectManyPriceProperty, value));

            //// Where
            //MethodCallExpression whereExp = Expression.Call(
            //    typeof(Enumerable),
            //    "Where",
            //    new Type[] { typeof(SelectManyDestination) },
            //    selectManyExp,
            //    Expression.Lambda<Func<SelectManyDestination,
            //    bool>>(or, selectManyDestinationParam));


            //// Select
            //MethodCallExpression selectExpression = Expression.Call(
            //    typeof(Enumerable),
            //    "Select",
            //    new Type[] { typeof(SelectManyDestination), typeof(int) },
            //    whereExp,
            //    Expression.Lambda<Func<SelectManyDestination, int>>(selectManyIdProperty, selectManyDestinationParam));

            //var productIdProperty = Expression.Property(productParam, "Id");

            //var contains = Expression.Call(
            //            typeof(Enumerable),
            //            "Contains",
            //            new[] { typeof(int) },
            //            selectExpression,
            //            productIdProperty);

            //Expression<Func<Product, bool>> exp = Expression.Lambda<Func<Product, bool>>(contains, productParam);



            //var trumpy = await context.Products
            //    .Where(x => context.Products
            //        .GroupJoin(x.PricePoints, x => x.Id, z => z.ProductId, (product, pricePoints) => new GroupJoinDestination
            //        {
            //            Product = product,
            //            PricePoints = pricePoints
            //        })
            //        .SelectMany(z => z.PricePoints.DefaultIfEmpty(), (a, b) => new SelectManyDestination
            //        {
            //            Id = a.Product.Id,
            //            MinPrice = a.Product.MinPrice,
            //            Price = b.Price
            //        })
            //        .Where(x => x.MinPrice > 10 || x.Price > 10)
            //        .Select(x => x.Id)
            //        .Contains(x.Id))
            //    .Select(x => x.Name)
            //    .ToListAsync();

















        }


        

    }
}