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


            IEnumerable<int> contentIds = await unitOfWork.ProductContent.GetCollection(x => x.ProductId == updatedPricePoint.ProductId, x => x.Id);



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






        [Route("Keyword")]
        [HttpPost]
        public async Task<ActionResult> AddKeyword(ItemViewModel keyword)
        {
            ProductKeyword newKeyword = new ProductKeyword
            {
                ProductId = keyword.Id,
                Name = keyword.Name
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
            foreach(int id in ids)
            {
                ProductKeyword keyword = await unitOfWork.ProductKeywords.Get(id);
                unitOfWork.ProductKeywords.Remove(keyword);
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





        [Route("Price")]
        [HttpPut]
        public async Task<ActionResult> UpdatePrice(UpdatedPrice updatedProduct)
        {
            Product product = await unitOfWork.Products.Get(updatedProduct.Id);

            product.MinPrice = updatedProduct.MinPrice;
            product.MaxPrice = updatedProduct.MaxPrice;

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




        [Route("Content")]
        [HttpPost]
        public async Task<ActionResult> AddContent(ItemViewModel content)
        {
            ProductContent newContent = new ProductContent
            {
                ProductId = content.Id,
                Name = ""
            };


            // Add and save
            unitOfWork.ProductContent.Add(newContent);
            await unitOfWork.Save();

            return Ok(newContent.Id);
        }



        [HttpDelete]
        [Route("Content")]
        public async Task<ActionResult> DeleteContent(int id)
        {
            ProductContent content = await unitOfWork.ProductContent.Get(id);

            unitOfWork.ProductContent.Remove(content);
            await unitOfWork.Save();

            return Ok();
        }




        [Route("PricePoint")]
        [HttpPost]
        public async Task<ActionResult> AddPricePoint(UpdatedProperty updatedProperty)
        {
            ProductPricePoint newPricePoint = new ProductPricePoint
            {
                ProductId = updatedProperty.ItemId,
                Index = await unitOfWork.ProductPricePoints.GetCount(x => x.ProductId == updatedProperty.ItemId)
            };

            unitOfWork.ProductPricePoints.Add(newPricePoint);
            await unitOfWork.Save();


            return Ok(newPricePoint.Id);
        }





        [HttpDelete]
        [Route("PricePoint")]
        public async Task<ActionResult> DeletePricePoints([FromQuery] int[] ids)
        {

            int productId = await unitOfWork.ProductPricePoints.Get(x => x.Id == ids[0], x => x.ProductId);


            IEnumerable<ProductPricePoint> productPricePoints = await unitOfWork.ProductPricePoints.GetCollection(x => x.ProductId == productId);


            IEnumerable<int> contentIds = await unitOfWork.ProductContent.GetCollection(x => x.ProductId == productId, x => x.Id);


            IEnumerable<PriceIndex> priceIndices = await unitOfWork.PriceIndices.GetCollection(x => contentIds.Contains(x.ProductContentId));


            ProductPricePoint[] pricePointsArray = productPricePoints.OrderBy(x => x.Index).ToArray();


            int index = 0;

            foreach(ProductPricePoint pricePoint in pricePointsArray)
            {
                if(ids.Contains(pricePoint.Id))
                {
                    unitOfWork.ProductPricePoints.Remove(pricePoint);

                    PriceIndex[] removedPriceIndices = priceIndices.Where(x => x.Index == pricePoint.Index).ToArray();

                    foreach(PriceIndex priceIndex in removedPriceIndices)
                    {
                        unitOfWork.PriceIndices.Remove(priceIndex);
                    }

                } else
                {
                    if(pricePoint.Index != index)
                    {
                        PriceIndex[] updatedPriceIndices = priceIndices.Where(x => x.Index == pricePoint.Index).ToArray();

                        foreach (PriceIndex priceIndex in updatedPriceIndices)
                        {
                            priceIndex.Index = index;
                            unitOfWork.PriceIndices.Update(priceIndex);
                        }

                        pricePoint.Index = index;
                        unitOfWork.ProductPricePoints.Update(pricePoint);
                    }

                    index++;

                }
            }



            await unitOfWork.Save();

            return Ok();
        }
    }
}
