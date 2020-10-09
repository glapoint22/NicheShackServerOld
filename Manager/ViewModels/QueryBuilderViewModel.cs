using DataAccess.Interfaces;
using DataAccess.Models;
using Manager.Classes;
using System.Linq;

namespace Manager.ViewModels
{
    public class QueryBuilderViewModel : IWhere<Product>
    {

        private readonly QueryBuilderData queryBuilderData;

        public QueryBuilderViewModel(QueryBuilderData queryBuilderData)
        {
            this.queryBuilderData = queryBuilderData;
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

            ////---------NICHES ONLY---------\\
            //source = source.Where(x => queryBuilderData.NicheIds.Contains(x.NicheId));


            ////---------RATING ONLY---------\\
            source = source.Where(x => x.Rating > 3);


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