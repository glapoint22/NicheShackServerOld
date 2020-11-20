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

        //private readonly QueryBuilderData queryBuilderData;

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
            List<BinaryExpression> logicalOperator = new List<BinaryExpression>();



            List<TempClass> tempList = new List<TempClass>();
            tempList.Add(new TempClass { QueryType = QueryType.Category, IntValue = 1, LogicalOperator = 0 });
            tempList.Add(new TempClass { QueryType = QueryType.Category, IntValue = 2, LogicalOperator = 2 });
            //tempList.Add(new TempClass { QueryType = QueryType.Category, IntValue = 3, LogicalOperator = 2 });
            //tempList.Add(new TempClass { QueryType = QueryType.Category, IntValue = 4, LogicalOperator = 1 });
            //tempList.Add(new TempClass { QueryType = QueryType.Category, IntValue = 5, LogicalOperator = 2 });



            







            PropertyInfo property1 = typeof(Product).GetProperty("Niche");
            PropertyInfo property2 = property1.PropertyType.GetProperty("Category");
            PropertyInfo property3 = property2.PropertyType.GetProperty("Id");

            
            MemberExpression niche = Expression.Property(product, property1);
            MemberExpression niche_Category = Expression.Property(niche, property2);
            MemberExpression niche_Category_Id = Expression.Property(niche_Category, property3);


            


            foreach (TempClass tmpList in tempList)
            {
                if (tmpList.QueryType == QueryType.Category)
                {




                    ConstantExpression value = Expression.Constant(tmpList.IntValue);
                    BinaryExpression result = Expression.Equal(niche_Category_Id, value);

                    if (logicalOperator.Count == 0)
                    {
                        logicalOperator.Add(result);
                    }






                    if (logicalOperator[logicalOperator.Count - 1] != result)
                    {

                        if (tmpList.LogicalOperator == 1)
                        {
                            logicalOperator.Add(Expression.AndAlso(logicalOperator[logicalOperator.Count - 1], result));
                        }
                        else if (tmpList.LogicalOperator == 2)
                        {
                            logicalOperator.Add(Expression.OrElse(logicalOperator[logicalOperator.Count - 1], result));
                        }


                    }
                }
            }


            var exp = Expression.Lambda<Func<Product, bool>>(logicalOperator[logicalOperator.Count - 1], product);
            source = source.Where(exp);






            //ParameterExpression param = Expression.Parameter(typeof(Product), "x");
            //MemberExpression property = Expression.Property(param, "Id");

            //ConstantExpression value;
            //BinaryExpression result;


            //List<BinaryExpression> logicalOperator = new List<BinaryExpression>();



            //foreach (TempClass tmpList in tempList)
            //{
            //    value = Expression.Constant(tmpList.IntValue);
            //    result = Expression.Equal(property, value);

            //    if(logicalOperator.Count == 0)
            //    {
            //        logicalOperator.Add(result);
            //    }

            //    if(logicalOperator[logicalOperator.Count - 1] != result)
            //    {

            //        if(tmpList.LogicalOperator == 1)
            //        {
            //            logicalOperator.Add(Expression.AndAlso(logicalOperator[logicalOperator.Count - 1], result));
            //        }
            //        else if (tmpList.LogicalOperator == 2)
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