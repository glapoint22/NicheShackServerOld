using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Services.Classes;
using Services.Interfaces;
using Website.Repositories;

namespace Website.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IPageService pageService;

        public HomeController(IUnitOfWork unitOfWork, IPageService pageService)
        {
            this.unitOfWork = unitOfWork;
            this.pageService = pageService;
        }



        // ..................................................................................Get.....................................................................
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            string pageContent = await unitOfWork.Pages.Get(x => x.DisplayType == (int)PageDisplayType.Home, x => x.Content);
            QueryParams queryParams = new QueryParams();

            queryParams.Cookies = Request.Cookies.ToList();


            return Ok(await pageService.GePage(pageContent, queryParams));
        }
    }
}