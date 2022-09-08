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
        public async Task<ActionResult> GetCategories()
        {
            return Ok(await unitOfWork.KeywordGroups.GetCollection<ItemViewModel<KeywordGroup>>(x => !x.ForProduct));
        }



        [HttpGet]
        [Route("Search")]
        public async Task<ActionResult> Search(string searchTerm)
        {
            return Ok(await unitOfWork.KeywordGroups.GetCollection<ItemViewModel<KeywordGroup>>(x => !x.ForProduct, searchTerm));
        }
    }
}
