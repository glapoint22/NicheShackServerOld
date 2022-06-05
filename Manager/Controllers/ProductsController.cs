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
                UrlId = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper(),
                UrlName = Utility.GetUrlName(product.Name)
            };

            unitOfWork.Products.Add(newProduct);
            await unitOfWork.Save();

            // Add a new row to the Product Prices table
            await AddPrice(newProduct.Id);





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
            }else
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





        [Route("Hoplink")]
        [HttpPut]
        public async Task<ActionResult> UpdateHoplink(ItemViewModel updatedProperty)
        {
            Product product = await unitOfWork.Products.Get(updatedProperty.Id);

            product.Hoplink = updatedProperty.Name;

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
        public async Task<ActionResult> UpdateProductMedia(UpdatedProperty updatedProductMedia)
        {
            ProductMedia productMedia = await unitOfWork.ProductMedia.Get(updatedProductMedia.ItemId);

            productMedia.MediaId = updatedProductMedia.PropertyId;

            // Update and save
            unitOfWork.ProductMedia.Update(productMedia);
            await unitOfWork.Save();

            return Ok();
        }




        [HttpPost]
        [Route("Media")]
        public async Task<ActionResult> AddProductMedia(UpdatedProperty productMedia)
        {

            ProductMedia newProductMedia = new ProductMedia
            {
                ProductId = productMedia.ItemId,
                MediaId = productMedia.PropertyId
            };

            unitOfWork.ProductMedia.Add(newProductMedia);

            await unitOfWork.Save();

            return Ok(newProductMedia.Id);
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




        [Route("Description")]
        [HttpPut]
        public async Task<ActionResult> UpdateDescription(ItemViewModel updatedProperty)
        {
            Product product = await unitOfWork.Products.Get(updatedProperty.Id);

            product.Description = updatedProperty.Name;

            // Update and save
            unitOfWork.Products.Update(product);
            await unitOfWork.Save();

            return Ok();
        }




        //public async Task<ActionResult> SearchProducts(string searchWords)
        //{
        //    return Ok(await unitOfWork.Products.GetCollection<ItemViewModel<Product>>(searchWords));
        //}



        //[HttpGet]
        //[Route("Search")]
        //public async Task<ActionResult> Search(string searchWords)
        //{
        //    return await SearchProducts(searchWords);
        //}


        [HttpGet]
        [Route("QueryBuilder/Search")]
        public async Task<ActionResult> SearchQueryBuilderProducts(string searchWords)
        {
            return Ok(await unitOfWork.Products.GetCollection<ItemViewModel<Product>>(searchWords));
        }




        [HttpGet]
        [Route("Link")]
        public async Task<ActionResult> Link(string searchWords)
        {
            return Ok(await unitOfWork.Products.GetCollection(searchWords, x => new
            {
                Id = x.Id,
                Name = x.Name,
                Link = x.UrlName + "/" + x.UrlId
            }));
        }





        [HttpGet]
        [Route("Filters")]
        public async Task<ActionResult> GetFilters(int productId, int filterId)
        {
            return Ok(await unitOfWork.Products.GetProductFilters(productId, filterId));
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











































        private async Task<int> AddPrice(int Id)
        {
            ProductPrice productPrice = new ProductPrice
            {
                ProductId = Id
            };

            // Add and save
            unitOfWork.ProductPrices.Add(productPrice);
            await unitOfWork.Save();

            return productPrice.Id;
        }







        [HttpPost]
        [Route("Price")]
        public async Task<ActionResult> AddProductPrice(ProductPriceProperties productPriceProperties)
        {
            var priceId = await AddPrice(productPriceProperties.ProductId);
            return Ok(priceId);
        }






        [Route("IsMultiPrice")]
        [HttpPut]
        public async Task<ActionResult> UpdateIsMultiPrice(ProductItem productItem)
        {
            Product product = await unitOfWork.Products.Get(x => x.Id == productItem.ProductId);

            //product.IsMultiPrice = productItem.IsMultiPrice;

            // Update and save
            unitOfWork.Products.Update(product);
            await unitOfWork.Save();

            return Ok();
        }




        [Route("Price")]
        [HttpPut]
        public async Task<ActionResult> UpdateProductPrice(ProductPriceProperties productPriceProperties)
        {
            ProductPrice productPrice = await unitOfWork.ProductPrices.Get(x => x.ProductId == productPriceProperties.ProductId && x.Id == productPriceProperties.Id);

            productPrice.Header = productPriceProperties.Header;
            productPrice.Quantity = productPriceProperties.Quantity;
            productPrice.ImageId = productPriceProperties.ImageId;
            productPrice.UnitPrice = productPriceProperties.UnitPrice;
            productPrice.Unit = productPriceProperties.Unit;
            productPrice.StrikethroughPrice = productPriceProperties.StrikethroughPrice;
            productPrice.Price = productPriceProperties.Price;
            //productPrice.Shipping = productPriceProperties.Shipping;
            //productPrice.ShippingPrice = productPriceProperties.ShippingPrice;



            // Update and save
            unitOfWork.ProductPrices.Update(productPrice);
            await unitOfWork.Save();

            return Ok();
        }




        [HttpDelete]
        [Route("Price")]
        public async Task<ActionResult> DeleteProductPrice(int productId, int priceId)
        {
            ProductPrice productPrice = await unitOfWork.ProductPrices.Get(x => x.ProductId == productId && x.Id == priceId);

            // Remove and save
            unitOfWork.ProductPrices.Remove(productPrice);
            await unitOfWork.Save();

            return Ok();
        }




        [HttpDelete]
        [Route("Prices")]
        public async Task<ActionResult> DeleteMultipleProductPrices(int productId)
        {
            // Get all the price points of the product
            IEnumerable<ProductPrice> productPrices = await unitOfWork.ProductPrices.GetCollection(x => x.ProductId == productId);

            // Remove all the price points of that product and save
            unitOfWork.ProductPrices.RemoveRange(productPrices);
            await unitOfWork.Save();

            // Add a new price
            var priceId = await AddPrice(productId);
            return Ok(priceId);
        }





    }
}
