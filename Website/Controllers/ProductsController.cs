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
        private readonly NicheShackContext context;

        public ProductsController(IUnitOfWork unitOfWork, SearchSuggestionsService searchSuggestionsService, QueryService queryService, NicheShackContext context)
        {
            this.unitOfWork = unitOfWork;
            this.searchSuggestionsService = searchSuggestionsService;
            this.queryService = queryService;
            this.context = context;
        }


        // ..................................................................................Get Product.....................................................................
        [HttpGet]
        public async Task<ActionResult> GetProduct(string id)
        {
            ProductDetailViewModel product = await unitOfWork.Products.Get(x => x.UrlId == id, x => new ProductDetailViewModel
            {
                Id = x.Id,
                Name = x.Name,
                UrlName = x.UrlName,
                UrlId = x.UrlId,
                Rating = x.Rating,
                TotalReviews = x.TotalReviews,
                MinPrice = x.MinPrice,
                MaxPrice = x.MaxPrice,
                OneStar = x.OneStar,
                TwoStars = x.TwoStars,
                ThreeStars = x.ThreeStars,
                FourStars = x.FourStars,
                FiveStars = x.FiveStars,
                Description = x.Description,
                NicheId = x.NicheId,
                Hoplink = x.Hoplink,
                ShippingType = x.ShippingType,
                RecurringPayment = new RecurringPayment
                {
                    TrialPeriod = x.TrialPeriod,
                    RecurringPrice = x.RecurringPrice,
                    RebillFrequency = x.RebillFrequency,
                    TimeFrameBetweenRebill = x.TimeFrameBetweenRebill,
                    SubscriptionDuration = x.SubscriptionDuration
                },
                Image = new ImageViewModel
                {
                    Name = x.Media.Name,
                    Src = x.Media.ImageAnySize
                }
            });





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






                // Subproducts
                var subproducts = await unitOfWork.Subproducts.GetCollection(x => x.ProductId == product.Id, x => new
                {
                    Name = x.Name,
                    Description = x.Description,
                    Image = new ImageViewModel
                    {
                        Name = x.Media.Name,
                        Src = x.Media.ImageAnySize
                    },
                    Value = x.Value,
                    Type = x.Type
                });


                if (subproducts.Count() > 0)
                {
                    product.Components = subproducts
                    .Where(x => x.Type == 0)
                    .Select(x => new SubproductViewModel
                    {
                        Name = x.Name,
                        Description = x.Description,
                        Image = x.Image,
                        Value = x.Value
                    }).ToList();

                    product.Bonuses = subproducts
                        .Where(x => x.Type == 1)
                        .Select(x => new SubproductViewModel
                        {
                            Name = x.Name,
                            Description = x.Description,
                            Image = x.Image,
                            Value = x.Value
                        }).ToList();
                }



                // Price Points
                product.PricePoints = await unitOfWork.ProductPrices.GetCollection(x => x.ProductId == product.Id, x => new PricePointViewModel
                {
                    Image = new ImageViewModel
                    {
                        Name = x.Media.Name,
                        Src = x.Media.ImageAnySize
                    },
                    Header = x.Header,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    Unit = x.Unit,
                    StrikethroughPrice = x.StrikethroughPrice,
                    Price = x.Price,
                    ShippingType = x.ShippingType,
                    RecurringPayment = new RecurringPayment
                    {
                        TrialPeriod = x.TrialPeriod,
                        RecurringPrice = x.RecurringPrice,
                        RebillFrequency = x.RebillFrequency,
                        TimeFrameBetweenRebill = x.TimeFrameBetweenRebill,
                        SubscriptionDuration = x.SubscriptionDuration
                    }
                });


                // Media
                product.Media = await unitOfWork.ProductMedia.GetCollection(x => x.ProductId == product.Id, x => new MediaViewModel
                {
                    Name = x.Media.Name,
                    VideoId = x.Media.VideoId,
                    VideoType = x.Media.VideoType,
                    Src = x.Media.ImageAnySize,
                    Type = x.Media.MediaType,
                    Thumbnail = x.Media.Thumbnail
                });



                // Breadcrumb
                UrlItemViewModel niche = await unitOfWork.Niches.Get(x => x.Id == product.NicheId, x => new UrlItemViewModel
                {
                    Id = x.CategoryId,
                    UrlId = x.UrlId,
                    Name = x.Name,
                    UrlName = x.UrlName
                });


                UrlItemViewModel category = await unitOfWork.Categories.Get(x => x.Id == niche.Id, x => new UrlItemViewModel
                {
                    UrlId = x.UrlId,
                    Name = x.Name,
                    UrlName = x.UrlName
                });


                product.Breadcrumb = new List<UrlItemViewModel> { category, niche };


                // Related Products
                ProductGroupWidget productGroupWidget = new ProductGroupWidget();
                productGroupWidget.Queries = new List<Query>
                {
                    new Query
                    {
                        IntValue = 2,
                        QueryType = QueryType.Auto
                    }
                };

                QueryParams queryParams = new QueryParams();
                queryParams.ProductId = product.Id;

                await productGroupWidget.SetData(context, queryParams);

                product.RelatedProducts = new ProductGroupViewModel();
                product.RelatedProducts.Caption = "More " + niche.Name + " products";
                product.RelatedProducts.Products = productGroupWidget.Products;


                return Ok(product);
            }

            return Ok();
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