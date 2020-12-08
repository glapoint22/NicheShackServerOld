using DataAccess.Models;
using DataAccess.ViewModels;
using Manager.Classes;
using Manager.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductReviewsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public ProductReviewsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }


        
        [HttpPut]
        public async Task<ActionResult> DeleteProductReview(ItemViewModel review)
        {


            //var alita = review.Id;

            ProductReview productReview = await unitOfWork.ProductReviews.Get(review.Id);

            productReview.Deleted = true;

            //product.MinPrice = updatedProduct.MinPrice;
            //product.MaxPrice = updatedProduct.MaxPrice;

            // Update and save
            unitOfWork.ProductReviews.Update(productReview);
            await unitOfWork.Save();

            return Ok();
        }
    }
}
