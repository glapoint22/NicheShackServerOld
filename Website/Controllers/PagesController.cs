using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.Repositories;

namespace Website.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagesController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public PagesController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }



        public async Task<ActionResult> GetPage(string id)
        {
            if (!await unitOfWork.Pages.Any(x => x.UrlId == id)) return NotFound();

            return Ok(await unitOfWork.Pages.Get(x => x.UrlId == id, x => x.Content));
        }


        [Route("Search")]
        [HttpGet]
        public async Task<ActionResult> GetSearchPage()
        {
            return Ok(await unitOfWork.Pages.Get(x => x.Name == "Search", x => x.Content));
        }




        [Route("Browse")]
        [HttpGet]
        public async Task<ActionResult> GetBrowsePage()
        {
            return Ok(await unitOfWork.Pages.Get(x => x.Name == "Browse", x => x.Content));
        }
    }
}
