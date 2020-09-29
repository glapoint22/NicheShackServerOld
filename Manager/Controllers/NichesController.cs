using System.Threading.Tasks;
using Manager.Repositories;
using DataAccess.ViewModels;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Models;
using Manager.Classes;
using System;
using System.Text.RegularExpressions;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NichesController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public NichesController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult> GetNiches(int categoryId)
        {
            return Ok(await unitOfWork.Niches.GetCollection<ItemViewModel<Niche>>(x => x.CategoryId == categoryId));
        }


        [Route("All")]
        [HttpGet]
        public async Task<ActionResult> GetAllNiches()
        {
            return Ok(await unitOfWork.Niches.GetCollection<ItemViewModel<Niche>>());
        }



        [HttpPut]
        public async Task<ActionResult> UpdateNicheName(ItemViewModel niche)
        {
            Niche updatedNiche = await unitOfWork.Niches.Get(niche.Id);

            updatedNiche.Name = niche.Name;

            // Update and save
            unitOfWork.Niches.Update(updatedNiche);
            await unitOfWork.Save();

            return Ok();
        }





        [HttpPost]
        public async Task<ActionResult> AddNiche(ItemViewModel niche)
        {
            Niche newNiche = new Niche
            {
                CategoryId = niche.Id,
                Name = niche.Name,
                UrlId = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper(),
                UrlName = Utility.GetUrlName(niche.Name)
            };

            unitOfWork.Niches.Add(newNiche);
            await unitOfWork.Save();

            return Ok(newNiche.Id);
        }





        [HttpDelete]
        public async Task<ActionResult> DeleteNiche(int id)
        {
            Niche niche = await unitOfWork.Niches.Get(id);

            unitOfWork.Niches.Remove(niche);
            await unitOfWork.Save();

            return Ok();
        }





        [Route("LeadPageIds")]
        [HttpGet]
        public async Task<ActionResult> GetLeadPageIds(int nicheId)
        {
            return Ok(await unitOfWork.LeadPages.GetCollection(x => x.NicheId == nicheId, x => x.Id));
        }


        [Route("LeadPage")]
        [HttpGet]
        public async Task<ActionResult> GetLeadPage(int leadPageId)
        {
            return Ok(await unitOfWork.LeadPages.Get(x => x.Id == leadPageId, x => x.Content));
        }




        [Route("LeadPage/Add")]
        [HttpGet]
        public async Task<ActionResult> AddLeadPage(int nicheId)
        {
            string leadPageName = "New Lead Page";
            string leadPageEmailName = "New Lead Page Email";


            // Create the new lead page
            LeadPage leadPage = new LeadPage
            {
                NicheId = nicheId,
                Name = leadPageName,
                Content = ""
            };

            // Add and save
            unitOfWork.LeadPages.Add(leadPage);
            await unitOfWork.Save();

            // Update the content with the new Id and update
            leadPage.Content = "{\"id\":" + leadPage.Id + ",\"name\":\"" + leadPageName + "\",\"background\":{\"color\":\"#ffffff\"}}";
            unitOfWork.LeadPages.Update(leadPage);




            // Create the new lead page email
            LeadPageEmail leadPageEmail = new LeadPageEmail
            {
                LeadPageId = leadPage.Id,
                Name = leadPageEmailName,
                Content = ""
            };


            // Add and save
            unitOfWork.LeadPageEmails.Add(leadPageEmail);
            await unitOfWork.Save();


            // Update the content with the new Id
            leadPageEmail.Content = "{\"id\":" + leadPageEmail.Id + ",\"name\":\"" + leadPageEmailName + "\",\"background\":{\"color\":\"#ffffff\"}}";


            // Update and save
            unitOfWork.LeadPageEmails.Update(leadPageEmail);
            await unitOfWork.Save();


            // Return the new lead page content
            return Ok(leadPage.Content);
        }




        [Route("LeadPage/Duplicate")]
        [HttpGet]
        public async Task<ActionResult> DuplicateLeadPage(int leadPageId)
        {
            // Get the lead page
            LeadPage leadPage = await unitOfWork.LeadPages.Get(leadPageId);
            leadPage.Id = 0;

            // Add the duplicated lead page and save
            unitOfWork.LeadPages.Add(leadPage);
            await unitOfWork.Save();


            // Update the content with the new id
            leadPage.Content = Regex.Replace(leadPage.Content, "^{\"id\":" + leadPageId, "{\"id\":" + leadPage.Id);
            unitOfWork.LeadPages.Update(leadPage);


            // Get the lead page email
            LeadPageEmail leadPageEmail = await unitOfWork.LeadPageEmails.Get(x => x.LeadPageId == leadPageId);

            // Get the current id for later use
            int leadPageEmailId = leadPageEmail.Id;

            // Reset the id and assign the new lead page id
            leadPageEmail.Id = 0;
            leadPageEmail.LeadPageId = leadPage.Id;


            // Add the new lead page email and save
            unitOfWork.LeadPageEmails.Add(leadPageEmail);
            await unitOfWork.Save();


            // Update the content and save
            leadPageEmail.Content = Regex.Replace(leadPageEmail.Content, "^{\"id\":" + leadPageEmailId, "{\"id\":" + leadPageEmail.Id);
            unitOfWork.LeadPageEmails.Update(leadPageEmail);
            await unitOfWork.Save();


            // Return the lead page content
            return Ok(leadPage.Content);
        }







        [Route("LeadPage")]
        [HttpPut]
        public async Task<ActionResult> UpdateLeadPage(UpdatedPage updatedPage)
        {
            LeadPage leadPage = await unitOfWork.LeadPages.Get(updatedPage.PageId);

            leadPage.Name = updatedPage.Name;
            leadPage.Content = updatedPage.Content;

            // Update and save
            unitOfWork.LeadPages.Update(leadPage);
            await unitOfWork.Save();

            return Ok();
        }





        [Route("LeadPageEmail")]
        [HttpGet]
        public async Task<ActionResult> GetLeadpageEmails(int leadPageId)
        {
            return Ok(await unitOfWork.LeadPageEmails.Get(x => x.LeadPageId == leadPageId, x => x.Content));
        }







        [Route("LeadPageEmail")]
        [HttpPut]
        public async Task<ActionResult> UpdateLeadPageEmail(UpdatedPage updatedPage)
        {
            LeadPageEmail leadPageEmail = await unitOfWork.LeadPageEmails.Get(updatedPage.PageId);

            leadPageEmail.Name = updatedPage.Name;
            leadPageEmail.Content = updatedPage.Content;

            // Update and save
            unitOfWork.LeadPageEmails.Update(leadPageEmail);
            await unitOfWork.Save();

            return Ok();
        }






        [HttpGet]
        [Route("Search")]
        public async Task<ActionResult> Search(string searchWords)
        {
            return Ok(await unitOfWork.Niches.GetCollection<ItemViewModel<Niche>>(searchWords));
        }






        [Route("LeadPage")]
        [HttpDelete]
        public async Task<ActionResult> DeleteLeadPage(int leadPageId)
        {
            LeadPage leadPage = await unitOfWork.LeadPages.Get(leadPageId);

            unitOfWork.LeadPages.Remove(leadPage);
            await unitOfWork.Save();


            return Ok();
        }
    }
}
