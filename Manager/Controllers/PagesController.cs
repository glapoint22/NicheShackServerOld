using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataAccess.Models;
using DataAccess.ViewModels;
using Manager.Classes;
using Manager.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Manager.Controllers
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


        [HttpGet]
        public async Task<ActionResult> GetPages()
        {
            return Ok(await unitOfWork.Pages.GetCollection<ItemViewModel<Page>>());
        }



        [HttpGet]
        [Route("Page")]
        public async Task<ActionResult> GetPage(int id)
        {
            return Ok(await unitOfWork.Pages.Get(x => x.Id == id, x => x.Content));
        }


        [HttpPut]
        [Route("Page")]
        public async Task<ActionResult> UpdatePage(UpdatedPage updatedPage)
        {
            Page page = await unitOfWork.Pages.Get(updatedPage.PageId);

            page.Name = updatedPage.Name;
            page.UrlName = Utility.GetUrlName(updatedPage.Name);
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
            Page page = new Page
            {
                Name = pageName,
                UrlId = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper(),
                UrlName = Utility.GetUrlName(pageName),
                Content = ""
            };

            // Add and save
            unitOfWork.Pages.Add(page);
            await unitOfWork.Save();

            // Update the content with the new Id and update
            page.Content = "{\"id\":" + page.Id + ",\"name\":\"" + pageName + "\",\"background\":{\"color\":\"#ffffff\"}}";
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
            Page page = await unitOfWork.Pages.Get(pageId);
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
            Page page = await unitOfWork.Pages.Get(pageId);

            unitOfWork.Pages.Remove(page);
            await unitOfWork.Save();

            return Ok();
        }








        [HttpGet]
        [Route("Search")]
        public async Task<ActionResult> Search(string searchWords)
        {
            return Ok(await unitOfWork.Pages.GetCollection<ItemViewModel<Page>>(searchWords));
        }



        [HttpGet]
        [Route("Link")]
        public async Task<ActionResult> Link(string searchWords)
        {
            return Ok(await unitOfWork.Pages.GetCollection(searchWords, x => new {
                Name = x.Name,
                Link = "cp/" + x.UrlName + "/" + x.UrlId
            }));
        }
    }
}
