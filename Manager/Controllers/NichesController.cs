using System.Threading.Tasks;
using Manager.Repositories;
using DataAccess.ViewModels;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Models;
using Manager.Classes;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NichesController : ControllerBase {
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
            return Ok(await unitOfWork.LeadPageEmails.Get(x => x.Id == leadPageId, x => x.Content));
        }







        [Route("LeadPageEmail")]
        [HttpPut]
        public async Task<ActionResult> UpdateLeadPageEmail(UpdatedPage updatedPage)
        {
            LeadPageEmail leadPageEmail = await unitOfWork.LeadPageEmails.Get(updatedPage.PageId);

            leadPageEmail.Subject = updatedPage.Name;
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
    }
}
