﻿using System.Threading.Tasks;
using Manager.Repositories;
using DataAccess.ViewModels;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Manager.Classes;
using System;
using Services.Classes;
using Services;
using Manager.ViewModels;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.IO;
using static Manager.Classes.Utility;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly QueryService queryService;

        public ProductsController(IUnitOfWork unitOfWork, QueryService queryService)
        {
            this.unitOfWork = unitOfWork;
            this.queryService = queryService;
        }



        [HttpGet]
        [Route("SetData")]
        public async Task<ActionResult> SetData()
        {
            var productData = TempProducts.GetProductData();

            var tempProducts = JsonSerializer.Deserialize<TempProducts>(productData, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });


            foreach (TempProduct tempProduct in tempProducts.Products)
            {
                Product product = new Product();

                // Vendor
                product.VendorId = await unitOfWork.Products.GetVendorId(tempProduct.VendorId);

                // Categories & Niches
                product.NicheId = await unitOfWork.Products.GetNicheId(tempProduct.NicheId);



                // Name
                product.Name = tempProduct.Name.Trim();
                product.UrlName = GetUrlName(product.Name);




                // UrlId
                product.UrlId = GetUrlId();


                // Hoplink
                product.Hoplink = tempProduct.Hoplink;


                // Description
                product.Description = GetDescription(tempProduct.Description);


                unitOfWork.Products.Add(product);


                await unitOfWork.Save();


                // Media
                await SetProductMedia(tempProduct.Id, product);


                // Prices
                await SetProductPrices(tempProduct.Id, product);
            }


            return Ok();
        }




        private async Task SetProductMedia(int productId, Product product)
        {
            List<TempMedia> tempMedia = await unitOfWork.Products.GetTempProductMedia(productId);
            int index = 0;
            foreach (TempMedia media in tempMedia)
            {
                ProductMedia productMedia = null;

                if (media.Type == 6)
                {

                    using (var formData = new MultipartFormDataContent())
                    {
                        using (FileStream fs = new FileStream("C:/inetpub/wwwroot/Manager/wwwroot/images/" + media.Url, FileMode.Open, FileAccess.Read))
                        {
                            using (HttpClient client = new HttpClient())
                            {
                                HttpContent fileStreamContent = new StreamContent(fs);
                                HttpContent imageName = new StringContent(media.Name);
                                HttpContent imageSize = new StringContent("500");

                                formData.Add(fileStreamContent, "image", media.Url);
                                formData.Add(imageName, "name");
                                formData.Add(imageSize, "imageSize");

                                var response = await client.PostAsync("http://localhost:9999/api/Media/Image", formData);

                                var contents = await response.Content.ReadAsStringAsync();

                                var image = JsonSerializer.Deserialize<Item>(contents, new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                });



                                productMedia = new ProductMedia
                                {
                                    ProductId = product.Id,
                                    MediaId = image.Id,
                                    Index = index
                                };

                                if (index == 0)
                                {
                                    product.ImageId = image.Id;
                                }



                            }

                        }
                    }
                }
                else if (media.Type == 8)
                {
                    VideoType videoType = VideoType.Vimeo;
                    string videoId = null;

                    Regex regex = new Regex(@"youtube");
                    if (regex.IsMatch(media.Url))
                    {
                        videoType = VideoType.YouTube;
                        regex = new Regex(@"https://www\.youtube\.com/embed/(.+)");

                        Match match = regex.Match(media.Url);
                        videoId = match.Groups[1].Value;
                    }
                    else
                    {
                        videoType = VideoType.Vimeo;
                        regex = new Regex(@"https://player\.vimeo\.com/video/(.+)");

                        Match match = regex.Match(media.Url);
                        videoId = match.Groups[1].Value;
                    }

                    MediaViewModel video = new MediaViewModel
                    {
                        VideoType = (int)videoType,
                        VideoId = videoId,
                        Name = media.Name
                    };

                    MediaController mediaController = new MediaController(unitOfWork);

                    Item newVideo = await mediaController.TempNewVideo(video);


                    if (newVideo != null)
                    {
                        productMedia = new ProductMedia
                        {
                            ProductId = product.Id,
                            MediaId = newVideo.Id,
                            Index = index
                        };
                    }

                }

                if (productMedia != null)
                    unitOfWork.ProductMedia.Add(productMedia);

                index++;
            }


            await unitOfWork.Save();

            //return tempMedia[0].Id;
        }



        private async Task SetProductPrices(int productId, Product product)
        {
            List<double> prices = await unitOfWork.Products.GetTempProductPrices(productId);

            if (prices.Count() == 1)
            {
                unitOfWork.ProductPrices.Add(new ProductPrice
                {
                    ProductId = product.Id,
                    Price = prices[0]
                });

                await unitOfWork.Save();
            }
            else
            {
                foreach (double price in prices)
                {
                    ProductPrice productPrice = new ProductPrice
                    {
                        ProductId = product.Id,
                        Price = price
                    };

                    unitOfWork.ProductPrices.Add(productPrice);


                    await unitOfWork.Save();

                    unitOfWork.PricePoints.Add(new PricePoint
                    {
                        ProductPriceId = productPrice.Id
                    });

                    await unitOfWork.Save();
                }
            }


        }

        private string GetDescription(string description)
        {
            Regex regex = new Regex(@"(?:(.+?)~~)?(?:(.+?)~~)?(?:(.+?)~~)?(?:(.+?)~~)?(?:(.+?)~~)?(.+)");


            MatchCollection matchCollection = regex.Matches(description);


            List<string> textList = matchCollection[0].Groups.Values
                .Skip(1)
                .Where(x => x.Value != string.Empty)
                .Select(x => x.Value)
                .ToList();

            List<TextBoxData> textBoxDataList = new List<TextBoxData>();

            for (int i = 0; i < textList.Count; i++)
            {
                string text = textList[i];

                // Text
                textBoxDataList.Add(new TextBoxData()
                {
                    ElementType = ElementType.Div,
                    Children = new List<TextBoxData>()
                    {
                        new TextBoxData()
                        {
                            ElementType = ElementType.Text,
                            Text = text
                        }
                    }
                });


                // Break
                if (i != textList.Count - 1)
                {
                    textBoxDataList.Add(new TextBoxData()
                    {
                        ElementType = ElementType.Div,
                        Indent = null,
                        Children = new List<TextBoxData>()
                        {
                            new TextBoxData()
                            {
                                ElementType = ElementType.Break,
                                Indent = null
                            }
                        }
                    });
                }
            }


            string textBoxDataString = JsonSerializer.Serialize(textBoxDataList, new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return textBoxDataString;
        }























































        [HttpGet]
        public async Task<ActionResult> GetProducts(int parentId)
        {
            if (parentId > 0)
            {
                return Ok(await unitOfWork.Products.GetCollection<ItemViewModel<Product>>(x => x.NicheId == parentId));
            }
            else
            {
                return Ok(await unitOfWork.Products.GetCollection<ItemViewModel<Product>>());
            }

        }



        [Route("Parent")]
        [HttpGet]
        public async Task<ActionResult> GetProductParent(int productId)
        {
            var parentNiche = await unitOfWork.Products.Get(x => x.Id == productId, x => x.Niche);
            return Ok(new { id = parentNiche.Id, name = parentNiche.Name });
        }






        [Route("NicheId_SubNicheId")]
        [HttpGet]
        public async Task<ActionResult> GetNicheIds(int productId)
        {
            var subNicheId = await unitOfWork.Products.Get(x => x.Id == productId, x => x.NicheId);
            var nicheId = await unitOfWork.Niches.Get(x => x.Id == subNicheId, x => x.CategoryId);

            return Ok(new { NicheId = nicheId, SubNicheId = subNicheId });
        }



        [Route("SubNiches_Products")]
        [HttpGet]
        public async Task<ActionResult> GetSubnichesAndProducts(int nicheId, int subNicheId)
        {
            var subNiches = await unitOfWork.Niches.GetCollection<ItemViewModel<Niche>>(x => x.CategoryId == nicheId);
            var products = await unitOfWork.Products.GetCollection<ItemViewModel<Product>>(x => x.NicheId == subNicheId);


            return Ok(new { subNiches, products });
        }






        [HttpGet]
        [Route("QueryBuilder")]
        public async Task<ActionResult> GetQueryBuilderProducts(string queryString)
        {
            Query query = JsonSerializer.Deserialize<Query>(queryString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            QueryParams queryParams = new QueryParams();

            queryParams.Query = query;
            queryParams.Limit = 24;
            queryParams.UsesFilters = false;


            return Ok(await queryService.GetProductGroup(queryParams));
        }




        //[HttpPost]
        //[Route("GridData")]
        //public async Task<ActionResult> GetGridData(QueryParams queryParams)
        //{
        //    queryParams.Cookies = Request.Cookies.ToList();
        //    return Ok(await queryService.GetGridData(queryParams));
        //}






        //[HttpPost]
        //[Route("ProductGroup")]
        //public async Task<ActionResult> GetProductGroup(QueryParams queryParams)
        //{
        //    queryParams.Cookies = Request.Cookies.ToList();
        //    return Ok(await queryService.GetProductGroup(queryParams));
        //}



        [HttpPut]
        public async Task<ActionResult> UpdateName(ItemViewModel product)
        {
            Product updatedProduct = await unitOfWork.Products.Get(product.Id);

            updatedProduct.Name = product.Name;
            updatedProduct.UrlName = Utility.GetUrlName(product.Name);

            // Update and save
            unitOfWork.Products.Update(updatedProduct);
            await unitOfWork.Save();

            return Ok();
        }





        [HttpPost]
        public async Task<ActionResult> AddProduct(ItemViewModel product)
        {
            Product newProduct = new Product
            {
                NicheId = product.Id,
                Name = product.Name,
                UrlId = Utility.GetUrlId(),
                UrlName = Utility.GetUrlName(product.Name)
            };

            unitOfWork.Products.Add(newProduct);
            await unitOfWork.Save();


            // Add keyword group to product
            KeywordGroup keywordGroup = new KeywordGroup
            {
                Name = product.Name,
                ForProduct = true
            };

            unitOfWork.KeywordGroups.Add(keywordGroup);
            await unitOfWork.Save();



            unitOfWork.KeywordGroups_Belonging_To_Product.Add(new KeywordGroup_Belonging_To_Product
            {
                ProductId = newProduct.Id,
                KeywordGroupId = keywordGroup.Id
            });


            int keywordId;
            Keyword keyword = await unitOfWork.Keywords.Get(x => x.Name == product.Name.ToLower());

            // If a keyword does NOT already contain a name that matches the name of this new product
            if (keyword == null)
            {
                // Then create a new keyword that has the same name as this new product
                Keyword newKeyword = new Keyword
                {
                    Name = product.Name.ToLower()
                };
                unitOfWork.Keywords.Add(newKeyword);
                await unitOfWork.Save();
                keywordId = newKeyword.Id;

                // If a keyword already exists that contains the same name as this new product
            }
            else
            {
                // Just use the id of that keyword
                keywordId = keyword.Id;
            }



            unitOfWork.Keywords_In_KeywordGroup.Add(new Keyword_In_KeywordGroup
            {
                KeywordGroupId = keywordGroup.Id,
                KeywordId = keywordId
            });


            unitOfWork.ProductKeywords.Add(new ProductKeyword
            {
                ProductId = newProduct.Id,
                KeywordId = keywordId
            });


            await unitOfWork.Save();

            return Ok(newProduct.Id);
        }



        [Route("Move")]
        [HttpPut]
        public async Task<ActionResult> MoveProduct(MoveItemViewModel moveItem)
        {
            Product productToBeMoved = await unitOfWork.Products.Get(moveItem.ItemToBeMovedId);

            productToBeMoved.NicheId = moveItem.DestinationItemId;

            // Update and save
            unitOfWork.Products.Update(productToBeMoved);
            await unitOfWork.Save();


            return Ok();
        }



        [HttpDelete]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            IEnumerable<int> KeywordGroupIds = await unitOfWork.KeywordGroups_Belonging_To_Product.GetCollection(x => x.ProductId == id && x.KeywordGroup.ForProduct, x => x.KeywordGroupId);

            IEnumerable<int> KeywordIds = await unitOfWork.Keywords_In_KeywordGroup.GetCollection(x => KeywordGroupIds.Contains(x.KeywordGroupId), x => x.KeywordId);

            unitOfWork.KeywordGroups.RemoveRange(await unitOfWork.KeywordGroups.GetCollection(x => KeywordGroupIds.Contains(x.Id)));

            unitOfWork.Keywords.RemoveRange(await unitOfWork.Keywords.GetCollection(x => KeywordIds.Contains(x.Id)));

            Product product = await unitOfWork.Products.Get(id);

            unitOfWork.Products.Remove(product);
            await unitOfWork.Save();

            return Ok();
        }



        [HttpDelete]
        [Route("Image")]
        public async Task<ActionResult> DeleteProductImage(int productId)
        {
            Product product = await unitOfWork.Products.Get(productId);

            product.ImageId = null;

            // Update and save
            unitOfWork.Products.Update(product);
            await unitOfWork.Save();

            return Ok();
        }



        [HttpPut]
        [Route("Image")]
        public async Task<ActionResult> UpdateProductImage(UpdatedProperty updatedProperty)
        {
            Product product = await unitOfWork.Products.Get(updatedProperty.ItemId);

            product.ImageId = updatedProperty.PropertyId;

            // Update and save
            unitOfWork.Products.Update(product);
            await unitOfWork.Save();

            return Ok();
        }






        [Route("Vendor")]
        [HttpPut]
        public async Task<ActionResult> UpdateProductVendor(UpdatedProperty updatedProperty)
        {
            Product product = await unitOfWork.Products.Get(updatedProperty.ItemId);

            product.VendorId = updatedProperty.PropertyId;

            // Update and save
            unitOfWork.Products.Update(product);
            await unitOfWork.Save();

            return Ok();
        }











        [Route("Filter")]
        [HttpPut]
        public async Task<ActionResult> UpdateProductFilter(UpdatedProductItem updatedProductFilter)
        {

            if (updatedProductFilter.Checked)
            {
                unitOfWork.ProductFilters.Add(new ProductFilter { ProductId = updatedProductFilter.ProductId, FilterOptionId = updatedProductFilter.Id });
            }
            else
            {
                ProductFilter productFilter = await unitOfWork.ProductFilters.Get(x => x.ProductId == updatedProductFilter.ProductId && x.FilterOptionId == updatedProductFilter.Id);
                unitOfWork.ProductFilters.Remove(productFilter);
            }

            await unitOfWork.Save();

            return Ok();
        }





        [HttpPut]
        [Route("Media")]
        public async Task<ActionResult> UpdateProductMedia(UpdatedProductMedia updatedProductMedia)
        {
            ProductMedia productMedia;

            if (updatedProductMedia.ProductMediaId != 0)
            {
                productMedia = await unitOfWork.ProductMedia.Get(updatedProductMedia.ProductMediaId);
                productMedia.MediaId = updatedProductMedia.mediaId;
                unitOfWork.ProductMedia.Update(productMedia);
            }
            else
            {
                productMedia = new ProductMedia
                {
                    ProductId = updatedProductMedia.ProductId,
                    MediaId = updatedProductMedia.mediaId,
                    Index = await unitOfWork.ProductMedia.GetCount(x => x.ProductId == updatedProductMedia.ProductId)
                };

                unitOfWork.ProductMedia.Add(productMedia);

                if (productMedia.Index == 0)
                {
                    Product product = await unitOfWork.Products.Get(updatedProductMedia.ProductId);

                    product.ImageId = productMedia.MediaId;
                    unitOfWork.Products.Update(product);
                }
            }

            await unitOfWork.Save();

            return Ok(productMedia.Id);
        }





        [HttpPut]
        [Route("Media/Indices")]
        public async Task<ActionResult> UpdateProductMediaIndices(UpdatedProductMediaIndices UpdatedProductMediaIndices)
        {
            IEnumerable<ProductMedia> productMedia = await unitOfWork.ProductMedia.GetCollection(x => x.ProductId == UpdatedProductMediaIndices.ProductId);

            foreach (ProductMedia media in productMedia)
            {
                media.Index = UpdatedProductMediaIndices.ProductMedia.Where(x => x.ProductMediaId == media.Id).Select(x => x.Index).Single();
                unitOfWork.ProductMedia.Update(media);

                if (media.Index == 0)
                {
                    Product product = await unitOfWork.Products.Get(UpdatedProductMediaIndices.ProductId);

                    product.ImageId = media.MediaId;
                    unitOfWork.Products.Update(product);
                }
            }

            await unitOfWork.Save();
            return Ok();
        }





        [HttpDelete]
        [Route("Media")]
        public async Task<ActionResult> DeleteProductMedia(int id)
        {
            ProductMedia productMedia = await unitOfWork.ProductMedia.Get(id);
            int productId = productMedia.ProductId;

            unitOfWork.ProductMedia.Remove(productMedia);

            await unitOfWork.Save();

            if (await unitOfWork.ProductMedia.GetCount(x => x.ProductId == productId) == 0)
            {
                Product product = await unitOfWork.Products.Get(productId);
                product.ImageId = null;
                unitOfWork.Products.Update(product);
                await unitOfWork.Save();
            }

            return Ok();
        }






        [Route("Description")]
        [HttpPut]
        public async Task<ActionResult> UpdateProductDescription(ProductDescription productDescription)
        {
            Product product = await unitOfWork.Products.Get(productDescription.ProductId);

            product.Description = productDescription.Description;

            // Update and save
            unitOfWork.Products.Update(product);
            await unitOfWork.Save();

            return Ok();
        }


        //[Route("Keyword")]
        //[HttpPost]
        //public async Task<ActionResult> AddProductKeyword(ProductItem keyword)
        //{
        //    ProductKeyword newKeyword = new ProductKeyword
        //    {
        //        ProductId = keyword.ProductId,
        //        KeywordId = keyword.ItemId
        //    };


        //    // Add and save
        //    unitOfWork.ProductKeywords.Add(newKeyword);
        //    await unitOfWork.Save();

        //    return Ok(newKeyword.Id);
        //}





        //[HttpDelete]
        //[Route("Keyword")]
        //public async Task<ActionResult> DeleteProductKeywords([FromQuery] int[] ids)
        //{
        //    foreach (int id in ids)
        //    {
        //        ProductKeyword keyword = await unitOfWork.ProductKeywords.Get(id);
        //        unitOfWork.ProductKeywords.Remove(keyword);
        //    }


        //    await unitOfWork.Save();

        //    return Ok();
        //}






        //[Route("Subgroup")]
        //[HttpPost]
        //public async Task<ActionResult> AddProductGroup(ProductItem subgroup)
        //{
        //    SubgroupProduct newSubgroup = new SubgroupProduct
        //    {
        //        ProductId = subgroup.ProductId,
        //        SubgroupId = subgroup.ItemId
        //    };


        //    // Add and save
        //    unitOfWork.SubgroupProducts.Add(newSubgroup);
        //    await unitOfWork.Save();

        //    return Ok(newSubgroup.Id);
        //}




        //[HttpDelete]
        //[Route("Subgroup")]
        //public async Task<ActionResult> DeleteProductGroups([FromQuery] int[] ids)
        //{
        //    foreach (int id in ids)
        //    {
        //        SubgroupProduct subgroup = await unitOfWork.SubgroupProducts.Get(id);
        //        unitOfWork.SubgroupProducts.Remove(subgroup);
        //    }


        //    await unitOfWork.Save();

        //    return Ok();
        //}




        //[Route("Subgroup")]
        //[HttpPut]
        //public async Task<ActionResult> UpdateProductGroup(UpdatedProductItem updatedProductGroup)
        //{

        //    if (updatedProductGroup.Checked)
        //    {
        //        unitOfWork.SubgroupProducts.Add(new SubgroupProduct { ProductId = updatedProductGroup.ProductId, SubgroupId = updatedProductGroup.Id });
        //    }
        //    else
        //    {
        //        SubgroupProduct subgroupProduct = await unitOfWork.SubgroupProducts.Get(x => x.ProductId == updatedProductGroup.ProductId && x.SubgroupId == updatedProductGroup.Id);
        //        unitOfWork.SubgroupProducts.Remove(subgroupProduct);
        //    }

        //    await unitOfWork.Save();

        //    return Ok();
        //}























        //[Route("Subproduct/Image")]
        //[HttpPut]
        //public async Task<ActionResult> UpdateSubproductImage(UpdatedProperty updatedProperty)
        //{
        //    Subproduct subproduct = await unitOfWork.Subproducts.Get(updatedProperty.ItemId);

        //    subproduct.ImageId = updatedProperty.PropertyId;

        //    // Update and save
        //    unitOfWork.Subproducts.Update(subproduct);
        //    await unitOfWork.Save();

        //    return Ok();
        //}





        //[HttpPut]
        //[Route("Subproduct/Name")]
        //public async Task<ActionResult> UpdateSubproductName(ItemViewModel subproduct)
        //{
        //    Subproduct updatedSubproduct = await unitOfWork.Subproducts.Get(subproduct.Id);

        //    updatedSubproduct.Name = subproduct.Name;

        //    // Update and save
        //    unitOfWork.Subproducts.Update(updatedSubproduct);
        //    await unitOfWork.Save();

        //    return Ok();
        //}






        //[HttpPut]
        //[Route("Subproduct/Value")]
        //public async Task<ActionResult> UpdateSubproductValue(SubproductValue subproductValue)
        //{
        //    Subproduct Subproduct = await unitOfWork.Subproducts.Get(subproductValue.SubproductId);

        //    Subproduct.Value = subproductValue.Value;

        //    // Update and save
        //    unitOfWork.Subproducts.Update(Subproduct);
        //    await unitOfWork.Save();

        //    return Ok();
        //}













        [HttpPost]
        [Route("Subproduct")]
        public async Task<ActionResult> AddSubproduct(NewSubproduct newSubproduct)
        {
            Subproduct subproduct = new Subproduct
            {
                ProductId = newSubproduct.ProductId,
                Value = 0,
                Type = newSubproduct.Type
            };

            // Add and save
            unitOfWork.Subproducts.Add(subproduct);
            await unitOfWork.Save();


            return Ok(subproduct.Id);
        }





        [HttpPut]
        [Route("Subproduct")]
        public async Task UpdateSubproduct(SubproductProperties subproductProperties)
        {
            Subproduct subproduct = await unitOfWork.Subproducts.Get(subproductProperties.Id);

            subproduct.ImageId = subproductProperties.ImageId;
            subproduct.Name = subproductProperties.Name;
            subproduct.Value = subproductProperties.Value;

            // Update and save
            unitOfWork.Subproducts.Update(subproduct);
            await unitOfWork.Save();
        }





        [Route("Subproduct/Description")]
        [HttpPut]
        public async Task<ActionResult> UpdateSubproductDescription(ProductDescription productDescription)
        {
            Subproduct subproduct = await unitOfWork.Subproducts.Get(productDescription.ProductId);

            subproduct.Description = productDescription.Description;

            // Update and save
            unitOfWork.Subproducts.Update(subproduct);
            await unitOfWork.Save();

            return Ok();
        }











        [HttpDelete]
        [Route("Subproduct/Image")]
        public async Task<ActionResult> DeleteSubproductImage(int subproductId)
        {
            Subproduct subproduct = await unitOfWork.Subproducts.Get(subproductId);

            subproduct.ImageId = null;

            // Update and save
            unitOfWork.Subproducts.Update(subproduct);
            await unitOfWork.Save();

            return Ok();
        }





        




        [HttpDelete]
        [Route("Subproduct")]
        public async Task<ActionResult> DeleteSubproduct(int id)
        {
            Subproduct subproduct = await unitOfWork.Subproducts.Get(id);

            // Remove and save
            unitOfWork.Subproducts.Remove(subproduct);
            await unitOfWork.Save();

            return Ok();
        }





















        //[HttpGet]
        //[Route("QueryBuilder/Search")]
        //public async Task<ActionResult> SearchQueryBuilderProducts(string searchWords)
        //{
        //    return Ok(await unitOfWork.Products.GetCollection<ItemViewModel<Product>>(searchWords));
        //}




        [HttpGet]
        [Route("Link")]
        public async Task<ActionResult> SearchProductsLink(string searchTerm)
        {
            return Ok(await unitOfWork.Products.GetCollection(searchTerm, x => new
            {
                Id = x.Id,
                Name = x.Name,
                Link = x.UrlName + "/" + x.UrlId
            }));
        }







        [HttpGet]
        [Route("Product")]
        public async Task<ActionResult> GetProduct(int productId)
        {
            return Ok(await unitOfWork.Products.GetProduct(productId));
        }


        [HttpGet]
        [Route("EmailIds")]
        public async Task<ActionResult> GetEmailIds(int productId)
        {
            return Ok(await unitOfWork.ProductEmails.GetCollection(x => x.ProductId == productId, x => x.Id));
        }


        [HttpGet]
        [Route("Email")]
        public async Task<ActionResult> GetEmails(int emailId)
        {
            return Ok(await unitOfWork.ProductEmails.Get(x => x.Id == emailId, x => x.Content));
        }





        [Route("Price")]
        [HttpPost]
        public async Task AddPrice(ProductPriceItem newProductPrice)
        {
            ProductPrice productPrice = new ProductPrice
            {
                ProductId = newProductPrice.ProductId,
                Price = newProductPrice.Price
            };


            // Update and save
            unitOfWork.ProductPrices.Add(productPrice);
            await unitOfWork.Save();
        }





        [Route("Price")]
        [HttpPut]
        public async Task UpdatePrice(ProductPriceItem updatedProductPrice)
        {
            ProductPrice productPrice = await unitOfWork.ProductPrices.Get(updatedProductPrice.ProductId);
            productPrice.Price = updatedProductPrice.Price;

            // Update and save
            unitOfWork.ProductPrices.Update(productPrice);
            await unitOfWork.Save();
        }






        [HttpDelete]
        [Route("Price")]
        public async Task DeletePrice(int productId)
        {
            ProductPrice productPrice = await unitOfWork.ProductPrices.Get(x => x.ProductId == productId);

            // Remove and save
            unitOfWork.ProductPrices.Remove(productPrice);


            await unitOfWork.Save();
        }




        [HttpPost]
        [Route("PricePoint")]
        public async Task<ActionResult> AddPricePoint(PricePointProperties pricePointProperties)
        {
            ProductPrice productPrice = new ProductPrice
            {
                ProductId = pricePointProperties.ProductId,
                Price = 0
            };

            unitOfWork.ProductPrices.Add(productPrice);


            await unitOfWork.Save();


            PricePoint pricePoint = new PricePoint
            {
                ProductPriceId = productPrice.Id
            };

            // Add and save
            unitOfWork.PricePoints.Add(pricePoint);
            await unitOfWork.Save();


            return Ok(pricePoint.Id);
        }







        [Route("PricePoint")]
        [HttpPut]
        public async Task<ActionResult> UpdatePricePoint(PricePointProperties pricePointProperties)
        {
            PricePoint pricePoint = await unitOfWork.PricePoints.Get(x => x.Id == pricePointProperties.Id);


            ProductPrice productPrice = await unitOfWork.ProductPrices.Get(pricePoint.ProductPriceId);

            productPrice.Price = pricePointProperties.Price;
            unitOfWork.ProductPrices.Update(productPrice);


            pricePoint.Header = pricePointProperties.Header;
            pricePoint.Quantity = pricePointProperties.Quantity;
            pricePoint.ImageId = pricePointProperties.ImageId;
            pricePoint.UnitPrice = pricePointProperties.UnitPrice;
            pricePoint.Unit = pricePointProperties.Unit;
            pricePoint.StrikethroughPrice = pricePointProperties.StrikethroughPrice;
            //pricePoint.ProductPrice.Price = pricePointProperties.Price;
            pricePoint.ShippingType = pricePointProperties.ShippingType;
            pricePoint.TrialPeriod = pricePointProperties.RecurringPayment.TrialPeriod;
            pricePoint.RecurringPrice = pricePointProperties.RecurringPayment.RecurringPrice;
            pricePoint.RebillFrequency = pricePointProperties.RecurringPayment.RebillFrequency;
            pricePoint.TimeFrameBetweenRebill = pricePointProperties.RecurringPayment.TimeFrameBetweenRebill;
            pricePoint.SubscriptionDuration = pricePointProperties.RecurringPayment.SubscriptionDuration;

            // Update and save
            unitOfWork.PricePoints.Update(pricePoint);
            await unitOfWork.Save();

            return Ok();
        }




        [HttpDelete]
        [Route("PricePoint")]
        public async Task DeletePricePoint(int pricePointId)
        {
            PricePoint pricePoint = await unitOfWork.PricePoints.Get(pricePointId);
            ProductPrice productPrice = await unitOfWork.ProductPrices.Get(pricePoint.ProductPriceId);

            // Remove and save
            unitOfWork.PricePoints.Remove(pricePoint);
            unitOfWork.ProductPrices.Remove(productPrice);


            await unitOfWork.Save();
        }













        [HttpPut]
        [Route("Shipping")]
        public async Task<ActionResult> UpdateProductShipping(ProductShipping productShipping)
        {
            Product updatedProduct = await unitOfWork.Products.Get(productShipping.Id);

            updatedProduct.ShippingType = productShipping.ShippingType;

            // Update and save
            unitOfWork.Products.Update(updatedProduct);
            await unitOfWork.Save();

            return Ok();
        }





        [HttpPut]
        [Route("RecurringPayment")]
        public async Task<ActionResult> UpdateProductRecurringPayment(ProductRecurringPayment productRecurringPayment)
        {
            Product updatedProduct = await unitOfWork.Products.Get(productRecurringPayment.Id);

            updatedProduct.TrialPeriod = productRecurringPayment.RecurringPayment.TrialPeriod;
            updatedProduct.RecurringPrice = productRecurringPayment.RecurringPayment.RecurringPrice;
            updatedProduct.RebillFrequency = productRecurringPayment.RecurringPayment.RebillFrequency;
            updatedProduct.TimeFrameBetweenRebill = productRecurringPayment.RecurringPayment.TimeFrameBetweenRebill;
            updatedProduct.SubscriptionDuration = productRecurringPayment.RecurringPayment.SubscriptionDuration;


            // Update and save
            unitOfWork.Products.Update(updatedProduct);
            await unitOfWork.Save();

            return Ok();
        }



        [HttpPut]
        [Route("Hoplink")]
        public async Task<ActionResult> UpdateProductHoplink(ProductHoplink productHoplink)
        {
            Product product = await unitOfWork.Products.Get(productHoplink.Id);

            product.Hoplink = productHoplink.Hoplink;

            // Update and save
            unitOfWork.Products.Update(product);
            await unitOfWork.Save();

            return Ok();
        }

    }
}
