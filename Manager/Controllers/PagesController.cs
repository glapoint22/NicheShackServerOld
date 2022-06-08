using System.Threading.Tasks;
using DataAccess.Models;
using DataAccess.ViewModels;
using Manager.Classes;
using Manager.Repositories;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

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


        //[HttpGet]
        //public async Task<ActionResult> GetPages()
        //{
        //    return Ok(await unitOfWork.Pages.GetCollection<ItemViewModel<DataAccess.Models.Page>>());
        //}



        //[HttpGet]
        //[Route("Page")]
        //public async Task<ActionResult> GetPage(int id)
        //{
        //    string pageContent = await unitOfWork.Pages.Get(x => x.Id == id, x => x.Content);

        //    QueryParams queryParams = new QueryParams();
        //    queryParams.Cookies = Request.Cookies.ToList();


        //    return Ok(await pageService.GePage(pageContent, queryParams));
        //}


        //[HttpPut]
        //[Route("Page")]
        //public async Task<ActionResult> UpdatePage(UpdatedPage updatedPage)
        //{
        //    DataAccess.Models.Page page = await unitOfWork.Pages.Get(updatedPage.PageId);

        //    page.Name = updatedPage.Name;
        //    page.UrlName = GetUrlName(updatedPage.Name);
        //    page.DisplayType = (int)updatedPage.DisplayType;
        //    page.Content = updatedPage.Content;

        //    // Update and save
        //    unitOfWork.Pages.Update(page);
        //    await unitOfWork.Save();


        //    return Ok();
        //}







        [HttpPost]
        public async Task<ActionResult> NewPage(PageViewModel newPage)
        {
            Page page = new Page
            {
                Name = newPage.Name,
                Content = newPage.Content,
                PageType = newPage.PageType
            };

            unitOfWork.Pages.Add(page);
            //await unitOfWork.Save();

            return Ok(page.Id);
        }






        [HttpPut]
        public async Task<ActionResult> UpdatePage(PageViewModel updatedPage)
        {
            //Page page = await unitOfWork.Pages.Get(updatedPage.Id);

            //page.Name = updatedPage.Name;
            //page.Content = updatedPage.Content;
            //page.PageType = updatedPage.PageType;


            //unitOfWork.Pages.Update(page);
            //await unitOfWork.Save();

            return Ok();
        }







        //[Route("Duplicate")]
        //[HttpGet]
        //public async Task<ActionResult> DuplicatePage(int pageId)
        //{
        //    // Get the page
        //    DataAccess.Models.Page page = await unitOfWork.Pages.Get(pageId);
        //    page.Id = 0;
        //    page.Name = "New Page";

        //    var pageContent = await pageService.GePage(page.Content, new QueryParams());

        //    // Add the duplicated page and save
        //    unitOfWork.Pages.Add(page);
        //    await unitOfWork.Save();


        //    // Update the content
        //    page.Content = Regex.Replace(page.Content, "^{\"id\":" + pageId, "{\"id\":" + page.Id);
        //    page.Content = Regex.Replace(page.Content, "\"referenceItems\":\\[[\\w\\d\\s\\W\\D\\S]*\\](?=,\"background\")", "\"referenceItems\": null");
        //    page.Content = Regex.Replace(page.Content, "\"name\":\"" + pageContent.Name + "\"", "\"name\":\"" + page.Name + "\"");
        //    pageContent.Name = page.Name;
        //    pageContent.ReferenceItems = null;
        //    pageContent.Id = page.Id;

        //    unitOfWork.Pages.Update(page);
        //    await unitOfWork.Save();


        //    // Return the page content
        //    return Ok(pageContent);
        //}





        [HttpDelete]
        public async Task<ActionResult> DeletePage(int pageId)
        {
            Page page = await unitOfWork.Pages.Get(pageId);

            unitOfWork.Pages.Remove(page);
            await unitOfWork.Save();

            return Ok();
        }








        [HttpGet]
        [Route("Search")]
        public async Task<ActionResult> Search(string searchTerm)
        {
            return Ok(await unitOfWork.Pages.GetCollection<ItemViewModel<Page>>(searchTerm));
        }



        [HttpGet]
        [Route("Link")]
        public async Task<ActionResult> Link(string searchTerm)
        {
            return Ok(await unitOfWork.Pages.GetCollection(searchTerm, x => new
            {
                Id = x.Id,
                Name = x.Name,
                Link = x.UrlName + "/" + x.UrlId
            }));
        }



        //[HttpGet]
        //[Route("Link")]
        //public async Task<ActionResult> Link(string searchWords)
        //{
        //    var pages = await unitOfWork.Pages.GetCollection(searchWords, x => new
        //    {
        //        x.Id,
        //        x.Name,
        //        x.DisplayType,
        //        x.UrlName,
        //        x.UrlId
        //    });



        //    return Ok(pages.Select(x => new
        //    {
        //        x.Id,
        //        x.Name,
        //        Link = GetPageDisplay((Services.Classes.PageDisplayType)x.DisplayType) + x.UrlName + "/" + x.UrlId
        //    }).ToList());
        //}


        //private string GetPageDisplay(Services.Classes.PageDisplayType pageDisplayType)
        //{
        //    string value = "";

        //    switch (pageDisplayType)
        //    {
        //        case Services.Classes.PageDisplayType.Custom:
        //            value = "cp/";
        //            break;
        //        case Services.Classes.PageDisplayType.Browse:
        //            value = "browse/";
        //            break;
        //    }

        //    return value;
        //}




        //[HttpPost]
        //[Route("PageReferenceItem")]
        //public async Task<ActionResult> AddPageReferenceItem(PageReferenceItemViewModel newPageReferenceItem)
        //{
        //    PageReferenceItem pageReferenceItem = new PageReferenceItem
        //    {
        //        PageId = newPageReferenceItem.PageId,
        //        ItemId = newPageReferenceItem.DisplayId
        //    };


        //    // Add and save
        //    unitOfWork.PageReferenceItems.Add(pageReferenceItem);
        //    await unitOfWork.Save();

        //    return Ok(pageReferenceItem.Id);
        //}





        //[HttpDelete]
        //[Route("PageReferenceItem")]
        //public async Task<ActionResult> DeletePageReferenceItem([FromQuery] int[] ids)
        //{
        //    foreach (int id in ids)
        //    {
        //        PageReferenceItem pageReferenceItem = await unitOfWork.PageReferenceItems.Get(id);
        //        unitOfWork.PageReferenceItems.Remove(pageReferenceItem);
        //    }

        //    await unitOfWork.Save();
        //    return Ok();
        //}
    }
}
