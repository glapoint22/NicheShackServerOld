using DataAccess.Interfaces;
using DataAccess.Models;
using Manager.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

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


            foreach (Query query in queries)
            {


                if (query.QueryType == QueryType.Category)
                {

                    IEnumerable<int> categoryIds = query.IntValue.ToArray();
                    source = source.Where(x => categoryIds.Contains(x.Niche.Category.Id));
                }



                if (query.QueryType == QueryType.Niche)
                {
                    IEnumerable<int> nicheIds = query.IntValue.ToArray();
                    source = source.Where(x => nicheIds.Contains(x.NicheId));
                }



                if (query.QueryType == QueryType.ProductIds)
                {
                    IEnumerable<int> productIds = query.IntValue.ToArray();
                    source = source.Where(x => productIds.Contains(x.Id));
                }



                if (query.QueryType == QueryType.ProductRating)
                {

                    for (var i = 0; i < query.DoubleValue.Count; i++)
                    {
                        var ratingValue = query.DoubleValue[i];

                        switch (query.Operator[i])
                        {
                            case OperatorType.Equals:
                                source = source.Where(x => x.Rating == ratingValue);
                                break;

                            case OperatorType.GreaterThan:
                                source = source.Where(x => x.Rating > ratingValue);
                                break;

                            case OperatorType.GreaterThanOrEqualTo:
                                source = source.Where(x => x.Rating >= ratingValue);
                                break;

                            case OperatorType.LessThan:
                                source = source.Where(x => x.Rating < ratingValue);
                                break;

                            case OperatorType.LessThanOrEqualTo:
                                source = source.Where(x => x.Rating <= ratingValue);
                                break;
                        }
                    }
                }



                if (query.QueryType == QueryType.ProductPrice)
                {

                    for (var i = 0; i < query.DoubleValue.Count; i++)
                    {
                        var priceValue = query.DoubleValue[i];

                        switch (query.Operator[i])
                        {
                            case OperatorType.Equals:
                                source = source.Where(x => (x.MaxPrice == 0 && x.MinPrice == priceValue) || (x.MaxPrice != 0 && x.MaxPrice == priceValue));
                                break;

                            case OperatorType.GreaterThan:
                                source = source.Where(x => (x.MaxPrice == 0 && x.MinPrice > priceValue) || (x.MaxPrice != 0 && x.MaxPrice > priceValue));
                                break;

                            case OperatorType.GreaterThanOrEqualTo:
                                source = source.Where(x => (x.MaxPrice == 0 && x.MinPrice >= priceValue) || (x.MaxPrice != 0 && x.MaxPrice >= priceValue));
                                break;

                            case OperatorType.LessThan:
                                source = source.Where(x => (x.MaxPrice == 0 && x.MinPrice < priceValue) || (x.MaxPrice != 0 && x.MaxPrice < priceValue));
                                break;

                            case OperatorType.LessThanOrEqualTo:
                                source = source.Where(x => (x.MaxPrice == 0 && x.MinPrice <= priceValue) || (x.MaxPrice != 0 && x.MaxPrice <= priceValue));
                                break;
                        }
                    }
                }


                if (query.QueryType == QueryType.ProductCreationDate)
                {

                    for (var i = 0; i < query.DateValue.Count; i++)
                    {
                        var dateValue = query.DateValue[i];

                        switch (query.Operator[i])
                        {
                            case OperatorType.Equals:
                                source = source.Where(x => x.Date == dateValue);
                                break;

                            case OperatorType.GreaterThan:
                                source = source.Where(x => x.Date > dateValue);
                                break;

                            case OperatorType.GreaterThanOrEqualTo:
                                source = source.Where(x => x.Date >= dateValue);
                                break;

                            case OperatorType.LessThan:
                                source = source.Where(x => x.Date < dateValue);
                                break;

                            case OperatorType.LessThanOrEqualTo:
                                source = source.Where(x => x.Date <= dateValue);
                                break;
                        }
                    }
                }





            }










            //source = source.Where(x => queries.ToArray()[0].Valu .Contains(x.NicheId));


            ////---------RATING ONLY---------\\
            //source = source.Where(x => x.Rating > 3);


            //////---------PRICE ONLY---------\\
            //source = source.Where(x => (x.MaxPrice == 0 && x.MinPrice > 17) || (x.MaxPrice != 0 && x.MaxPrice > 17));


            ////---------KEYWORDS ONLY---------\\
            //source = source.Where(x => queryBuilderData.ProductIds.Contains(x.Id));


            ////---------FEATURED ONLY---------\\
            //source = source.Where(x => featuredProductIds.Contains(x.Id));




            return source;
        }
    }
}