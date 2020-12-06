using DataAccess.Models;
using DataAccess.ViewModels;
using Manager.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeywordsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public KeywordsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }



        [HttpGet]
        public async Task<ActionResult> GetKeywords()
        {
            return Ok(await unitOfWork.Keywords.GetCollection<ItemViewModel<Keyword>>());
        }




        [HttpGet]
        [Route("Search")]
        public async Task<ActionResult> SearchKeywords(string searchWords)
        {
            return Ok(await unitOfWork.Keywords.GetCollection<ItemViewModel<Keyword>>(searchWords));
        }






        [HttpPost]
        public async Task<ActionResult> AddKeyword(ItemViewModel keyword)
        {
            Keyword newKeyword = new Keyword
            {
                Name = keyword.Name
            };


            // Add and save
            unitOfWork.Keywords.Add(newKeyword);
            await unitOfWork.Save();

            return Ok(newKeyword.Id);
        }





        [HttpPut]
        public async Task<ActionResult> UpdateKeyword(ItemViewModel updatedProperty)
        {
            Keyword keyword = await unitOfWork.Keywords.Get(updatedProperty.Id);

            keyword.Name = updatedProperty.Name;

            // Update and save
            unitOfWork.Keywords.Update(keyword);
            await unitOfWork.Save();

            return Ok();
        }




        [HttpDelete]
        public async Task<ActionResult> DeleteKeywords([FromQuery] int[] ids)
        {
            foreach (int id in ids)
            {
                Keyword keyword = await unitOfWork.Keywords.Get(id);
                unitOfWork.Keywords.Remove(keyword);
            }

            await unitOfWork.Save();
            return Ok();
        }
    }
}
