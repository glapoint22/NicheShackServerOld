using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Website.Repositories;

namespace Website.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }


        // ..................................................................................Get Categories.....................................................................
        [HttpGet]
        public async Task<ActionResult> GetCategories()
        {
            // Get all categories
            var categories = await unitOfWork.Categories.GetCollection(x => new
            {
                id = x.Id,
                name = x.Name,
                urlId = x.UrlId,
                urlName = x.UrlName
            });


            return Ok(categories);
        }
    }
}