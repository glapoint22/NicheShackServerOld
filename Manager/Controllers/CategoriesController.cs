using System.Threading.Tasks;
using Manager.Repositories;
using DataAccess.ViewModels;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Models;
using Manager.ViewModels;
using System.Linq;

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


        [HttpGet]
        [Route("Image")]
        public async Task<ActionResult> GetCategoryImage(int categoryId)
        {
            return Ok(await unitOfWork.Media.Get(x => x.Id == x.Categtories.Where(y => y.Id == categoryId).Select(y => y.ImageId).FirstOrDefault(), x => new ImageViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Url = x.Url
            }));
        }
    }
}