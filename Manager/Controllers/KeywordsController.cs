using DataAccess.Models;
using DataAccess.ViewModels;
using Manager.Classes;
using Manager.Repositories;
using Manager.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services.Classes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeywordsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public KeywordsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
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



        [HttpGet]
        [Route("Search")]
        public async Task<ActionResult> Search(string searchTerm)
        {
            return Ok(await unitOfWork.KeywordGroups.GetCollection<ItemViewModel<KeywordGroup>>(x => !x.ForProduct, searchTerm));
        }




        [HttpPut]
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
        [Route("Group")]
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

            return Ok(pageReferenceItem.Id);
        }




        [HttpGet]
        [Route("Group")]
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
        [Route("Group")]
        public async Task<ActionResult> DeletePageReferenceItem(int groupId, int pageId)
        {
            PageReferenceItem pageReferenceItem = await unitOfWork.PageReferenceItems.Get(x => x.KeywordGroupId == groupId && x.PageId == pageId);
            unitOfWork.PageReferenceItems.Remove(pageReferenceItem);

            var keywordInKeywordGroupIds = await unitOfWork.Keywords_In_KeywordGroup.GetCollection(x => x.KeywordGroupId == groupId, x => x.Id);
            var pageKeywords = await unitOfWork.PageKeywords.GetCollection(x => keywordInKeywordGroupIds.Contains(x.KeywordInKeywordGroupId) && x.PageId == pageId);

            unitOfWork.PageKeywords.RemoveRange(pageKeywords);

            await unitOfWork.Save();
            return Ok();
        }
    }
}
