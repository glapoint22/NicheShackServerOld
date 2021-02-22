using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Classes;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.Classes;
using Website.Repositories;

namespace Website.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagesController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IPageService pageService;

        public PagesController(IUnitOfWork unitOfWork, IPageService pageService)
        {
            this.unitOfWork = unitOfWork;
            this.pageService = pageService;
        }


        [HttpPost]
        public async Task<ActionResult> GetPage(QueryParams queryParams)
        {
            if (!await unitOfWork.Pages.Any(x => x.UrlId == queryParams.Id)) return NotFound();

            queryParams.Cookies = Request.Cookies.ToList();

            string pageContent = await unitOfWork.Pages.Get(x => x.UrlId == queryParams.Id, x => x.Content);

            return Ok(await pageService.GePage(pageContent, queryParams));
        }


        [Route("Search")]
        [HttpPost]
        public async Task<ActionResult> GetSearchPage(QueryParams queryParams)
        {
            string pageContent = null;
            int keywordId = await unitOfWork.Keywords.Get(x => x.Name == queryParams.Search, x => x.Id);

            queryParams.Cookies = Request.Cookies.ToList();

            if (keywordId > 0)
            {
                unitOfWork.KeywordSearchVolumes.Add(new KeywordSearchVolume
                {
                    KeywordId = keywordId,
                    Date = DateTime.Now
                });


                await unitOfWork.Save();

                int pageId = await unitOfWork.PageReferenceItems.Get(x => x.ItemId == keywordId, x => x.PageId);

                if (pageId > 0)
                {
                    pageContent = await unitOfWork.Pages.Get(x => x.Id == pageId && x.DisplayType == (int)PageDisplayType.Search, x => x.Content);

                    if (pageContent != null)
                    {
                        return Ok(await pageService.GePage(pageContent, queryParams));
                    }
                }
            }


            pageContent = await unitOfWork.Pages.Get(x => x.DisplayType == (int)PageDisplayType.Grid, x => x.Content);

            return Ok(await pageService.GePage(pageContent, queryParams));
        }




        [Route("Browse")]
        [HttpPost]
        public async Task<ActionResult> GetBrowsePage(QueryParams queryParams)
        {
            string pageContent = null;
            int nicheId = await unitOfWork.Niches.Get(x => x.UrlId == queryParams.Id, x => x.Id);

            queryParams.Cookies = Request.Cookies.ToList();

            if (nicheId == 0) return NotFound();

            int pageId = await unitOfWork.PageReferenceItems.Get(x => x.ItemId == nicheId, x => x.PageId);

            if (pageId > 0)
            {
                pageContent = await unitOfWork.Pages.Get(x => x.Id == pageId && x.DisplayType == (int)PageDisplayType.Browse, x => x.Content);

                if (pageContent != null)
                {

                    return Ok(await pageService.GePage(pageContent, queryParams));
                }
            }

            pageContent = await unitOfWork.Pages.Get(x => x.DisplayType == (int)PageDisplayType.Grid, x => x.Content);

            List<Query> queries = new List<Query>();
            queries.Add(new Query
            {
                StringValue = queryParams.Id,
                LogicalOperator = LogicalOperatorType.And,
                QueryType = QueryType.Niche,
            });


            queryParams.Queries = queries;
            

            return Ok(await pageService.GePage(pageContent, queryParams));
        }
    }
}
