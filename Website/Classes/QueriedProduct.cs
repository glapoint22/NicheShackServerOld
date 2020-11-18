using System.Collections.Generic;
using System.Linq;
using Website.ViewModels;

namespace Website.Classes
{
    public struct QueriedProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UrlName { get; set; }
        public string UrlId { get; set; }
        public double Rating { get; set; }
        public int TotalReviews { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public ImageViewModel Image { get; set; }
        public int NicheId { get; set; }
        public double Weight { get; set; }
    }

    public static class QueriedProductExtensions
    {
        // ..................................................................................Order By.....................................................................
        public static IOrderedEnumerable<QueriedProduct> OrderBy(this IEnumerable<QueriedProduct> source, ProductViewModel productViewModel)
        {
            return productViewModel.OrderBy(source);
        }



        // ..................................................................................Select.....................................................................
        public static IEnumerable<ProductViewModel> Select(this IEnumerable<QueriedProduct> source, ProductViewModel productViewModel)
        {
            return productViewModel.Select(source);
        }
    }
}
