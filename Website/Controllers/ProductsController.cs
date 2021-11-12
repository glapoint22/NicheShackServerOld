using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
        private readonly IPageService pageService;

        public ProductsController(IUnitOfWork unitOfWork, SearchSuggestionsService searchSuggestionsService, IPageService pageService)
        {
            this.unitOfWork = unitOfWork;
            this.searchSuggestionsService = searchSuggestionsService;
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
            return await unitOfWork.ProductMedia.GetCollection(x => x.ProductId == id, x => new ProductMediaViewModel
            {
                name = x.Media.Name,
                url = x.Media.Url,
                thumbnail = x.Media.Thumbnail,
                type = x.Media.Type
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


                // Set the query params
                QueryParams queryParams = new QueryParams();
                queryParams.Cookies = Request.Cookies.ToList();
                queryParams.ProductId = product.Id;


                // Set the page stuff
                string pageContent = null;
                int pageId = await unitOfWork.PageReferenceItems.Get(x => x.ItemId == product.Id, x => x.PageId);

                if (pageId > 0)
                {
                    pageContent = await unitOfWork.Pages.Get(x => x.Id == pageId && x.DisplayType == (int)PageDisplayType.Product, x => x.Content);
                }

                if (pageContent == null)
                {
                    pageContent = await unitOfWork.Pages.Get(x => x.DisplayType == (int)PageDisplayType.DefaultProduct, x => x.Content);
                }



                // Price
                product.Price = await unitOfWork.ProductPrices.GetCollection(x => x.ProductId == product.Id, x => new ProductPriceViewModel
                {
                    Id = x.Id,
                    Image = new ImageViewModel
                    {
                        Id = x.Media.Id,
                        Name = x.Media.Name,
                        Url = x.Media.Url
                    },
                    Header = x.Header,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    Unit = x.Unit,
                    StrikethroughPrice = x.StrikethroughPrice,
                    Price = x.Price,
                    AdditionalInfo = x.ProductPriceAdditionalInfo.Where(z => z.ProductPriceId == x.Id)
                        .Select(z => new AdditionalInfoViewModel
                        {
                            Id = z.Id,
                            IsRecurring = z.IsRecurring,
                            ShippingType = z.ShippingType,
                            RecurringPayment = new RecurringPayment
                            {
                                TrialPeriod = z.TrialPeriod,
                                Price = z.Price,
                                RebillFrequency = z.RebillFrequency,
                                TimeFrameBetweenRebill = z.TimeFrameBetweenRebill,
                                SubscriptionDuration = z.SubscriptionDuration
                            }
                        }).ToList()
                });





                product.AdditionalInfo = await unitOfWork.AdditionalInfo.GetCollection(x => x.ProductId == product.Id, z => new AdditionalInfoViewModel
                {
                    Id = z.Id,
                    IsRecurring = z.IsRecurring,
                    ShippingType = z.ShippingType,
                    RecurringPayment = new RecurringPayment
                    {
                        TrialPeriod = z.TrialPeriod,
                        Price = z.Price,
                        RebillFrequency = z.RebillFrequency,
                        TimeFrameBetweenRebill = z.TimeFrameBetweenRebill,
                        SubscriptionDuration = z.SubscriptionDuration
                    }
                });


                var productData = new
                {
                    productInfo = new
                    {
                        product,
                        media = await GetMedia(product.Id)
                    },
                    //content = await unitOfWork.ProductContent.GetCollection(x => x.ProductId == product.Id, x => new
                    //{
                    //    Icon = new
                    //    {
                    //        x.Media.Name,
                    //        x.Media.Url
                    //    },
                    //    x.Name,
                    //    PriceIndices = x.PriceIndices
                    //    .OrderBy(y => y.Index)
                    //    .Select(y => y.Index)
                    //    .ToList()
                    //}),
                    pageContent = await pageService.GePage(pageContent, queryParams),
                    //pricePoints = await unitOfWork.PricePoints.GetCollection(x => x.Index, x => x.ProductId == product.Id, y => new
                    //{
                    //    y.TextBefore,
                    //    y.WholeNumber,
                    //    y.Decimal,
                    //    y.TextAfter
                    //})
                };

                return Ok(productData);
            }

            return NotFound();
        }








        // ..................................................................................Get Suggestions.....................................................................
        [HttpGet]
        [Route("GetSuggestions")]
        public ActionResult GetSuggestions(string searchWords, string nicheId)
        {
            return Ok(searchSuggestionsService.GetSuggestions(searchWords, nicheId));
        }
    }
}