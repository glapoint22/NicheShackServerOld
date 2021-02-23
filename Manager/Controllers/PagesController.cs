using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataAccess.Models;
using DataAccess.ViewModels;
using Manager.Classes;
using Manager.Repositories;
using Manager.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services.Classes;
using Services.Interfaces;
using static Manager.Classes.Utility;

namespace Manager.Controllers
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


        [HttpGet]
        public async Task<ActionResult> GetPages()
        {
            return Ok(await unitOfWork.Pages.GetCollection<ItemViewModel<DataAccess.Models.Page>>());
        }



        [HttpGet]
        [Route("Page")]
        public async Task<ActionResult> GetPage(int id)
        {
            string pageContent = await unitOfWork.Pages.Get(x => x.Id == id, x => x.Content);

            QueryParams queryParams = new QueryParams();
            queryParams.Cookies = Request.Cookies.ToList();
            

            return Ok(await pageService.GePage(pageContent, queryParams));
        }


        [HttpPut]
        [Route("Page")]
        public async Task<ActionResult> UpdatePage(UpdatedPage updatedPage)
        {
            DataAccess.Models.Page page = await unitOfWork.Pages.Get(updatedPage.PageId);

            page.Name = updatedPage.Name;
            page.UrlName = GetUrlName(updatedPage.Name);
            page.DisplayType = (int)updatedPage.DisplayType;
            page.Content = updatedPage.Content;

            // Update and save
            unitOfWork.Pages.Update(page);
            await unitOfWork.Save();


            return Ok();
        }







        [Route("Create")]
        [HttpGet]
        public async Task<ActionResult> CreatePage()
        {
            string pageName = "New Page";


            // Create the new page
            DataAccess.Models.Page page = new DataAccess.Models.Page
            {
                Name = pageName,
                UrlId = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper(),
                UrlName = GetUrlName(pageName),
                Content = ""
            };

            // Add and save
            unitOfWork.Pages.Add(page);
            await unitOfWork.Save();

            // Update the content with the new Id and update
            page.Content = "{\"id\":" + page.Id + ",\"name\":\"" + pageName + "\",\"background\":{\"color\":\"#00000000\"}}";
            unitOfWork.Pages.Update(page);


            await unitOfWork.Save();


            // Return the new page content
            return Ok(page.Content);
        }








        [Route("Duplicate")]
        [HttpGet]
        public async Task<ActionResult> DuplicatePage(int pageId)
        {
            // Get the page
            DataAccess.Models.Page page = await unitOfWork.Pages.Get(pageId);
            page.Id = 0;

            // Add the duplicated page and save
            unitOfWork.Pages.Add(page);
            await unitOfWork.Save();


            // Update the content with the new id and save
            page.Content = Regex.Replace(page.Content, "^{\"id\":" + pageId, "{\"id\":" + page.Id);
            unitOfWork.Pages.Update(page);
            await unitOfWork.Save();


            // Return the page content
            return Ok(page.Content);
        }





        [HttpDelete]
        public async Task<ActionResult> DeletePage(int pageId)
        {
            DataAccess.Models.Page page = await unitOfWork.Pages.Get(pageId);

            unitOfWork.Pages.Remove(page);
            await unitOfWork.Save();

            return Ok();
        }








        [HttpGet]
        [Route("Search")]
        public async Task<ActionResult> Search(string searchWords)
        {
            return Ok(await unitOfWork.Pages.GetCollection<ItemViewModel<DataAccess.Models.Page>>(searchWords));
        }



        [HttpGet]
        [Route("Link")]
        public async Task<ActionResult> Link(string searchWords)
        {
            var pages = await unitOfWork.Pages.GetCollection(searchWords, x => new
            {
                x.Id,
                x.Name,
                x.DisplayType,
                x.UrlName,
                x.UrlId
            });



            return Ok(pages.Select(x => new
            {
                x.Id,
                x.Name,
                Link = GetPageDisplay((Services.Classes.PageDisplayType)x.DisplayType) + x.UrlName + "/" + x.UrlId
            }).ToList());
        }


        private string GetPageDisplay(Services.Classes.PageDisplayType pageDisplayType)
        {
            string value = "";

            switch (pageDisplayType)
            {
                case Services.Classes.PageDisplayType.Custom:
                    value = "cp/";
                    break;
                case Services.Classes.PageDisplayType.Browse:
                    value = "browse/";
                    break;
            }

            return value;
        }




        [HttpPost]
        [Route("PageReferenceItem")]
        public async Task<ActionResult> AddPageReferenceItem(PageReferenceItemViewModel newPageReferenceItem)
        {
            PageReferenceItem pageReferenceItem = new PageReferenceItem
            {
                PageId = newPageReferenceItem.PageId,
                ItemId = newPageReferenceItem.DisplayId
            };


            // Add and save
            unitOfWork.PageReferenceItems.Add(pageReferenceItem);
            await unitOfWork.Save();

            return Ok(pageReferenceItem.Id);
        }





        [HttpDelete]
        [Route("PageReferenceItem")]
        public async Task<ActionResult> DeletePageReferenceItem([FromQuery] int[] ids)
        {
            foreach (int id in ids)
            {
                PageReferenceItem pageReferenceItem = await unitOfWork.PageReferenceItems.Get(id);
                unitOfWork.PageReferenceItems.Remove(pageReferenceItem);
            }

            await unitOfWork.Save();
            return Ok();
        }
    }
}
