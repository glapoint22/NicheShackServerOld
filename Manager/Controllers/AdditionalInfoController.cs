using DataAccess.Models;
using Manager.Classes;
using Manager.Repositories;
using Microsoft.AspNetCore.Mvc;
using Services.Classes;
using System.Threading.Tasks;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdditionalInfoController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public AdditionalInfoController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }




        [HttpPut]
        [Route("Product")]
        public async Task<ActionResult> UpdateProductAdditionalInfo(AdditionalInfoViewModel additionalInfoViewModel)
        {
            await unitOfWork.ProductAdditionalInfo.Update(additionalInfoViewModel);

            return Ok();
        }



        [HttpPut]
        [Route("ProductPrice")]
        public async Task<ActionResult> UpdateProductPriceAdditionalInfo(AdditionalInfoViewModel additionalInfoViewModel)
        {
            await unitOfWork.ProductPriceAdditionalInfo.Update(additionalInfoViewModel);

            return Ok();
        }



        [HttpPost]
        [Route("Product")]
        public async Task<ActionResult> PostProductAdditionalInfo(NewAdditionalInfo newAdditionalInfo)
        {
            ProductAdditionalInfo additionalInfo = new ProductAdditionalInfo
            {
                ProductId = newAdditionalInfo.ProductId
            };

            return Ok(await unitOfWork.ProductAdditionalInfo.Post(additionalInfo));
        }




        [HttpPost]
        [Route("ProductPrice")]
        public async Task<ActionResult> PostProductPriceAdditionalInfo(NewAdditionalInfo newAdditionalInfo)
        {
            ProductPriceAdditionalInfo additionalInfo = new ProductPriceAdditionalInfo
            {
                ProductId = newAdditionalInfo.ProductId,
                ProductPriceId = newAdditionalInfo.ProductPriceId
            };

            return Ok(await unitOfWork.ProductPriceAdditionalInfo.Post(additionalInfo));
        }









        [HttpDelete]
        [Route("Product")]
        public async Task<ActionResult> DeleteProductAdditionalInfo(int id)
        {
            await unitOfWork.ProductAdditionalInfo.Delete(id);
            return Ok();
        }



        [HttpDelete]
        [Route("ProductPrice")]
        public async Task<ActionResult> DeleteProductPriceAdditionalInfo(int id)
        {
            await unitOfWork.ProductPriceAdditionalInfo.Delete(id);
            return Ok();
        }
    }
}
