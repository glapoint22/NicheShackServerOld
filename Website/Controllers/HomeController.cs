using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Website.Repositories;

namespace Website.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }



        // ..................................................................................Get.....................................................................
        public async Task<ActionResult> Get()
        {
            return Ok(await unitOfWork.Pages.Get(x => x.Name == "Home", x => x.Content));
        }
    }
}