using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Classes;
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
        public async Task<ActionResult> GetSearchPage(string searchTerm)
        {
            int keywordId = await unitOfWork.Keywords.Get(x => x.Name == searchTerm, x => x.Id);

            if(keywordId > 0)
            {
                int pageId = await unitOfWork.PageReferenceItems.Get(x => x.ItemId == keywordId, x => x.PageId);

                if(pageId > 0)
                {
                    return Ok(await unitOfWork.Pages.Get(x => x.Id == pageId, x => x.Content));
                }
            }



            return Ok(await unitOfWork.Pages.Get(x => x.DisplayType == (int)PageDisplayType.Grid, x => x.Content));
        }




        [Route("Browse")]
        [HttpGet]
        public async Task<ActionResult> GetBrowsePage(string urlId)
        {
            int nicheId = await unitOfWork.Niches.Get(x => x.UrlId == urlId, x => x.Id);

            if (nicheId == 0) return NotFound();

            int pageId = await unitOfWork.PageReferenceItems.Get(x => x.ItemId == nicheId, x => x.PageId);

            if (pageId > 0)
            {
                return Ok(await unitOfWork.Pages.Get(x => x.Id == pageId, x => x.Content));
            }

            return Ok(await unitOfWork.Pages.Get(x => x.DisplayType == (int)PageDisplayType.Grid, x => x.Content));
        }
    }
}
