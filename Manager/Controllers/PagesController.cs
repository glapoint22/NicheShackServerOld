using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;
using DataAccess.ViewModels;
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
        public async Task<ActionResult> GetPage(int pageId)
        {
            return Ok(await unitOfWork.Pages.Get(x => x.Id == pageId, x => x.Content));
        }
    }
}
