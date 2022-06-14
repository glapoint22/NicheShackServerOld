using System.Collections.Generic;
using System.Linq;
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



        // ************************************************************************************* PAGE ***********************************************************************

        [HttpGet]
        public async Task<ActionResult> GetPage(int id)
        {
            PageContent pageContent = null;

            // Get the page content
            string pageContentString = await unitOfWork.Pages.Get(x => x.Id == id, x => x.Content);

            if (pageContentString != null)
                pageContent = await pageService.GePage(pageContentString, new QueryParams());


            // Get the page properties
            PageData pageData = await unitOfWork.Pages.Get(x => x.Id == id, x => new PageData
            {
                Id = x.Id,
                Name = x.Name,
                PageType = x.PageType,
                Content = pageContent
            });

            return Ok(pageData);
        }










        [HttpPost]
        public async Task<ActionResult> NewPage(PageViewModel newPage)
        {
            Page page = new Page
            {
                Name = newPage.Name,
                Content = newPage.Content,
                PageType = newPage.PageType,
                UrlId = Utility.GetUrlId(),
                UrlName = Utility.GetUrlName(newPage.Name)
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

            if (page.PageType == (int)PageType.Custom)
            {
                page.UrlId = page.UrlId == null ? Utility.GetUrlId() : page.UrlId;
                page.UrlName = Utility.GetUrlName(page.Name);
            }
            else
            {
                page.UrlId = null;
                page.UrlName = null;
            }


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





        [HttpPost]
        [Route("Duplicate")]
        public async Task<ActionResult> Duplicate(Item page)
        {
            // Copy the page properties
            var currentPage = await unitOfWork.Pages.Get(page.Id);
            var duplicatePage = new Page
            {
                Name = currentPage.Name + " Copy",
                Content = currentPage.Content,
                UrlId = currentPage.UrlId,
                UrlName = currentPage.UrlName,
                PageType = currentPage.PageType
            };

            unitOfWork.Pages.Add(duplicatePage);
            await unitOfWork.Save();

            // If page type is browse or search
            if (duplicatePage.PageType == (int)PageType.Browse || duplicatePage.PageType == (int)PageType.Search)
            {
                // Get the page reference items
                var pageItems = await unitOfWork.PageReferenceItems.GetCollection(x => x.PageId == page.Id);

                if (pageItems.Count() > 0)
                {
                    pageItems.ToList().ForEach(x =>
                    {
                        x.Id = 0;
                        x.PageId = duplicatePage.Id;
                    });

                    // Duplicate the page reference items
                    unitOfWork.PageReferenceItems.AddRange(pageItems);

                    // If page type is search
                    if (duplicatePage.PageType == (int)PageType.Search)
                    {
                        // Get th page keywords
                        var pageKeywords = await unitOfWork.PageKeywords.GetCollection(x => x.PageId == page.Id);

                        if (pageKeywords.Count() > 0)
                        {
                            pageKeywords.ToList().ForEach(x =>
                            {
                                x.Id = 0;
                                x.PageId = duplicatePage.Id;
                            });

                            // Duplicate the page keywords
                            unitOfWork.PageKeywords.AddRange(pageKeywords);
                        }
                    }

                    await unitOfWork.Save();
                }

            }


            return Ok(duplicatePage.Id);
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





        // ************************************************************************** PAGE NICHES ***********************************************************************

        [HttpGet]
        [Route("Niche")]
        public async Task<ActionResult> GetNiches(int pageId)
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
        [Route("Niche")]
        public async Task<ActionResult> AddNiche(PageReferenceItemViewModel newPageReferenceItem)
        {
            PageReferenceItem pageReferenceItem = new PageReferenceItem
            {
                PageId = newPageReferenceItem.PageId,
                NicheId = newPageReferenceItem.ItemId
            };


            // Add and save
            unitOfWork.PageReferenceItems.Add(pageReferenceItem);
            await unitOfWork.Save();

            return Ok();
        }





        [HttpDelete]
        [Route("Niche")]
        public async Task<ActionResult> DeleteNiche(int id, int pageId)
        {
            PageReferenceItem pageReferenceItem = await unitOfWork.PageReferenceItems.Get(x => x.NicheId == id && x.PageId == pageId);
            unitOfWork.PageReferenceItems.Remove(pageReferenceItem);


            await unitOfWork.Save();
            return Ok();
        }






        // ************************************************************************** PAGE KEYWORDS ***********************************************************************

        [HttpGet]
        [Route("Keywords")]
        public async Task<ActionResult> GetKeywords(int groupId, int pageId)
        {
            var keywords = await unitOfWork.Keywords_In_KeywordGroup.GetCollection(x => x.KeywordGroupId == groupId, x => new
            {
                Id = x.Keyword.Id,
                Name = x.Keyword.Name,
                Checked = x.PageKeywords.Any(z => z.PageId == pageId && z.KeywordInKeywordGroupId == x.Id)
            });

            return Ok(keywords);
        }



        [HttpPut]
        [Route("Keywords")]
        public async Task<ActionResult> UpdatePageKeyword(PageKeywordChecked pageKeywordChecked)
        {
            var keywordInKeywordGroupId = await unitOfWork.Keywords_In_KeywordGroup.Get(x => x.KeywordGroupId == pageKeywordChecked.GroupId && x.KeywordId == pageKeywordChecked.KeywordId, x => x.Id);

            if (pageKeywordChecked.Checked)
            {
                unitOfWork.PageKeywords.Add(new PageKeyword { PageId = pageKeywordChecked.PageId, KeywordInKeywordGroupId = keywordInKeywordGroupId });
            }
            else
            {
                var pageKeyword = await unitOfWork.PageKeywords.Get(x => x.PageId == pageKeywordChecked.PageId && x.KeywordInKeywordGroupId == keywordInKeywordGroupId);
                unitOfWork.PageKeywords.Remove(pageKeyword);
            }

            await unitOfWork.Save();

            return Ok();
        }




        [HttpPost]
        [Route("KeywordGroup")]
        public async Task<ActionResult> AddPageReferenceItem(PageReferenceItemViewModel referenceItem)
        {
            PageReferenceItem pageReferenceItem = new PageReferenceItem
            {
                PageId = referenceItem.PageId,
                KeywordGroupId = referenceItem.ItemId
            };


            var pageKeywords = await unitOfWork.Keywords_In_KeywordGroup.GetCollection(x => x.KeywordGroupId == referenceItem.ItemId, x => new PageKeyword
            {
                PageId = referenceItem.PageId,
                KeywordInKeywordGroupId = x.Id
            });


            unitOfWork.PageKeywords.AddRange(pageKeywords);


            // Add and save
            unitOfWork.PageReferenceItems.Add(pageReferenceItem);
            await unitOfWork.Save();

            return Ok();
        }




        [HttpGet]
        [Route("KeywordGroup")]
        public async Task<ActionResult> GetPageReferenceItems(int pageId)
        {
            List<int?> itemIds = (List<int?>)await unitOfWork.PageReferenceItems.GetCollection(x => x.PageId == pageId, x => x.KeywordGroupId);
            var pageReferenceItems = await unitOfWork.KeywordGroups.GetCollection(x => itemIds.Contains(x.Id), x => new Item
            {
                Id = x.Id,
                Name = x.Name
            });

            return Ok(pageReferenceItems);
        }




        [HttpDelete]
        [Route("KeywordGroup")]
        public async Task<ActionResult> DeletePageReferenceItem(int id, int pageId)
        {
            PageReferenceItem pageReferenceItem = await unitOfWork.PageReferenceItems.Get(x => x.KeywordGroupId == id && x.PageId == pageId);
            unitOfWork.PageReferenceItems.Remove(pageReferenceItem);

            var keywordInKeywordGroupIds = await unitOfWork.Keywords_In_KeywordGroup.GetCollection(x => x.KeywordGroupId == id, x => x.Id);
            var pageKeywords = await unitOfWork.PageKeywords.GetCollection(x => keywordInKeywordGroupIds.Contains(x.KeywordInKeywordGroupId) && x.PageId == pageId);

            unitOfWork.PageKeywords.RemoveRange(pageKeywords);

            await unitOfWork.Save();
            return Ok();
        }
    }
}
