using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Classes;
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
        private readonly SearchSuggestionsService searchSuggestionsService;
        private readonly QueryService queryService;

        public ProductsController(IUnitOfWork unitOfWork, SearchSuggestionsService searchSuggestionsService, QueryService queryService)
        {
            this.unitOfWork = unitOfWork;
            this.searchSuggestionsService = searchSuggestionsService;
            this.queryService = queryService;
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






        [Route("PageContent")]
        [HttpGet]
        public async Task<ActionResult> GetPageContent(string urlId)
        {
            int productId = await unitOfWork.Products.Get(x => x.UrlId == urlId, x => x.Id);

            if (productId > 0)
            {
                int pageId = await unitOfWork.PageReferenceItems.Get(x => x.ItemId == productId, x => x.PageId);

                if (pageId > 0)
                {
                    string content = await unitOfWork.Pages.Get(x => x.Id == pageId && x.DisplayType == (int)PageDisplayType.Product, x => x.Content);

                    if (content != null) return Ok(content);
                }
            }


            return Ok();
            
        }







        // ..................................................................................Get Suggestions.....................................................................
        [HttpGet]
        [Route("GetSuggestions")]
        public ActionResult GetSuggestions(string searchWords, string categoryId)
        {
            return Ok(searchSuggestionsService.GetSuggestions(searchWords, categoryId));
        }





        // ..................................................................................Get Grid Data.....................................................................
        [HttpPost]
        [Route("GridData")]
        public async Task<ActionResult> GetGridData(QueryParams queryParams)
        {
            // If the query is a keyword, add it to the keyword search volumes
            if(queryParams.Search != null && queryParams.Search != string.Empty)
            {
                int keywordId = await unitOfWork.Keywords.Get(x => x.Name == queryParams.Search, x => x.Id);
                if (keywordId > 0)
                {
                    unitOfWork.KeywordSearchVolumes.Add(new KeywordSearchVolume
                    {
                        KeywordId = keywordId,
                        Date = DateTime.Now
                    });
                    await unitOfWork.Save();
                }
            }
            

            return Ok(await queryService.GetGridData(queryParams));
        }






        // ..................................................................................Get Product Group.....................................................................
        [HttpPost]
        [Route("ProductGroup")]
        public async Task<ActionResult> GetProductGroup(QueryParams queryParams)
        {
            return Ok(await queryService.GetProductGroup(queryParams));
        }
    }
}