using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Website.Repositories;
using Website.ViewModels;

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
            // Get all categories and their niches
            return Ok(await unitOfWork.Categories.GetCollection<UrlItemViewModel<Category>>());
        }
    }
}