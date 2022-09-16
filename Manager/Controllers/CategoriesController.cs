using System.Threading.Tasks;
using Manager.Repositories;
using DataAccess.ViewModels;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Models;
using Manager.ViewModels;
using System.Linq;
using Manager.Classes;
using System;

namespace Manager.Controllers
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



        [HttpGet]
        public async Task<ActionResult> GetCategories()
        {
            return Ok(await unitOfWork.Categories.GetCollection<ItemViewModel<Category>>());
        }




        [HttpGet]
        [Route("Detail")]
        public async Task<ActionResult> GetCategoriesDetails()
        {
            return Ok(await unitOfWork.Categories.GetCollection(x => new
            {
                id = x.Id,
                name = x.Name,
                urlName = x.UrlName,
                //icon = new
                //{
                //    name = x.Media.Name,
                //    url = x.Media.ImageAnySize
                //}
            }));
        }


        [HttpGet]
        [Route("Detail/Search")]
        public async Task<ActionResult> DetailSearch(string searchWords)
        {
            return Ok(await unitOfWork.Categories.GetCollection(searchWords, x => new
            {
                id = x.Id,
                name = x.Name,
                urlName = x.UrlName,
                //icon = new
                //{
                //    name = x.Media.Name,
                //    url = x.Media.ImageAnySize
                //}
            }));
        }



        [HttpPut]
        public async Task<ActionResult> UpdateCategoryName(ItemViewModel category)
        {
            Category updatedCategory = await unitOfWork.Categories.Get(category.Id);

            updatedCategory.Name = category.Name;
            updatedCategory.UrlName = Utility.GetUrlName(category.Name);

            // Update and save
            unitOfWork.Categories.Update(updatedCategory);
            await unitOfWork.Save();

            return Ok();
        }





        [HttpPost]
        public async Task<ActionResult> AddCategory(ItemViewModel category)
        {
            Category newCategory = new Category
            {
                Name = category.Name,
                UrlId = Utility.GetUrlId(),
                UrlName = Utility.GetUrlName(category.Name)
            };

            unitOfWork.Categories.Add(newCategory);
            await unitOfWork.Save();

            return Ok(newCategory.Id);
        }





        [HttpDelete]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            Category category = await unitOfWork.Categories.Get(id);

            unitOfWork.Categories.Remove(category);
            await unitOfWork.Save();

            return Ok();
        }





        //[HttpPut]
        //[Route("Image")]
        //public async Task<ActionResult> UpdateCategoryImage(UpdatedProperty updatedProperty)
        //{
        //    Category category = await unitOfWork.Categories.Get(updatedProperty.ItemId);

        //    category.ImageId = updatedProperty.PropertyId;

        //    // Update and save
        //    unitOfWork.Categories.Update(category);
        //    await unitOfWork.Save();

        //    return Ok();
        //}




        //[HttpGet]
        //[Route("Image")]
        //public async Task<ActionResult> GetCategoryImage(int categoryId)
        //{
        //    return Ok(await unitOfWork.Media.Get(x => x.Id == x.Categtories.Where(y => y.Id == categoryId).Select(y => y.ImageId).FirstOrDefault(), x => new ImageViewModel
        //    {
        //        Id = x.Id,
        //        Name = x.Name,
        //        Src = x.ImageAnySize
        //    }));
        //}



        [HttpGet]
        [Route("Search")]
        public async Task<ActionResult> Search(string searchWords)
        {
            var niches = await unitOfWork.Categories.GetCollection(searchWords, x => new SearchItem { Id = x.Id, Name = x.Name, Type = "Niche" });
            var subNiches = await unitOfWork.Niches.GetCollection(searchWords, x => new SearchItem { Id = x.Id, Name = x.Name, Type = "Sub Niche" });
            var products = await unitOfWork.Products.GetCollection(searchWords, x => new SearchItem { Id = x.Id, Name = x.Name, Type = "Product" });
            var searchResults = niches.Concat(subNiches).Concat(products).OrderBy(x => x.Name).ToList();
            return Ok(searchResults);
        }



        [Route("Children")]
        [HttpGet]
        public async Task<ActionResult> GetChildren(int parentId)
        {
            var subNiches = await unitOfWork.Niches.GetCollection(x => x.CategoryId == parentId, x => new ItemViewModel { Id = x.Id, Name = x.Name });
            var products = await unitOfWork.Products.GetCollection(x => subNiches.Select(y => y.Id).Contains(x.NicheId), x => new ItemViewModel { Id = x.Id, Name = x.Name });
            var children = subNiches.Concat(products);
            return Ok(children);
        }
    }
}