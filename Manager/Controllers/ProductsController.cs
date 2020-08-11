using System.Threading.Tasks;
using Manager.Repositories;
using DataAccess.ViewModels;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Manager.Classes;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public ProductsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult> GetProducts(int nicheId)
        {
            return Ok(await unitOfWork.Products.GetCollection<ItemViewModel<Product>>(x => x.NicheId == nicheId));
        }




        [HttpPut]
        public async Task<ActionResult> UpdateProductName(ItemViewModel product)
        {
            Product updatedProduct = await unitOfWork.Products.Get(product.Id);

            updatedProduct.Name = product.Name;

            // Update and save
            unitOfWork.Products.Update(updatedProduct);
            await unitOfWork.Save();


            // TODO: Update url name

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

            productEmail.Subject = updatedPage.Name;
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








        [Route("PricePoint")]
        [HttpPut]
        public async Task<ActionResult> UpdatePricePoint(ProductPricePoint updatedProductPricePoint)
        {
            ProductPricePoint productPricePoint = await unitOfWork.ProductPricePoints.Get(updatedProductPricePoint.Id);

            productPricePoint.TextBefore = updatedProductPricePoint.TextBefore;
            productPricePoint.WholeNumber = updatedProductPricePoint.WholeNumber;
            productPricePoint.Decimal = updatedProductPricePoint.Decimal;
            productPricePoint.TextAfter = updatedProductPricePoint.TextAfter;

            await unitOfWork.Save();

            return Ok();
        }





        [Route("PriceIndices")]
        [HttpPut]
        public async Task<ActionResult> UpdatePriceIndices(UpdatedPriceIndices updatedPriceIndices)
        {
            IEnumerable<PriceIndex> priceIndices = await unitOfWork.PriceIndices.GetCollection(x => x.ProductContentId == updatedPriceIndices.ProductContentId);


            for (int i = 0; i < updatedPriceIndices.PriceIndices.Count(); i++)
            {
                PriceIndex priceIndex = priceIndices.SingleOrDefault(x => x.Index == i);

                if (priceIndex != null)
                {
                    if (updatedPriceIndices.PriceIndices[i] == false)
                    {
                        unitOfWork.PriceIndices.Remove(priceIndex);
                    }
                }
                else
                {
                    if (updatedPriceIndices.PriceIndices[i] == true)
                    {
                        priceIndex = new PriceIndex { ProductContentId = updatedPriceIndices.ProductContentId, Index = i };
                        unitOfWork.PriceIndices.Add(priceIndex);
                    }
                }
            }

            await unitOfWork.Save();

            return Ok();
        }








        [Route("ContentTitle")]
        [HttpPut]
        public async Task<ActionResult> UpdateContentTitle(ItemViewModel updatedProperty)
        {
            ProductContent productContent = await unitOfWork.ProductContent.Get(updatedProperty.Id);

            productContent.Name = updatedProperty.Name;

            // Update and save
            unitOfWork.ProductContent.Update(productContent);
            await unitOfWork.Save();

            return Ok();
        }







        [Route("ContentIcon")]
        [HttpPut]
        public async Task<ActionResult> UpdateContentIcon(UpdatedProperty updatedProperty)
        {
            ProductContent productContent = await unitOfWork.ProductContent.Get(updatedProperty.ItemId);

            productContent.IconId = updatedProperty.PropertyId;

            // Update and save
            unitOfWork.ProductContent.Update(productContent);
            await unitOfWork.Save();

            return Ok();
        }








        [Route("PricePointMove")]
        [HttpPut]
        public async Task<ActionResult> PricePointMove(UpdatedPricePoint updatedPricePoint)
        {
            IEnumerable<ProductPricePoint> productPricePoints = await unitOfWork.ProductPricePoints
                .GetCollection(x => x.ProductId == updatedPricePoint.ProductId &&
                    (x.Index == updatedPricePoint.FromIndex || x.Index == updatedPricePoint.ToIndex));


            foreach (ProductPricePoint productPricePoint in productPricePoints)
            {
                if (productPricePoint.Index == updatedPricePoint.FromIndex)
                {
                    productPricePoint.Index = updatedPricePoint.ToIndex;

                }
                else
                {
                    productPricePoint.Index = updatedPricePoint.FromIndex;
                }

                unitOfWork.ProductPricePoints.Update(productPricePoint);
            }


            var contentIds = await unitOfWork.ProductContent.GetCollection(x => x.ProductId == updatedPricePoint.ProductId, x => x.Id);



            IEnumerable<PriceIndex> priceIndices = await unitOfWork.PriceIndices.GetCollection(x => contentIds.Contains(x.ProductContentId) &&
            (x.Index == updatedPricePoint.FromIndex || x.Index == updatedPricePoint.ToIndex));




            foreach (PriceIndex priceIndex in priceIndices)
            {
                if (priceIndex.Index == updatedPricePoint.FromIndex)
                {
                    priceIndex.Index = updatedPricePoint.ToIndex;

                }
                else
                {
                    priceIndex.Index = updatedPricePoint.FromIndex;
                }

                unitOfWork.PriceIndices.Update(priceIndex);
            }



            await unitOfWork.Save();

            return Ok();
        }






        [HttpPut]
        [Route("Media")]
        public async Task<ActionResult> UpdateProductMedia(UpdatedProductMedia updatedProductMedia)
        {
            ProductMedia productMedia = await unitOfWork.ProductMedia.Get(x => x.ProductId == updatedProductMedia.ProductId && x.MediaId == updatedProductMedia.OldMediaId);

            productMedia.MediaId = updatedProductMedia.NewMediaId;

            // Update and save
            unitOfWork.ProductMedia.Update(productMedia);
            await unitOfWork.Save();

            return Ok();
        }





        [Route("Keyword")]
        [HttpPut]
        public async Task<ActionResult> UpdateKeyword(ItemViewModel updatedProperty)
        {
            ProductKeyword keyword = await unitOfWork.ProductKeywords.Get(updatedProperty.Id);

            keyword.Name = updatedProperty.Name;

            // Update and save
            unitOfWork.ProductKeywords.Update(keyword);
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






        [HttpGet]
        [Route("Search")]
        public async Task<ActionResult> Search(string searchWords)
        {
            return Ok(await unitOfWork.Products.GetCollection<ItemViewModel<Product>>(searchWords));
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
    }
}
