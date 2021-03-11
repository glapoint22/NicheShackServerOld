using DataAccess.Models;
using DataAccess.ViewModels;
using Manager.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
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
            IEnumerable<ItemViewModel<Keyword>> keywords = await unitOfWork.Keywords.GetCollection<ItemViewModel<Keyword>>();
            return Ok(keywords.OrderBy(x => x.Name));
        }




        [HttpGet]
        [Route("Search")]
        public async Task<ActionResult> SearchKeywords(string searchWords)
        {
            IEnumerable<ItemViewModel<Keyword>> keywords = await unitOfWork.Keywords.GetCollection<ItemViewModel<Keyword>>(searchWords);
            return Ok(keywords.OrderBy(x => x.Name));
        }






        [HttpPost]
        public async Task<ActionResult> AddKeyword(ItemViewModel keyword)
        {
            string keywordName = keyword.Name.Trim().ToLower();

            if (await unitOfWork.Keywords.Any(x => x.Name == keywordName)) return Ok();

            Keyword newKeyword = new Keyword
            {
                Name = keywordName
            };


            // Add and save
            unitOfWork.Keywords.Add(newKeyword);
            await unitOfWork.Save();

            return Ok(newKeyword.Id);
        }





        [HttpPut]
        public async Task<ActionResult> UpdateKeyword(ItemViewModel updatedProperty)
        {
            string keywordName = updatedProperty.Name.Trim().ToLower();

            if (await unitOfWork.Keywords.Any(x => x.Name == keywordName)) return Ok();


            Keyword keyword = await unitOfWork.Keywords.Get(updatedProperty.Id);

            keyword.Name = keywordName;

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
