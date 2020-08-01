using System.Threading.Tasks;
using Manager.Repositories;
using DataAccess.ViewModels;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Models;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase {
        private readonly IUnitOfWork unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult> GetCategories()
        {
            return Ok(await unitOfWork.Categories.GetCollection<ItemViewModel<Category>>());
        }
    }
}