using System.Threading.Tasks;
using Manager.Repositories;
using DataAccess.ViewModels;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Models;
using Manager.ViewModels;
using System.Linq;
using Manager.Classes;

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



        [HttpPut]
        public async Task<ActionResult> UpdateCategoryName(ItemViewModel category)
        {
            Category updatedCategory = await unitOfWork.Categories.Get(category.Id);

            updatedCategory.Name = category.Name;

            // Update and save
            unitOfWork.Categories.Update(updatedCategory);
            await unitOfWork.Save();

            return Ok();
        }






        [HttpPut]
        [Route("Image")]
        public async Task<ActionResult> UpdateCategoryImage(UpdatedProperty updatedProperty)
        {
            Category category = await unitOfWork.Categories.Get(updatedProperty.ItemId);

            category.ImageId = updatedProperty.PropertyId;

            // Update and save
            unitOfWork.Categories.Update(category);
            await unitOfWork.Save();

            return Ok();
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



        [HttpGet]
        [Route("Search")]
        public async Task<ActionResult> Search(string searchWords)
        {
            return Ok(await unitOfWork.Categories.GetCollection<ItemViewModel<Category>>(searchWords));
        }
    }
}