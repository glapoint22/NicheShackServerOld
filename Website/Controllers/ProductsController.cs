using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Classes;
using Services.Interfaces;
using Website.Repositories;
using Website.ViewModels;

namespace Website.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SearchSuggestionsService searchSuggestionsService;
        private readonly QueryService queryService;
        private readonly IPageService pageService;

        public ProductsController(IUnitOfWork unitOfWork, SearchSuggestionsService searchSuggestionsService, QueryService queryService, IPageService pageService)
        {
            this.unitOfWork = unitOfWork;
            this.searchSuggestionsService = searchSuggestionsService;
            this.queryService = queryService;
            this.pageService = pageService;
        }



        // ..................................................................................Get Product.....................................................................
        [Route("Product")]
        [HttpGet]
        public async Task<ActionResult> Get(string id)
        {

            var product = await unitOfWork.Products.Get(x => x.UrlId == id, x => new ProductDetailViewModel
            {
                Id = x.Id,
                UrlId = x.UrlId,
                Name = x.Name,
                MinPrice = x.MinPrice,
                MaxPrice = x.MaxPrice,
                Rating = x.Rating,
                TotalReviews = x.TotalReviews,
                OneStar = x.OneStar,
                TwoStars = x.TwoStars,
                ThreeStars = x.ThreeStars,
                FourStars = x.FourStars,
                FiveStars = x.FiveStars
            });

            var mediaId = await unitOfWork.ProductMedia.Get(x => x.ProductId == product.Id, x => x.MediaId);

            product.Image = await unitOfWork.Media.Get(x => x.Id == mediaId, x => new ImageViewModel
            {
                Name = x.Name,
                Url = x.Url
            });



            return Ok(product);


        }



        // ..................................................................................Get Media.....................................................................
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



                // Get the products in the browse cookie
                string products = Request.Cookies["browse"];

                // If the cookie does not exist, add the first product
                if (products == null)
                {
                    products = product.Id.ToString();
                }
                else
                {
                    // Add this product to the list of products in the browse cookie
                    List<string> productsList = products.Split(',').ToList();
                    productsList.Insert(0, product.Id.ToString());
                    productsList = productsList.Take(20).Distinct().ToList();
                    products = string.Join(',', productsList);
                }


                // Set the cookie
                Response.Cookies.Append("browse", products, new CookieOptions
                {
                    Expires = new DateTimeOffset(DateTime.Now.AddYears(100))
                });


                string pageContent = null;
                QueryParams queryParams = new QueryParams();


                queryParams.Cookies = Request.Cookies.ToList();

                int pageId = await unitOfWork.PageReferenceItems.Get(x => x.ItemId == product.Id, x => x.PageId);


                if (pageId > 0)
                {
                    pageContent = await unitOfWork.Pages.Get(x => x.Id == pageId && x.DisplayType == (int)PageDisplayType.Product, x => x.Content);
                }

                if (pageContent == null)
                {
                    pageContent = await unitOfWork.Pages.Get(x => x.DisplayType == (int)PageDisplayType.DefaultProduct, x => x.Content);
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
                    pageContent = await pageService.GePage(pageContent, queryParams),
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






        //[Route("PageContent")]
        //[HttpGet]
        //public async Task<ActionResult> GetPageContent(string urlId)
        //{
        //    int productId = await unitOfWork.Products.Get(x => x.UrlId == urlId, x => x.Id);

        //    if (productId > 0)
        //    {
        //        int pageId = await unitOfWork.PageReferenceItems.Get(x => x.ItemId == productId, x => x.PageId);

        //        if (pageId > 0)
        //        {
        //            string content = await unitOfWork.Pages.Get(x => x.Id == pageId && x.DisplayType == (int)PageDisplayType.Product, x => x.Content);

        //            if (content != null) return Ok(content);
        //        }

        //        return Ok(await unitOfWork.Pages.Get(x => x.DisplayType == (int)PageDisplayType.DefaultProduct, x => x.Content));
        //    }


        //    return Ok();
        //}







        // ..................................................................................Get Suggestions.....................................................................
        [HttpGet]
        [Route("GetSuggestions")]
        public ActionResult GetSuggestions(string searchWords, string categoryId)
        {
            return Ok(searchSuggestionsService.GetSuggestions(searchWords, categoryId));
        }





        // ..................................................................................Get Grid Data.....................................................................
        //[HttpPost]
        //[Route("GridData")]
        //public async Task<ActionResult> GetGridData(QueryParams queryParams)
        //{
        //    // If the query is a keyword, add it to the keyword search volumes
        //    if (queryParams.Search != null && queryParams.Search != string.Empty)
        //    {
        //        int keywordId = await unitOfWork.Keywords.Get(x => x.Name == queryParams.Search, x => x.Id);
        //        if (keywordId > 0)
        //        {
        //            unitOfWork.KeywordSearchVolumes.Add(new KeywordSearchVolume
        //            {
        //                KeywordId = keywordId,
        //                Date = DateTime.Now
        //            });
        //            await unitOfWork.Save();
        //        }
        //    }


        //    return Ok(await queryService.GetGridData(queryParams));
        //}






        //// ..................................................................................Get Product Group.....................................................................
        //[HttpPost]
        //[Route("ProductGroup")]
        //public async Task<ActionResult> GetProductGroup(QueryParams queryParams)
        //{
        //    return Ok(await queryService.GetProductGroup(queryParams));
        //}
    }
}