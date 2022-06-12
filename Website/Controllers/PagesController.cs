using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Classes;
using Services.Interfaces;
using System;
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
        private readonly IPageService pageService;
        private readonly NicheShackContext context;

        public PagesController(IUnitOfWork unitOfWork, IPageService pageService, NicheShackContext context)
        {
            this.unitOfWork = unitOfWork;
            this.pageService = pageService;
            this.context = context;
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



            if (keywordId > 0)
            {
                unitOfWork.KeywordSearchVolumes.Add(new KeywordSearchVolume
                {
                    KeywordId = keywordId,
                    Date = DateTime.Now
                });


                await unitOfWork.Save();

                //********************************FIX*********************************
                //int pageId = await unitOfWork.PageReferenceItems.Get(x => x.ItemId == keywordId, x => x.PageId);

                //if (pageId > 0)
                //{
                //    pageContent = await unitOfWork.Pages.Get(x => x.Id == pageId && x.PageType == (int)PageType.Search, x => x.Content);
                //}
            }

            if (pageContent == null)
            {
                pageContent = await unitOfWork.Pages.Get(x => x.PageType == (int)PageType.Grid, x => x.Content);
            }



            return Ok(new
            {
                pageContent = await pageService.GePage(pageContent, queryParams)
            });
        }









        [Route("Browse")]
        [HttpPost]
        public async Task<ActionResult> GetBrowsePage(QueryParams queryParams)
        {
            string pageContent = null;
            int id;

            if (queryParams.CategoryId != null)
            {
                id = await unitOfWork.Categories.Get(x => x.UrlId == queryParams.CategoryId, x => x.Id);
            }
            else
            {
                id = await unitOfWork.Niches.Get(x => x.UrlId == queryParams.NicheId, x => x.Id);
            }

            if (id == 0) return Ok();


            int pageId = await unitOfWork.PageReferenceItems.Get(x => x.NicheId == id, x => x.PageId);

            if (pageId > 0)
            {
                pageContent = await unitOfWork.Pages.Get(x => x.Id == pageId && x.PageType == (int)PageType.Browse, x => x.Content);

            }

            if (pageContent == null)
            {
                pageContent = await unitOfWork.Pages.Get(x => x.PageType == (int)PageType.Grid, x => x.Content);
            }

            return Ok(new
            {
                pageContent = await pageService.GePage(pageContent, queryParams)
            });
        }



        [Route("GridData")]
        [HttpPost]
        public async Task<ActionResult> GridData(QueryParams queryParams)
        {
            GridWidget gridWidget = new GridWidget();
            await gridWidget.SetData(context, queryParams);

            return Ok(new
            {
                GridData = gridWidget.GridData
            });
        }
    }
}
