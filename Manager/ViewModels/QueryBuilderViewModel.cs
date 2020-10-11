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




            for (var i = 0; i < queries.ToArray().Length; i++)
            {

                


                //---------NICHES---------\\

                if (queries.ToArray()[i].QueryType == QueryType.Niche)
                {
                    IEnumerable<int> nicheIds = Array.ConvertAll(queries.ToArray()[i].Value.ToArray(), int.Parse);
                    source = source.Where(x => nicheIds.Contains(x.NicheId));
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