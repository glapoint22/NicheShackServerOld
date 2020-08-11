using System.Threading.Tasks;
using Manager.Repositories;
using Manager.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public MediaController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }


        [HttpGet]
        public async Task<ActionResult> Get(int type)
        {
            return Ok(await unitOfWork.Media.GetCollection<MediaViewModel>(x => x.Type == type));
        }



        [HttpGet]
        [Route("Search")]
        public async Task<ActionResult> Search(int type, string searchWords)
        {
            return Ok(await unitOfWork.Media.GetCollection<MediaViewModel>(x => x.Type == type, searchWords));
        }
    }
}