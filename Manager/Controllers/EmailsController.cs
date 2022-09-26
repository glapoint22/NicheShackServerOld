using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataAccess.Models;
using DataAccess.ViewModels;
using Manager.Classes;
using Manager.Repositories;
using Microsoft.AspNetCore.Mvc;
using Services.Classes;
using Services.Interfaces;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IPageService pageService;

        public EmailsController(IUnitOfWork unitOfWork, IPageService pageService)
        {
            this.unitOfWork = unitOfWork;
            this.pageService = pageService;
        }




        [HttpGet]
        public async Task<ActionResult> GetEmail(int id)
        {
            //return Ok(await unitOfWork.Emails.Get(x => x.Id == id, x => x.Content));

            PageContent pageContent = null;

            // Get the page
            Email Email = await unitOfWork.Emails.Get(id);

            // Get the page content
            if (Email != null && Email.Content != null)
                pageContent = await pageService.GePage(Email.Content, new QueryParams());



            return Ok(new PageData
            {
                Id = Email.Id,
                Name = Email.Name,
                Content = pageContent
            });
        }



        



        [HttpPost]
        public async Task<ActionResult> NewEmail(PageViewModel newPage)
        {
            Email email = new Email
            {
                Name = newPage.Name,
                Content = newPage.Content,
            };

            unitOfWork.Emails.Add(email);
            await unitOfWork.Save();

            return Ok(email.Id);
        }




        [HttpPut]
        public async Task<ActionResult> UpdateEmaill(PageViewModel updatedEmail)
        {
            Email email = await unitOfWork.Emails.Get(updatedEmail.Id);

            email.Name = updatedEmail.Name;
            email.Content = updatedEmail.Content;


            unitOfWork.Emails.Update(email);
            await unitOfWork.Save();

            return Ok();
        }




        [HttpPost]
        [Route("Duplicate")]
        public async Task<ActionResult> DuplicateEmail(Item page)
        {
            // Copy the page properties
            Email currentEmail = await unitOfWork.Emails.Get(page.Id);

            // Create the new email
            var duplicateEmail = new Email
            {
                Name = currentEmail.Name + " Copy",
                Content = currentEmail.Content,
            };

            unitOfWork.Emails.Add(duplicateEmail);
            await unitOfWork.Save();


            return Ok(duplicateEmail.Id);
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
        public async Task<ActionResult> SearchEmail(string searchTerm)
        {
            return Ok(await unitOfWork.Emails.GetCollection<ItemViewModel<Email>>(searchTerm));
        }
    }
}
