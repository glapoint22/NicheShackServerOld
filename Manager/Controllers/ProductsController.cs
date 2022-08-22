using System.Threading.Tasks;
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
        public async Task<ActionResult> GetNicheAndSubNicheIds(int productId)
        {
            var subNicheId = await unitOfWork.Products.Get(x => x.Id == productId, x => x.NicheId);
            var nicheId = await unitOfWork.Niches.Get(x => x.Id == subNicheId, x => x.CategoryId);

            return Ok(new { NicheId = nicheId, SubNicheId = subNicheId });
        }



        [Route("SubNiches_Products")]
        [HttpGet]
        public async Task<ActionResult> GetNichesAndSubNiches(int nicheId, int subNicheId)
        {
            var subNiches = await unitOfWork.Niches.GetCollection<ItemViewModel<Niche>>(x => x.CategoryId == nicheId);
            var products = await unitOfWork.Products.GetCollection<ItemViewModel<Product>>(x => x.NicheId == subNicheId);


            return Ok(new { subNiches, products });
        }






        [HttpGet]
        [Route("QueryBuilder")]
        public async Task<ActionResult> GetQueryBuilderProducts()
        {
            return Ok(await unitOfWork.Products.GetCollection<ItemViewModel<Product>>());
        }




        [HttpPost]
        [Route("GridData")]
        public async Task<ActionResult> GetGridData(QueryParams queryParams)
        {
            queryParams.Cookies = Request.Cookies.ToList();
            return Ok(await queryService.GetGridData(queryParams));
        }






        [HttpPost]
        [Route("ProductGroup")]
        public async Task<ActionResult> GetProductGroup(QueryParams queryParams)
        {
            queryParams.Cookies = Request.Cookies.ToList();
            return Ok(await queryService.GetProductGroup(queryParams));
        }



        [HttpPut]
        public async Task<ActionResult> UpdateProductName(ItemViewModel product)
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





        //[Route("Email")]
        //[HttpPut]
        //public async Task<ActionResult> UpdateProductEmail(UpdatedPage updatedPage)
        //{
        //    ProductEmail productEmail = await unitOfWork.ProductEmails.Get(updatedPage.Id);

        //    productEmail.Name = updatedPage.Name;
        //    productEmail.Content = updatedPage.Content;

        //    // Update and save
        //    unitOfWork.ProductEmails.Update(productEmail);
        //    await unitOfWork.Save();

        //    return Ok();
        //}







        [Route("Vendor")]
        [HttpPut]
        public async Task<ActionResult> UpdateVendor(UpdatedProperty updatedProperty)
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
        public async Task<ActionResult> UpdateFilter(UpdatedProductItem updatedProductFilter)
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
            if (updatedProductMedia.OldMediaId != 0)
            {
                ProductMedia productMedia = await unitOfWork.ProductMedia.Get(x => x.MediaId == updatedProductMedia.OldMediaId);

                unitOfWork.ProductMedia.Remove(productMedia);
            }

            ProductMedia newProductMedia = new ProductMedia
            {
                ProductId = updatedProductMedia.ProductId,
                MediaId = updatedProductMedia.NewMediaId
            };

            unitOfWork.ProductMedia.Add(newProductMedia);

            await unitOfWork.Save();

            return Ok();
        }






        [HttpDelete]
        [Route("Media")]
        public async Task<ActionResult> DeleteProductMedia(int productId, int mediaId)
        {

            ProductMedia productMedia = await unitOfWork.ProductMedia.Get(x => x.ProductId == productId && x.MediaId == mediaId);

            unitOfWork.ProductMedia.Remove(productMedia);

            await unitOfWork.Save();

            return Ok();
        }




        [Route("Keyword")]
        [HttpPost]
        public async Task<ActionResult> AddKeyword(ProductItem keyword)
        {
            ProductKeyword newKeyword = new ProductKeyword
            {
                ProductId = keyword.ProductId,
                KeywordId = keyword.ItemId
            };


            // Add and save
            unitOfWork.ProductKeywords.Add(newKeyword);
            await unitOfWork.Save();

            return Ok(newKeyword.Id);
        }





        [HttpDelete]
        [Route("Keyword")]
        public async Task<ActionResult> DeleteKeywords([FromQuery] int[] ids)
        {
            foreach (int id in ids)
            {
                ProductKeyword keyword = await unitOfWork.ProductKeywords.Get(id);
                unitOfWork.ProductKeywords.Remove(keyword);
            }


            await unitOfWork.Save();

            return Ok();
        }






        [Route("Subgroup")]
        [HttpPost]
        public async Task<ActionResult> AddSubgroup(ProductItem subgroup)
        {
            SubgroupProduct newSubgroup = new SubgroupProduct
            {
                ProductId = subgroup.ProductId,
                SubgroupId = subgroup.ItemId
            };


            // Add and save
            unitOfWork.SubgroupProducts.Add(newSubgroup);
            await unitOfWork.Save();

            return Ok(newSubgroup.Id);
        }




        [HttpDelete]
        [Route("Subgroup")]
        public async Task<ActionResult> DeleteSubgroups([FromQuery] int[] ids)
        {
            foreach (int id in ids)
            {
                SubgroupProduct subgroup = await unitOfWork.SubgroupProducts.Get(id);
                unitOfWork.SubgroupProducts.Remove(subgroup);
            }


            await unitOfWork.Save();

            return Ok();
        }




        [Route("Subgroup")]
        [HttpPut]
        public async Task<ActionResult> UpdateSubgroup(UpdatedProductItem updatedProductGroup)
        {

            if (updatedProductGroup.Checked)
            {
                unitOfWork.SubgroupProducts.Add(new SubgroupProduct { ProductId = updatedProductGroup.ProductId, SubgroupId = updatedProductGroup.Id });
            }
            else
            {
                SubgroupProduct subgroupProduct = await unitOfWork.SubgroupProducts.Get(x => x.ProductId == updatedProductGroup.ProductId && x.SubgroupId == updatedProductGroup.Id);
                unitOfWork.SubgroupProducts.Remove(subgroupProduct);
            }

            await unitOfWork.Save();

            return Ok();
        }




        [Route("Description")]
        [HttpPut]
        public async Task<ActionResult> UpdateDescription(ProductDescription productDescription)
        {
            Product product = await unitOfWork.Products.Get(productDescription.ProductId);

            product.Description = productDescription.Description;

            // Update and save
            unitOfWork.Products.Update(product);
            await unitOfWork.Save();

            return Ok();
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





        [Route("Subproduct/Image")]
        [HttpPut]
        public async Task<ActionResult> UpdateSubproductImage(UpdatedProperty updatedProperty)
        {
            Subproduct subproduct = await unitOfWork.Subproducts.Get(updatedProperty.ItemId);

            subproduct.ImageId = updatedProperty.PropertyId;

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





        [HttpPut]
        [Route("Subproduct/Name")]
        public async Task<ActionResult> UpdateSubproductName(ItemViewModel subproduct)
        {
            Subproduct updatedSubproduct = await unitOfWork.Subproducts.Get(subproduct.Id);

            updatedSubproduct.Name = subproduct.Name;

            // Update and save
            unitOfWork.Subproducts.Update(updatedSubproduct);
            await unitOfWork.Save();

            return Ok();
        }






        [HttpPut]
        [Route("Subproduct/Value")]
        public async Task<ActionResult> UpdateSubproductValue(SubproductValue subproductValue)
        {
            Subproduct Subproduct = await unitOfWork.Subproducts.Get(subproductValue.SubproductId);

            Subproduct.Value = subproductValue.Value;

            // Update and save
            unitOfWork.Subproducts.Update(Subproduct);
            await unitOfWork.Save();

            return Ok();
        }



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




        [HttpGet]
        [Route("QueryBuilder/Search")]
        public async Task<ActionResult> SearchQueryBuilderProducts(string searchWords)
        {
            return Ok(await unitOfWork.Products.GetCollection<ItemViewModel<Product>>(searchWords));
        }




        [HttpGet]
        [Route("Link")]
        public async Task<ActionResult> Link(string searchTerm)
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







        [HttpPost]
        [Route("PricePoint")]
        public async Task<ActionResult> AddPricePoint(PricePointProperties pricePointProperties)
        {
            PricePoint pricePoint = new PricePoint
            {
                ProductId = pricePointProperties.ProductId
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

            pricePoint.Header = pricePointProperties.Header;
            pricePoint.Quantity = pricePointProperties.Quantity;
            pricePoint.ImageId = pricePointProperties.ImageId;
            pricePoint.UnitPrice = pricePointProperties.UnitPrice;
            pricePoint.Unit = pricePointProperties.Unit;
            pricePoint.StrikethroughPrice = pricePointProperties.StrikethroughPrice;
            pricePoint.Price = pricePointProperties.Price;
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
        public async Task<ActionResult> DeletePricePoint(int pricePointId)
        {
            PricePoint pricePoint = await unitOfWork.PricePoints.Get(x => x.Id == pricePointId);

            // Remove and save
            unitOfWork.PricePoints.Remove(pricePoint);
            await unitOfWork.Save();

            return Ok();
        }






        [Route("MinMaxPrice")]
        [HttpPut]
        public async Task<ActionResult> UpdateMinMaxPrice(UpdatedMinMaxPrice updatedMinMaxPrice)
        {
            Product product = await unitOfWork.Products.Get(updatedMinMaxPrice.ProductId);

            product.MinPrice = updatedMinMaxPrice.MinPrice;
            product.MaxPrice = updatedMinMaxPrice.MaxPrice;

            // Update and save
            unitOfWork.Products.Update(product);
            await unitOfWork.Save();

            return Ok();
        }







        [HttpPut]
        [Route("Shipping")]
        public async Task<ActionResult> UpdateShipping(ProductShipping productShipping)
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
        public async Task<ActionResult> UpdateRecurringPayment(ProductRecurringPayment productRecurringPayment)
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
        public async Task<ActionResult> UpdateHoplink(ProductHoplink productHoplink)
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
