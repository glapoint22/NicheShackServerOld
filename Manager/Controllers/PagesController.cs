using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess.Models;
using DataAccess.ViewModels;
using Manager.Classes;
using Manager.Repositories;
using Manager.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services.Classes;
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





        [HttpGet]
        public async Task<ActionResult> GetPage(int id)
        {
            string pageContentString = await unitOfWork.Pages.Get(x => x.Id == id, x => x.Content);

            PageContent pageContent = await pageService.GePage(pageContentString, new QueryParams());



            PageData pageData = await unitOfWork.Pages.Get(x => x.Id == id, x => new PageData
            {
                Id = x.Id,
                Name = x.Name,
                PageType = x.PageType,
            });



            pageData.Content = pageContent;

            return Ok(pageData);
        }





        [HttpGet]
        [Route("PageReferenceItem")]
        public async Task<ActionResult> GetPageReferenceItems(int pageId)
        {
            List<int?> itemIds = (List<int?>)await unitOfWork.PageReferenceItems.GetCollection(x => x.PageId == pageId, x => x.NicheId);
            var pageReferenceItems = await unitOfWork.Niches.GetCollection(x => itemIds.Contains(x.Id), x => new Item
            {
                Id = x.Id,
                Name = x.Name
            });

            return Ok(pageReferenceItems);
        }




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
            await unitOfWork.Save();

            return Ok(page.Id);
        }






        [HttpPut]
        public async Task<ActionResult> UpdatePage(PageViewModel updatedPage)
        {
            Page page = await unitOfWork.Pages.Get(updatedPage.Id);

            page.Name = updatedPage.Name;
            page.Content = updatedPage.Content;
            page.PageType = updatedPage.PageType;


            unitOfWork.Pages.Update(page);
            await unitOfWork.Save();

            return Ok();
        }






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



        



        [HttpPost]
        [Route("PageReferenceItem")]
        public async Task<ActionResult> AddPageReferenceItem(PageReferenceItemViewModel newPageReferenceItem)
        {
            PageReferenceItem pageReferenceItem = new PageReferenceItem
            {
                PageId = newPageReferenceItem.PageId,
                NicheId = newPageReferenceItem.ItemId
            };


            // Add and save
            unitOfWork.PageReferenceItems.Add(pageReferenceItem);
            await unitOfWork.Save();

            return Ok(pageReferenceItem.Id);
        }





        [HttpDelete]
        [Route("PageReferenceItem")]
        public async Task<ActionResult> DeletePageReferenceItem(int nicheId, int pageId)
        {
            PageReferenceItem pageReferenceItem = await unitOfWork.PageReferenceItems.Get(x => x.NicheId == nicheId && x.PageId == pageId);
            unitOfWork.PageReferenceItems.Remove(pageReferenceItem);
            

            await unitOfWork.Save();
            return Ok();
        }
    }
}
