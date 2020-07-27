using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Website.Classes;
using DataAccess.Models;
using Website.Repositories;

namespace Website.Controllers
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


        // ..................................................................................Get Reviews.....................................................................
        [HttpGet]
        public async Task<ActionResult> GetReviews(string productId, int page, string sortBy = "")
        {
            return Ok(await unitOfWork.ProductReviews.GetReviews(productId, sortBy, page));
        }





        // ................................................................................Get Review Options...................................................................
        [Route("ReviewOptions")]
        [HttpGet]
        public ActionResult GetReviewOptions()
        {
            ProductReviewDTO productReviewDTO = new ProductReviewDTO();

            return Ok(new
            {
                sortOptions = productReviewDTO.GetSortOptions(),
                reviewsPerPage = productReviewDTO.GetReviewsPerPage()
            });
        }







        // .........................................................................Get Positive Negative Reviews...............................................................
        [Route("PositiveNegativeReviews")]
        [HttpGet]
        public async Task<ActionResult> GetPositiveNegativeReviews(string productId)
        {
            return Ok(new
            {
                positiveReview = await unitOfWork.ProductReviews.GetPositiveReview(productId),
                negativeReview = await unitOfWork.ProductReviews.GetNegativeReview(productId),
            });
        }





        // ..................................................................................Write Review.....................................................................
        [Route("WriteReview")]
        [HttpGet]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> WriteReview(string productId)
        {
            // Get the product based on the product id
            var product = await unitOfWork.Products.Get(x => x.UrlId == productId, x => new
            {
                id = x.Id,
                title = x.Name,
                //image = x.Image
            });


            // If product is null, retun a 404 error
            if (product == null) return NoContent();

            // Return the product
            return Ok(product);
        }






        // .......................................................................................Post Review...................................................................
        [HttpPost]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> PostReview(ProductReview review)
        {
            //if (review.ProductId == null)
            //{
            //    return BadRequest(ModelState);
            //}

            // Get the customer Id from the access token
            string customerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Assign the customer to the review
            review.CustomerId = customerId;

            // Add the new review
            unitOfWork.ProductReviews.Add(review);

            // Get the product that is associated with this review
            Product product = await unitOfWork.Products.Get(review.ProductId);

            // Increment the star based on the rating. So if the rating is 3, the threeStars property will be incremented
            switch (review.Rating)
            {
                case 1:
                    product.OneStar++;
                    break;

                case 2:
                    product.TwoStars++;
                    break;

                case 3:
                    product.ThreeStars++;
                    break;

                case 4:
                    product.FourStars++;
                    break;

                case 5:
                    product.FiveStars++;
                    break;
            }

            // Increment total reviews
            product.TotalReviews++;

            // Calculate the product's rating
            double sum = (5 * product.FiveStars) +
                         (4 * product.FourStars) +
                         (3 * product.ThreeStars) +
                         (2 * product.TwoStars) +
                         (1 * product.OneStar);

            product.Rating = Math.Round(sum / product.TotalReviews, 1);

            // Update the product and save the changes to the database
            unitOfWork.Products.Update(product);
            await unitOfWork.Save();

            return Ok();
        }








        // .......................................................................................Rate Review...................................................................
        [HttpPut]
        public async Task<ActionResult> RateReview(ReviewRating reviewRating)
        {
            // Get the whole review from the database based on the id from the reviewRating parameter
            ProductReview review = await unitOfWork.ProductReviews.Get(reviewRating.ReviewId);

            if (review != null)
            {
                // Increment the likes or dislikes based on the reviewRating parameter
                review.Likes += reviewRating.Likes;
                review.Dislikes += reviewRating.Dislikes;

                // Update and save
                unitOfWork.ProductReviews.Update(review);
                await unitOfWork.Save();

                return Ok();
            }

            return BadRequest();

        }



        // ..................................................................................Update Review Name.....................................................................
        [HttpPut]
        [Route("UpdateReviewName")]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> UpdateReviewName(UpdatedReviewName updatedReviewName)
        {
            // Get the customer from the database based on the customer id from the claims via the access token
            Customer customer = await unitOfWork.Customers.Get(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // If the customer is found, update the review name (user name for reviews)
            if (customer != null)
            {
                customer.ReviewName = updatedReviewName.ReviewName;

                // Update the name in the database
                unitOfWork.Customers.Update(customer);
                await unitOfWork.Save();

                //Return with the new updated user name
                return Ok(new
                {
                    userName = customer.ReviewName

                });
            }

            return BadRequest();
        }
    }
}