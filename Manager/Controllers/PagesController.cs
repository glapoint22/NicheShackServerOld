using System;
using System.Collections.Generic;
using System.Linq;
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
            page.Content = updatedPage.Content;

            // Update and save
            unitOfWork.Pages.Update(page);
            await unitOfWork.Save();

            return Ok();
        }



        [HttpGet]
        [Route("Search")]
        public async Task<ActionResult> Search(string searchWords)
        {
            return Ok(await unitOfWork.Pages.GetCollection<ItemViewModel<Page>>(searchWords));
        }
    }
}
