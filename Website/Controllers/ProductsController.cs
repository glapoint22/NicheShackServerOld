﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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

        public ProductsController(IUnitOfWork unitOfWork, SearchSuggestionsService searchSuggestionsService, QueryService queryService)
        {
            this.unitOfWork = unitOfWork;
            this.searchSuggestionsService = searchSuggestionsService;
            this.queryService = queryService;
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
                NicheId = x.NicheId
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
                        Url = x.Media.Image
                    },
                    Value = x.Value,
                    Type = x.Type
                });


                if(subproducts.Count() > 0)
                {
                    product.Components = subproducts
                    .Where(x => x.Type == 0)
                    .Select(x => new SubproductViewModel
                    {
                        Name = x.Name,
                        Description = x.Description,
                        Image = x.Image,
                        Value = x.Value
                    });

                    product.Bonuses = subproducts
                        .Where(x => x.Type == 1)
                        .Select(x => new SubproductViewModel
                        {
                            Name = x.Name,
                            Description = x.Description,
                            Image = x.Image,
                            Value = x.Value
                        });
                }
                


                // Price Points
                product.PricePoints = await unitOfWork.ProductPrices.GetCollection(x => x.ProductId == product.Id, x => new PricePointViewModel
                {
                    Image = new ImageViewModel
                    {
                        Name = x.Media.Name,
                        Url = x.Media.Image
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




                // Additional Info
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


                

                // Media
                product.Media = await unitOfWork.ProductMedia.GetCollection(x => x.ProductId == product.Id, x => new MediaViewModel
                {
                    Name = x.Media.Name,
                    VideoId = x.Media.VideoId,
                    VideoType = x.Media.VideoType,
                    Image = x.Media.Image,
                    Type = x.Media.MediaType,
                    Thumbnail = x.Media.Thumbnail
                });



                // Related Products
                QueryParams queryParams = new QueryParams();
                Query query = new Query();
                query.IntValue = 2;
                query.QueryType = QueryType.Auto;
                queryParams.Queries = new List<Query> { query };
                queryParams.ProductId = product.Id;
                queryParams.Limit = 24;
                queryParams.UsesFilters = false;
                string nicheName = await unitOfWork.Niches.Get(x => x.Id == product.NicheId, x => x.Name);

                product.RelatedProducts = new ProductGroupViewModel();
                product.RelatedProducts.Caption = "More " + nicheName + " products"; ;
                product.RelatedProducts.Products = await queryService.GetProductGroup(queryParams);


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