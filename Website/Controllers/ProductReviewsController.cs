using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Website.Classes;
using DataAccess.Models;
using Website.Repositories;
using Website.ViewModels;
using Services;
using Services.Classes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using DataAccess.Classes;
using System.Linq;

namespace Website.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductReviewsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly EmailService emailService;
        private readonly IWebHostEnvironment env;

        public ProductReviewsController(IUnitOfWork unitOfWork, EmailService emailService, IWebHostEnvironment env)
        {
            this.unitOfWork = unitOfWork;
            this.emailService = emailService;
            this.env = env;
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
            ProductReviewViewModel productReviewDTO = new ProductReviewViewModel();

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
                image = new
                {
                    name = x.Media.Name,
                    url = x.Media.Url
                }
            });


            // If product is null, retun a 404 error
            if (product == null) return NotFound();

            // Return the product
            return Ok(product);
        }






        // .......................................................................................Post Review...................................................................
        [HttpPost]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> PostReview(ReviewData review)
        {
            string customerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;


            // Add the review to the database
            unitOfWork.ProductReviews.Add(new ProductReview
            {
                ProductId = review.ProductId,
                CustomerId = customerId,
                Title = review.Title,
                Rating = review.Rating,
                Date = DateTime.Now,
                Text = review.Text
            });


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



            // Setup the email
            emailService.SetupEmail(SetupEmail, new EmailSetupParams
            {
                CustomerId = customerId,
                ProductId = product.Id,
                Host = GetHost(),
                ProductRating = review.Rating,
                Title = review.Title,
                Text = review.Text
            });




            return Ok();
        }





        // .........................................................................Setup Email.....................................................................
        private async Task SetupEmail(NicheShackContext context, object state)
        {
            EmailSetupParams emailSetupParams = (EmailSetupParams)state;

            Recipient recipient = await context.Customers
                .AsNoTracking()
                .Where(x => x.Id == emailSetupParams.CustomerId && x.EmailPrefReview == true)
                .Select(x => new Recipient
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email
                })
                .SingleOrDefaultAsync();

            if (recipient == null) return;


            ProductData product = await context.Products
                .AsNoTracking()
                .Where(x => x.Id == emailSetupParams.ProductId)
                .Select(x => new ProductData
                {
                    Name = x.Name,
                    Image = x.Media.Url,
                    Url = emailSetupParams.Host + "/" + x.UrlName + "/" + x.UrlId
                }).SingleAsync();






            emailService.AddToQueue(EmailType.Review, "Thank you for reviewing " + product.Name + " on Niche Shack", recipient, new EmailProperties
            {
                Host = emailSetupParams.Host,
                Product = product,
                Stars = await GetStarsImage(emailSetupParams.ProductRating, context),
                Var1 = emailSetupParams.Title,
                Var2 = emailSetupParams.Text
            });

        }



        private async Task<string> GetStarsImage(int rating, NicheShackContext context)
        {
            string imageName = null;

            switch (rating)
            {
                case 1:
                    imageName = "One Star";
                    break;

                case 2:
                    imageName = "Two Stars";
                    break;

                case 3:
                    imageName = "Three Stars";
                    break;

                case 4:
                    imageName = "Four Stars";
                    break;

                case 5:
                    imageName = "Five Stars";
                    break;
            }


            return await context.Media
                .AsNoTracking()
                .Where(x => x.Name == imageName)
                .Select(x => x.Url)
                .SingleAsync();

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




        // ..................................................................................Get Host.....................................................................
        private string GetHost()
        {
            if (env.IsDevelopment())
            {
                return "http://localhost:4200";
            }

            return HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value;
        }
    }
}