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
    public class EmailsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public EmailsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }


        [HttpGet]
        public async Task<ActionResult> GetEmails()
        {
            return Ok(await unitOfWork.Emails.GetCollection<ItemViewModel<Email>>());
        }



        [HttpGet]
        [Route("Page")]
        public async Task<ActionResult> GetEmail(int id)
        {
            return Ok(await unitOfWork.Emails.Get(x => x.Id == id, x => x.Content));
        }



        [HttpPut]
        [Route("Page")]
        public async Task<ActionResult> UpdateEmail(UpdatedPage updatedPage)
        {
            Email email = await unitOfWork.Emails.Get(updatedPage.PageId);

            email.Name = updatedPage.Name;
            email.Content = updatedPage.Content;

            // Update and save
            unitOfWork.Emails.Update(email);
            await unitOfWork.Save();

            return Ok();
        }



        [HttpGet]
        [Route("Search")]
        public async Task<ActionResult> Search(string searchWords)
        {
            return Ok(await unitOfWork.Emails.GetCollection<ItemViewModel<Email>>(searchWords));
        }
    }
}
