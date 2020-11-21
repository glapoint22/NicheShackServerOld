using DataAccess.Interfaces;
using DataAccess.Models;
using Manager.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Manager.ViewModels
{
    public class QueryBuilderViewModel : IWhere<Product>
    {
        private readonly IEnumerable<Query> queries;

        public QueryBuilderViewModel(IEnumerable<Query> queries)
        {
            this.queries = queries;
        }


        public QueryBuilderViewModel()
        {
        }


        public string Name { get; set; }
        public double Rating { get; set; }
        public int TotalReviews { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public string ImageName { get; set; }
        public string ImageUrl { get; set; }



        public IQueryable<Product> SetWhere(IQueryable<Product> source)
        {
            ParameterExpression product = Expression.Parameter(typeof(Product));
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


            var exp = Expression.Lambda<Func<Product, bool>>(left[^1], product);
            source = source.Where(exp);























            //ParameterExpression param = Expression.Parameter(typeof(Product), "x");
            //MemberExpression property = Expression.Property(param, "Id");

            //ConstantExpression value;
            //BinaryExpression result;


            //List<BinaryExpression> logicalOperator = new List<BinaryExpression>();



            //foreach (TempClass query in tempList)
            //{
            //    value = Expression.Constant(query.IntValue);
            //    result = Expression.Equal(property, value);

            //    if(logicalOperator.Count == 0)
            //    {
            //        logicalOperator.Add(result);
            //    }

            //    if(logicalOperator[logicalOperator.Count - 1] != result)
            //    {

            //        if(query.LogicalOperator == 1)
            //        {
            //            logicalOperator.Add(Expression.AndAlso(logicalOperator[logicalOperator.Count - 1], result));
            //        }
            //        else if (query.LogicalOperator == 2)
            //        {
            //            logicalOperator.Add(Expression.OrElse(logicalOperator[logicalOperator.Count - 1], result));
            //        }


            //    }
            //}



            //var exp = Expression.Lambda<Func<Product, bool>>(logicalOperator[logicalOperator.Count - 1], param);
            //source = source.Where(exp);





















            //source = source.Where(x => x.NicheId == 2);



            //foreach (Query query in queries)
            //{


            //    if (query.QueryType == QueryType.Category)
            //    {

            //        IEnumerable<int> categoryIds = query.IntValue.ToArray();
            //        source = source.Where(x => categoryIds.Contains(x.Niche.Category.Id));
            //    }



            //    if (query.QueryType == QueryType.Niche)
            //    {
            //        IEnumerable<int> nicheIds = query.IntValue.ToArray();
            //        source = source.Where(x => nicheIds.Contains(x.NicheId));
            //    }



            //    if (query.QueryType == QueryType.ProductIds)
            //    {
            //        IEnumerable<int> productIds = query.IntValue.ToArray();
            //        source = source.Where(x => productIds.Contains(x.Id));
            //    }



            //    if (query.QueryType == QueryType.ProductRating)
            //    {

            //        for (var i = 0; i < query.DoubleValue.Count; i++)
            //        {
            //            var ratingValue = query.DoubleValue[i];

            //            switch (query.Operator[i])
            //            {
            //                case OperatorType.Equals:
            //                    source = source.Where(x => x.Rating == ratingValue);
            //                    break;

            //                case OperatorType.GreaterThan:
            //                    source = source.Where(x => x.Rating > ratingValue);
            //                    break;

            //                case OperatorType.GreaterThanOrEqualTo:
            //                    source = source.Where(x => x.Rating >= ratingValue);
            //                    break;

            //                case OperatorType.LessThan:
            //                    source = source.Where(x => x.Rating < ratingValue);
            //                    break;

            //                case OperatorType.LessThanOrEqualTo:
            //                    source = source.Where(x => x.Rating <= ratingValue);
            //                    break;
            //            }
            //        }
            //    }



            //    if (query.QueryType == QueryType.ProductPrice)
            //    {

            //        for (var i = 0; i < query.DoubleValue.Count; i++)
            //        {
            //            var priceValue = query.DoubleValue[i];

            //            switch (query.Operator[i])
            //            {
            //                case OperatorType.Equals:
            //                    source = source.Where(x => (x.MaxPrice == 0 && x.MinPrice == priceValue) || (x.MaxPrice != 0 && x.MaxPrice == priceValue));
            //                    break;

            //                case OperatorType.GreaterThan:
            //                    source = source.Where(x => (x.MaxPrice == 0 && x.MinPrice > priceValue) || (x.MaxPrice != 0 && x.MaxPrice > priceValue));
            //                    break;

            //                case OperatorType.GreaterThanOrEqualTo:
            //                    source = source.Where(x => (x.MaxPrice == 0 && x.MinPrice >= priceValue) || (x.MaxPrice != 0 && x.MaxPrice >= priceValue));
            //                    break;

            //                case OperatorType.LessThan:
            //                    source = source.Where(x => (x.MaxPrice == 0 && x.MinPrice < priceValue) || (x.MaxPrice != 0 && x.MaxPrice < priceValue));
            //                    break;

            //                case OperatorType.LessThanOrEqualTo:
            //                    source = source.Where(x => (x.MaxPrice == 0 && x.MinPrice <= priceValue) || (x.MaxPrice != 0 && x.MaxPrice <= priceValue));
            //                    break;
            //            }
            //        }
            //    }


            //    if (query.QueryType == QueryType.ProductCreationDate)
            //    {

            //        for (var i = 0; i < query.DateValue.Count; i++)
            //        {
            //            var dateValue = query.DateValue[i];

            //            switch (query.Operator[i])
            //            {
            //                case OperatorType.Equals:
            //                    source = source.Where(x => x.Date == dateValue);
            //                    break;

            //                case OperatorType.GreaterThan:
            //                    source = source.Where(x => x.Date > dateValue);
            //                    break;

            //                case OperatorType.GreaterThanOrEqualTo:
            //                    source = source.Where(x => x.Date >= dateValue);
            //                    break;

            //                case OperatorType.LessThan:
            //                    source = source.Where(x => x.Date < dateValue);
            //                    break;

            //                case OperatorType.LessThanOrEqualTo:
            //                    source = source.Where(x => x.Date <= dateValue);
            //                    break;
            //            }
            //        }
            //    }





            //}














            return source;
        }
    }
}