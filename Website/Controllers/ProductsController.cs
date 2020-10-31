using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DataAccess.Models;
using DataAccess.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Website.Classes;
using Website.Repositories;
using Website.ViewModels;

namespace Website.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public ProductsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }



        // ..................................................................................Get Product.....................................................................
        [Route("Product")]
        [HttpGet]
        public async Task<ActionResult> Get(string id)
        {
            return Ok(await unitOfWork.Products.Get(x => x.UrlId == id, x => new
            {
                id = x.Id,
                urlId = x.UrlId,
                title = x.Name,
                minPrice = x.MinPrice,
                maxPrice = x.MaxPrice,
                image = new
                {
                    name = x.Media.Name,
                    url = x.Media.Url
                },
                rating = x.Rating,
                totalReviews = x.TotalReviews,
                oneStar = x.OneStar,
                twoStars = x.TwoStars,
                threeStars = x.ThreeStars,
                fourStars = x.FourStars,
                fiveStars = x.FiveStars
            }));


        }




        public async Task<IEnumerable<ProductMediaViewModel>> GetMedia(int id)
        {
            var mediaIds = await unitOfWork.ProductMedia.GetCollection(x => x.ProductId == id, x => x.MediaId);

            return await unitOfWork.Media.GetCollection(x => mediaIds.Contains(x.Id), x => new ProductMediaViewModel
            {
                name = x.Name,
                url = x.Url,
                thumbnail = x.Thumbnail,
                type = x.Type
            });
        }



        // ..................................................................................Get Product Detail.....................................................................
        [Route("ProductDetail")]
        [HttpGet]
        public async Task<ActionResult> GetProductDetail(string id)
        {
            // Get the product based on the id
            ProductDetailViewModel product = await unitOfWork.Products.Get<ProductDetailViewModel>(x => x.UrlId == id);



            // If the product is found in the database, return the product with other product details
            if (product != null)
            {
                if (User.Claims.Count() > 0)
                {
                    product.Hoplink = product.Hoplink + (product.Hoplink.Contains('?') ? "&" : "?") + "tid=" + product.UrlId + "_" + User.FindFirst(ClaimTypes.NameIdentifier).Value;
                }




                var response = new
                {
                    productInfo = new
                    {
                        product,
                        media = await GetMedia(product.Id)
                    },
                    content = await unitOfWork.ProductContent.GetCollection(x => x.ProductId == product.Id, x => new
                    {
                        Icon = new
                        {
                            x.Media.Name,
                            x.Media.Url
                        },
                        x.Name,
                        PriceIndices = x.PriceIndices
                        .OrderBy(y => y.Index)
                        .Select(y => y.Index)
                        .ToList()
                    }),
                    pricePoints = await unitOfWork.PricePoints.GetCollection(x => x.Index, x => x.ProductId == product.Id, y => new
                    {
                        y.TextBefore,
                        y.WholeNumber,
                        y.Decimal,
                        y.TextAfter
                    })
                };

                return Ok(response);
            }

            return NotFound();
        }





        [HttpGet]
        [Route("GetSuggestions")]
        public ActionResult SearchProducts(string searchWords, string categoryId)
        {
            return Ok(unitOfWork.Products.GetSuggestions(searchWords, categoryId));
        }





        // ..................................................................................Get Queried Products.....................................................................
        //[HttpGet]
        //public async Task<ActionResult> GetQueriedProducts(string query = "", string sort = "", int limit = 24, int categoryId = -1, int nicheId = -1, int page = 1, string filter = "")
        //{
        //    // Set the query params object
        //    QueryParams queryParams = new QueryParams(query, sort, categoryId, nicheId, filter);

        //    // Query the products
        //    IEnumerable<ProductViewModel> products = await unitOfWork.Products.GetQueriedProducts(queryParams);

        //    ProductViewModel productDTO = new ProductViewModel();

        //    var response = new
        //    {
        //        products = products.Skip((page - 1) * limit).Take(limit).ToList(),
        //        totalProducts = products.Count(),
        //        categories = await unitOfWork.Categories.GetQueriedCategories(queryParams, products),
        //        filters = await unitOfWork.Products.GetProductFilters(queryParams, products),
        //        numProductsPerPageOptions = productDTO.GetNumProductsPerPageOptions(),
        //        sortOptions = query != string.Empty ? productDTO.GetSearchSortOptions() : productDTO.GetBrowseSortOptions()
        //    };

        //    return Ok(response);
        //}
    }
}