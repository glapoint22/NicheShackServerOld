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
        public async Task<ActionResult> GetProducts(int nicheId)
        {
            if (nicheId > 0)
            {
                return Ok(await unitOfWork.Products.GetCollection<ItemViewModel<Product>>(x => x.NicheId == nicheId));
            }
            else
            {
                return Ok(await unitOfWork.Products.GetCollection<ItemViewModel<Product>>());
            }

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

            return Ok(newProduct.Id);
        }





        [HttpDelete]
        public async Task<ActionResult> DeleteProduct(int id)
        {
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





        [Route("Email")]
        [HttpPut]
        public async Task<ActionResult> UpdateProductEmail(UpdatedPage updatedPage)
        {
            ProductEmail productEmail = await unitOfWork.ProductEmails.Get(updatedPage.PageId);

            productEmail.Name = updatedPage.Name;
            productEmail.Content = updatedPage.Content;

            // Update and save
            unitOfWork.ProductEmails.Update(productEmail);
            await unitOfWork.Save();

            return Ok();
        }







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
        public async Task<ActionResult> UpdateFilter(UpdatedProductFilter updatedProductFilter)
        {

            if (updatedProductFilter.Checked)
            {
                unitOfWork.ProductFilters.Add(new ProductFilter { ProductId = updatedProductFilter.ProductId, FilterOptionId = updatedProductFilter.FilterOptionId });
            }
            else
            {
                ProductFilter productFilter = await unitOfWork.ProductFilters.Get(x => x.ProductId == updatedProductFilter.ProductId && x.FilterOptionId == updatedProductFilter.FilterOptionId);
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




        public async Task<ActionResult> SearchProducts(string searchWords)
        {
            return Ok(await unitOfWork.Products.GetCollection<ItemViewModel<Product>>(searchWords));
        }



        [HttpGet]
        [Route("Search")]
        public async Task<ActionResult> Search(string searchWords)
        {
            return await SearchProducts(searchWords);
        }


        [HttpGet]
        [Route("QueryBuilder/Search")]
        public async Task<ActionResult> SearchQueryBuilderProducts(string searchWords)
        {
            return await SearchProducts(searchWords);
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



















































        [HttpPost]
        [Route("Price")]
        public async Task<ActionResult> AddProductPrice(ProductPriceProperties productPriceProperties)
        {
            ProductPrice productPrice = new ProductPrice
            {
                ProductId = productPriceProperties.ProductId
            };

            

            // Update and save
            unitOfWork.ProductPrices.Add(productPrice);
            await unitOfWork.Save();

            return Ok(productPrice.Id);
        }






        [Route("IsMultiPrice")]
        [HttpPut]
        public async Task<ActionResult> UpdateIsMultiPrice(ProductItem productItem)
        {
            Product product = await unitOfWork.Products.Get(x => x.Id == productItem.ProductId);

            product.IsMultiPrice = productItem.IsMultiPrice;

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
            productPrice.Shipping = productPriceProperties.Shipping;
            productPrice.ShippingPrice = productPriceProperties.ShippingPrice;



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

            unitOfWork.ProductPrices.Remove(productPrice);

            await unitOfWork.Save();

            return Ok();
        }








    }
}
