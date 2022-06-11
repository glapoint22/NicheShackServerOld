﻿using DataAccess.Models;
using DataAccess.ViewModels;
using Manager.Repositories;
using Manager.ViewModels;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult> GetKeywords()
        {
            return Ok(await unitOfWork.Keywords.GetCollection<ItemViewModel<Keyword>>());
        }



        [HttpGet]
        [Route("Search")]
        public async Task<ActionResult> Search(string searchTerm)
        {
            return Ok(await unitOfWork.KeywordGroups.GetCollection<ItemViewModel<KeywordGroup>>(x => !x.ForProduct, searchTerm));
        }







        [HttpPost]
        [Route("PageReferenceItem")]
        public async Task<ActionResult> AddPageReferenceItem(PageReferenceItemViewModel referenceItem)
        {
            PageReferenceItem pageReferenceItem = new PageReferenceItem
            {
                PageId = referenceItem.PageId,
                ItemId = referenceItem.ItemId
            };


            var pageKeywords = await unitOfWork.Keywords_In_KeywordGroup.GetCollection(x => x.KeywordGroupId == referenceItem.ItemId, x => new PageKeyword
            {
                PageId = referenceItem.PageId,
                KeywordInKeywordGroupId = x.Id
            });


            unitOfWork.PageKeywords.AddRange(pageKeywords);


            // Add and save
            unitOfWork.PageReferenceItems.Add(pageReferenceItem);
            //await unitOfWork.Save();

            return Ok(pageReferenceItem.Id);
        }





        [HttpDelete]
        [Route("PageReferenceItem")]
        public async Task<ActionResult> DeletePageReferenceItem(int id)
        {
            PageReferenceItem pageReferenceItem = await unitOfWork.PageReferenceItems.Get(id);
            unitOfWork.PageReferenceItems.Remove(pageReferenceItem);



            //await unitOfWork.Save();
            return Ok();
        }
    }
}
