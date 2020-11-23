using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Services;
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

        public ProductsController(IUnitOfWork unitOfWork, SearchSuggestionsService searchSuggestionsService)
        {
            this.unitOfWork = unitOfWork;
            this.searchSuggestionsService = searchSuggestionsService;
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








        // ..................................................................................Get Suggestions.....................................................................
        [HttpGet]
        [Route("GetSuggestions")]
        public ActionResult GetSuggestions(string searchWords, string categoryId)
        {
            return Ok(searchSuggestionsService.GetSuggestions(searchWords, categoryId));
        }












        // ..................................................................................Get Products.....................................................................
        [HttpGet]
        public async Task<ActionResult> GetProducts(string query, string filters = "", string categoryId = "", string nicheId = "", string sort = "", double page = 1)
        {
            double productsPerPage = 40;
            QueryParams queryParams = new QueryParams(query, filters, categoryId, nicheId, sort);
            await queryParams.Init(unitOfWork);

            IEnumerable<QueriedProduct> products = await unitOfWork.Products.GetProducts(queryParams);

            // If the query is a keyword, add it to the keyword search volumes
            int keywordId = await unitOfWork.Keywords.Get(x => x.Name == query, x => x.Id);
            if (keywordId > 0)
            {
                unitOfWork.KeywordSearchVolumes.Add(new KeywordSearchVolume
                {
                    KeywordId = keywordId,
                    Date = DateTime.Now
                });
                await unitOfWork.Save();
            }



            ProductViewModel productViewModel = new ProductViewModel(queryParams);
            int totalProducts = products.Count();

            var response = new
            {
                products = products
                    .OrderBy(productViewModel)
                    .Select(productViewModel)
                    .Skip((int)((page - 1) * productsPerPage))
                    .Take((int)productsPerPage)
                    .ToList(),
                totalProducts = totalProducts,
                pageCount = Math.Ceiling(totalProducts / productsPerPage),
                filters = await unitOfWork.Products.GetProductFilters(products, queryParams),
                start = ((page - 1) * productsPerPage) + 1,
                end = Math.Min(page * productsPerPage, totalProducts),
                sortOptions = query != string.Empty ? productViewModel.GetSearchSortOptions() : productViewModel.GetBrowseSortOptions()
            };



            return Ok(response);
        }
    }
}