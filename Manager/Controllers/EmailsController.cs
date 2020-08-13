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






        [Route("Create")]
        [HttpGet]
        public async Task<ActionResult> CreateEmail()
        {
            string emailName = "None";


            // Create the new email
            Email email = new Email
            {
                Name = emailName,
                Content = ""
            };

            // Add and save
            unitOfWork.Emails.Add(email);
            await unitOfWork.Save();

            // Update the content with the new Id and update
            email.Content = "{\"id\":" + email.Id + ",\"name\":\"" + emailName + "\",\"background\":{\"color\":\"#ffffff\"}}";
            unitOfWork.Emails.Update(email);


            await unitOfWork.Save();


            // Return the new email content
            return Ok(email.Content);
        }






        [Route("Duplicate")]
        [HttpGet]
        public async Task<ActionResult> DuplicateEmail(int pageId)
        {
            // Get the page
            Email email = await unitOfWork.Emails.Get(pageId);
            email.Id = 0;

            // Add the duplicated email and save
            unitOfWork.Emails.Add(email);
            await unitOfWork.Save();


            // Update the content with the new id and save
            email.Content = Regex.Replace(email.Content, "^{\"id\":" + pageId, "{\"id\":" + email.Id);
            unitOfWork.Emails.Update(email);
            await unitOfWork.Save();


            // Return the page content
            return Ok(email.Content);
        }







        [HttpDelete]
        public async Task<ActionResult> DeleteEmail(int pageId)
        {
            Email email = await unitOfWork.Emails.Get(pageId);

            unitOfWork.Emails.Remove(email);
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
